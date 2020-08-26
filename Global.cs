using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DEService;

namespace MFService
{
	public static class Global
	{
		public static ICanDriver Can;
		public static Diag.A_Diag diag;
		public static EcuList EcuList;

		public static Stack<Control> BackHistory;
		public static Grid MainControl;

		static Global()
		{
			EcuList = new EcuList();
			BackHistory = new Stack<Control>();
		}
	}
}
