using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	public static class DTC_Category
	{		
		// Описание категорий DTC (3-й младший байт). См. ISO 15031-6 стр.7 и 118
		public enum Code
		{
			// **************************** General Failure Information ***********************************

			dctCat_NoSubtypeInformation = 0x00,         // Нет категории и подтипов для DTC
			dctCat_GeneralElectricalFailure = 0x01,     // Общая электрическая неисправность
			dctCat_GeneralSignalFailure = 0x02,         // Общая неисправность, связанная измерением сигналов
			dctCat_PWM = 0x03,                          // Общая неисправность, связанная с ШИМ
			dctCat_SystemInternalFailure = 0x04,        // Общая неисправность, связанная с внутренней аппаратной или программной проблемой ЭБУ
			dctCat_SystemProgrammingFailure = 0x05,     // Общая неисправность, связанная с калибровками/активацией ЭБУ
			dctCat_AlgorithmBasedFailure = 0x06,        // Общая неисправность, связанная с алгоритмом работы ЭБУ
			dctCat_MechanicalFailure = 0x07,            // Общая неисправность, связанная с механической неисправностью
			dctCat_BusSignalOrMessageFailure = 0x08,    // Общая неисправность, связанная с шиной данных
			dctCat_ComponentFailure = 0x09,             // Общая неисправность, связанная с состоянием/поведением компонентов

			// ********************************************************************************************



			// **************************** General Electrical Failures ***********************************
			// This category includes standard wiring failure modes (i.e. shorts and opens), and direct current (DC) quantities related by Ohm's Law.

			dctCat_CircuitShortToGround = 0x11,
			dctCat_CircuitShortToBattery = 0x12,
			dctCat_CircuitOpen = 0x13,
			dctCat_CircuitShortToGroundOrOpen = 0x14,
			dctCat_CircuitShortToBatteryOrOpen = 0x15,
			dctCat_CircuitVoltageBelowThreshold = 0x16,
			dctCat_CircuitVoltageAboveThreshold = 0x17,
			dctCat_CircuitCurrentBelowThreshold = 0x18,
			dctCat_CircuitCurrentAboveThreshold = 0x19,
			dctCat_CircuitResistanceBelowThreshold = 0x1A,
			dctCat_CircuitResistanceAboveThreshold = 0x1B,
			dctCat_CircuitVoltageOutOfRange = 0x1C,
			dctCat_CircuitCurrentOutOfRange = 0x1D,
			dctCat_CircuitResistanceOutOfRange = 0x1E,
			dctCat_CircuitIntermittent = 0x1F,          // control module momentarily detects one of the conditions defined above, but not long enough to set a specific subtype.

			// ********************************************************************************************



			// ***************************** General Signal Failures **************************************
			// specifies quantities related to amplitude, frequency or rate of change, and wave shape.

			dctCat_SignalAmplitudeLowerMinimum = 0x21,
			dctCat_SignalAmplitudeHigherMaximum = 0x22,
			dctCat_SignalStuckLow = 0x23,                   // Сигнал долгое время остается низким
			dctCat_SignalStuckHigh = 0x24,                  // Сигнал долгое время остается высоким
			dctCat_SignalWaveformFailure = 0x25,
			dctCat_SignalRateOfChangeBelowThreshold = 0x26, // Сигнал меняется слишком медленно
			dctCat_SignalRateOfChangeAboveThreshold = 0x27, // Сигнал меняется слишком быстро
			dctCat_SignalBiasLevelFailure = 0x28,           // Неверное смещение средней точки сигнала и т.п.
			dctCat_SignalInvalid = 0x29,                    // Неверный сигнал
			dctCat_SignalErratic = 0x2F,                    // Signal is momentarily implausible (not long enough for “signal invalid”) or discontinuous.

			// ********************************************************************************************



			// ************************** Pulse Width Modulated Failures **********************************
			// ..............................

			// ********************************************************************************************



			// ***************************** System Internal Failures *************************************

			dctCat_GeneralChecksumFailure = 0x41,           // Неверная контрольная сумма без уточнения типа памяти
			dctCat_GeneralMemoryFailure = 0x42,             // Ошибка памяти без уточнения ее типа
			dctCat_SpecialMemoryFailure = 0x43,             // Ошибка дополнительной памяти без уточнения ее типа
			dctCat_DataMemoryFailure = 0x44,                // Ошибка памяти данных (RAM)
			dctCat_ProgramMemoryFailure = 0x45,             // Ошибка программной памяти (ROM/Flash)
			dctCat_CalibrationMemoryFailure = 0x46,         // Ошибка памяти калибровок/параметров (EEPROM)
			dctCat_WatchdogOrSafetyFailure = 0x47,
			dctCat_SupervisionSoftwareFailure = 0x48,
			dctCat_InternalElectronicFailure = 0x49,        // Повреждение электроники ЭБУ
			dctCat_IncorrectComponentInstalled = 0x4A,      // К ЭБУ подключен неверный (неподдерживаемый) компонент
			dctCat_OverTemperature = 0x4B,                  // Температура ЭБУ за границами рабочего диапазона


			// ********************************************************************************************



			// **************************** System Programming Failures ***********************************

			dctCat_NotProgrammed = 0x51,
			dctCat_NotActivated = 0x52,                     // Некоторые части программы не включены
			dctCat_Deactivated = 0x53,                      // Некоторые части программы отключены
			dctCat_MissingCalibration = 0x54,               // Неверные калибровки для датчиков/актуаторов и т.п.
			dctCat_NotConfigured = 0x55,                    // ЭБУ не сконфигурирован

			// ********************************************************************************************



			// ****************************** Algorithm Based Failures ************************************

			dctCat_SignalCalculationFailure = 0x61,
			dctCat_SignalCompareFailure = 0x62,             // При сравнении 2-х или более сигналов на достоверность
			dctCat_CircuitOrComponentProtectionTimeout = 0x63,  // Если функция управления работает слишком долго
			dctCat_SignalPlausibilityFailure = 0x64,        // Недостоверный сигнал
			dctCat_SignalHasTooFewTransitions = 0x65,
			dctCat_SignalHasTooManyTransitions = 0x66,
			dctCat_SignalIncorrectAfterEvent = 0x67,
			dctCat_EventInformation = 0x68,

			// ********************************************************************************************



			// ********************************* Mechanical Failures **************************************

			dctCat_ActuatorStuck = 0x71,                    // Актуатор/реле/солиноид и т.п. не двигаются
			dctCat_ActuatorStuckOpen = 0x72,                // Актуатор/реле/солиноид и т.п. остается открытым по истечении заданного времени
			dctCat_ActuatorStuckClosed = 0x73,              // Актуатор/реле/солиноид и т.п. остается закрытым по истечении заданного времени
			dctCat_ActuatorSlipping = 0x74,                 // Актуатор/реле/солиноид и т.п. перемещаются слишком медленно
			dctCat_EmergencyPositionNotReachable = 0x75,    // 
			dctCat_WrongMountingPosition = 0x76,            // Неправильная установка компонента
			dctCat_CommandedPositionNotReachable = 0x77,    // Заданная позиция не достигнута
			dctCat_AlignmentOrAdjustmentIncorrect = 0x78,   // Неправильная регулировка компонента
			dctCat_MechanicalLinkageFailure = 0x79,         // Актуатор работает, но чем он управляет - не двигается (оторвано)
			dctCat_FluidLeakOrSealFailure = 0x7A,           // 
			dctCat_LowFluidLevel = 0x7B,                    // Низкий уровень жидкости

			// ********************************************************************************************



			// **************************** Bus Signal / Message Failures *********************************
			// specifies faults related to bus hardware and signal integrity. This category is also used when the
			// physical input for a signal is located in one control module and another control module diagnoses the circuit.

			dctCat_BusInvalidSerialDataReceived = 0x81,
			dctCat_BusAliveOrSequenceCounterIncorrect = 0x82,
			dctCat_BusValueOfSignalProtectionCalculationIncorrect = 0x83,
			dctCat_BusSignalBelowAllowableRange = 0x84,
			dctCat_BusSignalAboveAllowableRange = 0x85,
			dctCat_BusSignalInvalid = 0x86,
			dctCat_BusMissingMessage = 0x87,
			dctCat_BusOff = 0x88,

			// ********************************************************************************************



			// ********************************* Component Failures ***************************************

			dctCat_Parametric = 0x91,                       // Параметры компонента (емкость, сопротивление и т.п.) за пределами рабочего диапазона
			dctCat_PerformanceOrIncorrectOperation = 0x92,  // Производительность компонента за пределами или он работает неправильно
			dctCat_NoOperation = 0x93,                      // Компонент не работает
			dctCat_UnexpectedOperation = 0x94,              // Компонент работает не так как положено
			dctCat_IncorrectAssembly = 0x95,                // Компонент неправильно установлен. Например: неверная полярность, гидравлические трубки перепутаны
			dctCat_ComponentInternalFailure = 0x96,         // Когда "умный" компонент сообщил ЭБУ, что у него неисправность
			dctCat_ComponentOperationObstructedOrBlocked = 0x97,// Работе компонент препятствует преграда. Например, препятствие лучу радара
			dctCat_ComponentOverTemperature = 0x98,         // Перегрев компонента

			// ********************************************************************************************


		}



		public static string ToString(Code val)
		{
			switch (val)
			{
				// General Failure Information

				case Code.dctCat_NoSubtypeInformation:
					return "";
				case Code.dctCat_GeneralElectricalFailure:
					break;
				case Code.dctCat_GeneralSignalFailure:
					break;
				case Code.dctCat_PWM:
					break;
				case Code.dctCat_SystemInternalFailure:
					break;
				case Code.dctCat_SystemProgrammingFailure:
					break;
				case Code.dctCat_AlgorithmBasedFailure:
					break;
				case Code.dctCat_MechanicalFailure:
					break;
				case Code.dctCat_BusSignalOrMessageFailure:
					break;
				case Code.dctCat_ComponentFailure:
					break;

				// General Electrical Failures

				case Code.dctCat_CircuitShortToGround:
					return "Замыкание на минус";
				case Code.dctCat_CircuitShortToBattery:
					return "Замыкание на плюс";
				case Code.dctCat_CircuitOpen:
					return "Обрыв цепи";
				case Code.dctCat_CircuitShortToGroundOrOpen:
					return "Обрыв цепи или замыкание на минус";
				case Code.dctCat_CircuitShortToBatteryOrOpen:
					return "Обрыв цепи или замыкание на плюс";
				case Code.dctCat_CircuitVoltageBelowThreshold:
					return "Напряжение ниже установленного порога";
				case Code.dctCat_CircuitVoltageAboveThreshold:
					return "Напряжение выше установленного порога";
				case Code.dctCat_CircuitCurrentBelowThreshold:
					return "Ток ниже установленного порога";
				case Code.dctCat_CircuitCurrentAboveThreshold:
					return "Ток выше установленного порога";
				case Code.dctCat_CircuitResistanceBelowThreshold:
					return "Слишком низкое сопротивление";
				case Code.dctCat_CircuitResistanceAboveThreshold:
					return "Слишком высокое сопротивление";
				case Code.dctCat_CircuitVoltageOutOfRange:
					return "Напряжение за пределами рабочего диапазона";
				case Code.dctCat_CircuitCurrentOutOfRange:
					return "Ток за пределами рабочего диапазона";
				case Code.dctCat_CircuitResistanceOutOfRange:
					return "Сопротивление за пределами рабочего диапазона";
				case Code.dctCat_CircuitIntermittent:
					break;

				// General Signal Failures


				case Code.dctCat_SignalAmplitudeLowerMinimum:
					break;
				case Code.dctCat_SignalAmplitudeHigherMaximum:
					break;
				case Code.dctCat_SignalStuckLow:
					return "Сигнал долгое время остается низким";
				case Code.dctCat_SignalStuckHigh:
					return "Сигнал долгое время остается высоким";
				case Code.dctCat_SignalWaveformFailure:
					break;
				case Code.dctCat_SignalRateOfChangeBelowThreshold:
					break;
				case Code.dctCat_SignalRateOfChangeAboveThreshold:
					break;
				case Code.dctCat_SignalBiasLevelFailure:
                    return "Смещение средней точки";
					
				case Code.dctCat_SignalInvalid:
                    return "Сигнал недостоверен";
				case Code.dctCat_SignalErratic:
					break;

				
				// System Internal Failures


				case Code.dctCat_GeneralChecksumFailure:
					break;
				case Code.dctCat_GeneralMemoryFailure:
					break;
				case Code.dctCat_SpecialMemoryFailure:
					break;
				case Code.dctCat_DataMemoryFailure:
					break;
				case Code.dctCat_ProgramMemoryFailure:
					return "Ошибка программной памяти (ROM/Flash)";
				case Code.dctCat_CalibrationMemoryFailure:
					return "Ошибка памяти кодировок/параметров (EEPROM)";
				case Code.dctCat_WatchdogOrSafetyFailure:
					break;
				case Code.dctCat_SupervisionSoftwareFailure:
					break;
				case Code.dctCat_InternalElectronicFailure:
					break;
				case Code.dctCat_IncorrectComponentInstalled:
					break;
				case Code.dctCat_OverTemperature:
					break;


				// System Programming Failures


				case Code.dctCat_NotProgrammed:
					break;
				case Code.dctCat_NotActivated:
					break;
				case Code.dctCat_Deactivated:
					break;
				case Code.dctCat_MissingCalibration:
					break;
				case Code.dctCat_NotConfigured:
					break;


				// Algorithm Based Failures


				case Code.dctCat_SignalCalculationFailure:
					break;
				case Code.dctCat_SignalCompareFailure:
					break;
				case Code.dctCat_CircuitOrComponentProtectionTimeout:
					break;
				case Code.dctCat_SignalPlausibilityFailure:
					break;
				case Code.dctCat_SignalHasTooFewTransitions:
					break;
				case Code.dctCat_SignalHasTooManyTransitions:
					break;
				case Code.dctCat_SignalIncorrectAfterEvent:
					break;
				case Code.dctCat_EventInformation:
					break;


				// Mechanical Failures


				case Code.dctCat_ActuatorStuck:
					break;
				case Code.dctCat_ActuatorStuckOpen:
					return "Остается открытым по истечении заданного времени";
				case Code.dctCat_ActuatorStuckClosed:
					return "Остается закрытым по истечении заданного времени";
				case Code.dctCat_ActuatorSlipping:
					break;
				case Code.dctCat_EmergencyPositionNotReachable:
					break;
				case Code.dctCat_WrongMountingPosition:
					break;
				case Code.dctCat_CommandedPositionNotReachable:
					return "Заданная позиция не достигнута";
				case Code.dctCat_AlignmentOrAdjustmentIncorrect:
					break;
				case Code.dctCat_MechanicalLinkageFailure:
					break;
				case Code.dctCat_FluidLeakOrSealFailure:
					break;
				case Code.dctCat_LowFluidLevel:
					break;


				// Bus Signal / Message Failures


				case Code.dctCat_BusInvalidSerialDataReceived:
					break;
				case Code.dctCat_BusAliveOrSequenceCounterIncorrect:
					break;
				case Code.dctCat_BusValueOfSignalProtectionCalculationIncorrect:
					break;
				case Code.dctCat_BusSignalBelowAllowableRange:
					break;
				case Code.dctCat_BusSignalAboveAllowableRange:
					break;
				case Code.dctCat_BusSignalInvalid:
					break;
				case Code.dctCat_BusMissingMessage:
					break;
				case Code.dctCat_BusOff:
					break;


				// Component Failures


				case Code.dctCat_Parametric:
					break;
				case Code.dctCat_PerformanceOrIncorrectOperation:
					break;
				case Code.dctCat_NoOperation:
					break;
				case Code.dctCat_UnexpectedOperation:
					break;
				case Code.dctCat_IncorrectAssembly:
					break;
				case Code.dctCat_ComponentInternalFailure:
					break;
				case Code.dctCat_ComponentOperationObstructedOrBlocked:
					break;
				case Code.dctCat_ComponentOverTemperature:
					break;
			}

			return ((int)val).ToString();
		}
	}
}
