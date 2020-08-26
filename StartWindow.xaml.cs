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

namespace MFService
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
            }
            else
            {
                _timer.Stop();
                _timer.Elapsed -= TimerFunc;
                
            }
        }

		private async void SetEcuInfo()
		{
			List <string> info = await Global.EcuList.CurrentEcu.GetEcuInfo();

			if(info.Count == 4)
			{
                tbModel.Text = info[0];
                tbHW.Text = info[1];
                tbVersion.Text = info[2];
                tbHvVersion.Text = info[3];
            }
		}

        private async void SetDateTime()
        {
            List<Diag.ResponseData_ReadDataByIdentifier> response = await Global.EcuList.CurrentEcu.GetEcuTime();

            if (response.Count > 0)
            {
                Diag.ResponseData_ReadDataByIdentifier it = response[0];
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

		public void FindEcuList()
		{
			comboECU.ItemsSource = null;
			Global.EcuList.Items.Clear();

			foreach (byte i in Global.diag.ReadDiagID())
			{
				if (!Global.EcuList.Items.Exists((ecu) => (ecu.Address == i)) && (i > 0))
				{
					Global.EcuList.Items.Add(EcuList.CreateEcu(i));					
				}
			}

			comboECU.ItemsSource = Global.EcuList.Items.Select((ecu) => { return new StringBuilder(Enum.GetName(typeof(EcuModelId), ecu.ModelId) + "\t(" + ecu.Address.ToString() + ")").ToString(); });
			
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

		async public void ClearEcuFlashData()
		{
			bool result = await Global.EcuList.CurrentEcu.ClearFlashData();
			if (result != true)
				MessageBox.Show("Сброс памяти невозможен", "Ошибка");
			else
				MessageBox.Show("Память данных очищена", "Внимание");
		}

        private void btnClearFaults_Click(object sender, RoutedEventArgs e)
        {
            ClearEcuFaults();
        }

		private void btnClearFlash_Click(object sender, RoutedEventArgs e)
		{
			ClearEcuFlashData();
		}

	}
}
