using DataModels.LsRetail;
using DataModels.Shared;
using IDOLSelfCheckout.BankDevice;
using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using IDOLSelfCheckout.UserControls;
//using GeneralSCO.Core;
using Newtonsoft.Json;
//using On_Premises.Core;
using POS.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
//using Toshiba_SIT.Core;


#nullable enable
namespace IDOLSelfCheckout
{
    public class Basepage
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private readonly DataModels.LsRetail.view_models viewModels;
        public static string LogFilePath;
        public static string POS_Username;
        public static string POS_Password;
        public static string PaymentDeviceComPort;
        public static string OPOS_PrinterName;
        public static string PopId_Host;
        public static int PopId_Port;
        public static int PopId_SecondaryPort;
        public static string HostTerminalId;
        public static string StoreNumber;
        public static bool LoyaltyRequested;
        public static bool LoyaltyScaned;
        public static bool VoidRequested;
        public static bool VoidScaned;
        public static string ServerName;
        public static uint LedComPort;
        public static uint CCL_Lamp_Type;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void loadValues()
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                if (ConfigurationManager.AppSettings["LogFile"] != null)
                    Basepage.LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
                
                if (ConfigurationManager.AppSettings["POS_Username"] != null)
                    Basepage.POS_Username = ConfigurationManager.AppSettings["POS_Username"].ToString();
                
                if (ConfigurationManager.AppSettings["POS_Password"] != null)
                    Basepage.POS_Password = ConfigurationManager.AppSettings["POS_Password"].ToString();
                
                if (ConfigurationManager.AppSettings["PaymentDeviceComPort"] != null)
                    Basepage.PaymentDeviceComPort = ConfigurationManager.AppSettings["PaymentDeviceComPort"].ToString();
                
                if (ConfigurationManager.AppSettings["OPOS_PrinterName"] != null)
                    Basepage.OPOS_PrinterName = ConfigurationManager.AppSettings["OPOS_PrinterName"].ToString();

                //FacePay
                if (ConfigurationManager.AppSettings["PopId_Host"] != null)
                    Basepage.PopId_Host = ConfigurationManager.AppSettings["PopId_Host"].ToString();

                if (ConfigurationManager.AppSettings["PopId_Port"] != null)
                    Basepage.PopId_Port = Convert.ToInt32(ConfigurationManager.AppSettings["PopId_Port"]);

                if (ConfigurationManager.AppSettings["PopId_SecondaryPort"] != null)
                    Basepage.PopId_SecondaryPort = Convert.ToInt32(ConfigurationManager.AppSettings["PopId_SecondaryPort"]);

                //Service API
                if (ConfigurationManager.AppSettings["Terminal_Id"] != null)
                    Basepage.HostTerminalId = ConfigurationManager.AppSettings["Terminal_Id"].ToString();

                if (ConfigurationManager.AppSettings["Store_No"] != null)
                    Basepage.StoreNumber = ConfigurationManager.AppSettings["Store_No"].ToString();

                if (ConfigurationManager.AppSettings["ServerName"] != null)
                    Basepage.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();

                if (ConfigurationManager.AppSettings["LedComPort"] != null)
                    Basepage.LedComPort = Convert.ToUInt32(ConfigurationManager.AppSettings["LedComPort"]);

                if (ConfigurationManager.AppSettings["cclLampType"] != null)
                    Basepage.CCL_Lamp_Type = Convert.ToUInt32(ConfigurationManager.AppSettings["cclLampType"]);


