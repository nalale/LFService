using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
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
	public partial class Mcu_ECUCoding : UserControl, IEcu_Coding
	{
		Main_ECU mEcu;
		byte mEcu_id = 0;

		public Mcu_ECUCoding()
		{
			InitializeComponent();
            mEcu = Global.EcuList.CurrentEcu as Main_ECU;
        }

		public ECU CurrentEcu { get; set; }

		#region General

		void StructToForm()
		{
            // Общие
            tbDiagnosticAddress.Text = mEcu.Data.DiagnosticID.ToString();
            tbBaseCANID.Text = mEcu.Data.BaseCanId.ToString();

            // Ручка
            tbAcc1MaxPos.Text = mEcu.Data.AccPedalFstCh_MaxV.ToString();
            tbAcc1NPos.Text = mEcu.Data.AccPedalFstCh_0V.ToString();
            tbAcc2MaxPos.Text = mEcu.Data.AccPedalSndCh_MaxV.ToString();

            // Привод
            tbMaxForwardSpeed.Text = mEcu.Data.MaxMotorSpeedD.ToString();
            tbMaxTorque.Text = mEcu.Data.MaxTorque.ToString();
            tbMaxMotorT.Text = mEcu.Data.MaxMotorT.ToString();
            tbMaxInverterT.Text = mEcu.Data.MaxInverterT.ToString();
            tbInvCoolingOn.Text = mEcu.Data.InvCoolingOn.ToString();
            tbMotorCoolingOn.Text = mEcu.Data.MotorCoolingOn.ToString();

            // Рулевое управление
            tbsteerMaxVal.Text = mEcu.Data.SteeringMaxVal_0p1V.ToString();
            tbsteerMinVal.Text = mEcu.Data.SteeringMinVal_0p1V.ToString();
            tbsteerMaxCurrent.Text = mEcu.Data.SteeringMaxCurrent_0p1A.ToString();

            tbSpeed1.Text = mEcu.Data.SteeringBrakeSpeedTable[0].ToString();
            tbSpeed2.Text = mEcu.Data.SteeringBrakeSpeedTable[1].ToString();
            tbSpeed3.Text = mEcu.Data.SteeringBrakeSpeedTable[2].ToString();
            tbSpeed4.Text = mEcu.Data.SteeringBrakeSpeedTable[3].ToString();
            tbSpeed5.Text = mEcu.Data.SteeringBrakeSpeedTable[4].ToString();
            tbSpeed6.Text = mEcu.Data.SteeringBrakeSpeedTable[5].ToString();
            tbMin1.Text = mEcu.Data.SteeringBrakeSpeedTable[6].ToString();
            tbMin2.Text = mEcu.Data.SteeringBrakeSpeedTable[7].ToString();
            tbMin3.Text = mEcu.Data.SteeringBrakeSpeedTable[8].ToString();
            tbMin4.Text = mEcu.Data.SteeringBrakeSpeedTable[9].ToString();
            tbMin5.Text = mEcu.Data.SteeringBrakeSpeedTable[10].ToString();
            tbMin6.Text = mEcu.Data.SteeringBrakeSpeedTable[11].ToString();

            tbSteeringKp.Text = mEcu.Data.SteeringKp.ToString();
            tbSteeringKi.Text = mEcu.Data.SteeringKi.ToString();
            tbSteeringKd.Text = mEcu.Data.SteeringKd.ToString();

            // Трим
            tbtrimMaxVal.Text = mEcu.Data.TrimMaxVal_0p1V.ToString();
            tbtrimMinVal.Text = mEcu.Data.TrimMinVal_0p1V.ToString();
            tbtrimMaxCurrent.Text = mEcu.Data.TrimMaxCurrent_0p1A.ToString();

            

            // Разное
            checkPowerManager.IsChecked = Convert.ToBoolean(mEcu.IsPowerManger);
            tbPowerOffDelay.Text = mEcu.Data.PowerOffDelay_ms.ToString();
            tbKeyOffTime.Text = mEcu.Data.KeyOffTime_ms.ToString();

            tbMaxCharhCurrent.Text = mEcu.Data.MaxChargingCurrent_A.ToString();
            tbChargersNum.Text = mEcu.Data.ChargersNumber.ToString();
        }

		bool FormToStruct()
		{
            // Общие
            mEcu.Data.DiagnosticID = Convert.ToByte(tbDiagnosticAddress.Text);
            mEcu.Data.BaseCanId = Convert.ToUInt16(tbBaseCANID.Text);

            // Ручка
            mEcu.Data.AccPedalFstCh_MaxV = Convert.ToByte(tbAcc1MaxPos.Text);
            mEcu.Data.AccPedalFstCh_0V = Convert.ToByte(tbAcc1NPos.Text);
            mEcu.Data.AccPedalSndCh_MaxV = Convert.ToByte(tbAcc2MaxPos.Text);

            // Привод
            mEcu.Data.MaxMotorSpeedD = Convert.ToUInt16(tbMaxForwardSpeed.Text);
            mEcu.Data.MaxTorque = Convert.ToUInt16(tbMaxTorque.Text);
            mEcu.Data.MaxMotorT = Convert.ToInt16(tbMaxMotorT.Text);
            mEcu.Data.MaxInverterT = Convert.ToInt16(tbMaxInverterT.Text);
            mEcu.Data.InvCoolingOn = Convert.ToByte(tbInvCoolingOn.Text);
            mEcu.Data.MotorCoolingOn = Convert.ToByte(tbMotorCoolingOn.Text);

            // Рулевое управление
            mEcu.Data.SteeringMaxVal_0p1V = Convert.ToByte(tbsteerMaxVal.Text);
            mEcu.Data.SteeringMinVal_0p1V = Convert.ToByte(tbsteerMinVal.Text);
            mEcu.Data.SteeringMaxCurrent_0p1A = Convert.ToInt16(tbsteerMaxCurrent.Text);

            mEcu.Data.SteeringBrakeSpeedTable[0] = Convert.ToUInt16(tbSpeed1.Text);
            mEcu.Data.SteeringBrakeSpeedTable[1] = Convert.ToUInt16(tbSpeed2.Text);
            mEcu.Data.SteeringBrakeSpeedTable[2] = Convert.ToUInt16(tbSpeed3.Text);
            mEcu.Data.SteeringBrakeSpeedTable[3] = Convert.ToUInt16(tbSpeed4.Text);
            mEcu.Data.SteeringBrakeSpeedTable[4] = Convert.ToUInt16(tbSpeed5.Text);
            mEcu.Data.SteeringBrakeSpeedTable[5] = Convert.ToUInt16(tbSpeed6.Text);
            mEcu.Data.SteeringBrakeSpeedTable[6] = Convert.ToUInt16(tbMin1.Text);
            mEcu.Data.SteeringBrakeSpeedTable[7] = Convert.ToUInt16(tbMin2.Text);
            mEcu.Data.SteeringBrakeSpeedTable[8] = Convert.ToUInt16(tbMin3.Text);
            mEcu.Data.SteeringBrakeSpeedTable[9] = Convert.ToUInt16(tbMin4.Text);
            mEcu.Data.SteeringBrakeSpeedTable[10] = Convert.ToUInt16(tbMin5.Text);
            mEcu.Data.SteeringBrakeSpeedTable[11] = Convert.ToUInt16(tbMin6.Text);

            mEcu.Data.SteeringKp = Convert.ToByte(tbSteeringKp.Text);
            mEcu.Data.SteeringKi = Convert.ToByte(tbSteeringKi.Text);
            mEcu.Data.SteeringKd = Convert.ToByte(tbSteeringKd.Text);

            // Трим
            mEcu.Data.TrimMaxVal_0p1V = Convert.ToByte(tbtrimMaxVal.Text);
            mEcu.Data.TrimMinVal_0p1V = Convert.ToByte(tbtrimMinVal.Text);
            mEcu.Data.TrimMaxCurrent_0p1A = Convert.ToInt16(tbtrimMaxCurrent.Text);

            // Разное
            mEcu.IsPowerManger = Convert.ToBoolean(checkPowerManager.IsChecked);
            mEcu.Data.PowerOffDelay_ms = Convert.ToUInt16(tbPowerOffDelay.Text);
            mEcu.Data.KeyOffTime_ms = Convert.ToUInt16(tbKeyOffTime.Text);

            mEcu.Data.MaxChargingCurrent_A = Convert.ToByte(tbMaxCharhCurrent.Text);
            mEcu.Data.ChargersNumber = Convert.ToByte(tbChargersNum.Text);
            return true;
		}


		#endregion


		#region coding
		async public Task<bool> Download()
		{
            IntPtr ptr;
            int ProfileSize = Marshal.SizeOf(typeof(Main_ECU.CodingData_t));

            ptr = Marshal.AllocHGlobal(ProfileSize);
            byte[] buf = await Global.diag.ReadDataByID((byte)CurrentEcu.Address, 0);

            Marshal.Copy(buf, 0, ptr, ProfileSize);
            mEcu.Data = (Main_ECU.CodingData_t)Marshal.PtrToStructure(ptr, typeof(Main_ECU.CodingData_t));

            ushort loc_crc = Tools.CRC16(buf, ProfileSize / 4 - 1);
            if (mEcu.Data.CRC == loc_crc)
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
                int ProfileSize = Marshal.SizeOf(typeof(Main_ECU.CodingData_t));

                ptr = Marshal.AllocHGlobal(ProfileSize);
                Marshal.StructureToPtr(mEcu.Data, ptr, false);

                byte[] buf = new byte[ProfileSize];

                Marshal.Copy(ptr, buf, 0, buf.Length);
                mEcu.Data.CRC = Tools.CRC16(buf, buf.Length / 4 - 1);

                Marshal.StructureToPtr(mEcu.Data, ptr, false);
                Marshal.Copy(ptr, buf, 0, buf.Length);

                Marshal.FreeHGlobal(ptr);

                bool res = await Global.diag.WriteDataByID((byte)CurrentEcu.Address, 1, buf);

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

        #region XML

        XmlNode ToXml(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("Coding");

            XmlElement el;

            AddXmlElement(root, "EcuClassId", ((int)CurrentEcu.ModelId).ToString());

            AddXmlElement(root, "DiagnosticID", mEcu.Data.DiagnosticID.ToString());
            AddXmlElement(root, "AccPedalFstCh_0V", mEcu.Data.AccPedalFstCh_0V.ToString());
            AddXmlElement(root, "AccPedalFstCh_MaxV", mEcu.Data.AccPedalFstCh_MaxV.ToString());
            AddXmlElement(root, "fltSteeringPeriod", mEcu.Data.SteeringKp.ToString());
            AddXmlElement(root, "AccPedalSndCh_0V", mEcu.Data.SteeringKd.ToString());
            AddXmlElement(root, "AccPedalSndCh_MaxV", mEcu.Data.AccPedalSndCh_MaxV.ToString());
            AddXmlElement(root, "fltSteeringLength", mEcu.Data.SteeringKi.ToString());
            AddXmlElement(root, "BaseCanId", mEcu.Data.BaseCanId.ToString());
            AddXmlElement(root, "KeyOffTime_ms", mEcu.Data.KeyOffTime_ms.ToString());
            AddXmlElement(root, "MaxInverterT", mEcu.Data.MaxInverterT.ToString());

            AddXmlElement(root, "MaxMotorSpeedD", mEcu.Data.MaxMotorSpeedD.ToString());
            AddXmlElement(root, "MaxTorque", mEcu.Data.MaxTorque.ToString());
            AddXmlElement(root, "MaxMotorT", mEcu.Data.MaxMotorT.ToString());

            AddXmlElement(root, "InvCoolingOn", mEcu.Data.InvCoolingOn.ToString());
            AddXmlElement(root, "MotorCoolingOn", mEcu.Data.MotorCoolingOn.ToString());

            AddXmlElement(root, "PowerOffDelay_ms", mEcu.Data.PowerOffDelay_ms.ToString());
            AddXmlElement(root, "SteeringMaxVal_0p1V", mEcu.Data.SteeringMaxVal_0p1V.ToString());
            AddXmlElement(root, "SteeringMaxCurrent_0p1A", mEcu.Data.SteeringMaxCurrent_0p1A.ToString());
            AddXmlElement(root, "SteeringMinVal_0p1V", mEcu.Data.SteeringMinVal_0p1V.ToString());
            AddXmlElement(root, "TrimMaxVal_0p1V", mEcu.Data.TrimMaxVal_0p1V.ToString());
            AddXmlElement(root, "TrimMaxCurrent_0p1A", mEcu.Data.TrimMaxCurrent_0p1A.ToString());
            AddXmlElement(root, "TrimMinVal_0p1V", mEcu.Data.TrimMinVal_0p1V.ToString());
            AddXmlElement(root, "IsPowerManger", mEcu.IsPowerManger.ToString());


            // VoltageCCLpoint
            el = AddXmlElement(root, "SteeringBrakeSpeedTable", "");
            foreach (Int16 val in mEcu.Data.SteeringBrakeSpeedTable)
                AddXmlElement(el, "val", val.ToString());

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
                    mEcu.Data.DiagnosticID = (byte)Convert.ToByte(n.InnerText);
                else if (n.Name == "IsPowerManger")
                    mEcu.IsPowerManger = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "AccPedalFstCh_0V")
                    mEcu.Data.AccPedalFstCh_0V = Convert.ToByte(n.InnerText);
                else if (n.Name == "AccPedalFstCh_MaxV")
                    mEcu.Data.AccPedalFstCh_MaxV = Convert.ToByte(n.InnerText);
                else if (n.Name == "fltSteeringPeriod")
                    mEcu.Data.SteeringKp = Convert.ToByte(n.InnerText);
                else if (n.Name == "AccPedalSndCh_0V")
                    mEcu.Data.SteeringKd = Convert.ToByte(n.InnerText);
                else if (n.Name == "AccPedalSndCh_MaxV")
                    mEcu.Data.AccPedalSndCh_MaxV = Convert.ToByte(n.InnerText);
                else if (n.Name == "fltSteeringLength")
                    mEcu.Data.SteeringKi = Convert.ToByte(n.InnerText);
                else if (n.Name == "BaseCanId")
                    mEcu.Data.BaseCanId = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "KeyOffTime_ms")
                    mEcu.Data.KeyOffTime_ms = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MaxInverterT")
                    mEcu.Data.MaxInverterT = Convert.ToInt16(n.InnerText);
                else if (n.Name == "MaxMotorSpeedD")
                    mEcu.Data.MaxMotorSpeedD = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MaxTorque")
                    mEcu.Data.MaxTorque = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MaxMotorT")
                    mEcu.Data.MaxMotorT = Convert.ToInt16(n.InnerText);
                else if (n.Name == "PowerOffDelay_ms")
                    mEcu.Data.PowerOffDelay_ms = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "SteeringMaxCurrent_0p1A")
                    mEcu.Data.SteeringMaxCurrent_0p1A = Convert.ToInt16(n.InnerText);
                else if (n.Name == "SteeringMaxVal_0p1V")
                    mEcu.Data.SteeringMaxVal_0p1V = Convert.ToByte(n.InnerText);
                else if (n.Name == "SteeringMinVal_0p1V")
                    mEcu.Data.SteeringMinVal_0p1V = Convert.ToByte(n.InnerText);
                else if (n.Name == "TrimMaxCurrent_0p1A")
                    mEcu.Data.TrimMaxCurrent_0p1A = Convert.ToInt16(n.InnerText);
                else if (n.Name == "TrimMaxVal_0p1V")
                    mEcu.Data.TrimMaxVal_0p1V = Convert.ToByte(n.InnerText);
                else if (n.Name == "TrimMinVal_0p1V")
                    mEcu.Data.TrimMinVal_0p1V = Convert.ToByte(n.InnerText);
                else if (n.Name == "InvCoolingOn")
                    mEcu.Data.InvCoolingOn = Convert.ToByte(n.InnerText);
                else if (n.Name == "MotorCoolingOn")
                    mEcu.Data.MotorCoolingOn = Convert.ToByte(n.InnerText);
                else if (n.Name == "SteeringBrakeSpeedTable")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < 12)
                            mEcu.Data.SteeringBrakeSpeedTable[i] = Convert.ToUInt16(val.InnerText);
                        i++;
                    }
                }
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
