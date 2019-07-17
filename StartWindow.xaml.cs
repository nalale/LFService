using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BoadService
{
	/// <summary>
	/// Interaction logic for StartWindow.xaml
	/// </summary>
	public partial class StartWindow : UserControl
	{
		EcuCoding ecuCoding;
		ECUMeasure ecuMeasure;
		ECUReadDTC ecuReadFreezeFrames;
        System.Timers.Timer _timer;

		byte _ecuDiagAddress = 0;

		public StartWindow()
		{
			InitializeComponent();
			this.Loaded += StartWindow_Loaded;
		}

		private void StartWindow_Loaded(object sender, RoutedEventArgs e)
		{
            _timer = new Timer(1000);

            
            //Global.EcuList.Items.Add(EcuList.CreateEcu(1));
            //comboECU.ItemsSource = Global.EcuList.Items.Select((ecu) => { return new StringBuilder(ecu.Model + ecu.Address.ToString()).ToString(); });
            //if (comboECU.Items.Count > 0)
            //	comboECU.SelectedIndex = 0;
        }

        private void TimerFunc(object sender, ElapsedEventArgs e)
        {

            Dispatcher.BeginInvoke( new Action((SetDateTime)));
            
        }

		private void btnCoding_Click(object sender, RoutedEventArgs e)
		{
            if (ecuCoding == null && _ecuDiagAddress > 0)
                ecuCoding = new EcuCoding(_ecuDiagAddress);
            else if (_ecuDiagAddress == 0)
                return;

			Global.MainControl.Children.Clear();
			Global.MainControl.Children.Add(ecuCoding);
			Global.BackHistory.Push(this);
		}

		private void btnSearchNodes_Click(object sender, RoutedEventArgs e)
		{
			FindEcuList();
		}

		private void btnMeasuring_Click(object sender, RoutedEventArgs e)
		{
            if (ecuMeasure == null && _ecuDiagAddress > 0)
                ecuMeasure = new ECUMeasure();
            else if (_ecuDiagAddress == 0)
                return;

            Global.MainControl.Children.Clear();
			Global.MainControl.Children.Add(ecuMeasure);
			Global.BackHistory.Push(this);
		}

		private void btnFreezeFrames_Click(object sender, RoutedEventArgs e)
		{
			if (ecuReadFreezeFrames == null && _ecuDiagAddress > 0)
				ecuReadFreezeFrames = new ECUReadDTC();

			Global.MainControl.Children.Clear();
			Global.MainControl.Children.Add(ecuReadFreezeFrames);
			Global.BackHistory.Push(this);
		}

		private void comboECU_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (comboECU.SelectedItem != null)
			{
				Global.EcuList.CurrentEcu = Global.EcuList.Items[(byte)comboECU.SelectedIndex];
				_ecuDiagAddress = Global.EcuList.CurrentEcu.Address;
                SetEcuInfo();

                _timer.Elapsed += TimerFunc;
                //_timer.Start();
            }
            else
            {
                _timer.Stop();
                _timer.Elapsed -= TimerFunc;
                
            }
        }

		private async void SetEcuInfo()
		{
			List < ResponseData_ReadDataByIdentifier > response = await Global.EcuList.CurrentEcu.GetEcuInfo();

			if(response.Count > 0)
			{
				ResponseData_ReadDataByIdentifier it = response[0];
                // Найти в списке диагностических значений нужный 
                DiagnosticData dv = Global.EcuList.CurrentEcu.GetDiagnosticSets().Find((diag) => diag.DataID == it.DID);
                // Если ЭБУ поддерживает запрашиваемый DID
                if (dv == null)
                    return;

                if (it.DV.Count == 0)
                    return;

                for (int i = 0; i < it.DV.Count / 4; i++)
                    dv.AddValue(BitConverter.ToInt32(it.DV.ToArray(), i * 4));

                if (Convert.ToByte(dv.Value[0]) >= (byte)EcuDiagAddress.GENERAL_ECU_DIAG_ID && Convert.ToByte(dv.Value[0]) < (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID)
                {
                    tbModel.Text = "General ECU";
                }
                else if (Convert.ToByte(dv.Value[0]) >= (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID && Convert.ToByte(dv.Value[0]) < (byte)EcuDiagAddress.BATTERY_ECU_ID)
                {
                    tbModel.Text = "Main ECU";
                }
                else if (Convert.ToByte(dv.Value[0]) >= (byte)EcuDiagAddress.BATTERY_ECU_ID)
                {
                    tbModel.Text = "BMS ECU";
                }

                if(Convert.ToByte(dv.Value[1]) == 1)
                    tbHW.Text = "BMS COMBI";
                else if(Convert.ToByte(dv.Value[1]) == 2)
                    tbHW.Text = "MARINE ECU";

                tbVersion.Text = dv.Value[2];
                tbHvVersion.Text = dv.Value[3]; 
            }
		}

        private async void SetDateTime()
        {
            List<ResponseData_ReadDataByIdentifier> response = await Global.EcuList.CurrentEcu.GetEcuTime();

            if (response.Count > 0)
            {
                ResponseData_ReadDataByIdentifier it = response[0];
                // Найти в списке диагностических значений нужный 
                DiagnosticData dv = Global.EcuList.CurrentEcu.GetDiagnosticSets().Find((diag) => diag.DataID == it.DID);
                // Если ЭБУ поддерживает запрашиваемый DID
                if (dv == null)
                    return;
                

                UInt32 sec = BitConverter.ToUInt32(it.DV.ToArray(), 0);
                DateTime sys_time = (new DateTime(2000, 1, 1)).AddSeconds(sec);

                tbDateTime.Text = sys_time.ToString();
            }
        }

		async public void FindEcuList()
		{
			List<byte> res = await Global.diag.ReadDiagID();

			comboECU.ItemsSource = null;
			Global.EcuList.Items.Clear();

			foreach (byte i in res)
			{
				if (!Global.EcuList.Items.Exists((ecu) => (ecu.Address == i)) && (i > 0))
				{
					Global.EcuList.Items.Add(EcuList.CreateEcu(i));					
				}
			}

			comboECU.ItemsSource = Global.EcuList.Items.Select((ecu) => { return new StringBuilder(ecu.Model + ecu.Address.ToString()).ToString(); });
			if (comboECU.Items.Count > 0)
				comboECU.SelectedIndex = 0;
		}

        async public void ClearEcuFaults()
        {
            bool result = await Global.EcuList.CurrentEcu.ClearFaults();
            if (result != true)
                MessageBox.Show("Сброс ошибок невозможен", "Ошибка");
            else
                MessageBox.Show("Ошибки сброшены", "Внимание");
        }

        private void btnClearFaults_Click(object sender, RoutedEventArgs e)
        {
            ClearEcuFaults();
        }
    }
}
