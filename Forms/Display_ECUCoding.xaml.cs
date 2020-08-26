using System;
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
using System.Xml;
using System.Runtime.InteropServices;

namespace MFService
{
	/// <summary>
	/// Interaction logic for Mcu_ECUCoding.xaml
	/// </summary>
	public partial class Display_ECUCoding : UserControl, IEcu_Coding
	{
        Display_ECU dEcu;	

		public Display_ECUCoding()
		{
			InitializeComponent();				

			dEcu = Global.EcuList.CurrentEcu as Display_ECU;
		}

		public ECU CurrentEcu { get; set; }


		#region General		

		// Заполнение структуры данными из формы
		bool FormToStruct()
		{
			// Общие
			dEcu.Data.DiagnosticID = Convert.ToByte(tbDiagnosticAddress.Text);
			dEcu.Data.Index = Convert.ToByte(tbModuleID.Text);

			// Батареи
			dEcu.Data.MotorRpm[0] = Convert.ToInt16(tbSpeed1.Text);
			dEcu.Data.MotorRpm[1] = Convert.ToInt16(tbSpeed2.Text);
			dEcu.Data.MotorRpm[2] = Convert.ToInt16(tbSpeed3.Text);
			dEcu.Data.MotorRpm[3] = Convert.ToInt16(tbSpeed4.Text);
			dEcu.Data.MotorRpm[4] = Convert.ToInt16(tbSpeed5.Text);
			dEcu.Data.MotorRpm[5] = Convert.ToInt16(tbSpeed6.Text);
            dEcu.Data.MotorRpm[6] = Convert.ToInt16(tbMin1.Text);
            dEcu.Data.MotorRpm[7] = Convert.ToInt16(tbMin2.Text);
            dEcu.Data.MotorRpm[8] = Convert.ToInt16(tbMin3.Text);
            dEcu.Data.MotorRpm[9] = Convert.ToInt16(tbMin4.Text);
            dEcu.Data.MotorRpm[10] = Convert.ToInt16(tbMin5.Text);
            dEcu.Data.MotorRpm[11] = Convert.ToInt16(tbMin6.Text);

            dEcu.Data.SoC[0] = Convert.ToByte(tbSoc1.Text);
			dEcu.Data.SoC[1] = Convert.ToByte(tbSoc2.Text);
			dEcu.Data.SoC[2] = Convert.ToByte(tbSoc3.Text);
			dEcu.Data.SoC[3] = Convert.ToByte(tbSoc4.Text);
            dEcu.Data.SoC[4] = Convert.ToByte(tbSoc5.Text);
            dEcu.Data.SoC[5] = Convert.ToByte(tbSoc6.Text);
            dEcu.Data.SoC[6] = Convert.ToByte(tbSocPwm1.Text);
            dEcu.Data.SoC[7] = Convert.ToByte(tbSocPwm2.Text);
            dEcu.Data.SoC[8] = Convert.ToByte(tbSocPwm3.Text);
            dEcu.Data.SoC[9] = Convert.ToByte(tbSocPwm4.Text);
            dEcu.Data.SoC[10] = Convert.ToByte(tbSocPwm5.Text);
            dEcu.Data.SoC[11] = Convert.ToByte(tbSocPwm6.Text);

            dEcu.Data.TrimPosition[0] = Convert.ToByte(tbTrim1.Text);
            dEcu.Data.TrimPosition[1] = Convert.ToByte(tbTrim2.Text);
            dEcu.Data.TrimPosition[2] = Convert.ToByte(tbTrim3.Text);
            dEcu.Data.TrimPosition[3] = Convert.ToByte(tbTrim4.Text);
            dEcu.Data.TrimPosition[4] = Convert.ToByte(tbTrim5.Text);
            dEcu.Data.TrimPosition[5] = Convert.ToByte(tbTrim6.Text);
            dEcu.Data.TrimPosition[6] = Convert.ToByte(tbTrimPwm1.Text);
            dEcu.Data.TrimPosition[7] = Convert.ToByte(tbTrimPwm2.Text);
            dEcu.Data.TrimPosition[8] = Convert.ToByte(tbTrimPwm3.Text);
            dEcu.Data.TrimPosition[9] = Convert.ToByte(tbTrimPwm4.Text);
            dEcu.Data.TrimPosition[10] = Convert.ToByte(tbTrimPwm5.Text);
            dEcu.Data.TrimPosition[11] = Convert.ToByte(tbTrimPwm6.Text);

            dEcu.Data.SpecPower[0] = Convert.ToInt16(tbCons1.Text);
            dEcu.Data.SpecPower[1] = Convert.ToInt16(tbCons2.Text);
            dEcu.Data.SpecPower[2] = Convert.ToInt16(tbCons3.Text);
            dEcu.Data.SpecPower[3] = Convert.ToInt16(tbCons4.Text);
            dEcu.Data.SpecPower[4] = Convert.ToInt16(tbCons5.Text);
            dEcu.Data.SpecPower[5] = Convert.ToInt16(tbCons6.Text);
            dEcu.Data.SpecPower[6] = Convert.ToInt16(tbConsPwm1.Text);
            dEcu.Data.SpecPower[7] = Convert.ToInt16(tbConsPwm2.Text);
            dEcu.Data.SpecPower[8] = Convert.ToInt16(tbConsPwm3.Text);
            dEcu.Data.SpecPower[9] = Convert.ToInt16(tbConsPwm4.Text);
            dEcu.Data.SpecPower[10] = Convert.ToInt16(tbConsPwm5.Text);
            dEcu.Data.SpecPower[11] = Convert.ToInt16(tbConsPwm6.Text);


            dEcu.Data.PowerOffDelay_ms = Convert.ToUInt16(tbPowerOffDelay.Text);
            dEcu.Data.KeyOffTime_ms = Convert.ToUInt16(tbKeyOffTime.Text);

            return true;
		}
		void StructToForm()
		{
			tbDiagnosticAddress.Text = dEcu.Data.DiagnosticID.ToString();

			// Общие
			tbDiagnosticAddress .Text = dEcu.Data.DiagnosticID.ToString();
			tbModuleID.Text = dEcu.Data.Index.ToString();

            tbSpeed1.Text = Convert.ToString(dEcu.Data.MotorRpm[0]);
            tbSpeed2.Text = Convert.ToString(dEcu.Data.MotorRpm[1]);
            tbSpeed3.Text = Convert.ToString(dEcu.Data.MotorRpm[2]);
            tbSpeed4.Text = Convert.ToString(dEcu.Data.MotorRpm[3]);
            tbSpeed5.Text = Convert.ToString(dEcu.Data.MotorRpm[4]);
            tbSpeed6.Text = Convert.ToString(dEcu.Data.MotorRpm[5]);
            tbMin1.Text = Convert.ToString(dEcu.Data.MotorRpm[6]);
            tbMin2.Text = Convert.ToString(dEcu.Data.MotorRpm[7]);
            tbMin3.Text = Convert.ToString(dEcu.Data.MotorRpm[8]);
            tbMin4.Text = Convert.ToString(dEcu.Data.MotorRpm[9]);
            tbMin5.Text = Convert.ToString(dEcu.Data.MotorRpm[10]);
            tbMin6.Text = Convert.ToString(dEcu.Data.MotorRpm[11]);

            tbSoc1.Text = Convert.ToString(dEcu.Data.SoC[0]);
            tbSoc2.Text = Convert.ToString(dEcu.Data.SoC[1]);
            tbSoc3.Text = Convert.ToString(dEcu.Data.SoC[2]);
            tbSoc4.Text = Convert.ToString(dEcu.Data.SoC[3]);
            tbSoc5.Text = Convert.ToString(dEcu.Data.SoC[4]);
            tbSoc6.Text = Convert.ToString(dEcu.Data.SoC[5]);
            tbSocPwm1.Text = Convert.ToString(dEcu.Data.SoC[6]);
            tbSocPwm2.Text = Convert.ToString(dEcu.Data.SoC[7]);
            tbSocPwm3.Text = Convert.ToString(dEcu.Data.SoC[8]);
            tbSocPwm4.Text = Convert.ToString(dEcu.Data.SoC[9]);
            tbSocPwm5.Text = Convert.ToString(dEcu.Data.SoC[10]);
            tbSocPwm6.Text = Convert.ToString(dEcu.Data.SoC[11]);

            tbTrim1.Text = Convert.ToString(dEcu.Data.TrimPosition[0]);
            tbTrim2.Text = Convert.ToString(dEcu.Data.TrimPosition[1]);
            tbTrim3.Text = Convert.ToString(dEcu.Data.TrimPosition[2]);
            tbTrim4.Text = Convert.ToString(dEcu.Data.TrimPosition[3]);
            tbTrim5.Text = Convert.ToString(dEcu.Data.TrimPosition[4]);
            tbTrim6.Text = Convert.ToString(dEcu.Data.TrimPosition[5]);
            tbTrimPwm1.Text = Convert.ToString(dEcu.Data.TrimPosition[6]);
            tbTrimPwm2.Text = Convert.ToString(dEcu.Data.TrimPosition[7]);
            tbTrimPwm3.Text = Convert.ToString(dEcu.Data.TrimPosition[8]);
            tbTrimPwm4.Text = Convert.ToString(dEcu.Data.TrimPosition[9]);
            tbTrimPwm5.Text = Convert.ToString(dEcu.Data.TrimPosition[10]);
            tbTrimPwm6.Text = Convert.ToString(dEcu.Data.TrimPosition[11]);

            tbCons1.Text = Convert.ToString(dEcu.Data.SpecPower[0]);
            tbCons2.Text = Convert.ToString(dEcu.Data.SpecPower[1]);
            tbCons3.Text = Convert.ToString(dEcu.Data.SpecPower[2]);
            tbCons4.Text = Convert.ToString(dEcu.Data.SpecPower[3]);
            tbCons5.Text = Convert.ToString(dEcu.Data.SpecPower[4]);
            tbCons6.Text = Convert.ToString(dEcu.Data.SpecPower[5]);
            tbConsPwm1.Text = Convert.ToString(dEcu.Data.SpecPower[6]);
            tbConsPwm2.Text = Convert.ToString(dEcu.Data.SpecPower[7]);
            tbConsPwm3.Text = Convert.ToString(dEcu.Data.SpecPower[8]);
            tbConsPwm4.Text = Convert.ToString(dEcu.Data.SpecPower[9]);
            tbConsPwm5.Text = Convert.ToString(dEcu.Data.SpecPower[10]);
            tbConsPwm6.Text = Convert.ToString(dEcu.Data.SpecPower[11]);

            tbPowerOffDelay.Text = dEcu.Data.PowerOffDelay_ms.ToString();
            tbKeyOffTime.Text = dEcu.Data.KeyOffTime_ms.ToString();

        }

