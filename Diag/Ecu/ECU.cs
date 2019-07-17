
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	/// <summary>
	/// Описание параметров диагностируемого блока управления.
	/// </summary>
	public class ECU
	{

		public ECU(EcuModelId modelId, byte DiagAdressId)
		{
			ModelId = modelId;
			Address = DiagAdressId;
		}

		virtual public List<DiagnosticData> GetDiagnosticSets()
		{
			return null;
		}

		virtual public DiagnosticData GetFrzFramesSet()
		{
			return null;
		}

		virtual async public Task<List<ResponseData_ReadDataByIdentifier>> GetEcuInfo()
		{
			return null;
		}

        virtual async public Task<List<ResponseData_ReadDataByIdentifier>> GetEcuTime()
        {
            return null;
        }

        virtual async public Task<bool> ClearFaults()
        {
            return false;
        }

        #region Свойства

        //		protected List<DiagnosticValueSet> Sets { get; set; }

        public byte Address { get; set; }

		public EcuModelId ModelId { get; set; }
		public byte UdsVersion { get; set; }
		public string Model { get; set; }
		public string Hardware { get; set; }
		public UInt16 HwVersion { get; set; }
		public UInt16 FwVersion { get; set; }
		public string SerianNumber { get; set; }
		public string PartNumber { get; set; }
		public string ExtraInfo { get; set; }		
		
//		public string ClassIdString { get { return EcuClassId_String.ToString(ClassId); } }
//		public string ModelIdString { get { return EcuModelId_String.ToString(ModelId); } }

//		public virtual bool IsTimeServer { get { return false; } }
//		public virtual bool IsPowerManager { get { return false; } }

		#endregion

		

		//#region Запросы

		//public async Task<bool> GetInfo()
		//{
		//	List<A_Service> sList;


		//	// Запрос модели
		//	sList = await Global.uds.ReadDataByID(Address, DID.didModelStr);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//		if (s.Response.Count > 0)
		//			Model = s.Response[0].DV.GetStringValue();
		//	}

		//	// Запрос контроллера
		//	sList = await Global.uds.ReadDataByID(Address, DID.didHardwareStr);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//		if (s.Response.Count > 0)
		//			Hardware = s.Response[0].DV.GetStringValue();
		//	}

		//	// Запрос версии HW/FW
		//	sList = await Global.uds.ReadDataByID(Address, DID.didVersion);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;

		//		if (s.Response.Count >= 2)
		//		{
		//			HwVersion = (UInt16)s.Response[0].DV.GetNumericValue();
		//			FwVersion = (UInt16)s.Response[1].DV.GetNumericValue();
		//		}
		//	}

		//	// Запрос серийного номера
		//	sList = await Global.uds.ReadDataByID(Address, DID.didECUSerialNumberDataIdentifier);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//		if (s.Response.Count > 0)
		//			SerianNumber = s.Response[0].DV.GetStringValue();
		//		else
		//			SerianNumber = "";
		//	}

		//	// Запрос артикула
		//	sList = await Global.uds.ReadDataByID(Address, DID.didPartNumberStr);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//		if (s.Response.Count > 0)
		//			PartNumber = s.Response[0].DV.GetStringValue();
		//	}

		//	// Запрос дополнительной информации
		//	sList = await Global.uds.ReadDataByID(Address, DID.didExtraStr);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
		//			return false;

		//		A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//		if (s.Response.Count > 0)
		//			ExtraInfo = s.Response[0].DV.GetStringValue();
		//	}

		//	// Запрос статусов (могут быть не у всех ЭБУ)
		//	sList = await Global.uds.ReadDataByID(Address, DID.didStatus1);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State != A_ServiceState.Timeout && srv.NResult == N_Result.N_OK)
		//		{
		//			A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//			if (s.Response.Count > 0)
		//				ConvertStatuses(1, s.Response[0].DV);
		//		}
		//	}
		//	sList = await Global.uds.ReadDataByID(Address, DID.didStatus2);
		//	if (sList.Count == 1)
		//	{
		//		A_Service srv = sList[0];
		//		if (srv.State != A_ServiceState.Timeout && srv.NResult == N_Result.N_OK)
		//		{
		//			A_Service_ReadDataByIdentifier s = srv as A_Service_ReadDataByIdentifier;
		//			if (s.Response.Count > 0)
		//				ConvertStatuses(2, s.Response[0].DV);
		//		}
		//	}

		//	return true;
		//}

		//#endregion
	}

	// Типы датчиков тока
	public enum CurrentSensorType { DHAB_S34, HASS_50, HASS_100, HASS_200, HASS_300, HASS_400 }
	public static class CurrentSensorType_String
	{
		public static string ToString(CurrentSensorType t)
		{
			switch (t)
			{
				case CurrentSensorType.DHAB_S34:
					return "DHAB S34";
				case CurrentSensorType.HASS_50:
					return "HASS 50";
				case CurrentSensorType.HASS_100:
					return "HASS 100";
				case CurrentSensorType.HASS_200:
					return "HASS 200";
				case CurrentSensorType.HASS_300:
					return "HASS 300";
				case CurrentSensorType.HASS_400:
					return "HASS 400";
			}

			return "Неизвесно";
		}
	}

	// Классы ЭБУ
	public enum EcuModelId
	{
		none = 0,
		bms = 0x10,
		gEcu = 0x11,
		mEcu = 0x12,
	}

	public enum EcuDiagAddress
	{
		NONE = 0,
		GENERAL_ECU_DIAG_ID = 1,
		MAIN_ECU_DIAG_ID	= 10,
		BATTERY_ECU_ID		= 11,
	}


	public class DiagnosticData
	{
		public int DataID { get; set; }
		public string StringDescription { private get; set; }
		public double Factor { get; set; }
		public bool IsSigned { get; set; }
		public List<string> Value;
		public byte NoteNumber { get; set; }

		public string ToString(int NoteNum)
		{
			return StringDescription + " " + NoteNum.ToString();
		}

		public override string ToString()
		{
			return StringDescription;
		}

		Func<int, string> ValueHandler;

		public DiagnosticData(ushort did, string Description, double factor, bool isSigned, byte noteNumber, Func<int, string> Handler)
		{
			DataID = did;
			StringDescription = Description;
			Factor = factor;
			IsSigned = isSigned;
			NoteNumber = noteNumber;
			Value = new List<string>(NoteNumber);
			ValueHandler = Handler;
		}

		public void AddValue(int val)
		{
			if (ValueHandler != null)
			{
				Value.Add(ValueHandler(val));
			}
			else
				Value.Add(val.ToString());
		}
	}

}
