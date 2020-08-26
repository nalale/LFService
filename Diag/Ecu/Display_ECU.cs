using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MFService
{
	public class Display_ECU : ECU
	{
		const int REPEATER_TABLE_SIZE = 20;

		const int TABLE_CNT	=	6;

        Diag.A_Service_ReadDataByIdentifier _diagSrv;
        public CodingData_t Data;
        List<DiagnosticData> _diagData;
        int EcuAddress;

        public ushort ConfigLenght;		

		public Display_ECU(EcuModelId modelId, byte address)
			: base(modelId, address)
		{
			Data = new CodingData_t();
            _diagData = new List<DiagnosticData>();
            Data.MotorRpm = new short[TABLE_CNT*2];
			Data.SoC = new short[TABLE_CNT*2];
			Data.TrimPosition = new short[TABLE_CNT*2];

            _diagSrv = new Diag.A_Service_ReadDataByIdentifier();

            EcuAddress = address;
            SetDiagData();
        }


		#region Config struct
        
		[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
		public unsafe struct CodingData_t
		{
			// Общие
			public byte DiagnosticID;
			public byte Index;                   // Порядковый номер ECU.
			
			// Обороты мотора
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = TABLE_CNT*2)]
			public short[] MotorRpm;
			// Уровень заряда накопителя
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = TABLE_CNT*2)]
			public short[] SoC;
			// Положение трим
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = TABLE_CNT*2)]
			public short[] TrimPosition;
            // Удельный расход
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = TABLE_CNT*2)]
            public short[] SpecPower;

            public ushort PowerOffDelay_ms;
            public ushort KeyOffTime_ms;            

            public short addition_1;
			public short addition_2;
			public short addition_3;
			public short addition_4;
			public short addition_5;
			public short addition_6;
//			public short addition_7;
            
			// Контрольная сумма
			public ushort CRC;		
		}
        #endregion

        #region diag data

        private void SetDiagData()
        {
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuInfo, "ECU: Информация", 1, true, 4, Converter_EcuInformation));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDateTime, "Время системы", 1, true, 1, Converter_SystemTime));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInOutState, "Порты ввода вывода", 1, true, 1, Converter_DischargeMask));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMainState, "Режим работы блока", 1, true, 1, Converter_EcuState));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSubState, "Подрежим работы блока", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuVoltage, "Напряжение ECU", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPowerManagmentState, "Режим управления питанием", 1, true, 1, PowerManagerStateToString));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPwmOutState, "Выход ШИМ", 1, true, 4, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor1, "Датчик напряжения 1", 0.1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor2, "Датчик напряжения 2", 0.1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor3, "Датчик напряжения 3", 0.1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor4, "Датчик напряжения 4", 0.1, false, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor1, "Датчик тока 1", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor2, "Датчик тока 2", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor3, "Датчик тока 3", 0.1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDisplayRpm, "Дисплей: Обороты", 1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDisplaySOC, "Дисплей: Уровень заряда", 1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDisplayTrim, "Дисплей: положение трим", 1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDisplayConsumption, "Дисплей: удельная мощность", 0.1, false, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didFaults_Actual, "Активные ошибки", 1, true, (byte)FaultCodes.dtc_Count, Converter_Faults));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didFaults_History, "Сохраненные ошибки", 1, true, (byte)FaultCodes.dtc_Count, Converter_Faults));
        }

        #endregion

        public override List<DiagnosticData> GetDiagnosticSets()
        {
            foreach (DiagnosticData dd in _diagData)
                dd.ClearValues();

            return _diagData;
        }
        public override async Task<List<string>> GetEcuInfo()
        {
            DiagnosticData dv = this.GetDiagnosticSets().Find((diag) => diag.DataID == (int)ObjectsIndex_e.didEcuInfo);

            if (dv == null)
                return new List<string>();

            _diagSrv.AddRequestedDID((uint)dv.DataID);

            if (await _diagSrv.RequestService(EcuAddress))
            {
                var lst = _diagSrv.GetResponseValue(dv.DataID);
                foreach (int val in lst)
                    dv.AddValue(val);

                return dv.GetValue();
            }
            else
                return new List<string>();
        }
        public override async Task<bool> ClearFaults()
		{
			return await Global.diag.WriteDataByID((byte)EcuAddress, (byte)ObjectsIndex_e.didFaults_History, null);
		}
        public override DiagnosticData GetFrzFramesSet()
        {
            return _diagData.Find(dd => dd.DataID == (ushort)ObjectsIndex_e.didFaults_FreezeFrame);
        }
        public override async Task<List<Diag.ResponseData_ReadDataByIdentifier>> GetEcuTime()
        {
            return await Global.diag.ReadDataByIDs((byte)EcuAddress, new int[] { (byte)ObjectsIndex_e.didDateTime });
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

            didDisplaySOC,
            didDisplayRpm,
            didDisplayTrim,
            didDisplayConsumption,

            // Диагностика ошибок
            didFaults_Actual = 100,
            didFaults_History,

            didFaults_FreezeFrame,

            didFlashData = 105,

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

		string PowerManagerStateToString(Tuple<int, int> t)
        {
            string fault_desc;
            int Code = t.Item1;
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

		string Converter_EcuState(Tuple<int, int> t)
		{
            int Code = t.Item1;

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

		string Converter_Faults(Tuple<int, int> t)
        {
            int Code = t.Item1;
            FaultCodes fault_id = (FaultCodes)((Code & 0xff00) >> 8);
            int fault_cat = Code & 0xff;

            string fault_desc = FaultCodeToString(fault_id);
            string cat_desc = DTC_Category.ToString((DTC_Category.Code)fault_cat);

            return fault_desc + ", " + cat_desc;
        }

        string Converter_DischargeMask(Tuple<int, int> t)
        {
            int Code = t.Item1;
            return Convert.ToString(Code, 2).PadLeft(32, '0');
        }

        string Converter_SystemTime(Tuple<int, int> t)
        {
            int Code = t.Item1;
            // Точка отсчета 01.01.2000
            DateTime _actualDate = new DateTime(2000, 1, 1).AddSeconds(Code);
            //DateTime _actualDate = _startTime.AddSeconds(Code);

            return _actualDate.ToString();
        }

        new string Converter_EcuInformation(Tuple<int, int> t)
        {
            return base.Converter_EcuInformation(t);
        }

        #endregion
    }
}