                Basepage.logWrite("--------------------- Application Started -----------------------");
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Error loadValues:" + ex.ToString());
            }
        }

        //public bool startNewTransaction()
        //{
        //    bool flag = false;
        //    try
        //    {
        //        LSscoApi lsscoApi = new LSscoApi();
        //        flag = lsscoApi.getTerminalDetails();
        //        if (flag)
        //        {
        //            flag = lsscoApi.createNewTransactionNo(sco_data.StoreNumber, sco_data.TerminalNumber, sco_data.StaffId);
        //            if (flag)
        //                sco_data.TransactionProcess = "STARTED";
        //        }
        //        else
        //            this.errorMessage();
        //    }
        //    catch (Exception ex)
        //    {
        //        sco_data.ErrorMessage = ex.Message;
        //        this.errorMessage();
        //    }
        //    return flag;
        //}

        public bool addItem(string receiptNo, string barcodeNo, DataModels.prices product)
        {
            sco_data.ScannedBarcode = barcodeNo;
            sco_data.TransactionTotal = sco_data.TransactionTotal == "null" ? "0.00" : sco_data.TransactionTotal;
            sco_data.TransactionTotal = (Convert.ToDouble(sco_data.TransactionTotal) + Convert.ToDouble(product.price)).ToString("0.00");
            sco_data.TransactionVat = (Convert.ToDouble(sco_data.TransactionVat) + Convert.ToDouble(product.price) / 100.0 * 5.0).ToString("0.00");

            switch (ServerName)
            {
                case "SA":
                    Toshiba_SIT.Core.ToshibaSA toshibaSA = new Toshiba_SIT.Core.ToshibaSA();
                    toshibaSA.AddItemToReceipt(sco_data.ScannedBarcode);
                    break;
                case "LS":
                    GeneralSCO.Core.General_SCO lS_SCO = new GeneralSCO.Core.General_SCO();
                    //var a = lS_SCO.productList();
                    break;
                case "D3":
                    break;
                default:
                    AddItemOnPremises();
                    break;
            }
            return true;
        }

        public void AddItemOnPremises()
        {
            On_Premises.Core.OnPremises_SCO onPremises = new On_Premises.Core.OnPremises_SCO();
            Basepage basepage = new Basepage();
            var result = onPremises.AddItemOnPremises();

            if (result != null && result.items != null)
            {
                ucMainScreen.ItemListDataGrid.DataContext = (object)null;
                ucMainScreen.ItemListDataGrid.DataContext = (object)result;
                basepage.updateTransactionDetails();
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
            }
        }
        public void VoidItemOnPremises()
        {
            On_Premises.Core.OnPremises_SCO onPremises = new On_Premises.Core.OnPremises_SCO();
            Basepage basepage = new Basepage();
            var result = onPremises.VoidItemOnPremises();

            ucMainScreen.ItemListDataGrid.DataContext = (object)null;
            if (result != null && result.items != null)
            {
                ucMainScreen.ItemListDataGrid.DataContext = (object)result;
            }
            basepage.updateTransactionDetails();
            uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
        }

        public bool pressedTotal(string receiptNo)
        {
            bool flag = false;
            try
            {
                LSscoApi lsscoApi = new LSscoApi();
                if (flag)
                {
                    if (!flag)
                        ;
                }
                else
                    this.errorMessage();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public bool tenderPayment(
          string receiptNo,
          string tenderedAmount,
          string creditCardNo,
          string creditCardExpriyDate)
        {
            bool flag = false;
            try
            {
                flag = new Oma880().PaymentDeviceCom("100", tenderedAmount);
                if (flag)
                {
                    LSscoApi lsscoApi = new LSscoApi();
                    flag = true;
                    //flag = lsscoApi.tenderKeyPressed(receiptNo, tenderedAmount, creditCardNo, creditCardExpriyDate);
                    if (flag)
                    {
                        //flag = lsscoApi.FinishPosTransaction(receiptNo);
                        if (flag)
                            sco_data.TransactionProcess = "FINISHED";
                        sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                        sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                        ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                        ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
                        ucMainScreen.TransactionVat.Content = (object)("VAT   AED " + sco_data.TransactionVat);

                    }
                }
                else
                    this.errorMessage();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public bool tenderPaymentOffline(
          string receiptNo,
          string tenderedAmount,
          string creditCardNo,
          string creditCardExpriyDate)
        {
            bool flag = false;
            try
            {
                //SscoApi lsscoApi = new LSscoApi();
                flag = true;//lsscoApi.tenderKeyPressed(receiptNo, tenderedAmount, creditCardNo, creditCardExpriyDate);
                if (flag)
                {
                    //flag = lsscoApi.FinishPosTransaction(receiptNo);
                    if (flag)
                        sco_data.TransactionProcess = "FINISHED";
                    sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                    sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                    ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                    ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
                    ucMainScreen.TransactionVat.Content = (object)("VAT   AED " + sco_data.TransactionVat);
                }
                else
                    this.errorMessage();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public bool reprintLastTrnsaction()
        {
            bool flag = false;
            try
            {
                new OposPrinterCall().OPOSprint();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public void updateTransactionDetails(DataModels.prices product = null)
        {
            ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
            ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
            ucMainScreen.TransactionVat.Content = (object)("VAT   AED " + sco_data.TransactionVat);
            ucMainScreen.ItemInfo.Content = (object)sco_data.LastItemDescription;
        }

        public void errorMessage()
        {
            if (ucAsistantScreen.MessageText != null)
                ucAsistantScreen.MessageText.Text = sco_data.ErrorMessage;
        }

        public static void logWrite(string msg)
        {
            string format = "ddMMyyyy";
            string path = Basepage.LogFilePath + "POS" + DateTime.Now.ToString(format) + ".log";
            string str = "1.3.0.2";

            if (!File.Exists(path))
            {
                StreamWriter streamWriter = new StreamWriter(path);
                streamWriter.WriteLine("[" + str.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                streamWriter.Close();
            }
            else
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                    {
                        streamWriter.WriteLine("[" + str.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                        streamWriter.Close();
                    }
                }
            }
        }

        public static string hostDataLogFormat(byte[] bankFull, string msg)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                int num1 = 1;
                int num2 = 16;
                int num3 = 0;
                stringBuilder.Append(msg + "\r\n");
                stringBuilder.Append("0000   ");

                if (bankFull.Length > 16)
                {
                    foreach (byte num4 in bankFull)
                    {
                        string str1 = num4.ToString("X2") + " ";
                        if (num2 == num1 || num1 == bankFull.Length)
                        {
                            string str2 = "0000" + num2.ToString();
                            string str3 = str2.Substring(str2.Length - 4, 4);
                            byte[] destinationArray = new byte[16];
                            Array.Copy((Array)bankFull, num1 - 16, (Array)destinationArray, 0, 16);
                            string str4 = "";
                            foreach (byte num5 in destinationArray)
                                str4 = ((int)num5 & (int)byte.MaxValue) >= 32 ? str4 + ((char)num5).ToString() : str4 + ".";
                            stringBuilder.Append(str1 + str4 + "\r\n" + str3 + "   ");
                            ++num3;
                            num2 += 16;
                        }
                        else
                            stringBuilder.Append(str1);

                        ++num1;
                    }
                }
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("hex format error:" + ex.ToString());
                return ex.ToString();
            }
        }

    }
}
