using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDOLSelfCheckout.Classes
{
    public class ledconnection
    {
        SerialPort _serialPort;
        public void ledCon(char color, char voice)
        {
            try
            {
                //_serialPort = new SerialPort();
                //_serialPort.Open();
                Basepage.logWrite(" >> ledconnection...color:" + color + " voice:" + voice);
                Basepage.logWrite(" >> Basepage.LSRetail_LedComPort:" + Basepage.LSRetail_PaymentDeviceComPort);
                // Closing serial port if it is open
                if (_serialPort == null || _serialPort.IsOpen == false)
                {
                    // Setting serial port settings
                    _serialPort = new SerialPort(Basepage.LSRetail_LedComPort, 9600, Parity.None, 8, StopBits.One);
                    _serialPort.ReadTimeout = 3000;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.Open();

                    Basepage.logWrite("LED Connection opened");


                    byte[] data = new byte[] { (byte)'0' };
                    _serialPort.Write(data, 0, data.Length);
                    data = new byte[] { (byte)'2' };
                    _serialPort.Write(data, 0, data.Length);
                    data = new byte[] { (byte)'4' };
                    _serialPort.Write(data, 0, data.Length);
                    data = new byte[] { (byte)color };
                    _serialPort.Write(data, 0, data.Length);
                    if (color != ' ')
                        data = new byte[] { (byte)voice };
                    Basepage.logWrite("LED Write data: "+data);
                    _serialPort.Write(data, 0, data.Length);
                    Basepage.logWrite("LED Write data success");
                    _serialPort.Close();
                }
                else
                {
                    byte[] data = new byte[] { (byte)0x01 };
                    _serialPort.Write(data, 0, data.Length);
                    _serialPort.Close();
                }
                //Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Basepage.logWrite("error led:" + ex.Message);
            }
        }

    }
}
