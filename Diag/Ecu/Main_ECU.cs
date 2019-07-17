using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	public class Main_ECU : ECU
	{
        const int AN_OUT_COUNT = 4;

        public ushort ConfigLenght;
        public CodingData_t Data;
        List<DiagnosticData> _diagData;

        int EcuAddress;

        /// <summary>
        /// Если 1, то PowerMaganer.
        /// </summary>
        public bool IsPowerManger { get { return (Data.flags & 0x01) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x01) : (byte)(Data.flags & ~0x01); } }

        public Main_ECU(EcuModelId modelId, byte address)
			: base(modelId, address)
		{
			Data = new CodingData_t();
            _diagData = new List<DiagnosticData>();
            Data.SteeringBrakeSpeedTable = new ushort[12];

            EcuAddress = address;
            SetDiagData();
        }




		#region Config struct

		[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
		public unsafe struct CodingData_t
		{
            // Общие
            public byte DiagnosticID;                   // OBD ID
            public ushort BaseCanId;

            // Acceleration
            public byte AccPedalFstCh_MaxV;
            public byte AccPedalFstCh_0V;
            public byte AccPedalSndCh_MaxV;

            // Motor Data
            public UInt16 MaxMotorSpeedD;
            public UInt16 MaxTorque;
            public Int16 MaxMotorT;
            public Int16 MaxInverterT;

            // Steering Data
            public byte SteeringMinVal_0p1V;
            public byte SteeringMaxVal_0p1V;
            public short SteeringMaxCurrent_0p1A;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public UInt16[] SteeringBrakeSpeedTable;

            public byte SteeringKp;
            public byte SteeringKi;
            public byte SteeringKd;            

            // Трим
            public byte TrimMinVal_0p1V;
            public byte TrimMaxVal_0p1V;
            public short TrimMaxCurrent_0p1A;

            // Управление питанием
            public ushort PowerOffDelay_ms;
            public ushort KeyOffTime_ms;
            public byte flags;

            public byte InvCoolingOn;
            public byte MotorCoolingOn;

            // Charging
            public byte MaxChargingCurrent_A;
            public byte ChargersNumber;

            public byte addition_3;
            public short addition_4;
            public short addition_5;
            public short addition_6;
            public short addition_7;


            // Контрольная сумма
            public UInt16 CRC;
        }
        #endregion

        #region diag data

        private void SetDiagData()
        {
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDateTime, "Время системы", 1, true, 1, Converter_SystemTime));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInOutState, "Порты ввода вывода", 1, true, 1, Converter_DischargeMask));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuVoltage, "Напряжение ECU", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPowerManagmentState, "Режим управления питанием", 1, true, 1, PowerManagerStateToString));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPwmOutState, "Выход ШИМ", 1, true, 4, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMachineState, "Режим работы ECU", 1, true, 1, Converter_EcuState));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMachineSubState, "Подрежим ECU", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor1, "Датчик напряжения 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor2, "Датчик напряжения 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor3, "Датчик напряжения 3", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor4, "Датчик напряжения 4", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor1, "Датчик тока 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor2, "Датчик тока 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor3, "Датчик тока 3", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringTargetAngle, "Угол поворота руля", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringFB, "Обратная связь рулевой рейки", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimFB, "Обратная связь трим", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccCh1, "Ручка акселератора, канал 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccCh2, "Ручка акселератора, канал 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMotorTemp, "Температура мотора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInvTemp, "Температура инвертора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didActualRpm, "Текущие обороты", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterCurrent, "Ток инвертора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didBatteryCurrent, "Ток батареи", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimMovCmd, "Команда трим", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didActualGear, "Текущий режим движения", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTargetSpeed, "Требуемая скорость", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccPosition, "Ручка акселератора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didBatteryCmd, "Разрешение на включение накопителя", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterEnable, "Инвертор включен", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterState, "Состояние инвертора", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didWaterSwitches, "Датчики затопления", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringFBAngle, "Обратная связь угла поворота рулевой рейки", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didIsoIsolation, "Сопротивление изоляции, Ом/В", 1, true, 1, null));

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

            didMachineState,
            didMachineSubState,

            didInOutState,
            didPwmOutState,

            didSteeringTargetAngle,
            didSteeringFB,
            didSteeringCurent,
            didTrimFB,
            didAccCh1,
            didAccCh2,
            didMotorTemp,
            didInvTemp,
            didActualRpm,
            didInverterCurrent,
            didBatteryCurrent,

            didTrimMovCmd,
            didActualGear,
            didTargetSpeed,
            didAccPosition,
            didInverterVoltage,

            didBatteryCmd,

            didInverterEnable,
            didInverterState,
            didWaterSwitches,

            didSteeringFBAngle,
            didIsoIsolation,

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
            dtc_CAN_Inverter,
            dtc_CAN_Steering,
            dtc_CAN_Battery,

            dtc_BatteryFault,
            dtc_InverterFault,
            dtc_SteeringPosition,
            dtc_TrimPosition,
            dtc_Accelerator,

            dtc_SteeringFeedback,
            dtc_TrimFeedback,

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
                case FaultCodes.dtc_CAN_Inverter:
                    fault_desc = "Нет связи с инвертором";
                    break;
                case FaultCodes.dtc_CAN_Steering:
                    fault_desc = "Нет связи с рулевым управлением";
                    break;
                case FaultCodes.dtc_CAN_Battery:
                    fault_desc = "Нет связи с батареей";
                    break;
                case FaultCodes.dtc_BatteryFault:
                    fault_desc = "Батарея в ошибке";
                    break;
                case FaultCodes.dtc_InverterFault:
                    fault_desc = "Инвертор в ошибке";
                    break;
                case FaultCodes.dtc_SteeringPosition:
                    fault_desc = "Ошибка позиционирования рулевой рейки";
                    break;
                case FaultCodes.dtc_TrimPosition:
                    fault_desc = "Ошибка позиционирования трим";
                    break;
                case FaultCodes.dtc_Accelerator:
                    fault_desc = "Неисправность ручки акселератора";
                    break;
                case FaultCodes.dtc_SteeringFeedback:
                    fault_desc = "Обратная связь рулевого рейки";
                    break;
                case FaultCodes.dtc_TrimFeedback:
                    fault_desc = "Обратная связь трим";
                    break;

                case FaultCodes.dtc_PwmCircuit_1:
                    fault_desc = "Драйвер ШИМ 1";
                    break;
                case FaultCodes.dtc_PwmCircuit_2:
                    fault_desc = "Драйвер ШИМ 2";
                    break;
                case FaultCodes.dtc_PwmCircuit_3:
                    fault_desc = "Драйвер ШИМ 3";
                    break;

                case FaultCodes.dtc_MeasuringCircuit:
                    fault_desc = "Ошибка канала АЦП";
                    break;

                case FaultCodes.dtc_PowerSupplyCircuit:
                    fault_desc = "Драйвер питания";
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

        string Converter_EcuState(int Code)
        {
            switch(Code)
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

        #endregion
    }
}
