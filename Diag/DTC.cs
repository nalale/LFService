using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MFService
{
	public class DTC
	{

		#region Коды неисправностей

		// Код DTC состоит из 3-х байт(например P1999-FF) - ISO 15031-6 стр.7, 118

		// 1 буква обозначения типа оборудования:
		// P - Powertrain - собственно двигатель, силовая установка, силовой агрегат.
		// B - Body - кузов, устройства, связанные с кузовом (охрана, замки, свет и прочее).
		// C - Chassis - шасси, ходовая часть, тормоза, АБС, системы управления (рулевое, антипробуксовочная система, антиблокировочная система или ESP или IVD).
		// U - Network - бортовая сеть, проблемы, связанные имено с самой сетью.
		enum dtcGroup { DTC_GROUP_P, DTC_GROUP_C, DTC_GROUP_B, DTC_GROUP_U }

		// 2 цифра типа кода:
		// 0 - Код, обусловленный стандартом ISO/SAE.
		// 1 - Код назначается произоводителем марки автомобиля.
		// 2 - Для Powertrain - код, обусловленный стандартом ISO/SAE. Для остальных код назначается произоводителем марки автомобиля.
		// 3 - Код, обусловленный стандартом ISO/SAE.
		enum dtcType { DTC_TYPE_ISO, DTC_TYPE_MAN }

		// только для P: 3 цифра - проблемная область (место). От 0 до F
		enum dtcAreaP { DTC_AREA_P_GENERAL1, DTC_AREA_P_GENERAL2, DTC_AREA_P_VCU, DCT_AREA_P_BATTERY, DCT_AREA_P_HV, }
		
		// только для C: 3 цифра - проблемная область (место). От 0 до F
		enum dtcAreaC { DTC_AREA_C_GENERAL1, DCT_AREA_C_ASPU, }

		// только для B: 3 цифра - проблемная область (место). От 0 до F
		enum dtcAreaB { DTC_AREA_B_GENERAL1, DTC_AREA_B_ISO, };

		// 4-5 цифры - код неисправности от 0x00 до 0xFF
		// 6-7 цифры - это категория неисправности (см. dtcCategory_e)



		// Код DTC без категории неисправности (2 байта). 3-й младший байт - это значение из dtcCategory_e и заполняется динамически (см. dtcItem_t).
		const UInt16 DTC_CODE_P = ((int)dtcGroup.DTC_GROUP_P << 14) + ((int)dtcType.DTC_TYPE_MAN << 12);
		const UInt16 DTC_CODE_C = ((int)dtcGroup.DTC_GROUP_C << 14) + ((int)dtcType.DTC_TYPE_MAN << 12);
		const UInt16 DTC_CODE_B = ((int)dtcGroup.DTC_GROUP_B << 14) + ((int)dtcType.DTC_TYPE_MAN << 12);

		// DTC общих кодов
		const UInt16 DTC_GENERAL = DTC_CODE_P + ((int)dtcAreaP.DTC_AREA_P_GENERAL1 << 8);
		// DTC для модулей управление двигателем
		const UInt16 DTC_VCU = DTC_CODE_P + ((int)dtcAreaP.DTC_AREA_P_VCU << 8);
		// DTC для батарейных модулей
		const UInt16 DTC_BATTERY = DTC_CODE_P + ((int)dtcAreaP.DCT_AREA_P_BATTERY << 8);
		// DTC для HV модулей
		const UInt16 DTC_HV = DTC_CODE_P + ((int)dtcAreaP.DCT_AREA_P_HV << 8);
		// DTC для ASPU
		const UInt16 DTC_ASPU = DTC_CODE_C + ((int)dtcAreaC.DCT_AREA_C_ASPU << 8);
		// DTC для Iso
		const UInt16 DTC_ISO = DTC_CODE_B + ((int)dtcAreaB.DTC_AREA_B_ISO << 8);


		// ******************************** Коды неисправностей *******************************************

		public enum Codes
		{
			// ********************************************************************************************
			// Общие неисправности
			dtc_General = DTC_GENERAL,
			// Неисправности ЭБУ
			dtcGeneral_Ecu,                         // + категория из System Internal Failures или System Programming Failures (dtcCategory_e)

			// Неисправность CAN +dctCat_BusOff и т.п.
			dct_General_VcuCan,
			dct_General_InverterCan,
			dct_General_AspuCan,
			dct_General_BatteryCan,
			dct_General_HV1Can,
			dct_General_ChargerCan,
			dtc_General_J1939Can,                   // Нет связи со штатной системой
			dtc_General_UfcCan,
			dtc_General_HVSensorCan,
			dtc_General_IsoDetectorCan,
			dtc_General_EvCan,
			dtc_General_FanCan,

			// ......................
			// Резерв для CAN
			dtcGeneral_CAN = dct_General_VcuCan + 28,

			// Некорректные значения даты и времени
			dtcGeneral_EcuDataTimeNotCorrect,
			// Напряжения питания за пределами рабочего диапазона +dctCat_CircuitVoltageBelowThreshold и т.п.
			dtcGeneral_EcuSupplyOutOfRange,
			// Неожиданное отключение питания
			dtcGeneral_UnexpectedPowerOff,
			// Ошибка интерлок
			dtc_General_Interlock,

			dtc_TemperatureSensor1, dtc_TemperatureSensor2, dtc_TemperatureSensor3, dtc_TemperatureSensor4,
			dtc_TemperatureSensor5, dtc_TemperatureSensor6, dtc_TemperatureSensor7, dtc_TemperatureSensor8,
			dtc_TemperatureSensor9, dtc_TemperatureSensor10, dtc_TemperatureSensor11, dtc_TemperatureSensor12,


			// ************************ Неисправности управления двигателем *****************************

			dtc_Vcu = DTC_VCU,

			// Левая сторона
			dtc_Vcu_MotorOverCurrentLeft,           // Первышение тока статора двигателя
			dtc_Vcu_MotorOverSpeedLeft,             // Превышение допустимой скорости двигателя
			dtc_Vcu_MotorOverTempLeft,              // Перегрев двигателя
			dtc_Vcu_InverterHVOutOfRangeLeft,       // Напряжение на инверторе за пределами рабочего диапазона
			dtc_Vcu_InverterOverTempLeft,           // Перегрев инвертора
			dtc_Vcu_InverterCanOffLeft,             // Инвертор обнаружил неустойчивую связь по CAN
			dtc_Vcu_InverterFaultLeft,              // Ошибка инвертора

			// Правая сторона
			dtc_Vcu_MotorOverCurrentRight,          // Первышение тока статора двигателя
			dtc_Vcu_MotorOverSpeedRight,            // Превышение допустимой скорости двигателя
			dtc_Vcu_MotorOverTempRight,             // Перегрев двигателя
			dtc_Vcu_InverterHVOutOfRangeRight,      // Напряжение на инверторе за пределами рабочего диапазона
			dtc_Vcu_InverterOverTempRight,          // Перегрев инвертора
			dtc_Vcu_InverterCanOffRight,            // Инвертор обнаружил неустойчивую связь по CAN
			dtc_Vcu_InverterFaultRight,             // Ошибка инвертора

			dtc_Vcu_IsoError,                       // Неисправность изоляции

			dtc_Vcu_ChargerCircuit,                 // Неисправность в цепи зарядного устройства
			dtc_Vcu_ChargerCanError,                // Зарядное устройство обнаружило неустойчивую связь по CAN

			dtc_Vcu_Iso2Error,                      // Неисправность изоляции
						
			dtc_Vcu_FastchargingFault,              // Неисправность в процессе быстрой зарядки

			dtc_EVCC_General,
			dtc_EVCC_PWM,   // при старте по таймауту
			dtc_EVCC_SLAC,
			dtc_EVCC_CableCheck,
			dtc_EVCC_Precharge,
			dtc_EVCC_Charge,
			dtc_EVCC_DutyCycle,
			dtc_EVCC_PP,

			// ****************************** Неисправности батареи ***********************************

			dtc_Battery = DTC_BATTERY,

			// Низкий уровень заряда
			dtc_Battery_LowSOC,
			// Перегрузка по току
			dtc_Battery_CurrentOverload,
			// Напряжение на ячейках за пределами рабочего диапазона
			dtc_Battery_CellVoltageOutOfRange,
			// Перегрев/переохлаждение батареи
			dtc_Battery_TemperatureOutOfRange,
			// Неисправность в цепи датчика тока
			dtc_Battery_CurrentSensor,              // + dctCat_CircuitShortToGroundOrOpen, dctCat_CircuitShortToBattery, dctCat_CircuitVoltageAboveThreshold, dctCat_CircuitCurrentBelowThreshold
													// Предзаряд
			dtc_Battery_Precharge,                      // предзаряд + dctCat_CircuitShortToGround
														// Неверное количество аккумуляторных модулей
			dtc_Battery_WrongModuleCount,
			// Неисправность контактора
			dtc_Battery_ContactorFeedback,
			// Мастер не в сети
			dtc_Battery_MasterTimeout,

			// Неисправности мастера

			// Slave в ошибке
			dtc_BatteryMaster_SlaveFault,
			// Не все батареи в сети
			dtc_BatteryMaster_WrongBatteryCount,
			// Большая разница напряжений батарей
			dtc_BatteryMaster_VoltageDisbalance,
			// Большая разница токов батарей
			dtc_BatteryMaster_CurrentDisbalance,
			// VCU не в сети		
			//dtc_BatteryMaster_VCU_TimeOut,

			// ********************************************************************************************


			// ***************************** Неисправности ASPU *******************************************
			dtc_Aspu = DTC_ASPU,
			dtc_Aspu_PedalAcc,
			dtc_Aspu_Retarder,
			dtc_Aspu_Selector,

			// ********************************************************************************************


			// ***************************** Неисправности HV *******************************************
			dtc_HV = DTC_HV,
			dtc_HV_Pantograph,                  // Ошибка привода пантографа  +dctCat_CommandedPositionNotReachable
			dtc_HV_CurrentSensor1,              // + dctCat_CircuitShortToGroundOrOpen, dctCat_CircuitShortToBattery, dctCat_CircuitVoltageAboveThreshold, dctCat_CircuitCurrentBelowThreshold
			dtc_HV_CurrentSensor2,              // + dctCat_CircuitShortToGroundOrOpen, dctCat_CircuitShortToBattery, dctCat_CircuitVoltageAboveThreshold, dctCat_CircuitCurrentBelowThreshold
			dtc_HV_ContactorFeedback,           // Ошибка обратной связи контактора  +dctCat_ActuatorStuckClosed, dctCat_ActuatorStuckOpen

			dtc_Iso = DTC_ISO,
			dtc_Iso_Connection,                     // Обрыв в цепи - +dctCat_CircuitShortToGroundOrOpen, dctCat_CircuitShortToBatteryOrOpen, dctCat_GeneralElectricalFailure
			dtc_Iso_ExcVoltage,                     // Напряжение возбуждения за пределами допустимого диапазона +dctCat_CircuitVoltageOutOfRange
			dtc_Iso_PwrSupply,                      // Напряжение питания за пределами допустимого диапазона +dctCat_CircuitVoltageOutOfRange

		}






		public enum DTCFormatIdentifier
		{
			SAE_J2012_DA_DTCFormat_00 = 0,
			ISO_14229_1_DTCFormat,
			SAE_J1939_73_DTCFormat,
			ISO_11992_4_DTCFormat,
			SAE_J2012_DA_DTCFormat_04
		}


		#endregion


		#region Static

		public static string ToString(Codes val)
		{
			switch (val)
			{
				// Общие
				case Codes.dtcGeneral_Ecu:
					return "Неисправность ЭБУ";

				
			}

			return "Неизвесно";
		}

		// Эта функция преобразует в текст только 2 старших байта (без категории)
		public static string ToAlphanumeric(Codes val)
		{
			string res = "";

			UInt16 cod = (UInt16)val;

			// 1-й символ
			int tmp = cod >> 14;
			if (tmp == (int)dtcGroup.DTC_GROUP_P)
				res += "P";
			else if (tmp == (int)dtcGroup.DTC_GROUP_C)
				res += "C";
			else if (tmp == (int)dtcGroup.DTC_GROUP_B)
				res += "B";
			else if (tmp == (int)dtcGroup.DTC_GROUP_U)
				res += "U";

			// 2-й символ
			tmp = (cod >> 12) & 3;
			res += tmp.ToString();

			// 3-й символ
			tmp = (cod >> 8) & 0x0F;
			res += tmp.ToString("X1");

			// 4-й символ
			tmp = (cod >> 4) & 0x0F;
			res += tmp.ToString("X1");

			// 5-й символ
			tmp = cod & 0x0F;
			res += tmp.ToString("X1");


			return res;
		}


		//public static string Stat

		#endregion

	}


	//[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	public class dtcItem
	{
		public dtcItem()
		{
			FRZF = new List<DiagnosticData>();
		}

		public UInt16 Code;
		public byte Category;
		public byte Status;

		public List<DiagnosticData> FRZF;
	}
}
