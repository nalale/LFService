using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MFService
{
	public static class Tools
	{

		#region ComboBox

		public static void SetComboItem(System.Windows.Controls.Primitives.Selector combo, object obj)
		{
			if (combo == null)
				return;

			foreach (object it in combo.Items)
			{
				object tag = null;

				if (it is ComboBoxItem)
					tag = ((ComboBoxItem)it).Tag;				
				else
					continue;

				if (obj is string)  // Для строк
				{
					if (tag.ToString() == obj.ToString())
					{
						combo.SelectedItem = it;
						return;
					}
				}
				else if (tag == obj)
				{
					combo.SelectedItem = it;
					return;
				}
			}
		}
		public static void SetComboItem(System.Windows.Controls.Primitives.Selector combo, int item)
		{
			if (combo == null)
				return;

			foreach (object it in combo.Items)
			{
				object tag = null;

				if (it is ComboBoxItem)
					tag = ((ComboBoxItem)it).Tag;
				else
					continue;

				try
				{
					if (tag != null && (int)tag == item)
					{
						combo.SelectedItem = it;
						return;
					}
				}
				catch (Exception e)
				{
				}

			}
		}

		public static object GetComboItem(System.Windows.Controls.Primitives.Selector combo)
		{
			if (combo.SelectedItem == null)
				return null;

			return combo.SelectedIndex;

/*			if (combo.SelectedItem is ComboBoxItem)
				return ((ComboBoxItem)combo.SelectedItem).Tag;
			else
				return null;
				*/
		}
		//public static List<object> GetCheckedItems(System.Windows.Controls.Primitives.Selector combo)
		//{
		//	List<object> list = new List<object>();
		//	foreach (var it in combo.Items)
		//	{				
		//		if (it.IsChecked)
		//			list.Add(it.Tag);
		//	}

		//	return list;
		//}

		#endregion			


		public static string GetVersionString(UInt16 ver)
		{
			return (ver >> 8) + "." + (ver & 0xFF).ToString("d2");
		}

		public static long GetTimeMs()
		{
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public static long GetTimeMsFrom(long time)
		{
			return Tools.GetTimeMs() - time;
		}


		/// <summary>
		/// Функция рассчёта контрольной суммы для 16-битных последовательностей.
		/// </summary>
		/// <param name="buf"></param>
		public static UInt16 CRC16(UInt16[] buf, int len)
		{
			UInt16 crc = 0xFFFF;
			UInt16 DummyInt;
			for (int pos = 0; pos < len; pos++)
			{
				DummyInt = buf[pos];
				crc ^= DummyInt;
				for (int i = 8; i != 0; i--)
				{
					if ((crc & 0x0001) != 0)
					{
						crc >>= 1;
						crc ^= 0xA001;
					}
					else
						crc >>= 1;
				}
			}
			// Помните, что младший и старший байты поменяны местами, используйте соответственно (или переворачивайте)
			return crc;
		}

		public static UInt16 CRC16(short[] buf, int len)
		{
			ushort[] loc_buf = new ushort[buf.Length];
			for (int i = 0; i < buf.Length; i++)
				loc_buf[i] = (ushort)buf[i];

			return CRC16(loc_buf, len);
		}

		public static UInt16 CRC16(byte[] buf, int len)
		{
			UInt16 crc = 0xFFFF;

			for (int pos = 0; pos < len; pos++)
			{
				UInt16 DummyInt = (UInt16)((buf[pos * 2 + 1] << 8) + buf[pos * 2]);
				crc ^= DummyInt;
				for (int i = 8; i != 0; i--)
				{
					if ((crc & 0x0001) != 0)
					{
						crc >>= 1;
						crc ^= 0xA001;
					}
					else crc >>= 1;
				}
			}
			// Помните, что младший и старший байты поменяны местами, используйте соответственно (или переворачивайте)

			return crc;
		}

		/// <summary>
		/// Ищет активное окно
		/// </summary>
		public static Window GetActiveWindow()
		{
			foreach (Window win in App.Current.Windows)
			{
				if (win.IsActive)
					return win;
			}

			return null;
		}
		
	}

}
