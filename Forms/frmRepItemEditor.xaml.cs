using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static BoadService.General_ECU;

namespace BoadService
{
	/// <summary>
	/// Логика взаимодействия для frmSearchECU.xaml
	/// </summary>
	public partial class frmRepItemEditor : Window
	{
		public canRepItem edItem;

		public frmRepItemEditor()
		{
			InitializeComponent();

			Window owner = Tools.GetActiveWindow();
			// Если не нашли активное окно, то делаем диалог Topmost
			if (owner != null)
				this.Owner = owner;
			else
				this.Topmost = true;

			this.Loaded += FrmSearchECU_Loaded;
			this.Unloaded += FrmSearchECU_Unloaded;
		}



		#region Event Handlers

		private void FrmSearchECU_Loaded(object sender, RoutedEventArgs e)
		{
			FillForm();
		}

		private void FrmSearchECU_Unloaded(object sender, RoutedEventArgs e)
		{

		}


		private void CommitButtonsClick(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			
			if (tbCanId1.Text.Trim() == "")
			{
				MessageBox.Show("Введите значение CAN ID", "");
				return;
			}

			UInt32 id1 = Convert.ToUInt32(tbCanId1.Text, 16);
            UInt32 id2 = Convert.ToUInt32(tbCanId2.Text, 16);			

			edItem.Id1 = id1;
            edItem.Id2 = id2;
            edItem.Ext1 = checkExt1.IsChecked.Value;
            edItem.Ext2 = checkExt2.IsChecked.Value;
            edItem.SendPeriod = Convert.ToUInt16(tbSendPeriod.Text);
			edItem.Direction = checkDirection.IsChecked.Value;
			edItem.RepCount = Convert.ToByte(tbRepCount.Text);

			this.DialogResult = true;

			this.Close();
		}

		#endregion


		#region General

		void FillForm()
		{
			if (edItem.Id1 != 0xFFFFFFFF)
				tbCanId1.Text = "0x" + edItem.Id1.ToString("X8");

            if (edItem.Id2 != 0xFFFFFFFF)
                tbCanId2.Text = "0x" + edItem.Id2.ToString("X8");

            checkExt1.IsChecked = edItem.Ext1;
            checkExt2.IsChecked = edItem.Ext2;
            tbSendPeriod.Text = edItem.SendPeriod.ToString();
			checkDirection.IsChecked = edItem.Direction;
			tbRepCount.Text = edItem.RepCount.ToString();

			tbCanId1.Focus();
		}

		#endregion
	}
}
