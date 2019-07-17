using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	public class Bms_ECU : ECU
	{
		const int CCL_DCL_POINTS_NUM = 6;
		// Количество строк в том же самом
		const int CCL_DCL_LINES_NUM = 3;
        
		public const int VoltageArrayLen = CCL_DCL_POINTS_NUM * CCL_DCL_LINES_NUM;
		public const int TemperatureArrayLen = CCL_DCL_POINTS_NUM * (CCL_DCL_LINES_NUM - 1);
        public const int OCVArrayLen = (CCL_DCL_POINTS_NUM * 2) * CCL_DCL_LINES_NUM;
        public CodingData_t Data;
		//DiagData _diagData;
		List<DiagnosticData> _diagData;
		int EcuAddress;


		public ushort ConfigLenght;
		/// <summary>
		/// Если 0, то Master. Иначе Slave.
		/// </summary>
		public bool IsMaster { get { return (Data.flags & 0x01) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x01) : (byte)(Data.flags & ~0x01); } }
		/// <summary>
		/// Проверять залипание контакторов или нет.
		/// </summary>
		public bool CheckContactor { get { return (Data.flags & 0x02) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x02) : (byte)(Data.flags & ~0x02); } }
		/// <summary>
		/// Проверять интерлок или нет.
		/// </summary>
		public bool CheckInterlock { get { return (Data.flags & 0x04) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x04) : (byte)(Data.flags & ~0x04); } }
		/// <summary>
		/// Является ли ЭБУ сервером времени или нет.
		/// </summary>
		public bool IsTimeServer { get { return (Data.flags & 0x08) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x08) : (byte)(Data.flags & ~0x08); } }
		/// <summary>
		/// Управляется по команде с VCU или включается автоматически.
		/// </summary>
		public bool VcuControled { get { return (Data.flags & 0x10) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x10) : (byte)(Data.flags & ~0x10); } }
		/// <summary>
		/// Выключен. Работает только диагностический протокол.
		/// </summary>
		public bool Offline { get { return (Data.flags & 0x20) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x20) : (byte)(Data.flags & ~0x20); } }
		/// <summary>
		/// Выключен. Работает только диагностический протокол.
		/// </summary>
		public bool IsPowerManager { get { return (Data.flags & 0x40) != 0; } set { Data.flags = value ? (byte)(Data.flags | 0x40) : (byte)(Data.flags & ~0x40); } }

		public Bms_ECU(EcuModelId modelId, byte address)
			: base(modelId, address)
		{
			Data = new CodingData_t();
			_diagData = new List<DiagnosticData>();
			Data.VoltageCCLpoint = new short[Bms_ECU.VoltageArrayLen];
			Data.TemperatureCCLpoint = new short[Bms_ECU.TemperatureArrayLen];
            Data.OCVpoint = new short[Bms_ECU.OCVArrayLen];

			EcuAddress = address;
			SetDiagData();
		}

        #region Config struct        

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
        public unsafe struct CodingData_t
        {
            // Общие
            public byte DiagnosticID;
            public byte BatteryIndex;                   // Порядковый номер батареи в системе (0 - это мастер).
            public byte ModuleIndex;                    // Порядковый номер слейва в сети мастера
            public byte flags;

            public UInt32 BaseID;                       // Базовый CAN ID

            public byte CurrentSensType;                // Тип датчика тока
            public byte CurrentSensDirection;           // Положение установки датчика: 0 обычное, 1 противоположное

            public byte MaxVoltageDisbalanceS;           // Количество батарей в системе
            public ushort TotalCapacity;                 // Суммарная емкость аккумулятора в Ач
            public ushort MaxVoltageDisbalanceP;         // Максимально-допустимая разбалансировка батарей по напряжению в вольтах

            public short MaxDCL;                        // Максимальный DCL, А
            public short MaxCCL;                        // Максимальный CCL, А

            public byte ModulesCountS;                  // Количество модулей в накопителе, соединенных последовательно
            public byte ModulesCountP;                  // Количество модулей в накопителе, соединенных параллельно

            public byte CellNumber;                      // Количество ячеек в модуле
            public ushort MaxCellVoltage_mV;             // Максимальное напряжение на ячейке в мВ
            public ushort MinCellVoltage_mV;             // Минимальное напряжение на ячейке в мВ	
                                                         // Балансировка
            public ushort MinBalancerLevel_mV;           // Минимальный порог напряжения для разрешения балансировки			

            // Опорные точки зависимости максимального тока от напряжения наиболее заряженного аккумулятора
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CCL_DCL_LINES_NUM * CCL_DCL_POINTS_NUM)]
            public short[] VoltageCCLpoint;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (CCL_DCL_LINES_NUM - 1) * CCL_DCL_POINTS_NUM)]
            // Опорные точки зависимости максимального тока от температуры самого холодного аккумулятора
            public short[] TemperatureCCLpoint;

            // Предзаряд
            public ushort PreMaxDuration;                // Максимальное время предзаряда
            public ushort PreZeroCurrentDuration;         // Длительность нулевого тока в мс после которого считаем что предзаряд выполнен
            public ushort PreZeroCurrent;                 // Нулевой ток в амперах (когда считается, что предзаряд прошел)
            public ushort PreMaxCurrent;                  // Максимальный ток предзаряда в амперах

            public byte BalancingTime_s;
            public byte MaxBalancingDiff_mV;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (CCL_DCL_LINES_NUM - 1) * CCL_DCL_POINTS_NUM * 2)]
            // Опорные точки зависимости максимального тока от температуры самого холодного аккумулятора
            public short[] OCVpoint;

            public short PowerOffDelay_ms;
            public short KeyOffTime_ms;
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
            _diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didEcuInfo, "Информация ECU", 1, true, 4, null));
            _diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didDateTime, "Время системы", 1, true, 1, Converter_SystemTime));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didEcuVoltage, "Напряжение питания ECU", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didPowerManagmentState, "Режим управления питанием", 1, true, 1, PowerManagerStateToString));//
			// Параметры ECU
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didVoltageSensor1, "Напряжение датчика тока 1", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didVoltageSensor2, "Напряжение датчика тока 2", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didVoltageSensor3, "Датчик напряжения 1", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didVoltageSensor4, "Датчик напряжение 2", 1, true, 1, null));

			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMachineState, "Режим работы блока", 1, true, 1, Converter_EcuState));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMachineSubState, "Подрежим работы блока", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didCellsVoltages, "Напряжение ячеек", 1, true, 24, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModuleTemperatures, "Датчики температуры", 1, true, 4, null));			
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMaxCellVoltage, "Макс напряжение ячейки модуля", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMinCellVoltage, "Мин напряжение ячейки модуля", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMaxTemperature, "Макс температура модуля", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModMinTemperature, "Мин температура модуля", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModTotalVoltage, "Напряжение модуля", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModDischargeCellsFlag, "Флаги балансировки", 1, true, 1, Converter_DischargeMask));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didModInOutState, "Порты ввода вывода", 1, true, 1, null));

			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMaxCellVoltage, "Макс напряжение ячейки батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMinCellVoltage, "Мин напряжение ячейки батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMaxTemperature, "Макс температура батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMinTemperature, "Мин температура батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMaxVoltage, "Макс напряжение модулей батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatMinVoltage, "Мин напряжение модулей батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatTotalCurrent, "Ток батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatTotalVoltage, "Напряжение батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatStateOfCharge, "Состояние заряда батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatEnergy, "Энергия батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatCCL, "Максимальный ток заряда батареи", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didBatDCL, "Максимальный ток разряда батареи", 1, true, 1, null));

			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstCellMaxVoltage, "Макс напряжение ячейки системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstCellMinVoltage, "Мин напряжение ячейки системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMaxTemperature, "Макс температура системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMinTemperature, "Мин температура системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMaxVoltage, "Макс напряжение батарей системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMinVoltage, "Мин напряжение батарей системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMaxCurrent, "Макс ток батарей системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstMinCurrent, "Мин ток батарей системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstTotalCurrent, "Ток системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstTotalVoltage, "Напряжение системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstStateOfCharge, "Состояние заряда системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstEnergy, "Энергия системы", 1, true, 4, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstCCL, "Максимальный ток заряда системы", 1, true, 1, null));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didMstDCL, "Максимальный ток разряда системы", 1, true, 1, null));

			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didFaults_Actual, "Активные ошибки", 1, true, (byte)BmsFaultCodes.dtc_Count, Converter_Faults));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didFaults_History, "История ошибки", 1, true, (byte)BmsFaultCodes.dtc_Count, Converter_Faults));
			_diagData.Add(new DiagnosticData((ushort)BmsObjectsIndex_e.didFaults_FreezeFrame, "Стоп кадры", 1, true, (byte)BmsFaultCodes.dtc_Count, Converter_Faults));
		}



		public override List<DiagnosticData> GetDiagnosticSets()
		{
			foreach (DiagnosticData dd in _diagData)
				dd.Value.Clear();

			return _diagData;
		}

		public override DiagnosticData GetFrzFramesSet()
		{
			return _diagData.Find(dd => dd.DataID == (ushort)BmsObjectsIndex_e.didFaults_FreezeFrame);
		}

		#endregion

		public override async Task<List < ResponseData_ReadDataByIdentifier>> GetEcuInfo()
		{
			return await Global.diag.ReadDataByIDs((byte)EcuAddress, new byte[] { (byte)BmsObjectsIndex_e.didEcuInfo });			
		}

        public override async Task<List<ResponseData_ReadDataByIdentifier>> GetEcuTime()
        {
            return await Global.diag.ReadDataByIDs((byte)EcuAddress, new byte[] { (byte)BmsObjectsIndex_e.didDateTime });
        }

        public override async Task<bool> ClearFaults()
        {
            return await Global.diag.WriteDataByID((byte)EcuAddress, (byte)BmsObjectsIndex_e.didFaults_History, new byte[] { 0 });
        }

        #region DiagConverter

        string Converter_Faults(int Code)
		{
			BmsFaultCodes fault_id = (BmsFaultCodes)((Code & 0xff00) >> 8);
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
            // Точка отсчета 01.01.2000
            DateTime _startTime = new DateTime(2000, 1, 1);
            DateTime _actualDate = _startTime.AddSeconds(Code);                

            return _actualDate.ToString();
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

		string Converter_EcuState(int Code)
		{
			switch (Code)
			{
				case 0:
					return "Инициализация";
				case 1:
					return "Предрабочий";
				case 2:
					return "Рабочий ";
				case 3:
					return "Выключение";
				case 4:
					return "Ошибка";
				case 5:
					return "Заряд";
				default:
					return Code.ToString();
			}
		}

		#endregion

		string FaultCodeToString(BmsFaultCodes Code)
        {
            string fault_desc;
            switch (Code)
            {
                case BmsFaultCodes.dtc_General_EcuConfig:
                    fault_desc = "Ошибка памяти";
                    break;
                case BmsFaultCodes.dtc_General_EcuDataTimeNotCorrect:
                    fault_desc = "Некорректное дата/время";
                    break;
                case BmsFaultCodes.dtc_General_EcuSupplyOutOfRange:
                    fault_desc = "Напряжение питания";
                    break;
                case BmsFaultCodes.dtc_General_Interlock:
                    fault_desc = "Интерлок";
                    break;
                case BmsFaultCodes.dtc_General_UnexpectedPowerOff:
                    fault_desc = "Неожиданное отключение питания";
                    break;
                case BmsFaultCodes.dtc_Mod_Contactor:
                    fault_desc = "Контактор";
                    break;
                case BmsFaultCodes.dtc_Mod_CellTempOutOfRange:
                    fault_desc = "Температура модуля";
                    break;               
                case BmsFaultCodes.dtc_Mod_MeasuringCircuit:
                    fault_desc = "Цепь измерения ячейки";
                    break;
                case BmsFaultCodes.dtc_Bat_ModVoltageDiff:
                    fault_desc = "Разброс напряжений модулей";
                    break;
                case BmsFaultCodes.dtc_Bat_OverCurrent:
                    fault_desc = "Перегрузка по току";
                    break;
                case BmsFaultCodes.dtc_Bat_Precharge:
                    fault_desc = "Предзаряд";
                    break;
                case BmsFaultCodes.dtc_Bat_WrongModNumber:
                    fault_desc = "Некорректное количество модулей";
                    break;
                case BmsFaultCodes.dtc_Bat_WrongModState:
                    fault_desc = "Некорректное состояние модуля";
                    break;
                case BmsFaultCodes.dtc_Bat_CurrentSensor:
                    fault_desc = "Датчик тока";
                    break;
                case BmsFaultCodes.dtc_Mst_BatVoltageDiff:
                    fault_desc = "Разброс напряжений батарей";
                    break;
                case BmsFaultCodes.dtc_Mst_WrongBatNumber:
                    fault_desc = "Некорректное количество батарей";
                    break;
                case BmsFaultCodes.dtc_Mst_WrongBatState:
                    fault_desc = "Некорректное состояние параллели";
                    break;
                case BmsFaultCodes.dtc_Mst_CellVoltageOutOfRange:
                    fault_desc = "Напряжение ячейки";
                    break;
                case BmsFaultCodes.dtc_CAN_Bms:
                case BmsFaultCodes.dtc_CAN_generalVcu:
                case BmsFaultCodes.dtc_CAN_mainVcu:
                    fault_desc = "Ошибка CAN";
                    break;

				case BmsFaultCodes.dtc_CAN_PM:
					fault_desc = "CAN: Нет связи с Power Manager";
					break;


				default:
                    fault_desc = Code.ToString();
                    break;
            }

            return fault_desc;
        }

    }


    #region ObjectsIndexes

    public enum BmsObjectsIndex_e
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

        // Параметры модуля батареи
        didModMachineState,
        didModMachineSubState,
        didCellsVoltages,
        didModuleTemperatures,

        didModTotalVoltage,
        didModMaxTemperature,
        didModMinTemperature,
        didModMaxCellVoltage,
        didModMinCellVoltage,
        didModDischargeCellsFlag,
        didModInOutState,

        // Параметры батареи
        didBatTotalCurrent,
        didBatTotalVoltage,
        didBatMaxVoltage,
        didBatMinVoltage,
        didBatMaxTemperature,
        didBatMinTemperature,
        didBatMaxCellVoltage,
        didBatMinCellVoltage,
        didBatStateOfCharge,
        didBatEnergy,
        didBatCCL,
        didBatDCL,

        // Параметры мастера
        didMstTotalCurrent,
        didMstTotalVoltage,
        didMstMaxCurrent,
        didMstMinCurrent,
        didMstMaxVoltage,
        didMstMinVoltage,
        didMstMaxTemperature,
        didMstMinTemperature,
        didMstCellMaxVoltage,
        didMstCellMinVoltage,
        didMstStateOfCharge,
        didMstEnergy,
        didMstCCL,
        didMstDCL,

        // Диагностика ошибок
        didFaults_Actual = 100,
        didFaults_History,

        didFaults_FreezeFrame,
    };

    public enum BmsFaultCodes
    {
        dtc_General_EcuConfig = 0,
        dtc_General_EcuSupplyOutOfRange,
        dtc_General_UnexpectedPowerOff,
        dtc_General_Interlock,
        dtc_General_EcuDataTimeNotCorrect,

        // Таймаут CAN
        dtc_CAN_Bms = 10,
        dtc_CAN_generalVcu,
        dtc_CAN_mainVcu,
		dtc_CAN_PM,

		dtc_Mod_Contactor = 20,
        dtc_Mod_CellTempOutOfRange,
        dtc_Mod_MeasuringCircuit,

        dtc_Bat_WrongModNumber = 32,
        dtc_Bat_WrongModState,
        dtc_Bat_OverCurrent,
        dtc_Bat_ModVoltageDiff,
        dtc_Bat_Precharge,
        dtc_Bat_CurrentSensor,

        dtc_Mst_WrongBatNumber = 44,
        dtc_Mst_WrongBatState,
        dtc_Mst_BatVoltageDiff,
        dtc_Mst_CellVoltageOutOfRange,


        dtc_Count
    }



    #endregion

}
