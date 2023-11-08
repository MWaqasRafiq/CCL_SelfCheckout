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
        [DllImport("CCLLampDll452.dll", EntryPoint = "InitCCLLamp")]
        public static extern bool InitCCLLamp(uint lampType, uint portNo);

        [DllImport("CCLLampDll452.dll", EntryPoint = "RedOpenCCLLamp")]
        public static extern void RedOpenCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "RedCloseCCLLamp")]
        public static extern void RedCloseCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "GreenOpenCCLLamp")]
        public static extern void GreenOpenCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "GreenCloseCCLLamp")]
        public static extern void GreenCloseCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "BlueOpenCCLLamp")]
        public static extern void BlueOpenCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "BlueCloseCCLLamp")]
        public static extern void BlueCloseCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "YellowOpenCCLLamp")]
        public static extern void YellowOpenCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "YellowCloseCCLLamp")]
        public static extern void YellowCloseCCLLamp();

        [DllImport("CCLLampDll452.dll", EntryPoint = "CloseConnectCCLLamp")]
        public static extern void CloseConnectCCLLamp();

        uint cbPort = 1;
        uint lampType = 6;
        
        public CCL_Lamp()
        {
            btnOpen();
        }
        
        public void btnOpen()
        {
            try
            {
                InitCCLLamp(lampType, cbPort);
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
                RedOpenCCLLamp();
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
                Basepage.logWrite("Lamp Error Close red:" + ex.Message);
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
                Basepage.logWrite("Lamp Error Close green:" + ex.Message);
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
                Basepage.logWrite("Lamp Error Close Blue:" + ex.Message);
            }
        }
        
        public void Close()
        {
            try
            {
                btnOpen();
                CloseConnectCCLLamp();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error Close:" + ex.Message);
            }
        }

        public async Task BlueBlinkOpen(int seconds)
        {
            try
            {
                btnOpen();
                var curTime = DateTime.Now.AddSeconds(seconds + 1);
                while (curTime > DateTime.Now)
                {
                    BlueOpenCCLLamp();
                    Thread.Sleep(300);
                    BlueCloseCCLLamp();
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Lamp Error White blink:" + ex.Message);
            }
        }
    }
}
