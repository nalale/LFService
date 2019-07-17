using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BoadService
{
	/// <summary>
	/// Логика взаимодействия для ECUMeasure.xaml
	/// </summary>
	public partial class ECUMeasure : UserControl
	{
		Thread task;
		volatile bool procStop;

		ECU ecu;
		string dvsPath = "";
		string logPath = "";
		List<DiagnosticData> dvsList;
		//List<byte> didList;

		A_Service_ReadDataByIdentifier DiagSrv;
		int comStat;
		int rxCnt;

		public ECUMeasure()
		{
			InitializeComponent();

			this.Loaded += ECUMeasure_Loaded;
			this.Unloaded += ECUMeasure_Unloaded;
			this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

			listVal.SelectionChanged += ListVal_SelectionChanged;

			dvsPath = Path.Combine(Environment.CurrentDirectory, "DVS");
			if (Directory.Exists(dvsPath) == false)
				Directory.CreateDirectory(dvsPath);

			logPath = Path.Combine(Environment.CurrentDirectory, "LogFiles");
			if (Directory.Exists(logPath) == false)
				Directory.CreateDirectory(logPath);
		}




		#region Event Handlers

		bool isSet;

		private void ECUMeasure_Loaded(object sender, RoutedEventArgs e)
		{
			if (isSet)
				return;

			isSet = true;


			btClearFilter.IsEnabled = false;
			btSave.IsEnabled = false;
			tbFilter.Text = "";

			gridListVal.IsEnabled = true;

			ecu = Global.EcuList.CurrentEcu;
			dvsList = ecu.GetDiagnosticSets();

			
			DiagSrv = new A_Service_ReadDataByIdentifier();
			FormatGrid_Vars();
			FillValueList();

			lbEcuInfo.Content = ecu.Model + " (" + ecu.Address + ")";

			procStop = false;
			task = new Thread(DoWork);
			task.Start();
		}

		private void ECUMeasure_Unloaded(object sender, RoutedEventArgs e)
		{
			isSet = false;

			if (!procStop)
			{
                parList.Clear();
                DiagSrv.RequestDIDs.Clear();
                procStop = true;
				task.Join();

			}
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			ECUMeasure_Unloaded(sender, null);
		}

		// При выборе диагностической величины
		private void ListVal_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			List<byte> a = new List<byte>();
			//a.Add((byte)dvsList[listVal.SelectedIndex].DataID);
			//foreach (byte v in a)
			//{
			//	if (DiagSrv.RequestDIDs.Contains(v))
			//	{
			//		DiagSrv.RequestDIDs.Remove(v);
			//	}

			//	DiagSrv.RequestDIDs.Add(v);
			//}
			//foreach (string it in listVal.Items)
			//{
			//	//DiagnosticValueSet s = it.Tag as DiagnosticValueSet;
			//	if (it.IsSelected)
			//		a.Add((byte)it.Tag);
			//	else
			//		d.Add((byte)it.Tag);
			//}

			ListBoxItem lbItem = listVal.SelectedItem as ListBoxItem;

            if (lbItem == null)
                return;

			int i = Convert.ToInt32(lbItem.Tag);
			byte v = Convert.ToByte(lbItem.Tag);

			

			if (DiagSrv.RequestDIDs.Contains(v))
			{
				DiagSrv.RequestDIDs.Remove(v);
                parList.Clear();
                lbItem.Background = Brushes.Transparent;
			}
			else
			{				
				DiagSrv.RequestDIDs.Add(v);
				lbItem.Background = Brushes.LightGray;
			}



			//btSave.IsEnabled = DiagSrv.RequestDIDs.Count > 0;
		}

		// Очистить фильтр
		private void btClearFilter_Click(object sender, RoutedEventArgs e)
		{
			tbFilter.Text = "";
		}

		// При вводе фильтра
		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (tbFilter.Text.Length == 0)
			{
				btClearFilter.IsEnabled = false;
			}
			else
			{
				btClearFilter.IsEnabled = true;
			}

			FillValueList();
		}

		private void CommitButtonCLick(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;

			if (btn == null)
			{
				e.Handled = true;
				return;
			}

			if (btn.Name == "btnBack")
			{
				Global.MainControl.Children.Clear();
				Global.MainControl.Children.Add(Global.BackHistory.Pop());
			}
			else if (btn.Name == "btnOpen")
			{

			}
			else if (btn.Name == "btnSave")
			{

			}
		}

		#endregion


		#region General

		void FillValueList()
		{
			string f = tbFilter.Text.ToLower();
			if (f.Length < 2)
				f = "";

			listVal.Items.Clear();

			foreach (DiagnosticData it in dvsList)
			{
				ListBoxItem lbItem = new ListBoxItem();
				lbItem.Content = it.ToString();
				lbItem.Tag = it.DataID;
				string li = it.ToString();
				listVal.Items.Add(lbItem);
			}
		}
		
		void UpdateCommunicationStatus()
		{
			switch (comStat)
			{
				case 0:
					lbConnStat.Content = "|";
					break;
				case 1:
					lbConnStat.Content = "/";
					break;
				case 2:
					lbConnStat.Content = "—";
					break;
				case 3:
					lbConnStat.Content = "\\";
					break;
				default:
					break;
			}

			comStat++;
			if (comStat > 3)
				comStat = 0;
		}

		async void DoWork()
		{
			const int requestPeriod = 500;
			
			List<ResponseData_ReadDataByIdentifier> Response;
			Dictionary<int, int> sList = new Dictionary<int, int>();

			while (!procStop)
			{
				if (DiagSrv.RequestDIDs.Count > 0)
				{
					rxCnt++;

					// Запрос диагностических значений
					Response = await Global.diag.ReadDataByIDs(ecu.Address, DiagSrv.RequestDIDs.ToArray<byte>());

					// Визуализация значений
					await Dispatcher.BeginInvoke(new Action(() =>
					{
						UpdateCommunicationStatus();
						FillGrid_Vars(Response);
					}));
				}
                //else
                //{
                //	Dispatcher.BeginInvoke(new Action(() =>
                //	{
                //		FillGrid_Vars(new List<ResponseData_ReadDataByIdentifier>());
                //	}));

                //}

                Thread.Sleep(requestPeriod);
			}
		}		

		#endregion



		#region DataGrid

		ObservableCollection<GridPar> parList = new ObservableCollection<GridPar>();


		void FormatGrid_Vars()
		{
			//dataGrid2.Items.Clear();
			dGrid.ItemsSource = null;
			dGrid.Columns.Clear();

			// Фон
			dGrid.Background = Brushes.Transparent;
			//dGrid.BorderBrush = Brushes.Transparent;

			// Режим выделения
			dGrid.SelectionMode = DataGridSelectionMode.Single;
			dGrid.SelectionUnit = DataGridSelectionUnit.Cell;

			// Запрещаем редактирование пользователю
			//dGrid.IsReadOnly = true;
			dGrid.CanUserAddRows = false;
			dGrid.CanUserResizeRows = false;

			// Цвета чередующихся строк
			dGrid.RowBackground = Brushes.White;
			//dGrid.AlternatingRowBackground = Brushes.Beige;

			// Нет сетки
			//dGrid.GridLinesVisibility = DataGridGridLinesVisibility.None;
			dGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;
			dGrid.HorizontalGridLinesBrush = Brushes.Beige;
			dGrid.VerticalGridLinesBrush = Brushes.Beige;

			// Отображать только заголовки столбцов
			dGrid.HeadersVisibility = DataGridHeadersVisibility.Column;

			//dGrid.HorizontalContentAlignment = HorizontalAlignment.Stretch;


			DataGridTextColumn col = new DataGridTextColumn();
			//col.Header = Res.General.Name;
			col.MinWidth = 250;
			Binding b = new Binding("Name");
			col.IsReadOnly = true;
			col.Binding = b;
			//col.EditingElementStyle = (Style)dataGrid2.FindResource("errorStyle");
			dGrid.Columns.Add(col);


			col = new DataGridTextColumn();
			col.IsReadOnly = true;      // Правка только через форму
										//col.Header = Res.General.Value;
			col.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
			col.Binding = new Binding("Value");

			// Создаем стиль для отображения фона ячеек, если значение pnValue_Color
			Style st = new Style(typeof(TextBlock));
			Setter s = new Setter(TextBlock.BackgroundProperty, new Binding("BackColor"));
			st.Setters.Add(s);
			st = new Style(typeof(TextBlock));
			s = new Setter(TextBlock.ForegroundProperty, new Binding("ForeColor"));
			st.Setters.Add(s);

			s = new Setter(MarginProperty, new Thickness(2, 0, 2, 0));
			st.Setters.Add(s);

			// ToolTip
			s = new Setter(TextBlock.ToolTipProperty, new Binding("TT"));
			st.Setters.Add(s);
			s = new Setter(ToolTipService.InitialShowDelayProperty, 0);
			st.Setters.Add(s);
			s = new Setter(ToolTipService.ShowDurationProperty, 600000);
			st.Setters.Add(s);

			col.ElementStyle = st;
			dGrid.Columns.Add(col);


			dGrid.ItemsSource = parList;
		}


		void FillGrid_Vars(List<ResponseData_ReadDataByIdentifier> vals)
		{
			//dGrid.ItemsSource = null;
			parList.Clear();

			foreach (byte did in DiagSrv.RequestDIDs)
			{
				////Сопоставляем присланные данные с запрошенными, т.к. некоторые данные погут не поддерживаться ЭБУ.
				DiagnosticData dv = null;

				foreach (ResponseData_ReadDataByIdentifier it in vals)
				{
					if (it.DID == did)
					{
						// Найти в списке диагностических значений нужный 
						dv = ecu.GetDiagnosticSets().Find((diag) => diag.DataID == did);
						// Если ЭБУ поддерживает запрашиваемый DID
						if (dv == null)
							break;

						for (int i = 0; i < it.DV.Count / 4; i++)
							dv.AddValue(BitConverter.ToInt32(it.DV.ToArray(), i * 4));
						break;						
					}
				}

				if (dv != null)     // Если ЭБУ поддерживает запрашиваемый DID
				{
					int i = 0;
					if (dv.Value.Count > 0)
					{
						foreach (string value in dv.Value)
						{
							parList.Add(new GridPar()
							{
								DID = did,
								//Param = dv.ToString(),
								Name = dv.ToString(++i),
								Value = value,
							});
						}
					}
					else
					{
						parList.Add(new GridPar()
						{
							DID = did,
							//Param = dv.ToString(),
							Name = dv.ToString(++i),
							Value = "-",
						});
					}
				}
				else    // Если ЭБУ не поддерживает запрашиваемый DID
				{
					parList.Add(new GridPar()
					{
						DID = 0,						
						Name = "Некорректный DID",
						Value = " - ",
						TT = null,
						BackColor = Brushes.Transparent,
						ForeColor = Brushes.Black,
					});
				}
			}
		}



		class GridPar : INotifyPropertyChanged
		{

			#region Свойства

			string vl = "";
			string tt;
			Brush bc = Brushes.Transparent;
			Brush fc = Brushes.Black;

			public UInt16 DID { get; set; }
			public string Param { set; get; }
			public string Name { set; get; }

			public string Value
			{
				set
				{
					if (value != vl)
					{
						vl = value;
						OnPropertyChanged("Value");
					}
				}
				get { return vl; }
			}

			// ToolTip
			public string TT
			{
				set
				{
					if (value != tt)
					{
						tt = value;
						OnPropertyChanged("TT");
					}
				}
				get { return tt; }
			}


			public Brush ForeColor
			{
				set
				{
					if (value != fc)
					{
						fc = value;
						OnPropertyChanged("ForeColor");
					}
				}
				get { return fc; }
			}

			public Brush BackColor
			{
				set
				{
					if (value != bc)
					{
						bc = value;
						OnPropertyChanged("BackColor");
					}
				}
				get { return bc; }
			}

			// Т.к. переменные могут содержать разные типы данных, то здесь храним реальные значения переменных.
			// Это поле скрыто в таблице
			public object RawValue { set; get; }

			#endregion


			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(PropertyChangedEventArgs e)
			{
				PropertyChanged?.Invoke(this, e);
			}
			protected void OnPropertyChanged(string propertyName)
			{
				OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
			}

			#endregion

		}

		#endregion

	}

	public class A_Service_ReadDataByIdentifier
	{
		public A_Service_ReadDataByIdentifier()
		{
			RequestDIDs = new List<byte>();
			Response = new List<ResponseData_ReadDataByIdentifier>();
		}


		#region Свойства

		public List<byte> RequestDIDs { get; set; }
		public List<ResponseData_ReadDataByIdentifier> Response { get; private set; }


		#endregion
	}




	public class ResponseData_ReadDataByIdentifier
	{
		public short DID;
		public List<byte> DV;
		public byte Result { get; set; }

		public ResponseData_ReadDataByIdentifier(short did)
		{
			DID = did;
			DV = new List<byte>();
		}
	}
	
}


