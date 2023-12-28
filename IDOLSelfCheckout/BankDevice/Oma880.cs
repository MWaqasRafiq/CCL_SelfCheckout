using DataModels.Shared;
using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IDOLSelfCheckout.BankDevice
{
    public class Oma880
    {
        SerialPort _serialPort;
        string ErrorCode = "";
        string CommandType = "";
        string ResponseCode = "";
        string TxnDescription = "";
        string Reversed = "";
        int countloop = 0;

        byte[] ack = new byte[] { (byte)0x06 };

        public Boolean PaymentDeviceCom(string cmdType, string amount)
        {
            Boolean statu = false;
            try
            {
                amount = amount.Replace(".", "");
                Basepage.logWrite(" >> PaymentDeviceCom...cmdType:" + cmdType);
                Basepage.logWrite(" >> Basepage.PaymentDeviceComPort:" + Basepage.PaymentDeviceComPort);
                // Closing serial port if it is open
                if (_serialPort == null || _serialPort.IsOpen == false)
                {
                    // Setting serial port settings
                    _serialPort = new SerialPort(Basepage.PaymentDeviceComPort, 115200, Parity.None, 8, StopBits.One);
                    _serialPort.ReadTimeout = 40000;
                    _serialPort.WriteTimeout = 50000;
                    _serialPort.Open();
                    Thread.Sleep(100);
                    //   _serialPort.DataReceived +=
                    //new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                    

                    // _serialPort.DataReceived += _serialPort_DataReceived;
                }

                byte[] data = reqData(amount, cmdType, "");
                _serialPort.Write(data, 0, data.Length);
                statu = listenDevice();
                Basepage.logWrite("response status:" + statu);
               
            }
            catch(Exception ex)
            {
                Basepage.logWrite("1-Error:" + ex.Message);
                sco_data.ErrorMessage = ex.Message;
                statu = false;
            }

            if (_serialPort != null && _serialPort.IsOpen == true)
            {
                StopListening();
            }

            return statu;
        }

        public Boolean listenDevice()
        {
            Boolean statu= false;
            try
            {
                Basepage.logWrite(" >> listening started...");
          
                var serialPort = _serialPort;

                countloop = 0;
                Boolean debug = true;
                while (debug)
                {
                    
                    int lenn = serialPort.BytesToRead;
                    if (lenn > 10)
                    {
                        Basepage.logWrite(" lenn1.:" + lenn);
                        Thread.Sleep(300);
                        lenn = serialPort.BytesToRead;
                        Basepage.logWrite(" lenn2.:" + lenn);
                        byte[] buffer = new byte[lenn];
                        serialPort.Read(buffer, 0, buffer.Length);
                        string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, lenn);
                        //Basepage.logWrite(" >>data received..:" + responseData);
                        string d = Basepage.hostDataLogFormat(buffer, "Response=>");

                        string val1 = buffer[0].ToString("X2");
                        string val2 = buffer[1].ToString("X2");
                        string val3 = buffer[lenn - 2].ToString("X2");
                        string val4 = buffer[lenn - 1].ToString("X2");
                        Basepage.logWrite("val1:" + val1 + " - val2:" + val2 + " - val3:" + val3 + " - val4:" + val4 + " - lenn:" + lenn);
                        if (val3 == "03")
                        {
                            if (val1 == "06" && val2 == "02")
                            {
                                responseData = responseData.Substring(2, responseData.Length - 4);
                            }
                            else
                            {
                                responseData = responseData.Substring(1, responseData.Length - 3);
                            }
                            Basepage.logWrite("ResponseDataUpdated:" + responseData);

                            List<string> list = new List<string>();

                            list.Add("CommandType");
                            list.Add("ErrorCode");
                            list.Add("ResponseCode");
                            list.Add("TxnDescription");
                            list.Add("Reversed");
                            list.Add("MaskCardNumber");
                            list.Add("ExpiryDate");
                            list.Add("AuthCode");

                            List<string> a = xPathReader(responseData, list);
                            if (a != null)
                            {
                                Basepage.logWrite("CommandType:" + a[0].ToString() + " - ErrorCode:" + a[1].ToString());
                                string CommandType = a[0].ToString();
                                //showPinpadMessage(CommandType);

                                if (CommandType == "047")
                                {
                                    serialPort.Write(ack, 0, ack.Length);
                                    Basepage.logWrite("send ack 047 CommandType:" + CommandType);
                                    statu = false;
                                    debug = false;
                                    StopListening();
                                    PaymentDeviceCom("106", "");// need to confirm the bank
                                }
                                else
                                {
                                    ErrorCode = a[1].ToString();

                                    if (ErrorCode.Length > 3)
                                    {
                                        ResponseCode = a[2].ToString();
                                        TxnDescription = a[3].ToString();
                                        Reversed = a[4].ToString();

                                        serialPort.Write(ack, 0, ack.Length);
                                        Basepage.logWrite("send ack CommandType:" + CommandType);
                                        Basepage.logWrite("ErrorCode:" + ErrorCode);
                                        sco_data.ErrorMessage = ErrorCode+ "   "+ResponseCode;
                                        if (ErrorCode == "E067")
                                        {
                                            statu = false;
                                            PaymentDeviceCom("106", "");// need to confirm the bank
                                        }
                                        else if(ErrorCode == "E000" && ResponseCode == "APPROVED" && TxnDescription== "SALE" && Reversed=="0")
                                        {
                                            sco_data.MaskCardNumber = a[5].ToString();
                                            sco_data.ExpiryDate = a[6].ToString();
                                            sco_data.AuthCode = a[7].ToString();
                                            statu = true;
                                        }
                                        else
                                        {
                                            Basepage.logWrite("Else condition of: ErrorCode == E067");
                                        }
                                       
                                        debug = false;
                                        StopListening();
                                    }
                                    else
                                    {
                                        Basepage.logWrite("Else condition of: ErrorCode.Length > 3");
                                    }
                                }
                            }
                            else
                            {
                                Basepage.logWrite("Else condition of:  if (a != null)");
                            }
                        }
                        else
                        {
                            Basepage.logWrite("Else condition of:  if (val3 == 03)");
                        }
                        Basepage.logWrite(" >> Completed. >>>>>>");
                    }
                    else
                    {
                        Thread.Sleep(800);
                        countloop++;
                        if (countloop > 50)
                        {
                            debug = false;
                            Basepage.logWrite("timeout:" + countloop);
                        }
                        else
                            Basepage.logWrite("countloop:" + countloop);
                    }
                }

            }
            catch (Exception ex)
            {
                Basepage.logWrite("4-Error:" + ex.Message);
                statu = false;
            }
            return statu;

        }

        public void showPinpadMessage(string cmdType)
        {
            ucCreditCardScreen.PinPadInfo.Dispatcher.BeginInvoke(new Action(delegate () { ucCreditCardScreen.PinPadInfo.Content = "Label text is change"+ cmdType; }));
            if (cmdType=="01")
            {
                sco_data.TransactionPinpadInfo =  "Code:"+cmdType+" Enter card Displayed..";
                ucCreditCardScreen.PinPadInfo.Dispatcher.BeginInvoke(new Action(delegate () { ucCreditCardScreen.PinPadInfo.Content = "Label text is change1"; }));
            }
            if (cmdType == "08")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Online Processing..";
            }
            if (cmdType == "09")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Response received from the host..";
            }
            if (cmdType == "13")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Transaction Successful..";
            }
            if (cmdType == "05")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Card Swiped..";
            }
            if (cmdType == "06")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Enter PIN..";
            }
            if (cmdType == "07")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " PIN Entered..";
            }
            if (cmdType == "44")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Contactless (Tap card)..";
            }
            if (cmdType == "52")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " GPRS Communication..";
            }
            if (cmdType == "100")
            {
                sco_data.TransactionPinpadInfo = "Code:" + cmdType + " Completed..";
            }


        }

        /// Closes the serial port
        public void StopListening()
        {
            countloop = 51;
            _serialPort.Close();
            Basepage.logWrite("serial port stopped.");
        }


        private byte[] reqData(string amount, string CmdTyp, string InvoiceNumber)
        {

            string ddd = "";
            if (CmdTyp == "119")
            { // void
                ddd = "<EFTData>" +
                        "<CommandType>" + CmdTyp + "</CommandType>" +
                        "<Amount>" + amount + "</Amount>" +
                        "<InvoiceNo>" + InvoiceNumber + "</InvoiceNo>" +
                        "<MREFValue></MREFValue>" +
                        "<AuthCode></AuthCode>" +
                        "</EFTData>";
            }
            else if (CmdTyp == "112")
            { //pickup reconsulation
                ddd = "<EFTData>" +
                        "<CommandType>" + CmdTyp + "</CommandType>" +
                        "</EFTData>";
            }
            else
            {
                ddd = "<EFTData>" +
                        "<CommandType>" + CmdTyp + "</CommandType>" +
                        "<Amount>" + amount + "</Amount>" +
                        "<MREFValue></MREFValue>" +
                        "<AuthCode></AuthCode>" +
                        "</EFTData>";
            }


            //byte[] dest=new 

            byte[] data1 = System.Text.Encoding.ASCII.GetBytes(ddd.ToString());
            //Array.Copy(data1, 0, bb, 4, aa.Length);

            byte[] stx = new byte[] { (byte)0x02 };
            byte[] etx = new byte[] { (byte)0x03 };

            MemoryStream outputStream = new MemoryStream();
            outputStream.Write(data1);
            outputStream.Write(etx);

            byte[] dataxor = outputStream.ToArray();

            byte[] field_lrc = new byte[1];
            byte b = 0;
            foreach (byte array in dataxor)
            {
                b ^= array;
            }
            field_lrc[0] = b;

            MemoryStream outputStreamdata = new MemoryStream();
            outputStreamdata.Write(stx);
            outputStreamdata.Write(dataxor);
            outputStreamdata.Write(field_lrc);

            byte[] data = outputStreamdata.ToArray();

            Basepage.hostDataLogFormat(data, "dataReq1 =>");

            return data;
        }



        public List<string> xPathReader(string xml, List<string> xpathList)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                List<string> result = new List<string>();
                foreach (string xpath in xpathList)
                {
                    XmlNodeList nodeList = document.DocumentElement.GetElementsByTagName(xpath);
                    string value = string.Empty;
                    foreach (XmlNode node in nodeList)
                    {
                        value = node.InnerText;
                    }
                    result.Add(value);
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }






        //void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        Basepage.logWrite(" >> listening started...");
        //        Thread.Sleep(500);
        //        var serialPort = (SerialPort)sender;

        //        int lenn = serialPort.BytesToRead;
        //        byte[] buffer = new byte[lenn];
        //        serialPort.Read(buffer, 0, buffer.Length);
        //        string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, lenn);
        //        Basepage.logWrite(" >>data received..:" + responseData);
        //        string d = Basepage.hostDataLogFormat(buffer, "Response=>");

        //        string val1 = buffer[0].ToString("X2");
        //        string val2 = buffer[1].ToString("X2");
        //        string val3 = buffer[lenn - 2].ToString("X2");
        //        string val4 = buffer[lenn - 1].ToString("X2");
        //        Basepage.logWrite("val1:" + val1 + " - val2:" + val2 + " - val3:" + val3 + " - val4:" + val4 + " - lenn:" + lenn);
        //        if (val3 == "03")
        //        {
        //            if (val1 == "06" && val2 == "02")
        //            {
        //                responseData = responseData.Substring(2, responseData.Length - 4);
        //            }
        //            else
        //            {
        //                responseData = responseData.Substring(1, responseData.Length - 3);
        //            }
        //            Basepage.logWrite("ResponseDataUpdated:" + responseData);

        //            List<string> list = new List<string>();

        //            list.Add("CommandType");
        //            list.Add("ErrorCode");
        //            list.Add("ResponseCode");
        //            list.Add("TxnDescription");
        //            list.Add("Reversed");

        //            List<string> a = xPathReader(responseData, list);
        //            if (a != null)
        //            {
        //                Basepage.logWrite("CommandType:" + a[0].ToString() + " - ErrorCode:" + a[1].ToString());
        //                string CommandType = a[0].ToString();
        //                if (CommandType == "047")
        //                {
        //                    serialPort.Write(ack, 0, ack.Length);
        //                    Basepage.logWrite("send ack CommandType:" + CommandType);
        //                    StopListening();
        //                    PaymentDeviceCom("106");
        //                }
        //                else
        //                {
        //                    string ErrorCode = a[1].ToString(); ;

        //                    if (ErrorCode.Length > 3)
        //                    {
        //                        serialPort.Write(ack, 0, ack.Length);
        //                        Basepage.logWrite("send ack CommandType:" + CommandType);
        //                        StopListening();
        //                        if (ErrorCode == "E067")
        //                        {
        //                            PaymentDeviceCom("106");
        //                        }
        //                        Basepage.logWrite("ErrorCode:" + ErrorCode);


        //                    }
        //                }

        //            }
        //            else
        //            {

        //            }
        //        }
        //        Basepage.logWrite(" >> Completed. >>>>>>");


        //    }
        //    catch (Exception ex)
        //    {
        //        Basepage.logWrite("4-Error:" + ex.Message);
        //    }

        //}


    }
}
