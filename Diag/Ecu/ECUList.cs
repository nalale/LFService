using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoadService
{
	public class EcuList
	{
		public EcuList()
		{
			Items = new List<ECU>();
		}

		static EcuList()
		{
		}


		#region Свойства

		public List<ECU> Items { get; set; }

		/// <summary>
		/// Текущий ЭБУ, выбранный для диагностики.
		/// </summary>
		public ECU CurrentEcu { get; set; }

		#endregion


		#region General

		public void Clear()
		{
			Items.Clear();
		}

		public ECU GetByAddress(byte address)
		{
			foreach (ECU it in Items)
			{
				if (address == it.Address)
				{
					return it;
				}
			}

			return null;
		}

		
		public void SortByAddress()
		{
			Items.Sort(new Comparer_ByAddress());
		}

		class Comparer_ByAddress : IComparer<ECU>
		{
			public int Compare(ECU x, ECU y)
			{
				return x.Address - y.Address;
			}
		}

		#endregion


		#region Static

		public static ECU CreateEcu(byte DiagAddress)
		{
			EcuModelId ecuModel = EcuModelId.none;

			if (DiagAddress >= (byte)EcuDiagAddress.GENERAL_ECU_DIAG_ID && DiagAddress < (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID)
			{
				ecuModel = EcuModelId.gEcu;
			}
			else if (DiagAddress >= (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID && DiagAddress < (byte)EcuDiagAddress.BATTERY_ECU_ID)
			{
				ecuModel = EcuModelId.mEcu;
			}
			else if (DiagAddress >= (byte)EcuDiagAddress.BATTERY_ECU_ID)
			{
				ecuModel = EcuModelId.bms;
			}			

			switch (ecuModel)
			{
				case EcuModelId.gEcu:
					return new General_ECU(ecuModel, DiagAddress);
				case EcuModelId.mEcu:
					return new Main_ECU(ecuModel, DiagAddress);
				case EcuModelId.bms:
					return new Bms_ECU(ecuModel, DiagAddress);

				default:
					return new ECU(ecuModel, DiagAddress);

			}
		}

		

		#endregion

	}
}
