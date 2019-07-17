using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	public class General_ECU : ECU
	{
		const int REPEATER_TABLE_SIZE = 20;

		const int AN_IN_COUNT	=	4;
		const int DIG_IN_COUNT	=	10;
		const int AN_OUT_COUNT	=	4;
		const int DIG_OUT_COUNT	=	10;

		
		public CodingData_t Data;
        List<DiagnosticData> _diagData;
        int EcuAddress;

        public ushort ConfigLenght;

        /// <summary>
		/// Если 1, то PowerMaganer.
		/// </summary>
		public bool IsPowerManger { get { return (Data.flags & 0x01) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x01) : (byte)(Data.flags & ~0x01); } }

        public bool PullUp_IN1 { get { return (Data.flags & 0x10) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x10) : (byte)(Data.flags & ~0x10); } }
        public bool PullUp_IN2 { get { return (Data.flags & 0x20) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x20) : (byte)(Data.flags & ~0x20); } }
        public bool PullUp_IN3 { get { return (Data.flags & 0x40) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x40) : (byte)(Data.flags & ~0x40); } }
        public bool PullUp_IN4 { get { return (Data.flags & 0x80) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x80) : (byte)(Data.flags & ~0x80); } }

        public int RepeaterTableSize { get { return REPEATER_TABLE_SIZE; } private set {; } }
		

		public General_ECU(EcuModelId modelId, byte address)
			: base(modelId, address)
		{
			Data = new CodingData_t();
            _diagData = new List<DiagnosticData>();
            Data.AnalogOutput = new byte[AN_OUT_COUNT];
			Data.DigitalOutput = new byte[DIG_OUT_COUNT];
			Data.CurrentThreshold_A = new byte[AN_OUT_COUNT];
			Data.tab = new canRepItem[REPEATER_TABLE_SIZE];

            EcuAddress = address;
            SetDiagData();
        }


		#region Config struct

		[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
		public struct canRepItem
		{
			public UInt32 Id1;
            public UInt32 Id2;
			public UInt16 SendPeriod;
			public byte flags;
			public byte RepCount;


			public bool IsActive { get { return (flags & 0x01) != 0; } set { flags = value ? (byte)(flags | 0x01) : (byte)(flags & ~0x01); } }
			public bool Ext1 { get { return (flags & 0x02) != 0; } set { flags = value ? (byte)(flags | 0x02) : (byte)(flags & ~0x02); } }            
            public bool Direction { get { return (flags & 0x04) != 0; } set { flags = value ? (byte)(flags | 0x04) : (byte)(flags & ~0x04); } }
            public bool Ext2 { get { return (flags & 0x08) != 0; } set { flags = value ? (byte)(flags | 0x08) : (byte)(flags & ~0x08); } }

            public override string ToString()
			{
				string s = "";
				s += "External CAN ID: " + "0x" + Id1.ToString("X8");				
                s += ", PCAN ID: " + "0x" + Id2.ToString("X8");

                if (SendPeriod != 0)
					s += ", период " + SendPeriod + " мс";

                if (Direction)
                    s += ", прием из PCan";
                else
                    s += ", прием из external Can";

                return s;
			}
		}


		[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
		public unsafe struct CodingData_t
		{
			// Общие
			public byte DiagnosticID;
			public byte Index;                   // Порядковый номер ECU.
			
			// Номер байтов в сообщении для аналоговых выходов ECU
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = AN_OUT_COUNT)]
			public byte[] AnalogOutput;
			// Номер битов в сообщении для дискретных выходов ECU
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = DIG_OUT_COUNT)]
			public byte[] DigitalOutput;

			// ограничение тока для всех аналоговых выходов
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = AN_OUT_COUNT)]
			public byte[] CurrentThreshold_A;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = REPEATER_TABLE_SIZE)]
			public canRepItem[] tab;

            public ushort PowerOffDelay_ms;
            public ushort KeyOffTime_ms;
            public byte flags;

            public short addition_1;
			public short addition_2;
			public short addition_3;
			public short addition_4;
			public short addition_5;
			public short addition_6;
			public short addition_7;
            
			// Контрольная сумма
			public ushort CRC;		
		}
        #endregion

        #region diag data

        private void SetDiagData()
        {
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDateTime, "Время системы", 1, true, 1, Converter_SystemTime));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInOutState, "Порты ввода вывода", 1, true, 1, Converter_DischargeMask));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMainState, "Режим работы блока", 1, true, 1, Converter_EcuState));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSubState, "Подрежим работы блока", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuVoltage, "Напряжение ECU", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPowerManagmentState, "Режим управления питанием", 1, true, 1, PowerManagerStateToString));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPwmOutState, "Выход ШИМ", 1, true, 4, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor1, "Датчик напряжения 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor2, "Датчик напряжения 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor3, "Датчик напряжения 3", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor4, "Датчик напряжения 4", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor1, "Датчик тока 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor2, "Датчик тока 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor3, "Датчик тока 3", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didFaults_Actual, "Активные ошибки", 1, true, (byte)FaultCodes.dtc_Count, Converter_Faults));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didFaults_History, "Сохраненные ошибки", 1, true, (byte)FaultCodes.dtc_Count, Converter_Faults));
        }

        #endregion

        public override List<DiagnosticData> GetDiagnosticSets()
        {
            foreach (DiagnosticData dd in _diagData)
                dd.Value.Clear();

            return _diagData;
        }

        public override async Task<List<ResponseData_ReadDataByIdentifier>> GetEcuInfo()
        {
            return await Global.diag.ReadDataByIDs((byte)EcuAddress, new byte[] { (byte)ObjectsIndex_e.didEcuInfo });
        }

		public override async Task<bool> ClearFaults()
		{
			return await Global.diag.WriteDataByID((byte)EcuAddress, (byte)ObjectsIndex_e.didFaults_History, new byte[] { 0 });
		}

		#region ObjectIndexes

		public enum ObjectsIndex_e
        {
            didEcuInfo = 0,
            didConfigStructIndex,
            didDateTime,
            didMainState,
            didSubState,
            didInOutState,
            didEcuVoltage,
            didPowerManagmentState,

            didVoltageSensor1 = 10,
            didVoltageSensor2,
            didVoltageSensor3,
            didVoltageSensor4,
            didVoltageSensor5,
            didVoltageSensor6,
            didVoltageSensor7,
            didVoltageSensor8,

            didCurrentSensor1,
            didCurrentSensor2,
            didCurrentSensor3,
            didCurrentSensor4,
            
            didPwmOutState,

            // Диагностика ошибок
            didFaults_Actual = 100,
            didFaults_History,

            didFaults_FreezeFrame,

        };

        public enum FaultCodes
        {
            dtc_General_EcuConfig = 0,
            dtc_General_EcuSupplyOutOfRange,
            dtc_General_UnexpectedPowerOff,
            dtc_General_Interlock,
            dtc_General_EcuDataTimeNotCorrect,

            // Таймаут CAN
            dtc_CAN_ExtCan,
            dtc_CAN_PCAN,

            dtc_PwmCircuit_1,
            dtc_PwmCircuit_2,
            dtc_PwmCircuit_3,
            dtc_MeasuringCircuit,
            dtc_PowerSupplyCircuit,


            dtc_Count
        }

        string FaultCodeToString(FaultCodes Code)
        {
            string fault_desc;
            switch (Code)
            {
                case FaultCodes.dtc_General_EcuConfig:
                    fault_desc = "Ошибка памяти";
                    break;
                case FaultCodes.dtc_General_EcuDataTimeNotCorrect:
                    fault_desc = "Некорректное дата/время";
                    break;
                case FaultCodes.dtc_General_EcuSupplyOutOfRange:
                    fault_desc = "Напряжение питания";
                    break;
                case FaultCodes.dtc_General_Interlock:
                    fault_desc = "Интерлок";
                    break;
                case FaultCodes.dtc_General_UnexpectedPowerOff:
                    fault_desc = "Неожиданное отключение питания";
                    break;
                case FaultCodes.dtc_CAN_ExtCan:
                    fault_desc = "Оффлайн CAN 2";
                    break;
                case FaultCodes.dtc_CAN_PCAN:
                    fault_desc = "Оффлайн CAN 1";
                    break;
                case FaultCodes.dtc_PwmCircuit_1:
                    fault_desc = "Цепи ШИМ 1";
                    break;
                case FaultCodes.dtc_PwmCircuit_2:
                    fault_desc = "Цепи ШИМ 2";
                    break;
                case FaultCodes.dtc_PwmCircuit_3:
                    fault_desc = "Цепи ШИМ 3";
                    break;
                case FaultCodes.dtc_MeasuringCircuit:
                    fault_desc = "Цепи измерения АЦП";
                    break;
                case FaultCodes.dtc_PowerSupplyCircuit:
                    fault_desc = "Цепи питания ECU";
                    break;

                default:
                    fault_desc = Code.ToString();
                    break;
            }

            return fault_desc;
        }

		string PowerManagerStateToString(int Code)
        {
            string fault_desc;
            switch (Code)
            {
                case 0:
                    fault_desc = "Питание отсутствует";
                    break;
                case 1:
                    fault_desc = "Питание присутствует";
                    break;
				case 2:
					fault_desc = "Питание присутствует. Поддержка питания";
					break;
                case 3:
                    fault_desc = "Выключение";
                    break;
                default:
                    fault_desc = Code.ToString();
                    break;
            }

            return fault_desc;
        }
		#endregion

		#region DiagConverter

		string Converter_EcuState(int Code)
		{
			switch (Code)
			{
				case 0:
					return "Инициализация";
				case 1:
					return "Рабочий ";
				case 2:
					return "Выключение";
				case 3:
					return "Ошибка";
				case 4:
					return "Заряд";
				default:
					return Code.ToString();
			}
		}

		string Converter_Faults(int Code)
        {
            FaultCodes fault_id = (FaultCodes)((Code & 0xff00) >> 8);
            int fault_cat = Code & 0xff;

            string fault_desc = FaultCodeToString(fault_id);
            string cat_desc = DTC_Category.ToString((DTC_Category.Code)fault_cat);

            return fault_desc + ", " + cat_desc;
        }

        string Converter_DischargeMask(int Code)
        {
            return Convert.ToString(Code, 2).PadLeft(32, '0');
        }

        string Converter_SystemTime(int Code)
        {
            DateTime dt = new DateTime();
            dt.AddSeconds(Code);

            return dt.ToString();
        }

        #endregion
    }
}
