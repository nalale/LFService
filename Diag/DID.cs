using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFService
{
	//TODO: Написать функцию проверки, чтобы исключить дублирование DID параметров.
	// См. ISO 14229-1, стр.345 (C.1 DID parameter definitions)
	//NOTE: Расположение параметров не менять!!!
	public enum DID
	{
		didReserved = 0,

		// 0x0100 – 0xA5FF
		VehicleManufacturerSpecific = 0x100,

		// ***************************** Общие параметры ****************************
		didGeneral = VehicleManufacturerSpecific + 1,

		// Каждый ЭБУ может содержать до 10-ти различных статусных состояний, которые могут быть только считаны
		didStatus1,
		didStatus2,
		didStatus3,
		didStatus4,
		didStatus5,
		didStatus6,
		didStatus7,
		didStatus8,
		didStatus9,
		didStatus10,

		// Каждый ЭБУ может содержать до 10-ти различных наборов данных (например, настройки), которые могут быть считаны или записаны
		didData1,
		didData2,
		didData3,
		didData4,
		didData5,
		didData6,
		didData7,
		didData8,
		didData9,
		didData10,

		// Статическая информация об ЭБУ
		didClassModel,                  // Класс ЭБУ и версия диагностического протокола
		didModelStr,                    // Модель - строка до 20 символов. Название ЭБУ (N16, UMKA ...)
		didHardwareStr,                 // Контроллер (железо) - строка до 20 символов (BMU v2.2 ...). Разные платы могут выполнять одни и теже функции ЭБУ (см. Класс)
		didVersion,                     // Версия прошивки ЭБУ
		//didSerialNumber,                // Серийный номер ЭБУ - строка до 20 символов
		didPartNumberStr,               // Номер ЭБУ для заказа - строка до 20 символов
		didExtraStr,                    // Дополнительная информация - строка до 20 символов
		didConfigStructIndex,           // Номер структуры данных для конфигурации ЭБУ. У разных ЭБУ одного класса могут быть разные параметры для конфигурации.
		didStaticReserved1,
		didStaticReserved2,
		didStaticReserved3,
		didStaticReserved4,
		didStaticReserved5,

		// Общие параметры ЭБУ
		didEcuSupply,                   // Напряжение питания ЭБУ в мВ
		didEcuTemperature,              // Температура ЭБУ
		didAirTemperature,              // Температура окружающей среды
		didDateTime,                    // Текущие дата и время
		didLifetime,                    // Время во включенном состоянии за весь жизненный цикл в секундах.
		didPowerOnTime,                 // Время работы с последнего включения в секундах
		didAfterResetTime,              // Время работы с последнего ресета в секундах
		didResetSource,                 // Источник последнего ресета
		didWorkState,                   // Текущий режим работы
		// ......................
		// Резерв 
		didGeneral_ = didEcuSupply + 20,


		didCan1RxErrorCount,            // Количество ошибок приема с последнего включения
		didCan1TxErrorCount,            // Количество ошибок передачи с последнего включения
		didCan1BusLoad,                 // Загрузка шины в %
		didCan2RxErrorCount,
		didCan2TxErrorCount,
		didCan2BusLoad,
		didCan3RxErrorCount,
		didCan3TxErrorCount,
		didCan3BusLoad,

		didAccessTimeout,               // Время блокировки входа в закрытую область в секундах
		didFRZF_General,                // Стандартный стоп-кадр для ЭБУ
		didFRZF_Vehicle,                // Стандартный стоп-кадр для ЭБУ в транспортном средстве
		didIO1,                         // Состояние дискретных портов ввода вывода
		didIO2,
		didSensorVoltage1,              // Напряжение на 1-м измерительном канале АЦП
		didSensorVoltage2,              // Напряжение на 2-м измерительном канале АЦП
		didSensorVoltage3,              // Напряжение на 3-м измерительном канале АЦП
		didSensorVoltage4,              // Напряжение на 4-м измерительном канале АЦП
		didSensorVoltage5,              // Напряжение на 5-м измерительном канале АЦП
		didSensorVoltage6,              // Напряжение на 6-м измерительном канале АЦП
		didSensorVoltage7,              // Напряжение на 7-м измерительном канале АЦП
		didSensorVoltage8,              // Напряжение на 8-м измерительном канале АЦП

		didVehicleSpeed,                // Скорость ТС в км/ч
		didStartStatDate,               // Дата, с которой ведется запись данных статистики
		didGeneralNumber8,              // Номер какого-либо параметра (1 байт)
		didGeneralNumber16,             // Номер какого-либо параметра (2 байта)
		didGeneralNumber32,             // Номер какого-либо параметра (4 байта)
		didCurrentSensor1,              // Ток кокого-либо датчика в амперах * 10
		didCurrentSensor2,
		didCurrentSensor3,
		didCurrentSensor4,
		didCurrentSensor5,
		didCurrentSensor6,

		didVcc1,                        // Напряжение питания низковольтных цепей платы
		didVcc2,
		didVcc3,
		didVcc4,
		didVcc5,
		didVcc6,
		didVcc7,
		didVcc8,

		// Датчики температуры
		didTemperatureSensor1, didTemperatureSensor2, didTemperatureSensor3, didTemperatureSensor4,
		didTemperatureSensor5, didTemperatureSensor6, didTemperatureSensor7, didTemperatureSensor8,
		didTemperatureSensor9, didTemperatureSensor10, didTemperatureSensor11, didTemperatureSensor12,



		// ****************************** Параметры VCU *****************************
		didVCU = didGeneral + 0x200,
		didVCU_MotorLeftTargetTorque,                   // Требуемый крутящий момент левого ЭМ в процентах (Нагрузка)
		didVCU_MotorLeftActualTorque,                   // Текущий крутящий момент левого ЭМ в процентах
		didVCU_MotorLeftActualVelocity,                 // Текущая скорость левого ЭМ в об/мин
		didVCU_MotorLeftCurrentAC,                      // Текущий ток статора левого ЭМ в амперах
		didVCU_MotorLeftVoltageAC,                      // Текущее фазное напряжение статора левого ЭМ в вольтах
		didVCU_MotorLeftTemperature,                    // Температура обмотки статора левого ЭМ в градусах
		didVCU_InverterLeftStatusWord,                  // Режим работы инвертора левого ЭМ
		didVCU_InverterLeftTemperature,                 // Температура инвертора левого ЭМ	в градусах
		didVCU_InverterLeftTargetIq,                          // Компонента вектора напряжения левого ЭМ вдоль оси q в вольтах
		didVCU_InverterLeftTargetId,                          // Компонента вектора напряжения левого ЭМ вдоль оси d в вольтах
		didVCU_InverterLeftIq,                          // Компонента вектора тока левого ЭМ вдоль оси q в вольтах
		didVCU_InverterLeftId,                          // Компонента вектора тока левого ЭМ вдоль оси d в вольтах
		didVCU_InverterLeftCurrentDC,                   // Текущий ток батареи к инвертору левого ЭМ в вольтах
		didVCU_InverterLeftVoltageDC,                   // Текущее напряжение батареи на левом инверторе
		didVCU_InverterLeftFaultNumber,                 // Код ошибки левого инвертора

		didVCU_MotorRightTargetTorque,                  // Требуемый крутящий момент правого ЭМ в процентах (Нагрузка)
		didVCU_MotorRightActualTorque,                  // Текущий крутящий момент правого ЭМ в процентах
		didVCU_MotorRightActualVelocity,                // Текущая скорость правого ЭМ в об/мин
		didVCU_MotorRightCurrentAC,                     // Текущий ток статора правого ЭМ в амперах
		didVCU_MotorRightVoltageAC,                     // Текущее фазное напряжение статора правого ЭМ в вольтах
		didVCU_MotorRightTemperature,                   // Температура обмотки статора правого ЭМ в градусах
		didVCU_InverterRightStatusWord,                 // Режим работы инвертора правого ЭМ
		didVCU_InverterRightTemperature,                // Температура инвертора правого ЭМ	в градусах
		didVCU_InverterRightTargetIq,                         // Компонента вектора напряжения правого ЭМ вдоль оси q в вольтах
		didVCU_InverterRightTargetId,                         // Компонента вектора напряжения правого ЭМ вдоль оси d в вольтах
		didVCU_InverterRightIq,                         // Компонента вектора тока левого ЭМ вдоль оси q в вольтах
		didVCU_InverterRightId,                         // Компонента вектора тока левого ЭМ вдоль оси d в вольтах
		didVCU_InverterRightCurrentDC,                  // Текущий ток батареи к инвертору правого ЭМ в вольтах
		didVCU_InverterRightVoltageDC,                  // Текущее напряжение батареи на правом инверторе
		didVCU_InverterRightFaultNumber,                // Код ошибки правого инвертора

		didVCU_DriveModeSelector,
		didVCU_Limit,
		didVCU_RangeReserve,                            // Запас хода в км
		didVCU_AverageConsumption,                      // Удельные средний расход в кВт*ч/км
		didVCU_SpecificConsumption,                     // Удельные расход в кВт*ч/км
		didVCU_RegenerateKoef,                          // Коэффициент рекуперации



		// ***************************** Параметры батарей **************************
		didPower = didVCU + 0x200,
		// Батарейная система в целом
		didMaster_LifetimeDischargeEnergy,  // Количество энергии отданной за все время работы в Ah * 10
		didMaster_SOC,                      // Уровень заряда
		didMaster_TotalCurrent,             // Суммарный ток всей системы
		didMaster_TotalVoltage,             // Суммарное напряжение всей системы
		didMaster_CCL,                      // Ограничение тока заряда в амперах			
		didMaster_DCL,                      // Ограничение тока разряда в амперах
		didMaster_OnlineBatteriesFlags,     // Флаги батарей в сети
		didMaster_OnlineBatteriesCount,     // Сколько всего батарей в сети
		didMaster_MaxBatteryCurrent,        // Максимальный ток батареи в амперах * 10. +Номер батареи с максимальным током
		didMaster_MinBatteryCurrent,        // Минимальный ток батареи в амперах * 10. +Номер батареи с минимальным током
		didMaster_MaxBatteryVoltage,        // Максимальное напряжение из всех батарей в вольтах. +Номер батареи с максимальным напряжением
		didMaster_MinBatteryVoltage,        // Минимальное напряжение из всех батарей. +Номер батареи с минимальным напряжением
		didMaster_BalancingVoltage,         // Напряжение балансировки в мВ
		didMaster_MaxCellVoltage,           // Напряжение наиболее заряженной ячейки в мВ. +Номер/модуль/батарея ячейки
		didMaster_MinCellVoltage,           // Напряжение наименее заряженной ячейки в мВ. +Номер/модуль/батарея ячейки
		didMaster_AvgCellVoltage,           // Среднее значение напряжения на ячейках системы в мВ
		didMaster_MaxModuleTemperature,     // Максимальная температура модуля. +Номер/батарея модуля
		didMaster_MinModuleTemperature,     // Минимальная температура модуля. +Номер/батарея модуля

		// Батарея
		didBattery_Voltage,                 // Напряжение батареи в вольтах
		didBattery_Current,                 // Ток батареи
		didBattery_ModulesCount,            // Количество модулей в батарее
		didBattery_OnlineModulesFlags,      // Переменная, хранящая статус модулей (есть/нет в CAN)
		didBattery_LastPrechargeDuration,   // Длительность последнего предзаряда в мс
		didBattery_LastPrechargeMaxCurrent, // Максимальный ток последнего предзаряда в амперах * 10
		didBattery_MaxCellVoltage,          // Напряжение наиболее заряженной ячейки в мВ. +Номер ячейки и модуля
		didBattery_MinCellVoltage,          // Напряжение наименее заряженной ячейки в мВ. +Номер ячейки и модуля
		didBattery_AvgCellVoltage,          // Среднее значение напряжения на ячейках батареи в мВ
		didBattery_MaxModuleTemperature,    // Максимальная температура модуля. +Номер модуля
		didBattery_MinModuleTemperature,    // Минимальная температура модуля. +Номер модуля
		didBattery_AvgModuleTemperature,    // Средняя температура модулей

		didCellVoltageModule0_3,            // Напряжение ячеек модулей 0-3
		didCellVoltageModule4_7,            // Напряжение ячеек модулей 4-7
		didCellVoltageModule8_11,           // Напряжение ячеек модулей 8-11
		didCellVoltageModule12_15,          // Напряжение ячеек модулей 12-15
		didCellVoltageModule16_19,          // Напряжение ячеек модулей 16-19
		didCellVoltageModule20_21,          // Напряжение ячеек модулей 20-21

		// Статистика работы батарей
		didBattery_LastMaxCloseCurrent,     // Максимальный ток последнего включения контакторов
		didBattery_MaxCloseCurrent,         // Максимальный ток за все время включения контакторов
		didBattery_CloseOverCurrentCount,   // Количество включений контакторов под током	
		didBattery_LastMaxOpenCurrent,      // Максимальный ток последнего отключения контакторов
		didBattery_MaxOpenCurrent,          // Максимальный ток за все время отключения контакторов
		didBattery_OpenOverCurrentCount,    // Количество отключений контакторов под током
		didBattery_MaxOpenCurrent_Charge,   // Максимальный ток за все время отключения контакторов при зарядке
		didBattery_OpenOverCurrentCount_Charge,// Количество отключений контакторов под током при зарядке


		// Добавить
		// Статусы:
		// - Cool/Heat
		// - Master/Slave
		// - Контакторы + обратная связь
		// - 

		// ASPU
		didAspu = didPower + 0x100,
		didAspuPedalAcc,
		didAspuRetarder,
		didAspuDriveMode,
		didAspuDcDcCurrentOut,

		// HV
		didHV = didAspu + 0x100,
		didHvDriveCurrent,              // Ток привода пантографа в А * 10
		didHvDriveCommand,              // Команда привода пантографа, 0 - стоп, 1 - вверх, 2 - вниз
		didHvChargingCurrent,           // Ток заряда в А * 10

		didIsoStatus,
		didIsoErrors,
		didIsoIsolation,                // Изоляция Ом/В
		didIsoIsolationUncertainity,    // Неопределенность измерения изоляции в %
		didIsoStoredEnergy,             // Сохраненная энергия в емкости между батареей и шасси в мДж
		didIsoCapUncertainity,          // Неопределенность измерения емкости в %
		didIsoRp,                       // Сопротивление изоляции между плюсом и шасси в кОм
		didIsoRn,                       // Сопротивление изоляции между минусом и шасси в кОм
		didIsoCp,                       // Емкость изоляции между плюсом и шасси в нФ
		didIsoCn,                       // Емкость изоляции между минусом и шасси в нФ
		didIsoVp,                       // Напряжение между плюсом и шасси в вольтах
		didIsoVn,                       // Напряжение между минусом и шасси в вольтах
		didIsoVb,                       // Напряжение батареи в вольтах
		didIsoVmax,                     // Максимальное напряжение батареи за все время работы в вольтах

		
		// Разное
		didMisc = didHV + 0x50,
		didAirPressure1,                // Давление воздуха в 1-м контуре в kPa/8
		didAirPressure2,                // Давление воздуха во 2-м контуре в kPa/8
		didCharger_ErrorFlags,          // Флаг ошибок бортовой зарядки
		didSafeDataCrcOk,
		didSafeDataCorrect,
		// значения общего пользования
		didGenVal1, didGenVal2, didGenVal3, didGenVal4, didGenVal5, didGenVal6, didGenVal7, didGenVal8, didGenVal9, didGenVal10,
		didGenVal11, didGenVal12, didGenVal13, didGenVal14, didGenVal15, didGenVal16, didGenVal17, didGenVal18, didGenVal19, didGenVal20,



		// PLC
		didPlc = didMisc + 0x100,
		didPlcDcTemperature,
		didPlcAcTemperature,
		didPlcContactorVoltage,
		didPlcPlugPresentStatus,
		didPlcInletMotorStatus,
		didPlcCP_DutyCycle,
		didPlcCP_PwmVoltage,
		didPlcStateMachineSt,
		didEVSEMinCurrent,
		didPlcChargePermission,

		// DEBUG
		didDebug = 0xF000,
		didDbgReset,
		didDbgLoc,

		// Стандартные DID:

		// Текущая диагностическая сессия (см. A_DiagnosticSession)
		didActiveDiagnosticSessionDataIdentifier = 0xF186,
		// Дата производства ЭБУ. Формат ASCII: Year, Month, Day
		didECUManufacturingDateDataIdentifier = 0xF18B,
		// Серийный номер ЭБУ - строка до 20 символов
		didECUSerialNumberDataIdentifier = 0xF18C,
		// VIN
		didVINDataIdentifier = 0xF190,
		// Версия HW
		didsystemSupplierECUHardwareVersionNumberDataIdentifier = 0xF193,
		// Версия FW
		didSystemSupplierECUSoftwareVersionNumberDataIdentifier = 0xF195,
		// Номер мастерской или диагностического ПО, которая/которым последний раз программировали ЭБУ
		didrepairShopCodeOrTesterSerialNumberDataIdentifier = 0xF198,
		// Дата, когда последний раз программировался ЭБУ. Формат ASCII: Year, Month, Day
		didProgrammingDateDataIdentifier = 0xF199,
		// Номер мастерской или диагностического ПО, которая/которым последний раз меняли настройки ЭБУ
		didCalibrationRepairShopCodeOrCalibrationEquipmentSerialNumberDataIdentifier = 0xF19A,
		// Дата, когда последний раз меняли настройки ЭБУ. Формат ASCII: Year, Month, Day
		didCalibrationDateDataIdentifier = 0xF19B,
		// Версия диагностического ПО, которым последний раз меняли настройки ЭБУ
		didCalibrationEquipmentSoftwareNumberDataIdentifier = 0xF19C,
		// Дата, когда ЭБУ был установлен в ТС. Формат ASCII: Year, Month, Day
		didECUInstallationDateDataIdentifier = 0xF19D,
		// Версия UDS протокола. См. ISO 14229-1, стр.360 (Table C.11 — Coding of UDS version number DID 0xFF00)
		didUDSVersionDataIdentifier = 0xFF00,
	}
}
