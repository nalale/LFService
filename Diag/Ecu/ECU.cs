
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFService
{
	interface IEcu
	{
		Task<List<string>> GetEcuInfo();
		List<DiagnosticData> GetDiagnosticSets();
		DiagnosticData GetFrzFramesSet();
		Task<List<Diag.ResponseData_ReadDataByIdentifier>> GetEcuTime();
		Task<bool> ClearFaults();
	}
	/// <summary>
	/// Описание параметров диагностируемого блока управления.
	/// </summary>
	public abstract class ECU : IEcu
	{

		public ECU(EcuModelId modelId, byte DiagAdressId)
		{
			ModelId = modelId;
			Address = DiagAdressId;
		}



		abstract public List<DiagnosticData> GetDiagnosticSets();

		abstract public DiagnosticData GetFrzFramesSet();

		abstract public Task<List<string>> GetEcuInfo();

		abstract public Task<List<Diag.ResponseData_ReadDataByIdentifier>> GetEcuTime();

		abstract public Task<bool> ClearFaults();
		virtual public async Task<bool> ClearFlashData()
		{
			return await new TaskFactory().StartNew(() => { return false; });
		}

		public string Converter_EcuInformation(Tuple<int, int> t)
		{
			string result = "";
			switch (t.Item2)
			{
				case 0:
					{
						if (t.Item1 >= (byte)EcuDiagAddress.GENERAL_ECU_DIAG_ID
							&& t.Item1 < (byte)EcuDiagAddress.DISPLAY_ECU_DIAG_ID)
						{
							result = "General ECU";
						}
						else if (t.Item1 >= (byte)EcuDiagAddress.DISPLAY_ECU_DIAG_ID
							&& t.Item1 < (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID)
						{
							result = "Display ECU";
						}
						else if (t.Item1 >= (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID
							&& t.Item1 < (byte)EcuDiagAddress.BATTERY_ECU_ID)
						{
							result = "Main ECU";
						}
						else if (t.Item1 >= (byte)EcuDiagAddress.BATTERY_ECU_ID)
						{
							result = "BMS ECU";
						}
						
					}
					break;

				case 1:
					{
						if (t.Item1 == 1)
							result = "BMS COMBI";
						else if (t.Item1 == 2)
							result = "MARINE ECU";
					}
					break;

				case 2:
					{
						int Ver_mj = t.Item1 >> 8;
						int Ver_mn = t.Item1 & 0xff;
						result = Ver_mj.ToString() + "." + Ver_mn.ToString();
					}
					break;

				case 3:
					{
						int Ver_mj = t.Item1 >> 8;
						int Ver_mn = t.Item1 & 0xff;
						result = Ver_mj.ToString() + "." + Ver_mn.ToString();
					}
					break;
			}

			return result;
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

		//		public virtual bool IsTimeServer { get { return false; } }
		//		public virtual bool IsPowerManager { get { return false; } }

		#endregion
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
        dEcu = 0x13,
	}

	public enum EcuDiagAddress
	{
		NONE = 0,
		GENERAL_ECU_DIAG_ID = 1,
		DISPLAY_ECU_DIAG_ID = 6,
		MAIN_ECU_DIAG_ID	= 10,
		BATTERY_ECU_ID		= 11,
        
	}


	public class DiagnosticData
	{
		public int DataID { get; set; }
		public string StringDescription { private get; set; }
		public double Factor { get; set; }
		public bool IsSigned { get; set; }        
		public byte NoteNumber { get; set; }

		public string ToString(int NoteNum)
		{
			return StringDescription + " " + NoteNum.ToString();
		}

		public override string ToString()
		{
			return StringDescription;
		}		

		public DiagnosticData(ushort did, string Description, double factor, bool isSigned, byte noteNumber, Func<Tuple<int, int>, string> Handler = null)
		{
			DataID = did;
			StringDescription = Description;
			Factor = factor;
			IsSigned = isSigned;
			NoteNumber = noteNumber;
			Value = new List<string>(NoteNumber);
            rowValue = new List<int>(NoteNumber);
            ValueHandler = Handler;
		}

		public void ClearValues()
		{
			Value.Clear();
		}
		public void AddValue(int val)
		{
            if (rowValue.Count >= NoteNumber)
                rowValue.Clear();

            rowValue.Add(val);
		}

        public List<string> GetValue()
        {
            int _currentNote = 0;
            Value.Clear();
            foreach (var v in rowValue)
            {
                if (ValueHandler != null)
                    Value.Add(ValueHandler(new Tuple<int, int>(v, _currentNote++)));
                else
                {
                    string format = (Factor == 1) ? "N0" : "N3";
                    Value.Add(((float)v * Factor).ToString(format));
                }
            }

            rowValue.Clear();
            return Value;
        }

        List<int> rowValue;
        Func<Tuple<int, int>, string> ValueHandler;
		List<string> Value;
	}   

}
