using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BoadService
{
    public enum ReadDataResult_e
    {
        RES_SUCCESSFUL = 0,
        RES_CRC_ERROR,
        RES_TIMEOUT,
        RES_INCORRECT_DATA,
        RES_OTHER_ERROR,
    };
	/// <summary>
	/// Interaction logic for EcuCoding.xaml
	/// </summary>
	public partial class EcuCoding : UserControl
	{
		IEcu_Coding codingForm;
		byte DiagAddress;

		public EcuCoding(byte diagAddress)
		{
			InitializeComponent();
			DiagAddress = diagAddress;

			this.Loaded += EcuCoding_Loaded;
		}

		private void EcuCoding_Loaded(object sender, RoutedEventArgs e)
		{
			codingForm = null;
			OpenEcuCodingForm();
			//Download();
		}

		private void  CommitButtonCLick(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;

			if (btn == null)
			{
				e.Handled = true;
				return;
			}

			if(btn.Name == "btnBack")
			{
				Global.MainControl.Children.Clear();
				Global.MainControl.Children.Add(Global.BackHistory.Pop());
			}
			else if(btn.Name == "btnDownload")
			{
				Download();
			}
			else if(btn.Name == "btnUpload")
			{
				Upload();
			}
			else if(btn.Name == "btnOpen")
			{
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".xml";
                dlg.Filter = "Файл кодировок |*.xml";

                if (dlg.ShowDialog() != true)
                    return;

                string path = dlg.FileName;
                if (!File.Exists(path))
                    return;

                codingForm?.Open(path);
            }
			else if(btn.Name == "btnSave")
			{
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".xml";
                dlg.Filter = "Файл кодировок (.xml)|*.xml";

                if (dlg.ShowDialog() != true)
                    return;

                string path = dlg.FileName;
                codingForm?.Save(path);
            }
		}

		async void Download()
		{
			bool res = false;
			
			if (codingForm != null)
				res = await codingForm.Download();

			if (res == false)
				MessageBox.Show("Не удалось скачать кодировку с ЭБУ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		async void Upload()
		{
            ReadDataResult_e res = await codingForm.Upload();

			if (res == ReadDataResult_e.RES_SUCCESSFUL)
				MessageBox.Show("Кодировка успешно применена.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
			else
				MessageBox.Show("Не удалось применить кодировку к ЭБУ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		void OpenEcuCodingForm()
		{
			//! DEBUG
			//Global.EcuList.CurrentEcu = new Battery_ECU();

			EcuModelId ecuModel = EcuModelId.none;

			if (DiagAddress >= (byte)EcuDiagAddress.GENERAL_ECU_DIAG_ID && DiagAddress < (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID)
			{
				ecuModel = EcuModelId.gEcu;
			}
			else if(DiagAddress >= (byte)EcuDiagAddress.MAIN_ECU_DIAG_ID && DiagAddress < (byte)EcuDiagAddress.BATTERY_ECU_ID)
			{
				ecuModel = EcuModelId.mEcu;
			}
			else if(DiagAddress >= (byte)EcuDiagAddress.BATTERY_ECU_ID && DiagAddress < (byte)EcuDiagAddress.DISPLAY_ECU_DIAG_ID)
			{
				ecuModel = EcuModelId.bms;
			}
            else if (DiagAddress >= (byte)EcuDiagAddress.DISPLAY_ECU_DIAG_ID)
            {
                ecuModel = EcuModelId.dEcu;
            }

            if (ecuModel == EcuModelId.none)
				return;
			
			codingForm = null; 

			switch(Global.EcuList.CurrentEcu.ModelId)
			{
				case EcuModelId.bms:
					codingForm = new Bms_ECUCoding();
					break;
				case EcuModelId.gEcu:
					codingForm = new General_ECUCoding();
					break;
				case EcuModelId.mEcu:
					codingForm = new Mcu_ECUCoding();
					break;
                case EcuModelId.dEcu:
                    codingForm = new Display_ECUCoding();
                    break;
			}


			grid.Children.Clear();
			if (codingForm != null)
			{
				codingForm.CurrentEcu = Global.EcuList.CurrentEcu;
				grid.Children.Add(codingForm as UIElement);
			}
		}


	}

	public interface IEcu_Coding
	{
		ECU CurrentEcu { get; set; }
		//bool AllowUploadCoding { get; set; }


		Task<bool> Download();
		Task<ReadDataResult_e> Upload();
		void Open(string path);
		void Save(string path);
	}
}
