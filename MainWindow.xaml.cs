using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DEService;

namespace BoadService
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{		
		ECU curEcu;
        System.Timers.Timer timer;

        public MainWindow()
		{
			InitializeComponent();

			this.Loaded += FrmMainWin_Loaded;
			this.Unloaded += MainWindow_Unloaded;
			this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			MainWindow_Unloaded(sender, null);
		}

		private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
		{
            timer.Elapsed -= Timer_Elapsed;
            Global.Can.Stop();
		}


		#region Event Handlers

		private void FrmMainWin_Loaded(object sender, RoutedEventArgs e)
		{
			Global.Can = new UCan();
			Global.Can.Start(0, 250);
			Global.Can.SetDiagnosticMode(false);

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            Global.MainControl = grid;

			Global.diag = new Diag.A_Diag(Global.Can);
		}


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {                
                if (Global.Can.IsConnected)
                {
                    sbCanAdapter.Text = "Адаптер: подключен";
                    sbCanSpeed.Text = "Скорость: " + Global.Can.BitRate + " кБит/с";
                    sbCanLoad.Text = "Загрузка: " + Global.Can.CanLoadPercent + "%" + "  (Tx/c: " + Global.Can.CanLoadTx + ", Rx/c: " + Global.Can.CanLoadRx + ")";
                    
                }
                else
                {
                    sbCanAdapter.Text = "Адаптер: не подключен.";
                    sbCanSpeed.Text = "Скорость: -";
                    sbCanLoad.Text = "Загрузка: -";
                        
                }
            }));
        }


        //		private void btnCoding_Click(object sender, RoutedEventArgs e)
        //		{


        ///*			Bms_ECUCoding ecu_form = new Bms_ECUCoding();
        //			ecu_form.ShowDialog();
        //			*/
        //		}

        //		private void btnMeasuring_Click(object sender, RoutedEventArgs e)
        //		{

        //		}

        //		private void btnSearchNodes_Click(object sender, RoutedEventArgs e)
        //		{
        //			SearchNodes();
        //		}

        //		private void comboECU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //		{
        //			Global.EcuList.CurrentEcu = Tools.GetComboItem(comboECU) as ECU;
        //			curEcu = Global.EcuList.CurrentEcu;

        //			if (Global.EcuList.CurrentEcu != null)
        //			{
        //				panelEcuHeader.Visibility = Visibility.Visible;
        //				GetInfo(curEcu);
        //			}
        //			else
        //				panelEcuHeader.Visibility = Visibility.Collapsed;
        //		}

        #endregion

        //		async void GetInfo(ECU ecu)
        //		{

        //		}

        //		async void SearchNodes()
        //		{
        //			//Global.EcuList = 
        //		}


    }
}
