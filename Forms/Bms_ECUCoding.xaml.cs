using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
	public partial class Bms_ECUCoding : UserControl, IEcu_Coding
	{
        System.Timers.Timer timer;
        Bms_ECU bms;
        public ECU CurrentEcu { get; set; }

        public Bms_ECUCoding()
		{
			InitializeComponent();

            this.Loaded += FrmBmsCoding_Loaded;
            //bms = new Bms_ECU(EcuModelId.bms, );

            bms = Global.EcuList.CurrentEcu as Bms_ECU;
			FillLists();			
		}

        private void FrmBmsCoding_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer(1000);
        }



        #region General

        void FillLists()
		{
			int sz = Marshal.SizeOf(typeof(Bms_ECU.CodingData_t));
			if ((sz & 1) != 0)
				throw new Exception("Размер структуры кодировки должен быть четным.");
						
			cbWorkMode.Items.Add("Подчиненный");
			cbWorkMode.Items.Add("Мастер");

			Tools.SetComboItem(cbWorkMode, 0);
			// cstDHAB_S34, cstHASS_50, cstHASS_100, cstHASS_200, cstHASS_300, cstHASS_400
			cbSensorType.Items.Add("cstDHAB_S34");
			cbSensorType.Items.Add("cstHASS_50");
			cbSensorType.Items.Add("cstHASS_100");
			cbSensorType.Items.Add("cstHASS_200");
			cbSensorType.Items.Add("cstHASS_300");
			cbSensorType.Items.Add("cstHASS_400");
			Tools.SetComboItem(cbSensorType, 2);

			cbSensorDirection.Items.Add("Прямое");
			cbSensorDirection.Items.Add("Обратное");
		}

		// Заполнение структуры данными из формы
		bool FormToStruct()
		{
			byte adr = Convert.ToByte(tbDiagnosticAddress.Text);

			// Общие
			bms.Data.DiagnosticID = adr;
			bms.Data.BatteryIndex = Convert.ToByte(tbBatteryID.Text);
			bms.Data.ModuleIndex = Convert.ToByte(tbModuleID.Text);
			bms.Data.BaseID = Convert.ToUInt32(tbBaseCANID.Text);
			bms.IsMaster = (int)Tools.GetComboItem(cbWorkMode) == 1;

			bms.Data.CurrentSensType = (byte)(int)Tools.GetComboItem(cbSensorType);
			bms.Data.CurrentSensDirection = (byte)(int)Tools.GetComboItem(cbSensorDirection);

			// Батареи			
			bms.Data.ModulesCountS = Convert.ToByte(tbModuleCountS.Text);
			bms.Data.ModulesCountP = Convert.ToByte(tbModuleCountP.Text);
			bms.Data.TotalCapacity = Convert.ToUInt16(tbTotalCapacity.Text);
			bms.Data.MaxVoltageDisbalanceP = Convert.ToUInt16(tbMaxDisbalanceVoltage.Text);
			bms.Data.MaxVoltageDisbalanceS = Convert.ToByte(tbMaxDisbalanceVoltageS.Text);

			// DCL/CCL
			bms.Data.MaxDCL = Convert.ToInt16(tbMaxDCL.Text);
			bms.Data.MaxCCL = Convert.ToInt16(tbMaxCCL.Text);

			bms.Data.VoltageCCLpoint[0] = Convert.ToInt16(tbCellV1.Text);
			bms.Data.VoltageCCLpoint[1] = Convert.ToInt16(tbCellV2.Text);
			bms.Data.VoltageCCLpoint[2] = Convert.ToInt16(tbCellV3.Text);
			bms.Data.VoltageCCLpoint[3] = Convert.ToInt16(tbCellV4.Text);
			bms.Data.VoltageCCLpoint[4] = Convert.ToInt16(tbCellV5.Text);
			bms.Data.VoltageCCLpoint[5] = Convert.ToInt16(tbCellV6.Text);
			bms.Data.VoltageCCLpoint[6] = Convert.ToInt16(tbCCL1.Text);
			bms.Data.VoltageCCLpoint[7] = Convert.ToInt16(tbCCL2.Text);
			bms.Data.VoltageCCLpoint[8] = Convert.ToInt16(tbCCL3.Text);
			bms.Data.VoltageCCLpoint[9] = Convert.ToInt16(tbCCL4.Text);
			bms.Data.VoltageCCLpoint[10] = Convert.ToInt16(tbCCL5.Text);
			bms.Data.VoltageCCLpoint[11] = Convert.ToInt16(tbCCL6.Text);
			bms.Data.VoltageCCLpoint[12] = Convert.ToInt16(tbDCL1.Text);
			bms.Data.VoltageCCLpoint[13] = Convert.ToInt16(tbDCL2.Text);
			bms.Data.VoltageCCLpoint[14] = Convert.ToInt16(tbDCL3.Text);
			bms.Data.VoltageCCLpoint[15] = Convert.ToInt16(tbDCL4.Text);
			bms.Data.VoltageCCLpoint[16] = Convert.ToInt16(tbDCL5.Text);
			bms.Data.VoltageCCLpoint[17] = Convert.ToInt16(tbDCL6.Text);

            // Temp characteristic
			bms.Data.TemperatureCCLpoint[0] = Convert.ToInt16(tbCellT1.Text);
			bms.Data.TemperatureCCLpoint[1] = Convert.ToInt16(tbCellT2.Text);
			bms.Data.TemperatureCCLpoint[2] = Convert.ToInt16(tbCellT3.Text);
			bms.Data.TemperatureCCLpoint[3] = Convert.ToInt16(tbCellT4.Text);
			bms.Data.TemperatureCCLpoint[4] = Convert.ToInt16(tbCellT5.Text);
			bms.Data.TemperatureCCLpoint[5] = Convert.ToInt16(tbCellT6.Text);
			bms.Data.TemperatureCCLpoint[6] = Convert.ToInt16(tbCL1.Text);
			bms.Data.TemperatureCCLpoint[7] = Convert.ToInt16(tbCL2.Text);
			bms.Data.TemperatureCCLpoint[8] = Convert.ToInt16(tbCL3.Text);
			bms.Data.TemperatureCCLpoint[9] = Convert.ToInt16(tbCL4.Text);
			bms.Data.TemperatureCCLpoint[10] = Convert.ToInt16(tbCL5.Text);
			bms.Data.TemperatureCCLpoint[11] = Convert.ToInt16(tbCL6.Text);

            // OCV characteristic
            bms.Data.OCVpoint[0] = Convert.ToInt16(tbSoc1.Text);
            bms.Data.OCVpoint[1] = Convert.ToInt16(tbSoc2.Text);
            bms.Data.OCVpoint[2] = Convert.ToInt16(tbSoc3.Text);
            bms.Data.OCVpoint[3] = Convert.ToInt16(tbSoc4.Text);
            bms.Data.OCVpoint[4] = Convert.ToInt16(tbSoc5.Text);
            bms.Data.OCVpoint[5] = Convert.ToInt16(tbSoc6.Text);
            bms.Data.OCVpoint[6] = Convert.ToInt16(tbSoc7.Text);
            bms.Data.OCVpoint[7] = Convert.ToInt16(tbSoc8.Text);
            bms.Data.OCVpoint[8] = Convert.ToInt16(tbSoc9.Text);
            bms.Data.OCVpoint[9] = Convert.ToInt16(tbSoc10.Text);
            bms.Data.OCVpoint[10] = Convert.ToInt16(tbSoc11.Text);
            bms.Data.OCVpoint[11] = Convert.ToInt16(tbSoc12.Text);
            bms.Data.OCVpoint[12] = Convert.ToInt16(tbSoc1Volt.Text);
            bms.Data.OCVpoint[13] = Convert.ToInt16(tbSoc2Volt.Text);
            bms.Data.OCVpoint[14] = Convert.ToInt16(tbSoc3Volt.Text);
            bms.Data.OCVpoint[15] = Convert.ToInt16(tbSoc4Volt.Text);
            bms.Data.OCVpoint[16] = Convert.ToInt16(tbSoc5Volt.Text);
            bms.Data.OCVpoint[17] = Convert.ToInt16(tbSoc6Volt.Text);
            bms.Data.OCVpoint[18] = Convert.ToInt16(tbSoc7Volt.Text);
            bms.Data.OCVpoint[19] = Convert.ToInt16(tbSoc8Volt.Text);
            bms.Data.OCVpoint[20] = Convert.ToInt16(tbSoc9Volt.Text);
            bms.Data.OCVpoint[21] = Convert.ToInt16(tbSoc10Volt.Text);
            bms.Data.OCVpoint[22] = Convert.ToInt16(tbSoc11Volt.Text);
            bms.Data.OCVpoint[23] = Convert.ToInt16(tbSoc12Volt.Text);


            // Модули
            bms.Data.CellNumber = Convert.ToByte(tbCellNum.Text);
			bms.Data.MaxCellVoltage_mV = Convert.ToUInt16(tbMaxCellVoltage.Text);
			bms.Data.MinCellVoltage_mV = Convert.ToUInt16(tbMinCellVoltage.Text);
			bms.Data.MinBalancerLevel_mV = Convert.ToUInt16(tbMinBalancerLevel.Text);
            bms.Data.BalancingTime_s = Convert.ToByte(tbBalancingTime.Text);
            bms.Data.MaxBalancingDiff_mV = Convert.ToByte(tbMaxBalancingDiff.Text);

            // Предзаряд
            bms.Data.PreMaxDuration = Convert.ToUInt16(tbPreMaxDuration.Text);
			bms.Data.PreZeroCurrent = Convert.ToUInt16(tbPreZeroCurrent.Text);
			bms.Data.PreZeroCurrentDuration = Convert.ToUInt16(tbPreZeroDuration.Text);
			bms.Data.PreMaxCurrent = Convert.ToUInt16(tbPreMaxCurrent.Text);

			bms.CheckContactor = checkContactor.IsChecked.Value;
			bms.CheckInterlock = checkInterlock.IsChecked.Value;
			bms.IsTimeServer = checkTimeServer.IsChecked.Value;
			bms.VcuControled = checkVcuControled.IsChecked.Value;
			bms.Offline = checkOffline.IsChecked.Value;
			bms.IsPowerManager = checkDebug.IsChecked.Value;         
			
			bms.Data.PowerOffDelay_ms = Convert.ToInt16(tbPowerOffDelay.Text);
			bms.Data.KeyOffTime_ms = Convert.ToInt16(tbIgnitionReact.Text);

			return true;
		}

		void StructToForm()
		{
			tbDiagnosticAddress.Text = bms.Data.DiagnosticID.ToString();

			// Общие
			tbBatteryID.Text = bms.Data.BatteryIndex.ToString();
			tbModuleID.Text = bms.Data.ModuleIndex.ToString();
			tbBaseCANID.Text = bms.Data.BaseID.ToString();
			cbWorkMode.SelectedIndex = Convert.ToInt32(bms.IsMaster);

			cbSensorType.SelectedIndex = bms.Data.CurrentSensType;
			cbSensorDirection.SelectedIndex = bms.Data.CurrentSensDirection;

			// Батареи
			tbMaxDisbalanceVoltageS.Text = bms.Data.MaxVoltageDisbalanceS.ToString();
			tbModuleCountS.Text = bms.Data.ModulesCountS.ToString();
			tbModuleCountP.Text = bms.Data.ModulesCountP.ToString();
			tbTotalCapacity.Text = bms.Data.TotalCapacity.ToString();
			tbMaxDisbalanceVoltage.Text = bms.Data.MaxVoltageDisbalanceP.ToString();

			// DCL/CCL
			tbMaxDCL.Text = bms.Data.MaxDCL.ToString();
			tbMaxCCL.Text = bms.Data.MaxCCL.ToString();

			tbCellV1.Text = bms.Data.VoltageCCLpoint[0].ToString();
			tbCellV2.Text = bms.Data.VoltageCCLpoint[1].ToString();
			tbCellV3.Text = bms.Data.VoltageCCLpoint[2].ToString();
			tbCellV4.Text = bms.Data.VoltageCCLpoint[3].ToString();
			tbCellV5.Text = bms.Data.VoltageCCLpoint[4].ToString();
			tbCellV6.Text = bms.Data.VoltageCCLpoint[5].ToString();
			tbCCL1.Text = bms.Data.VoltageCCLpoint[6].ToString();
			tbCCL2.Text = bms.Data.VoltageCCLpoint[7].ToString();
			tbCCL3.Text = bms.Data.VoltageCCLpoint[8].ToString();
			tbCCL4.Text = bms.Data.VoltageCCLpoint[9].ToString();
			tbCCL5.Text = bms.Data.VoltageCCLpoint[10].ToString();
			tbCCL6.Text = bms.Data.VoltageCCLpoint[11].ToString();
			tbDCL1.Text = bms.Data.VoltageCCLpoint[12].ToString();
			tbDCL2.Text = bms.Data.VoltageCCLpoint[13].ToString();
			tbDCL3.Text = bms.Data.VoltageCCLpoint[14].ToString();
			tbDCL4.Text = bms.Data.VoltageCCLpoint[15].ToString();
			tbDCL5.Text = bms.Data.VoltageCCLpoint[16].ToString();
			tbDCL6.Text = bms.Data.VoltageCCLpoint[17].ToString();

			tbCellT1.Text = bms.Data.TemperatureCCLpoint[0].ToString();
			tbCellT2.Text = bms.Data.TemperatureCCLpoint[1].ToString();
			tbCellT3.Text = bms.Data.TemperatureCCLpoint[2].ToString();
			tbCellT4.Text = bms.Data.TemperatureCCLpoint[3].ToString();
			tbCellT5.Text = bms.Data.TemperatureCCLpoint[4].ToString();
			tbCellT6.Text = bms.Data.TemperatureCCLpoint[5].ToString();
			tbCL1.Text = bms.Data.TemperatureCCLpoint[6].ToString();
			tbCL2.Text = bms.Data.TemperatureCCLpoint[7].ToString();
			tbCL3.Text = bms.Data.TemperatureCCLpoint[8].ToString();
			tbCL4.Text = bms.Data.TemperatureCCLpoint[9].ToString();
			tbCL5.Text = bms.Data.TemperatureCCLpoint[10].ToString();
			tbCL6.Text = bms.Data.TemperatureCCLpoint[11].ToString();

            // OCV characteristic
            tbSoc1.Text = bms.Data.OCVpoint[0].ToString();
            tbSoc2.Text = bms.Data.OCVpoint[1].ToString();
            tbSoc3.Text = bms.Data.OCVpoint[2].ToString();
            tbSoc4.Text = bms.Data.OCVpoint[3].ToString();
            tbSoc5.Text = bms.Data.OCVpoint[4].ToString();
            tbSoc6.Text = bms.Data.OCVpoint[5].ToString();
            tbSoc7.Text = bms.Data.OCVpoint[6].ToString();
            tbSoc8.Text = bms.Data.OCVpoint[7].ToString();
            tbSoc9.Text = bms.Data.OCVpoint[8].ToString();
            tbSoc10.Text = bms.Data.OCVpoint[9].ToString();
            tbSoc11.Text = bms.Data.OCVpoint[10].ToString();
            tbSoc12.Text = bms.Data.OCVpoint[11].ToString();
            tbSoc1Volt.Text = bms.Data.OCVpoint[12].ToString();
            tbSoc2Volt.Text = bms.Data.OCVpoint[13].ToString();
            tbSoc3Volt.Text = bms.Data.OCVpoint[14].ToString();
            tbSoc4Volt.Text = bms.Data.OCVpoint[15].ToString();
            tbSoc5Volt.Text = bms.Data.OCVpoint[16].ToString();
            tbSoc6Volt.Text = bms.Data.OCVpoint[17].ToString();
            tbSoc7Volt.Text = bms.Data.OCVpoint[18].ToString();
            tbSoc8Volt.Text = bms.Data.OCVpoint[19].ToString();
            tbSoc9Volt.Text = bms.Data.OCVpoint[20].ToString();
            tbSoc10Volt.Text = bms.Data.OCVpoint[21].ToString();
            tbSoc11Volt.Text = bms.Data.OCVpoint[22].ToString();
            tbSoc12Volt.Text = bms.Data.OCVpoint[23].ToString();

            // Балансировка
            tbCellNum.Text = bms.Data.CellNumber.ToString();
			tbMaxCellVoltage.Text = bms.Data.MaxCellVoltage_mV.ToString();
			tbMinCellVoltage.Text = bms.Data.MinCellVoltage_mV.ToString();
			tbMinBalancerLevel.Text = bms.Data.MinBalancerLevel_mV.ToString();
            tbBalancingTime.Text = bms.Data.BalancingTime_s.ToString();
            tbMaxBalancingDiff.Text = bms.Data.MaxBalancingDiff_mV.ToString();
            // Предзаряд
            tbPreMaxDuration.Text = bms.Data.PreMaxDuration.ToString();
			tbPreZeroDuration.Text = bms.Data.PreZeroCurrentDuration.ToString();
			tbPreZeroCurrent.Text = bms.Data.PreZeroCurrent.ToString();
			tbPreMaxCurrent.Text = bms.Data.PreMaxCurrent.ToString();

			checkContactor.IsChecked = bms.CheckContactor;
			checkInterlock.IsChecked = bms.CheckInterlock;
			checkTimeServer.IsChecked = bms.IsTimeServer;
			checkVcuControled.IsChecked = bms.VcuControled;
			checkOffline.IsChecked = bms.Offline;
			checkDebug.IsChecked = bms.IsPowerManager;

			tbPowerOffDelay.Text = bms.Data.PowerOffDelay_ms.ToString();
			tbIgnitionReact.Text = bms.Data.KeyOffTime_ms.ToString();
		}

		#endregion



		#region Coding


		async public Task<bool> Download()
		{
			IntPtr ptr;
			int ProfileSize = Marshal.SizeOf(typeof(Bms_ECU.CodingData_t));
			
			ptr = Marshal.AllocHGlobal(ProfileSize);
			byte[] buf = await Global.diag.ReadDataByID((byte)Global.EcuList.CurrentEcu.Address , (byte)BmsObjectsIndex_e.didConfigStructIndex); 

            Marshal.Copy(buf, 0, ptr, ProfileSize);
			bms.Data = (Bms_ECU.CodingData_t)Marshal.PtrToStructure(ptr, typeof(Bms_ECU.CodingData_t));

			ushort loc_crc = Tools.CRC16(buf, ProfileSize / 4 - 1);
            if (bms.Data.CRC == loc_crc)
            {
                StructToForm();

                if(bms.IsTimeServer)
                {
                    //byte[] data = await Global.diag.ReadDataByID((byte)Global.EcuList.CurrentEcu.Address, (byte)BmsObjectsIndex_e.didDateTime);
                    //UInt32 _boardTime_s = BitConverter.ToUInt32(data, 0);

                    List<ResponseData_ReadDataByIdentifier> response = await CurrentEcu.GetEcuTime();

                    if (response.Count > 0)
                    {
                        ResponseData_ReadDataByIdentifier it = response[0];
                        // Найти в списке диагностических значений нужный 
                        DiagnosticData dv = Global.EcuList.CurrentEcu.GetDiagnosticSets().Find((diag) => diag.DataID == it.DID);
                        // Если ЭБУ поддерживает запрашиваемый DID
                        if (dv == null)
                            return true;

                        UInt32 _boardTime_s = BitConverter.ToUInt32(it.DV.ToArray(), 0);

                        // Точка отсчета 01.01.2000
                        TimeSpan _sysTime = DateTime.Now - new DateTime(2000, 1, 1);
                        UInt32 _systemTime = Convert.ToUInt32(_sysTime.TotalSeconds);

                        if ((_boardTime_s < _systemTime - 600) || (_boardTime_s > _systemTime + 600))
                        {
                            MessageBoxResult res = MessageBox.Show("Время устройства отличается от системного времени.\n Установить системное время?", "Системное время", MessageBoxButton.YesNo);

                            if (res == MessageBoxResult.Yes)
                            {
                                byte[] data = BitConverter.GetBytes(_systemTime);
                                await Global.diag.WriteDataByID((byte)Global.EcuList.CurrentEcu.Address, (byte)BmsObjectsIndex_e.didDateTime, data);
                            }

                        }
                    }
                }
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
				int ProfileSize = Marshal.SizeOf(typeof(Bms_ECU.CodingData_t));		
				
				ptr = Marshal.AllocHGlobal(ProfileSize);
				Marshal.StructureToPtr(bms.Data, ptr, false);

				byte[] buf = new byte[ProfileSize];				

				Marshal.Copy(ptr, buf, 0, buf.Length);
				bms.Data.CRC = Tools.CRC16(buf, buf.Length / 4 - 1);

				Marshal.StructureToPtr(bms.Data, ptr, false);
				Marshal.Copy(ptr, buf, 0, buf.Length);				

				Marshal.FreeHGlobal(ptr);

                bool res = await Global.diag.WriteDataByID((byte)Global.EcuList.CurrentEcu.Address, (byte)BmsObjectsIndex_e.didConfigStructIndex, buf);

				return (res)? ReadDataResult_e.RES_SUCCESSFUL : ReadDataResult_e.RES_OTHER_ERROR;
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

            AddXmlElement(root, "DiagnosticID", bms.Data.DiagnosticID.ToString());
            AddXmlElement(root, "BatteryIndex", bms.Data.BatteryIndex.ToString());
            AddXmlElement(root, "ModuleIndex", bms.Data.ModuleIndex.ToString());
            AddXmlElement(root, "IsMaster", bms.IsMaster.ToString());
            AddXmlElement(root, "CheckContactor", bms.CheckContactor.ToString());
            AddXmlElement(root, "CheckInterlock", bms.CheckInterlock.ToString());
            AddXmlElement(root, "IsTimeServer", bms.IsTimeServer.ToString());
            AddXmlElement(root, "VcuControled", bms.VcuControled.ToString());
            AddXmlElement(root, "Offline", bms.Offline.ToString());
            AddXmlElement(root, "IsPowerManager", bms.IsPowerManager.ToString());

            AddXmlElement(root, "BaseID", bms.Data.BaseID.ToString());
            AddXmlElement(root, "CurrentSensType", bms.Data.CurrentSensType.ToString());
            AddXmlElement(root, "CurrentSensDirection", bms.Data.CurrentSensDirection.ToString());
            
            AddXmlElement(root, "MaxVoltageDisbalanceS", bms.Data.MaxVoltageDisbalanceS.ToString());
            AddXmlElement(root, "TotalCapacity", bms.Data.TotalCapacity.ToString());
            AddXmlElement(root, "MaxVoltageDisbalanceP", bms.Data.MaxVoltageDisbalanceP.ToString());

            // Батареи
            AddXmlElement(root, "MaxDCL", bms.Data.MaxDCL.ToString());
            AddXmlElement(root, "MaxCCL", bms.Data.MaxCCL.ToString());
            AddXmlElement(root, "ModulesCountS", bms.Data.ModulesCountS.ToString());
            AddXmlElement(root, "ModulesCountP", bms.Data.ModulesCountP.ToString());
            AddXmlElement(root, "CellNumber", bms.Data.CellNumber.ToString());
            AddXmlElement(root, "MaxCellVoltage_mV", bms.Data.MaxCellVoltage_mV.ToString());
            AddXmlElement(root, "MinCellVoltage_mV", bms.Data.MinCellVoltage_mV.ToString());
            AddXmlElement(root, "MinBalancerLevel_mV", bms.Data.MinBalancerLevel_mV.ToString());

            // VoltageCCLpoint
            el = AddXmlElement(root, "VoltageCCLpoint", "");
            foreach (Int16 val in bms.Data.VoltageCCLpoint)
                AddXmlElement(el, "val", val.ToString());
            // TemperatureCCLpoint
            el = AddXmlElement(root, "TemperatureCCLpoint", "");
            foreach (Int16 val in bms.Data.TemperatureCCLpoint)
                AddXmlElement(el, "val", val.ToString());

            // Предзаряд
            AddXmlElement(root, "PreMaxDuration", bms.Data.PreMaxDuration.ToString());
            AddXmlElement(root, "PreZeroCurrentDuration", bms.Data.PreZeroCurrentDuration.ToString());
            AddXmlElement(root, "PreZeroCurrent", bms.Data.PreZeroCurrent.ToString());
            AddXmlElement(root, "PreMaxCurrent", bms.Data.PreMaxCurrent.ToString());

            // Балансировка
            AddXmlElement(root, "BalancingTime_s", bms.Data.BalancingTime_s.ToString());           
            AddXmlElement(root, "MaxBalancingDiff_mV", bms.Data.MaxBalancingDiff_mV.ToString());

            // VoltageCCLpoint
            el = AddXmlElement(root, "OCVpoint", "");
            foreach (Int16 val in bms.Data.OCVpoint)
                AddXmlElement(el, "val", val.ToString());


			AddXmlElement(root, "PowerOffDelay_ms", bms.Data.PowerOffDelay_ms.ToString());
			AddXmlElement(root, "KeyOffTime_ms", bms.Data.KeyOffTime_ms.ToString());

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
                    bms.Data.DiagnosticID = (byte)Convert.ToInt32(n.InnerText);
                else if (n.Name == "BatteryIndex")
                    bms.Data.BatteryIndex = (byte)Convert.ToInt32(n.InnerText);
                else if (n.Name == "ModuleIndex")
                    bms.Data.ModuleIndex = (byte)Convert.ToInt32(n.InnerText);

                // Flags
                else if (n.Name == "IsMaster")
                    bms.IsMaster = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "CheckContactor")
                    bms.CheckContactor = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "CheckInterlock")
                    bms.CheckInterlock = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "IsTimeServer")
                    bms.IsTimeServer = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "VcuControled")
                    bms.VcuControled = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "Offline")
                    bms.Offline = Convert.ToBoolean(n.InnerText);
                else if (n.Name == "IsPowerManager")
                    bms.IsPowerManager = Convert.ToBoolean(n.InnerText);

                // Батареи
                else if (n.Name == "BaseID")
                    bms.Data.BaseID = Convert.ToUInt32(n.InnerText);
                else if (n.Name == "CurrentSensType")
                    bms.Data.CurrentSensType = Convert.ToByte(n.InnerText);
                else if (n.Name == "CurrentSensDirection")
                    bms.Data.CurrentSensDirection = Convert.ToByte(n.InnerText);
                else if (n.Name == "MaxVoltageDisbalanceS")
                    bms.Data.MaxVoltageDisbalanceS = Convert.ToByte(n.InnerText);
                else if (n.Name == "TotalCapacity")
                    bms.Data.TotalCapacity = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MaxVoltageDisbalanceP")
                    bms.Data.MaxVoltageDisbalanceP = Convert.ToUInt16(n.InnerText);


                // DCL/CCL
                else if (n.Name == "MaxDCL")
                    bms.Data.MaxDCL = Convert.ToInt16(n.InnerText);
                else if (n.Name == "MaxCCL")
                    bms.Data.MaxCCL = Convert.ToInt16(n.InnerText);
                else if (n.Name == "ModulesCountS")
                    bms.Data.ModulesCountS = Convert.ToByte(n.InnerText);
                else if (n.Name == "ModulesCountP")
                    bms.Data.ModulesCountP = Convert.ToByte(n.InnerText);

                // Балансировка
                else if (n.Name == "CellNumber")
                    bms.Data.CellNumber = Convert.ToByte(n.InnerText);
                else if (n.Name == "MaxCellVoltage_mV")
                    bms.Data.MaxCellVoltage_mV = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MinCellVoltage_mV")
                    bms.Data.MinCellVoltage_mV = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "MinBalancerLevel_mV")
                    bms.Data.MinBalancerLevel_mV = Convert.ToUInt16(n.InnerText);


                else if (n.Name == "VoltageCCLpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.VoltageArrayLen)
                            bms.Data.VoltageCCLpoint[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }
                else if (n.Name == "TemperatureCCLpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.TemperatureArrayLen)
                            bms.Data.TemperatureCCLpoint[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }

                // Предзаряд
                else if (n.Name == "PreMaxDuration")
                    bms.Data.PreMaxDuration = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "PreZeroCurrentDuration")
                    bms.Data.PreZeroCurrentDuration = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "PreZeroCurrent")
                    bms.Data.PreZeroCurrent = Convert.ToUInt16(n.InnerText);
                else if (n.Name == "PreMaxCurrent")
                    bms.Data.PreMaxCurrent = Convert.ToUInt16(n.InnerText);

                // Термостатирование
                else if (n.Name == "BalancingTime_s")
                    bms.Data.BalancingTime_s = Convert.ToByte(n.InnerText);
                else if (n.Name == "MaxBalancingDiff_mV")
                    bms.Data.MaxBalancingDiff_mV = Convert.ToByte(n.InnerText);


                else if (n.Name == "OCVpoint")
                {
                    int i = 0;
                    foreach (XmlNode val in n.ChildNodes)
                    {
                        if (i < Bms_ECU.OCVArrayLen)
                            bms.Data.OCVpoint[i] = Convert.ToInt16(val.InnerText);
                        i++;
                    }
                }

				else if (n.Name == "PowerOffDelay_ms")
					bms.Data.PowerOffDelay_ms = Convert.ToByte(n.InnerText);
				else if (n.Name == "KeyOffTime_ms")
					bms.Data.KeyOffTime_ms = Convert.ToByte(n.InnerText);

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
