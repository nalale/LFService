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

namespace MFService
{
	/// <summary>
	/// Логика взаимодействия для ECUMeasure.xaml
	/// </summary>
	public partial class ECUMeasure : UserControl
	{
		Task taskShow;
		CancellationTokenSource cancelTaskShow;
		

		ECU ecu;
		string dvsPath = "";
		string logPath = "";

		List<DiagnosticData> dvsList;
		Diag.A_Service_ReadDataByIdentifier DiagSrv;
		
		int rxCnt;

		public ECUMeasure()
		{
			InitializeComponent();

			this.Loaded += ECUMeasure_Loaded;
			this.Unloaded += ECUMeasure_Unloaded;
			this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

			dvsPath = Path.Combine(Environment.CurrentDirectory, "DVS");
			if (Directory.Exists(dvsPath) == false)
				Directory.CreateDirectory(dvsPath);

			logPath = Path.Combine(Environment.CurrentDirectory, "LogFiles");
			if (Directory.Exists(logPath) == false)
				Directory.CreateDirectory(logPath);

			
			
		}



		#region Event Handlers		

		private void ECUMeasure_Loaded(object sender, RoutedEventArgs e)
		{
			ecu = Global.EcuList.CurrentEcu;
			dvsList = ecu.GetDiagnosticSets();

			
			DiagSrv = new Diag.A_Service_ReadDataByIdentifier();
			FormatGrid_Vars();
			FillValueList();

			cancelTaskShow = new CancellationTokenSource();
			CancellationToken token = cancelTaskShow.Token;

			Task.Factory.StartNew(async () =>
			{
				while (!token.IsCancellationRequested)
				{
					if (DiagSrv.RequestedDIDs.Count > 0)
					{
						foreach (int i in DiagSrv.RequestedDIDs)
						{
							rxCnt++;
							Thread.Sleep(10);
							// Отправить запрос
							if (await DiagSrv.RequestService(ecu.Address))
							{
								// Визуализация значений
								await Dispatcher.BeginInvoke(new Action(() =>
								{
									UpdateCommunicationStatus();
									ShowResponse(DiagSrv);
								}));
							}
						}
					}
					else
						Dispatcher.Invoke(ClearGrid_Vars);

					Thread.Sleep(250);
				}
			}, token);

		}

		private void ECUMeasure_Unloaded(object sender, RoutedEventArgs e)
		{
			cancelTaskShow.Cancel();
			parList.Clear();
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			ECUMeasure_Unloaded(sender, null);
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
		private void ParameterSelected(object sender, RoutedEventArgs e)
		{
			if (!(sender is CheckBox cb_param))
				return;

			byte v = Convert.ToByte(cb_param.Tag);

			if (cb_param.IsChecked == false)
				DiagSrv.RemoveRequestedDID(v);
			else if (cb_param.IsChecked == true)
				DiagSrv.AddRequestedDID(v);


		}
		#endregion


		#region General

		void FillValueList()
		{
			listVal.Items.Clear();

			foreach (DiagnosticData it in dvsList)
			{
                CheckBox cb = new CheckBox() { Content = it.ToString(), Tag = it.DataID };
                cb.Checked += ParameterSelected;
                cb.Unchecked += ParameterSelected;            

				listVal.Items.Add(new ListBoxItem { Content = cb });
			}
		}
        void UpdateCommunicationStatus()
		{

            if (pbConnStat.Value < pbConnStat.Maximum)
                pbConnStat.Value++;
            else
                pbConnStat.Value = pbConnStat.Minimum;
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
        void ShowResponse(Diag.A_Service_ReadDataByIdentifier srvDataById)
        {            
            parList.Clear();

            foreach (var did in srvDataById.RequestedDIDs)
            {                
                DiagnosticData dv = null;                
                
                // Найти в списке диагностических значений нужный 
                dv = ecu.GetDiagnosticSets().Find((diag) => diag.DataID == did);

                // Если ЭБУ поддерживает запрашиваемый DID
                if (dv != null)     
                {
                    foreach (int val in srvDataById.GetResponseValue(did))
                        dv.AddValue(val);                      
                
                    int i = 0;
                    List<string> values = dv.GetValue();

                    if (values.Count > 0)
                    {
                        foreach (string value in values)
                        {
                            parList.Add(new GridPar()
                            {
                                DID = (ushort)did,
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
                            DID = (ushort)did,
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
		void ClearGrid_Vars()
        {
            parList.Clear();
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

	
	
}


