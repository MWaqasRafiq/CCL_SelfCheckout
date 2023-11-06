using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Threading;

namespace IDOLSelfCheckout.Classes
{
    public class CCL_Lamp
    {
        [DllImport("CCLLampDll452.dll", EntryPoint = "InitAutoCCLLamp")]
        public static extern bool InitAutoCCLLamp();
        [DllImport("CCLLampDll452.dll", EntryPoint = "InitCCLLamp")]
        public static extern bool InitCCLLamp(uint lampType, uint portNo);
        [DllImport("CCLLampDll452.dll", EntryPoint = "RedOpenCCLLamp")]
        public static extern void RedOpenCCLLamp();
        /// <summary>
        /// 红灯灭
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "RedCloseCCLLamp")]
        public static extern void RedCloseCCLLamp();
        /// <summary>
        /// 绿灯亮
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "GreenOpenCCLLamp")]
        public static extern void GreenOpenCCLLamp();
        /// <summary>
        /// 绿灯灭
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "GreenCloseCCLLamp")]
        public static extern void GreenCloseCCLLamp();
        /// <summary>
        /// 蓝灯亮
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "BlueOpenCCLLamp")]
        public static extern void BlueOpenCCLLamp();
        /// <summary>
        /// 蓝灯灭
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "BlueCloseCCLLamp")]
        public static extern void BlueCloseCCLLamp();
        /// <summary>
        /// 黄灯亮
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "YellowOpenCCLLamp")]
        public static extern void YellowOpenCCLLamp();
        /// <summary>
        /// 黄灯灭
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "YellowCloseCCLLamp")]
        public static extern void YellowCloseCCLLamp();
        /// <summary>
        /// 关闭连接
        /// </summary>
        [DllImport("CCLLampDll452.dll", EntryPoint = "CloseConnectCCLLamp")]
        public static extern void CloseConnectCCLLamp();
        uint cbPort = 1;
        //string cbPortName = "COM1";
        //string cbLampTypeName = "JF11_GPIO";
        int cbLampType = 6;
        uint lampType = 6;
        //public MainWindow()
        //{
        //    InitializeComponent();
        //}

        //private void Window_Loaded()
        //{
        //    List<string> list = new List<string> { "Z2_GPIO_JF_i3i5", "Z5/B1_GPIO_H2A",
        //        "A1_GPIO_MBYT18", "7000_XinBu_AEON_JP", "S1_AEON_JP",
        //        "6000_COM", "JF11_GPIO", "JF6412_GPIO" };
        //    cbLampType.ItemsSource = list;
        //    string[] itemName = SerialPort.GetPortNames();
        //    cbPort.ItemsSource = itemName;
        //    int selected = IniReadValue("CONFIG", "LAMP");
        //    if (selected >= list.Count || selected < 0)
        //    {
        //        selected = 0;
        //    }
        //    cbLampType.SelectedIndex = selected;
        //    cbPort.SelectedIndex = 0;

        //    setBtnEnable(false);
        //}
        public CCL_Lamp()
        {
            
            btnOpen();
            Thread.Sleep(500);

            //RedCloseCCLLamp();
            //GreenCloseCCLLamp();
            //BlueCloseCCLLamp();
            //YellowCloseCCLLamp();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
   
        //public static int IniReadValue(string Section, string Key)
        //{
        //    try
        //    {
        //        StringBuilder temp = new StringBuilder(255);
        //        int i = GetPrivateProfileString(Section, Key, "", temp, 255,
        //            AppDomain.CurrentDomain.BaseDirectory + @"\PosCfg.ini");
        //        return Convert.ToInt32(temp.ToString());
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        //void setBtnEnable(bool isEnable)
        //{
        //    btnOpen.IsEnabled = !isEnable;

        //    btnRedOpen.IsEnabled = isEnable;
        //    btnRedClose.IsEnabled = isEnable;
        //    btnGreenOpen.IsEnabled = isEnable;
        //    btnGreenClose.IsEnabled = isEnable;
        //    if (lampType == 5)//6000只有黄灯，没有蓝灯
        //    {
        //        btnBlueOpen.IsEnabled = false;
        //        btnBlueClose.IsEnabled = false;
        //        btnYellowOpen.IsEnabled = isEnable;
        //        btnYellowClose.IsEnabled = isEnable;
        //    }
        //    else
        //    {
        //        btnBlueOpen.IsEnabled = isEnable;
        //        btnBlueClose.IsEnabled = isEnable;
        //        btnYellowOpen.IsEnabled = false;
        //        btnYellowClose.IsEnabled = false;
        //    }
        //}

        //private void cbLampType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    lampType = (uint)cbLampType.SelectedIndex;
        //    if (lampType == 3 || lampType == 4 || lampType == 5)
        //    {
        //        cbPort.IsEnabled = true;
        //    }
        //    else
        //    {
        //        cbPort.IsEnabled = false;
        //    }
        //}

        //public void btnOpenAuto()
        //{
        //    try
        //    {
        //        bool ret = InitAutoCCLLamp();
        //        //if (!ret)
        //        //{
        //        //    MessageBox.Show("OpenFail");
        //        //}
        //        //else
        //        //{
        //        //    setBtnEnable(true);
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Basepage.logWrite("Lamp Error Auto:" + ex.Message);
        //    }
        //}
        public void btnOpen()
        {
            try
            {
                Basepage.logWrite("Lamp Open:" + lampType + " : "+ cbPort.ToString());
                //bool ret = InitAutoCCLLamp();
                bool ret = InitCCLLamp(lampType, cbPort);
                if (!ret)
                {
                    Basepage.logWrite("Lamp Not Opened:" + lampType + " : "+ cbPort.ToString());
                }
                else
                {
                    Basepage.logWrite("Lamp Opened:" + lampType + " : "+ cbPort.ToString());
                }
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Open:" + ex.Message);
            }
        }
        public void RedOpen()
        {
            try
            {
                //InitCCLLamp(lampType, cbPort);
                //Thread.Sleep(500);
                Basepage.logWrite("Lamp Opening Red.");
                RedOpenCCLLamp();
                Basepage.logWrite("Lamp Opened Red.");
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Red:" + ex.Message);
            }
        }
        public void RedClose()
        {
            try
            {
                RedCloseCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void GreenOpen()
        {
            try
            {
                GreenOpenCCLLamp();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Green:" + ex.Message);
            }
        }
        public void GreenClose()
        {
            try
            {
                GreenCloseCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void BlueOpen()
        {
            try
            {
                BlueOpenCCLLamp();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Blue:" + ex.Message);
            }
        }
        public void BlueClose()
        {
            try
            {
                BlueCloseCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Close()
        {
            try
            {
                CloseConnectCCLLamp();
                //setBtnEnable(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void YellowOpen()
        {
            try
            {
                YellowOpenCCLLamp();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Yellow:" + ex.Message);
            }
        }
        public void YellowClose()
        {
            try
            {
                YellowCloseCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
