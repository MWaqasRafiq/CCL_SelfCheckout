using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLamp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        [DllImport("CCLLampDll452.dll", EntryPoint = "InitAutoCCLLamp")]
        public static extern bool InitAutoCCLLamp();
        /// <summary>
        /// 备用，初始化，需要传入警示灯类型
        /// </summary>
        /// <param name="lampType">警示灯类型</param>
        /// <param name="portNo">串口号</param>
        /// <returns></returns>
        [DllImport("CCLLampDll452.dll", EntryPoint = "InitCCLLamp")]
        public static extern bool InitCCLLamp(uint lampType, uint portNo);
        /// <summary>
        /// 红灯亮
        /// </summary>
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

        uint lampType = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string> { "Z2_GPIO_JF_i3i5", "Z5/B1_GPIO_H2A",
                "A1_GPIO_MBYT18", "7000_XinBu_AEON_JP", "S1_AEON_JP",
                "6000_COM", "JF11_GPIO", "JF6412_GPIO" };
            cbLampType.ItemsSource = list;
            string[] itemName = SerialPort.GetPortNames();
            cbPort.ItemsSource = itemName;
            int selected = IniReadValue("CONFIG", "LAMP");
            if (selected >= list.Count || selected < 0)
            {
                selected = 0;
            }
            cbLampType.SelectedIndex = selected;
            cbPort.SelectedIndex = 0;

            setBtnEnable(false);
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        //读INI文件
        public static int IniReadValue(string Section, string Key)
        {
            try
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255,
                    AppDomain.CurrentDomain.BaseDirectory + @"\PosCfg.ini");
                return Convert.ToInt32(temp.ToString());
            }
            catch
            {
                return 0;
            }
        }

        void setBtnEnable(bool isEnable)
        {
            btnOpen.IsEnabled = !isEnable;

            btnRedOpen.IsEnabled = isEnable;
            btnRedClose.IsEnabled = isEnable;
            btnGreenOpen.IsEnabled = isEnable;
            btnGreenClose.IsEnabled = isEnable;
            if (lampType == 5)//6000只有黄灯，没有蓝灯
            {
                btnBlueOpen.IsEnabled = false;
                btnBlueClose.IsEnabled = false;
                btnYellowOpen.IsEnabled = isEnable;
                btnYellowClose.IsEnabled = isEnable;
            }
            else
            {
                btnBlueOpen.IsEnabled = isEnable;
                btnBlueClose.IsEnabled = isEnable;
                btnYellowOpen.IsEnabled = false;
                btnYellowClose.IsEnabled = false;
            }
        }

        private void cbLampType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lampType = (uint)cbLampType.SelectedIndex;
            if (lampType == 3 || lampType == 4 || lampType == 5)
            {
                cbPort.IsEnabled = true;
            }
            else
            {
                cbPort.IsEnabled = false;
            }
        }

        private void btnOpenAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool ret = InitAutoCCLLamp();
                if (!ret)
                {
                    MessageBox.Show("OpenFail");
                }
                else
                {
                    setBtnEnable(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint port = 1;
                if (cbPort.SelectedItem != null)
                {
                    port = uint.Parse(cbPort.SelectedItem.ToString().Substring(3));
                }


                bool ret = InitCCLLamp(lampType, port);
                if (!ret)
                {
                    MessageBox.Show("OpenFail");
                }
                else
                {
                    setBtnEnable(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRedOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RedOpenCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRedClose_Click(object sender, RoutedEventArgs e)
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

        private void btnGreenOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GreenOpenCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGreenClose_Click(object sender, RoutedEventArgs e)
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

        private void btnBlueOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BlueOpenCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBlueClose_Click(object sender, RoutedEventArgs e)
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseConnectCCLLamp();
                setBtnEnable(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnYellowOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YellowOpenCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnYellowClose_Click(object sender, RoutedEventArgs e)
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

        public void btnOpen_Click()
        {
            try
            {
                bool ret = InitCCLLamp(6, 1);
                if (!ret)
                {
                    MessageBox.Show("OpenFail");
                }
                else
                {
                    //setBtnEnable(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BlueOpen_Click()
        {
            try
            {
                BlueOpenCCLLamp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