		#endregion



		#region Coding
		async public Task<bool> Download()
		{
			IntPtr ptr;
			int ProfileSize = Marshal.SizeOf(typeof(Display_ECU.CodingData_t));
			
			ptr = Marshal.AllocHGlobal(ProfileSize);
			byte[] buf = await Global.diag.ReadDataByID((byte)CurrentEcu.Address, 0);

			Marshal.Copy(buf, 0, ptr, ProfileSize);
			dEcu.Data = (Display_ECU.CodingData_t)Marshal.PtrToStructure(ptr, typeof(Display_ECU.CodingData_t));

			ushort loc_crc = Tools.CRC16(buf, ProfileSize / 4 - 1);
			if (dEcu.Data.CRC == loc_crc)
			{
				StructToForm();
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
				int ProfileSize = Marshal.SizeOf(typeof(Display_ECU.CodingData_t));		
				
				ptr = Marshal.AllocHGlobal(ProfileSize);
				Marshal.StructureToPtr(dEcu.Data, ptr, false);

				byte[] buf = new byte[ProfileSize];				

				Marshal.Copy(ptr, buf, 0, buf.Length);
				dEcu.Data.CRC = Tools.CRC16(buf, buf.Length / 4 - 1);

				Marshal.StructureToPtr(dEcu.Data, ptr, false);
				Marshal.Copy(ptr, buf, 0, buf.Length);				

				Marshal.FreeHGlobal(ptr);

				bool res = await Global.diag.WriteDataByID((byte)CurrentEcu.Address, (byte)Display_ECU.ObjectsIndex_e.didConfigStructIndex, buf);

                return (res) ? ReadDataResult_e.RES_SUCCESSFUL : ReadDataResult_e.RES_OTHER_ERROR;
            }
            return ReadDataResult_e.RES_INCORRECT_DATA;
        }

