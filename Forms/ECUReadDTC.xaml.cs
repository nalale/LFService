using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BoadService
{
	/// <summary>
	/// Логика взаимодействия для ECUReadDTC.xaml
	/// </summary>
	public partial class ECUReadDTC : UserControl
	{

		A_Service_ReadDataByIdentifier DiagSrv;

		public ECUReadDTC()
		{
			InitializeComponent();
			Translate();

			this.Loaded += ECUReadDTC_Loaded;
			this.Unloaded += ECUReadDTC_Unloaded;
			this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}



		#region Свойства

		byte DTCStatusAvailabilityMask;

		/// <summary>
		/// Режим работы элемента управления:
		/// 0 - Запросить и отобразить коды неисправности указанного ЭБУ.
		/// 1 - Запросить и отобразить все поддерживаемые коды неисправности указанного ЭБУ.
		/// 2 - Отобразить указанные коды неисправности.
		/// </summary>
		public int Mode { get; set; }


		#endregion


		#region Event Handlers

		bool isSet;

		private void ECUReadDTC_Loaded(object sender, RoutedEventArgs e)
		{
			if (isSet)
				return;

			DiagSrv = new A_Service_ReadDataByIdentifier();

			isSet = true;

			tbLog.Clear();
			tbLog.IsEnabled = false;
			
			tbWait.Visibility = Visibility.Visible;

			if (Mode == 0)
			{
				btnClear.IsEnabled = true;
				ReadDTC();
			}
			else if (Mode == 1)
			{
				// Кнопка "Очистить" недоступна
				btnClear.IsEnabled = false;
				//ReadSupportedDTC();
			}
			else if (Mode == 2)
			{
				btnClear.IsEnabled = true;
				//ReadAllDTC();				
			}
		}

		private void ECUReadDTC_Unloaded(object sender, RoutedEventArgs e)
		{
			isSet = false;
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			ECUReadDTC_Unloaded(sender, null);
		}


		private void CommitButtonCLick(object sender, RoutedEventArgs e)
		{
			Button btn = (Button)sender;
			if (btn.Name == btnBack.Name)
			{
				if (Global.BackHistory.Count > 0)
				{
					Global.MainControl.Children.Clear();
					Global.MainControl.Children.Add(Global.BackHistory.Pop());
				}
			}
			// Очистить
			else if (btn.Name == btnClear.Name)
			{
				ClearDTC();
			}
			// Сохранить
			else if (btn.Name == btnSave.Name)
			{
				// Копируем текст
//				tbLog.SelectLines(0, int.MaxValue);
//				tbLog.Copy(null);

				if (Clipboard.ContainsText() == false)
					return;


				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt";
				dlg.Filter = tr.TextFile + "|*.txt";

				if (dlg.ShowDialog() != true)
					return;

				string path = dlg.FileName;
				File.WriteAllText(path, Clipboard.GetText());
			}
		}

		#endregion


		#region General

		// Запрос кодов неисправностей
		async void ReadDTC()
		{
			tbLog.Clear();

			foreach (ECU curEcu in Global.EcuList.Items)
			{
				byte id = (byte)curEcu.GetFrzFramesSet().DataID;
				List<ResponseData_ReadDataByIdentifier> Response = await Global.diag.ReadDataByIDs(curEcu.Address, new byte[] { id });

				if (Response[0].Result != 0)
				{
					tbLog.AppendText("\nНет ответа");
				}
				else
				{
					//A_Service_ReadDTCInformation s = srv as A_Service_ReadDTCInformation;

					List<EcuDTCList> list = new List<EcuDTCList>();
					EcuDTCList ecuDTCList = new EcuDTCList();
					ecuDTCList.Ecu = curEcu;
					list.Add(ecuDTCList);

					// Запрос стоп-кадров для полученных неисправностей
					//foreach (dtcItem dtc in s.Response_DTCByStatusMask.DTCList)
					//{
					//	srv = await Global.uds.ReadDTC(curEcu.Address, A_ReadDTCInformation_Subfunctions.reportDTCSnapshotRecordByDTCNumber, 0, dtc);
					//	if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
					//	{
					//		if (srv.State == A_ServiceState.Timeout)
					//			tbLog.AddLine(new HistoryText(tr.EcuNotRespond));
					//		else
					//			tbLog.AddLine(new HistoryText(tr.WrongRespond));
					//	}
					//	else
					//	{
					//		A_Service_ReadDTCInformation sFrzf = srv as A_Service_ReadDTCInformation;
					//		dtc.FRZF = sFrzf.Response_DTCSnapshotRecordByDTCNumber.DTC.FRZF;
					//	}
					//}



					//ecuDTCList.DtcList = s.Response_DTCByStatusMask.DTCList;
					
				}
			}
			//ShowDTC(list);

			tbLog.IsEnabled = true;
			tbWait.Visibility = Visibility.Collapsed;
		}

		// Очистить все неисправности
		async void ClearDTC()
		{
			//tbLog.Clear();
			//commitButtons.IsEnabled = false;
			//tbWait.Visibility = Visibility.Visible;

			//if (Mode == 0)
			//{
			//	ECU curEcu = Global.EcuList.CurrentEcu;

			//	A_Service srv = await Global.uds.ClearDTC(curEcu.Address, 5000);
			//	if (srv.State == A_ServiceState.Timeout || srv.NResult != N_Result.N_OK)
			//	{
			//		if (srv.State == A_ServiceState.Timeout)
			//			tbLog.AddLine(new HistoryText(tr.EcuNotRespond));
			//		else
			//			tbLog.AddLine(new HistoryText(tr.WrongRespond));
			//	}

			//	// Снова проверим неисправности
			//	ReadDTC();
			//}
			//else if (Mode == 2)
			//{
			//	A_Service srv = await Global.uds.ClearDTC(0, 5000);
			//	ReadAllDTC();
			//}

			commitButtons.IsEnabled = true;
		}


		void ShowDTC(List<EcuDTCList> res)
		{
			const string indent1 = "      ";
			const string indent2 = indent1 + indent1;


			string s = "";
			TextBox ht;
			//List<HistoryText> lines = new List<HistoryText>();
			//bool isDtcExist = false;

			//foreach (EcuDTCList it in res)
			//{
			//	// Информация об ЭБУ: Класс, Модель, контроллер, версия ПО, серийный номер
			//	ht = new HistoryText();
			//	ht.Append(Res.General.Address + it.Ecu.Address + " ( " + it.Ecu.ClassIdString + " )\n", TextColor.Digit);
			//	ht.Append(Res.General.Model + it.Ecu.Model + "\n");
			//	ht.Append(Res.General.Controller + it.Ecu.Hardware + "\n");
			//	ht.Append(Res.General.FwVersion + Tools.GetVersionString(it.Ecu.FwVersion) + "\n");


			//	//ht.Append(new string('-', 80) + "\n");
			//	lines.Add(ht);


			//	// Список DTC
			//	if (it.DtcList.Count > 0)
			//	{
			//		isDtcExist = true;

			//		ht = new HistoryText();
			//		ht.Append(tr.FoundDTC, TextColor.Error);
			//		ht.Append(it.DtcList.Count.ToString(), TextColor.Error);
			//		ht.Append(":\n", TextColor.Error);
			//		lines.Add(ht);

			//		int n = 1;
			//		foreach (dtcItem dtc in it.DtcList)
			//		{
			//			DTC.Codes cod = (DTC.Codes)dtc.Code;
			//			DTC_Category.Code cat = (DTC_Category.Code)dtc.Category;

			//			ht = new HistoryText();
			//			ht.Append(n + ". ");
			//			if (dtc.Category == 0)
			//			{
			//				ht.Append("DTC " + DTC.ToAlphanumeric(cod), TextColor.Ok);
			//				ht.Append(" - " + DTC.ToString(cod) + "\n");
			//			}
			//			else
			//			{
			//				ht.Append("DTC " + DTC.ToAlphanumeric(cod) + "-" + dtc.Category.ToString("X2"), TextColor.Ok);
			//				ht.Append(" - " + DTC.ToString(cod) + ": " + DTC_Category.ToString(cat) + "\n");
			//			}

			//			// Статус
			//			s = indent1 + tr.Status + Convert.ToString(dtc.Status, 2).PadLeft(8, '0') + " - ";
			//			if ((dtc.Status & 0x01) != 0)
			//				s += tr.Active;
			//			else
			//				s += tr.NotActive;
			//			ht.Append(s + "\n");

			//			// FRZF
			//			s = indent1 + tr.FreezeFrame;
			//			string fv = "\n";
			//			foreach (DiagnosticValue dv in dtc.FRZF)
			//			{
			//				if (dv.Name != "")
			//					fv += dv.Name + ": " + dv.ToString() + "\n";
			//				else
			//					fv += dv.ToString() + "\n";

			//			}
			//			//if (dtc.FRZF.Count == 0)
			//			//{

			//			//}
			//			fv = fv.Replace("\n", "\n" + indent2);
			//			s += fv;
			//			ht.Append(s);

			//			lines.Add(ht);
			//			n++;
			//		}
			//	}
			//	else
			//	{
			//		ht = new HistoryText();
			//		ht.Append(tr.DtcNotFound, TextColor.Ok);
			//		lines.Add(ht);
			//	}

			//	ht = new HistoryText();
			//	ht.Append("\n");
			//	ht.Append(new string('-', 80) + "\n");
			//	lines.Add(ht);
			//}

			//tbLog.AddLines(lines);
			//tbLog.MoveHome();

			//// Кнопка "Очистить" недоступна если нет ошибок
			//commitButtons.Button3.IsEnabled = isDtcExist;
		}

		#endregion


		#region Translate

		void Translate()
		{
			tbWait.Text = tr.InProgress;
		}

		static class tr
		{
			public static string InProgress { get { return "Операция выполняется ... "; } }
			
			public static string TextFile { get { return "Текстовый файл"; } }
			public static string EcuNotRespond { get { return "ЭБУ не отвечает."; } }
			public static string WrongRespond { get { return "Неверный формат ответа."; } }



			public static string FoundDTC { get { return "Найдено неисправностей - "; } }
			public static string Status { get { return "Статус неисправности: "; } }
			public static string Active { get { return "Активная"; } }
			public static string NotActive { get { return "Неактивная"; } }
			public static string FreezeFrame { get { return "Стоп-кадр:"; } }
			public static string DtcNotFound { get { return "Неисправностей не найдено."; } }

			public static string SupportedDtc { get { return "Количество поддерживаемых кодов неисправностей: "; } }
			public static string DtcNotSupported { get { return "Коды неисправностей не поддерживаются"; } }
		}


		#endregion


		public class EcuDTCList
		{
			public ECU Ecu;
			public List<dtcItem> DtcList;
		}
	}
}
