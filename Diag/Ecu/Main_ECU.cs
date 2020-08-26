using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MFService
{
	public class Main_ECU : ECU
	{
        const int AN_OUT_COUNT = 4;

        public ushort ConfigLenght;
        public CodingData_t Data;
        List<DiagnosticData> _diagData;
        Diag.A_Service_ReadDataByIdentifier _diagSrv;
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
            _diagSrv = new Diag.A_Service_ReadDataByIdentifier();

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] TorqueDemandTable;

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

            public short SteeringKp;
            public short SteeringKi;
            public short SteeringKd;            

            // Трим
            public byte TrimMinVal_0p1V;
            public byte TrimMaxVal_0p1V;
            public short TrimUpLimitDrive_0p1V;

            // Управление питанием
            public ushort PowerOffDelay_ms;
            public ushort KeyOffTime_ms;
            public byte flags;

            public byte InvCoolingOn;
            public byte MotorCoolingOn;

            // Charging
            public byte MaxChargingCurrent_A;
            public byte ChargersNumber;

            public ushort RateMotorTorque_Nm;
            public short addition_3;
            public short addition_4;
            public short addition_5;
            public short addition_6;
            //public short addition_7;


            // Контрольная сумма
            public UInt16 CRC;
        }
        #endregion

        #region diag data

        private void SetDiagData()
        {
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuInfo, "ECU: Информация", 1, true, 4, Converter_EcuInformation));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didDateTime, "Время системы", 1, true, 1, Converter_SystemTime));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInOutState, "Порты ввода вывода", 1, true, 1, Converter_DischargeMask));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didEcuVoltage, "Напряжение ECU", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPowerManagmentState, "Режим управления питанием", 1, true, 1, PowerManagerStateToString));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didPwmOutState, "Выход ШИМ", 1, true, 4, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMachineState, "Режим работы ECU", 1, true, 1, Converter_EcuState));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMachineSubState, "Подрежим ECU", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor1, "Датчик напряжения 1", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor2, "Датчик напряжения 2", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor3, "Датчик напряжения 3", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didVoltageSensor4, "Датчик напряжения 4", 0.1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor1, "Датчик тока 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor2, "Датчик тока 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didCurrentSensor3, "Датчик тока 3", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didHelmDemandAngle, "Helm: Угол поворота", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didHelmStatus, "Helm: Состояние", 1, true, 1, Converter_InverterStatus)); 

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringFB, "Steering: Напряжение обратной связи", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringFBAngle, "Steering: Угол поворота", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringCurent, "Steering: Ток привода", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didSteeringStatus, "Steering: Состояние", 1, true, 1, Converter_InverterStatus));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimFB, "Trim: Обратная связь трим", 0.1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimMovCmd, "Trim: Команда трим", 1, true, 1, Converter_TrimCmd));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimPosition, "Trim: Положение", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTrimStatus, "Trim: Состояние", 1, true, 1, Converter_InverterStatus));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccCh1, "Ручка акселератора: канал 1", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccCh2, "Ручка акселератора: канал 2", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccPosition, "Ручка акселератора: Запрос тяги", 1, false, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didAccDemandDirection, "Ручка акселератора: Направление тяги", 1, false, 1, Converter_AccHandleDir));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didGearActualDirection, "Трансмиссия: Напрявление тяги", 1, false, 1, Converter_AccHandleDir));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didGearIsShifting, "Трансмиссия: Переключение в процессе", 1, false, 1)); 
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didGearActuatorState, "Трансмиссия: положение актуатора", 1, false, 1));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didMotorTemp, "Инвертор: Температура мотора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInvTemp, "Инвертор: Температура инвертора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didActualRpm, "Инвертор: Текущие обороты", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterCurrent, "Инвертор: Ток AC", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterVoltage, "Инвертор: Напряжение инвертора", 1, true, 1, null)); 
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didBatteryCurrent, "Инвертор: Ток DC", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didActualGear, "Инвертор: Направление движения", 1, true, 1, Converter_InverterDir));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didTargetSpeed, "Инвертор: Запрашиваемый крутящий момент", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterEnable, "Инвертор: Запрос включения инвертора", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didInverterState, "Инвертор: Состояние инвертора", 1, true, 1, Converter_InverterStatus));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didWaterSwitches, "Датчики затопления", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didBatteryCmd, "Батарея: Разрешение работы", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didIsoIsolation, "ISO: Сопротивление изоляции, Ом/В", 1, true, 1, null));

            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didObcStatus, "OBC: состояние", 1, true, 1, Converter_ObcStatus));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didObcOnlineNumber, "OBC: количество модулей онлайн", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didObcDemandTotalCurrent, "OBC: Запрашиваемый ток заряда", 1, true, 1, null));
            _diagData.Add(new DiagnosticData((ushort)ObjectsIndex_e.didObcTotalCurrent, "OBC: Общий ток заряда", 1, true, 1, null));

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
            return await Global.diag.WriteDataByID((byte)EcuAddress, (byte)ObjectsIndex_e.didFaults_History, new byte[] { 0 });
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
            didMachineState,
            didMachineSubState,
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

            didHelmDemandAngle,
            didHelmStatus,

            didSteeringFB,
            didSteeringCurent,
            didSteeringFBAngle,
            didSteeringStatus,

            didTrimFB,
            didTrimMovCmd,
            didTrimPosition,
            didTrimStatus,

            didAccCh1,
            didAccCh2,
            didAccPosition,
            didAccDemandDirection,

            didMotorTemp,
            didInvTemp,
            didActualRpm,
            didInverterCurrent,
            didBatteryCurrent,
            didActualGear,
            didTargetSpeed,
            didInverterVoltage,
            didInverterEnable,
            didInverterState,

            didWaterSwitches,

            didBatteryCmd,

            didIsoIsolation,

            didObcStatus,
            didObcOnlineNumber,
            didObcTotalCurrent,
            didObcDemandTotalCurrent,

            didGearActualDirection,
            didGearIsShifting,
            didGearActuatorState,

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
            DateTime _startTime = new DateTime(2000, 1, 1);
            DateTime _actualDate = _startTime.AddSeconds(Code);

            return _actualDate.ToString();
        }

        string Converter_EcuState(Tuple<int, int> t)
        {
            int Code = t.Item1;
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

        new string Converter_EcuInformation(Tuple<int, int> t)
        {
            return base.Converter_EcuInformation(t);
        }
        string Converter_AccHandleDir(Tuple<int, int> t)
        {
            int Code = t.Item1;
            switch (Code)
            {
                case 0:
                    return "Нейтраль";
                case 1:
                    return "Вперед";
                case 2:
                    return "Назад";
                default:
                    return Code.ToString();
            }
        }

        string Converter_TrimCmd(Tuple<int, int> t)
        {
            int Code = t.Item1;
            switch (Code)
            {
                case 0:
                    return "Остановка";
                case 1:
                    return "Вниз";
                case 2:
                    return "Вверх"; 
                default:
                    return Code.ToString();
            }
        }

        string Converter_InverterDir(Tuple<int, int> t)
        {
            int Code = t.Item1;
            switch (Code)
            {
                case 0:
                    return "Назад";
                case 1:
                    return "Вперед";
                default:
                    return Code.ToString();
            }
        }

        string Converter_InverterStatus(Tuple<int, int> t)
        {
            int Code = t.Item1;
            switch (Code)
            {
                case 0:
                    return "Выключен";
                case 1:
                    return "Рабочий режим";
                case 2:
                    return "Предупреждение";
                case 3:
                    return "Авария";
                default:
                    return Code.ToString();
            }        
        }

        string Converter_ObcStatus(Tuple<int, int> t)
        {
            int Code = t.Item1;
            switch (Code)
            {
                case 0:
                    return "Выключен";
                case 1:
                    return "Рабочий режим";
                case 2:
                    return "Заряд окончен";
                case 3:
                    return "Авария";
                default:
                    return Code.ToString();
            }
        }

        #endregion
    }
}