		public void Open(string path)
		{
            // Загружаем файл
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path);
                if (LoadFromXml(doc.DocumentElement))
                {
                    StructToForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить файл. \nОшибка: " + ex.Message);
            }
        }
		public void Save(string path)
		{
            if (FormToStruct() == false)
                return;

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(ToXml(doc));


            // Запись документа в файл
            XmlTextWriter writer = new XmlTextWriter(path, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = ' '; // задаем отступ
            doc.WriteContentTo(writer);
            writer.Flush();
            writer.Close();
        }

		#endregion

		private void apply_Click(object sender, RoutedEventArgs e)
		{

		}

        #region XML

        XmlNode ToXml(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("Coding");

            XmlElement el;

            AddXmlElement(root, "EcuClassId", ((int)CurrentEcu.ModelId).ToString());

            AddXmlElement(root, "DiagnosticID", dEcu.Data.DiagnosticID.ToString());
            AddXmlElement(root, "Index", dEcu.Data.Index.ToString());

            // MotorRpm
            el = AddXmlElement(root, "MotorRpmpoint", "");
            foreach (Int16 val in dEcu.Data.MotorRpm)
                AddXmlElement(el, "val", val.ToString());
            // SOCpoint
            el = AddXmlElement(root, "Socpoint", "");
            foreach (Int16 val in dEcu.Data.SoC)
                AddXmlElement(el, "val", val.ToString());
            // TrimPospoint
            el = AddXmlElement(root, "TrimPospoint", "");
            foreach (Int16 val in dEcu.Data.TrimPosition)
                AddXmlElement(el, "val", val.ToString());
            // SpecPowerpoint
            el = AddXmlElement(root, "SpecPowerpoint", "");
            foreach (Int16 val in dEcu.Data.SpecPower)
                AddXmlElement(el, "val", val.ToString());


            AddXmlElement(root, "PowerOffDelay_ms", dEcu.Data.PowerOffDelay_ms.ToString());
            AddXmlElement(root, "KeyOffTime_ms", dEcu.Data.KeyOffTime_ms.ToString());

            return root;
        }

        bool LoadFromXml(XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "EcuClassId")
                {
                    EcuModelId cid = (EcuModelId)Convert.ToInt32(n.InnerText);
                    if (cid != CurrentEcu.ModelId)
                    {
                        MessageBox.Show("Файл кодировок предназначен для ЭБУ класса \"" + cid.ToString() + "\".");
                        return false;
                    }
                }
                else if (n.Name == "DiagnosticID")
                    dEcu.Data.DiagnosticID = (byte)Convert.ToByte(n.InnerText);
                else if (n.Name == "Index")
                    dEcu.Data.Index = (byte)Convert.ToByte(n.InnerText);

                else if (n.Name == "MotorRpmpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.VoltageArrayLen)
                            dEcu.Data.MotorRpm[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }
                else if (n.Name == "Socpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.TemperatureArrayLen)
                            dEcu.Data.SoC[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }
                else if (n.Name == "TrimPospoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.OCVArrayLen)
                            dEcu.Data.TrimPosition[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }
                else if (n.Name == "SpecPowerpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.OCVArrayLen)
                            dEcu.Data.SpecPower[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }

                else if (n.Name == "PowerOffDelay_ms")
                    dEcu.Data.PowerOffDelay_ms = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "KeyOffTime_ms")
                    dEcu.Data.KeyOffTime_ms = Convert.ToUInt16(n.InnerText);

            }

            return true;
        }


        XmlElement AddXmlElement(XmlElement root, string name, string val)
        {
            XmlElement el = root.OwnerDocument.CreateElement(name);
            el.InnerText = val;
            root.AppendChild(el);
            return el;
        }

        #endregion
    }
}
