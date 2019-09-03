﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace BoadService
{
	/// <summary>
	/// Interaction logic for Mcu_ECUCoding.xaml
	/// </summary>
	public partial class General_ECUCoding : UserControl, IEcu_Coding
	{
		General_ECU gEcu;	

		public General_ECUCoding()
		{
			InitializeComponent();

			//gEcu = new General_ECU(EcuModelId.gEcu);			

			gEcu = Global.EcuList.CurrentEcu as General_ECU;

			this.PreviewKeyDown += ListRoute_PreviewKeyDown;
			listRoutes.MouseDoubleClick += ListRoute_MouseDoubleClick;
		}

		public ECU CurrentEcu { get; set; }


		#region event handler

		private void ListRoute_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (tabControl1.SelectedItem != tabItem3)
				return;

			switch (e.Key)
			{
				case Key.Insert:
					routeAdd();
					break;
				case Key.Delete:
					routeDelete();
					break;
			}
		}

		private void ListRoute_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			routeEdit();
		}

		#endregion

		#region General		

		// Заполнение структуры данными из формы
		bool FormToStruct()
		{
			// Общие
			gEcu.Data.DiagnosticID = Convert.ToByte(tbDiagnosticAddress.Text);
			gEcu.Data.Index = Convert.ToByte(tbModuleID.Text);

			// Батареи
			gEcu.Data.DigitalOutput[0] = Convert.ToByte(tbDigOut_1.Text);
			gEcu.Data.DigitalOutput[1] = Convert.ToByte(tbDigOut_2.Text);
			gEcu.Data.DigitalOutput[2] = Convert.ToByte(tbDigOut_3.Text);
			gEcu.Data.DigitalOutput[3] = Convert.ToByte(tbDigOut_4.Text);
			gEcu.Data.DigitalOutput[4] = Convert.ToByte(tbDigOut_5.Text);
			gEcu.Data.DigitalOutput[5] = Convert.ToByte(tbDigOut_6.Text);
			gEcu.Data.DigitalOutput[6] = Convert.ToByte(tbDigOut_7.Text);
			gEcu.Data.DigitalOutput[7] = Convert.ToByte(tbDigOut_8.Text);
			gEcu.Data.DigitalOutput[8] = Convert.ToByte(tbDigOut_9.Text);
			gEcu.Data.DigitalOutput[9] = Convert.ToByte(tbDigOut_10.Text);

			gEcu.Data.AnalogOutput[0] = Convert.ToByte(tbAnOut_1.Text);
			gEcu.Data.AnalogOutput[1] = Convert.ToByte(tbAnOut_2.Text);
			gEcu.Data.AnalogOutput[2] = Convert.ToByte(tbAnOut_3.Text);
			gEcu.Data.AnalogOutput[3] = Convert.ToByte(tbAnOut_4.Text);

            gEcu.Data.CurrentThreshold_A[0] = Convert.ToByte(tbCurLimPwmOut_1.Text);
            gEcu.Data.CurrentThreshold_A[1] = Convert.ToByte(tbCurLimPwmOut_2.Text);
            gEcu.Data.CurrentThreshold_A[2] = Convert.ToByte(tbCurLimPwmOut_3.Text);
            gEcu.Data.CurrentThreshold_A[3] = Convert.ToByte(tbCurLimPwmOut_4.Text);

            gEcu.IsPowerManger = Convert.ToBoolean(cbIsPM.IsChecked);
            gEcu.Data.PowerOffDelay_ms = Convert.ToUInt16(tbPowerOffDelay.Text);
            gEcu.Data.KeyOffTime_ms = Convert.ToUInt16(tbKeyOffTime.Text);

            gEcu.PullUp_IN1 = Convert.ToBoolean(cbPuIn_1.IsChecked);
            gEcu.PullUp_IN2 = Convert.ToBoolean(cbPuIn_2.IsChecked);
            gEcu.PullUp_IN3 = Convert.ToBoolean(cbPuIn_3.IsChecked);
            gEcu.PullUp_IN4 = Convert.ToBoolean(cbPuIn_4.IsChecked);

            return true;
		}

		void StructToForm()
		{
			tbDiagnosticAddress.Text = gEcu.Data.DiagnosticID.ToString();

			// Общие
			tbDiagnosticAddress .Text = gEcu.Data.DiagnosticID.ToString();
			tbModuleID.Text = gEcu.Data.Index.ToString();

			// Батареи
			tbDigOut_1.Text = gEcu.Data.DigitalOutput[0].ToString();
			tbDigOut_2.Text = gEcu.Data.DigitalOutput[1].ToString();
			tbDigOut_3.Text = gEcu.Data.DigitalOutput[2].ToString();
			tbDigOut_4.Text = gEcu.Data.DigitalOutput[3].ToString();
			tbDigOut_5.Text = gEcu.Data.DigitalOutput[4].ToString();
			tbDigOut_6.Text = gEcu.Data.DigitalOutput[5].ToString();
			tbDigOut_7.Text = gEcu.Data.DigitalOutput[6].ToString();
			tbDigOut_8.Text = gEcu.Data.DigitalOutput[7].ToString();
			tbDigOut_9.Text = gEcu.Data.DigitalOutput[8].ToString();
			tbDigOut_10.Text = gEcu.Data.DigitalOutput[9].ToString();

			tbAnOut_1.Text = gEcu.Data.AnalogOutput[0].ToString();
			tbAnOut_2.Text = gEcu.Data.AnalogOutput[1].ToString();
			tbAnOut_3.Text = gEcu.Data.AnalogOutput[2].ToString();
			tbAnOut_4.Text = gEcu.Data.AnalogOutput[3].ToString();

            tbCurLimPwmOut_1.Text = gEcu.Data.CurrentThreshold_A[0].ToString();
            tbCurLimPwmOut_2.Text = gEcu.Data.CurrentThreshold_A[1].ToString();
            tbCurLimPwmOut_3.Text = gEcu.Data.CurrentThreshold_A[2].ToString();
            tbCurLimPwmOut_4.Text = gEcu.Data.CurrentThreshold_A[3].ToString();

            cbIsPM.IsChecked = Convert.ToBoolean(gEcu.IsPowerManger);
            tbPowerOffDelay.Text = gEcu.Data.PowerOffDelay_ms.ToString();
            tbKeyOffTime.Text = gEcu.Data.KeyOffTime_ms.ToString();

            cbPuIn_1.IsChecked = Convert.ToBoolean(gEcu.PullUp_IN1);
            cbPuIn_2.IsChecked = Convert.ToBoolean(gEcu.PullUp_IN2);
            cbPuIn_3.IsChecked = Convert.ToBoolean(gEcu.PullUp_IN3);
            cbPuIn_4.IsChecked = Convert.ToBoolean(gEcu.PullUp_IN4);
        }

		void FillRoutes()
		{
			listRoutes.ItemsSource = null;
			List<string> items = new List<string>();

			for (int i = 0; i < gEcu.RepeaterTableSize; i++)
			{
				if (gEcu.Data.tab[i].IsActive)
				{		
					items.Add(gEcu.Data.tab[i].ToString());
				}
			}

			listRoutes.ItemsSource = items;
		}

		void routeAdd()
		{
			int ind = 0;
			for (int i = 0; i < gEcu.RepeaterTableSize; i++)
			{
				if (gEcu.Data.tab[i].IsActive)
					ind++;
				else
					break;
			}

			if (ind >= gEcu.RepeaterTableSize)
				return;

			frmRepItemEditor dlg = new frmRepItemEditor();
			dlg.edItem = gEcu.Data.tab[ind];
			dlg.edItem.Id1 = 0xFFFFFFFF;
            dlg.edItem.Id2 = 0xFFFFFFFF;
            dlg.edItem.Ext1 = false;
            dlg.edItem.Ext2 = false;
            dlg.edItem.SendPeriod = 0;
			dlg.edItem.RepCount = 10;
			dlg.edItem.Direction = false;

			dlg.ShowDialog();
			if (dlg.DialogResult != true)
				return;

			dlg.edItem.IsActive = true;
			gEcu.Data.tab[ind] = dlg.edItem;
			FillRoutes();
		}

		void routeDelete()
		{
			if (listRoutes.SelectedItem == null)
				return;

			int ind = (int)listRoutes.SelectedIndex;
			if (ind >= gEcu.RepeaterTableSize)
				return;

			// Удаляем
			gEcu.Data.tab[ind].IsActive = false;

            // Пересоздаем список активных сообщений
            List<General_ECU.canRepItem> list = new List<General_ECU.canRepItem>();
            for (int i = 0; i < gEcu.RepeaterTableSize; i++)
            {
                if (gEcu.Data.tab[i].IsActive)
                    list.Add(gEcu.Data.tab[i]);
            }

            Array.Clear(gEcu.Data.tab, 0, gEcu.Data.tab.Length);

            // Перемещаем все активные в начало списка
            for (int i = 0; i < gEcu.RepeaterTableSize; i++)
            {
                if (i < list.Count)
                    gEcu.Data.tab[i] = list[i];
                else
                    gEcu.Data.tab[i].IsActive = false;
            }

            FillRoutes();
		}

		void routeEdit()
		{
			if (listRoutes.SelectedItem == null)
				return;
			int ind = (int)listRoutes.SelectedIndex;
			if (ind >= gEcu.RepeaterTableSize)
				return;

			frmRepItemEditor dlg = new frmRepItemEditor();
			dlg.edItem = gEcu.Data.tab[ind];

			dlg.ShowDialog();
			if (dlg.DialogResult != true)
				return;

			gEcu.Data.tab[ind] = dlg.edItem;
			FillRoutes();
		}

		#endregion



		#region Coding


		async public Task<bool> Download()
		{
			IntPtr ptr;
			int ProfileSize = Marshal.SizeOf(typeof(General_ECU.CodingData_t));
			
			ptr = Marshal.AllocHGlobal(ProfileSize);
			byte[] buf = await Global.diag.ReadDataByID((byte)CurrentEcu.Address, 0);

			Marshal.Copy(buf, 0, ptr, ProfileSize);
			gEcu.Data = (General_ECU.CodingData_t)Marshal.PtrToStructure(ptr, typeof(General_ECU.CodingData_t));

			ushort loc_crc = Tools.CRC16(buf, ProfileSize / 4 - 1);
			if (gEcu.Data.CRC == loc_crc)
			{
				StructToForm();
				FillRoutes();
			}
			else
				return false;

			return true; 


		}

		public async Task<ReadDataResult_e> Upload()
		{
			IntPtr ptr;
			if (FormToStruct())
			{				
				int ProfileSize = Marshal.SizeOf(typeof(General_ECU.CodingData_t));		
				
				ptr = Marshal.AllocHGlobal(ProfileSize);
				Marshal.StructureToPtr(gEcu.Data, ptr, false);

				byte[] buf = new byte[ProfileSize];				

				Marshal.Copy(ptr, buf, 0, buf.Length);
				gEcu.Data.CRC = Tools.CRC16(buf, buf.Length / 4 - 1);

				Marshal.StructureToPtr(gEcu.Data, ptr, false);
				Marshal.Copy(ptr, buf, 0, buf.Length);				

				Marshal.FreeHGlobal(ptr);

				bool res = await Global.diag.WriteDataByID((byte)CurrentEcu.Address, (byte)General_ECU.ObjectsIndex_e.didConfigStructIndex, buf);

                return (res) ? ReadDataResult_e.RES_SUCCESSFUL : ReadDataResult_e.RES_OTHER_ERROR;
            }
            return ReadDataResult_e.RES_INCORRECT_DATA;
        }

		public void Open(string path)
		{

		}

		public void Save(string path)
		{

		}

		#endregion

		private void apply_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
