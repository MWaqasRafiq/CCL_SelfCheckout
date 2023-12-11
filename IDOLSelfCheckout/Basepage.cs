// Decompiled with JetBrains decompiler
// Type: IDOLSelfCheckout.Basepage
// Assembly: IDOLSelfCheckout, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C67170A7-BC98-4C94-BBAA-42FB9B93CC1B
// Assembly location: C:\Users\Hi\Downloads\APP Masafi\IDOLSelfCheckout.dll

using DataModels.LsRetail;
using DataModels.Shared;
using IDOLSelfCheckout.BankDevice;
using IDOLSelfCheckout.Classes;
//using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using IDOLSelfCheckout.UserControls;
using Newtonsoft.Json;
using POS.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;


#nullable enable
namespace IDOLSelfCheckout
{
    public class Basepage
    {
        private readonly DataModels.LsRetail.view_models viewModels;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static string LogFilePath;
        public static string LSRetail_Header_Username;
        public static string LSRetail_Header_Password;
        public static string LSRetail_UserID;
        public static string LSRetail_TerminalDetails;
        public static string LSRetail_CreateNewTransactionNo;
        public static string LSRetail_AddTransaction;
        public static string LSRetail_TotalPressed;
        public static string LSRetail_TenderKeyPressed;
        public static string LSRetail_FinishPosTransaction;
        public static string LSRetail_ItemDetails;
        public static string LSRetail_ReceiptDetails;
        public static string LSRetail_PrintReceipt;
        public static string LSRetail_PaymentDeviceComPort;
        public static string LSRetail_LedComPort;
        public static string OPOS_PrinterName;
        public static string PopId_Host;
        public static int PopId_Port;
        public static int PopId_SecondaryPort;
        public static string HostTerminalId;
        public static string StoreNumber;
        public static bool LoyaltyRequested;
        public static bool LoyaltyScaned;
        public static string ServerName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void loadValues()
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                if (ConfigurationManager.AppSettings["LogFile"] != null)
                    Basepage.LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_Header_Username"] != null)
                    Basepage.LSRetail_Header_Username = ConfigurationManager.AppSettings["LSRetail_Header_Username"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_Header_Password"] != null)
                    Basepage.LSRetail_Header_Password = ConfigurationManager.AppSettings["LSRetail_Header_Password"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_UserID"] != null)
                    Basepage.LSRetail_UserID = ConfigurationManager.AppSettings["LSRetail_UserID"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_TerminalDetails"] != null)
                    Basepage.LSRetail_TerminalDetails = ConfigurationManager.AppSettings["LSRetail_TerminalDetails"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_CreateNewTransactionNo"] != null)
                    Basepage.LSRetail_CreateNewTransactionNo = ConfigurationManager.AppSettings["LSRetail_CreateNewTransactionNo"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_AddTransaction"] != null)
                    Basepage.LSRetail_AddTransaction = ConfigurationManager.AppSettings["LSRetail_AddTransaction"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_TotalPressed"] != null)
                    Basepage.LSRetail_TotalPressed = ConfigurationManager.AppSettings["LSRetail_TotalPressed"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_TenderKeyPressed"] != null)
                    Basepage.LSRetail_TenderKeyPressed = ConfigurationManager.AppSettings["LSRetail_TenderKeyPressed"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_FinishPosTransaction"] != null)
                    Basepage.LSRetail_FinishPosTransaction = ConfigurationManager.AppSettings["LSRetail_FinishPosTransaction"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_ItemDetails"] != null)
                    Basepage.LSRetail_ItemDetails = ConfigurationManager.AppSettings["LSRetail_ItemDetails"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_ReceiptDetails"] != null)
                    Basepage.LSRetail_ReceiptDetails = ConfigurationManager.AppSettings["LSRetail_ReceiptDetails"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_PrintReceipt"] != null)
                    Basepage.LSRetail_PrintReceipt = ConfigurationManager.AppSettings["LSRetail_PrintReceipt"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_PaymentDeviceComPort"] != null)
                    Basepage.LSRetail_PaymentDeviceComPort = ConfigurationManager.AppSettings["LSRetail_PaymentDeviceComPort"].ToString();
                if (ConfigurationManager.AppSettings["LSRetail_LedComPort"] != null)
                    Basepage.LSRetail_LedComPort = ConfigurationManager.AppSettings["LSRetail_LedComPort"].ToString();
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
                //if (ConfigurationManager.AppSettings["IsLocalConsumption"] != null)
                    //Basepage.IsLocalConsumption = Convert.ToBoolean(ConfigurationManager.AppSettings["IsLocalConsumption"]);
                if (ConfigurationManager.AppSettings["Terminal_Id"] != null)
                    Basepage.HostTerminalId = ConfigurationManager.AppSettings["Terminal_Id"].ToString();
                if (ConfigurationManager.AppSettings["Store_No"] != null)
                    Basepage.StoreNumber = ConfigurationManager.AppSettings["Store_No"].ToString();
                if (ConfigurationManager.AppSettings["ServerName"] != null)
                    Basepage.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
                Basepage.logWrite("--------------------- Application Started -----------------------");
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Error loadValues:" + ex.ToString());
            }
        }

        public bool startNewTransaction()
        {
            bool flag = false;
            try
            {
                LSscoApi lsscoApi = new LSscoApi();
                flag = lsscoApi.getTerminalDetails();
                if (flag)
                {
                    flag = lsscoApi.createNewTransactionNo(sco_data.StoreNumber, sco_data.TerminalNumber, sco_data.StaffId);
                    if (flag)
                        sco_data.TransactionProcess = "STARTED";
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

        public bool addItem(string receiptNo, string barcodeNo, prices product)
        {
            bool flag = true;
            if (flag)
            {
                logWrite("addItem0");
                sco_data.TransactionTotal = (Convert.ToDouble(sco_data.TransactionTotal) + Convert.ToDouble(product.price)).ToString("0.00");
                sco_data.TransactionVat = (Convert.ToDouble(sco_data.TransactionVat) + Convert.ToDouble(product.price) / 100.0 * 5.0).ToString("0.00");
                if (flag)
                {
                    logWrite("addItem1");
                    List<DataModels.LsRetail.items> itemList = sco_data.ItemList;
                    if(itemList.Where(x=>x.Name == product.name && x.Price == product.price).Any())
                    {
                        itemList.Where(w => w.Name == product.name && w.Price == product.price)
                                .ToList().ForEach(w => w.Qty = (Convert.ToInt32(w.Qty) + 1).ToString());
                    }
                    else
                    {
                        itemList.Add(new DataModels.LsRetail.items()
                        {
                            Name = product.name,
                            Qty = "1",
                            Price = product.price
                        });
                    }
                    logWrite("addItem2");
                    sco_data.LastItemDescription = product.name;
                    ucMainScreen.ItemListDataGrid.DataContext = (object)null;
                    if (itemList.Count <= 0)
                        return false;
                    logWrite("addItem3");
                    DataModels.LsRetail.view_models viewModels = new DataModels.LsRetail.view_models()
                    {
                        items = (IEnumerable<DataModels.LsRetail.items>)itemList
                    };
                    logWrite("addItem4");
                    ucMainScreen.ItemListDataGrid.DataContext = (object)viewModels;
                    logWrite("addItem5");
                    return true;
                }
            }
            return true;
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
                new OposPrinterCall().print();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public void updateTransactionDetails(prices product = null)
        {
            ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
            ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
            ucMainScreen.TransactionVat.Content = (object)("VAT   AED " + sco_data.TransactionVat);
            ucMainScreen.ItemInfo.Content = (object)sco_data.LastItemDescription;
        }

        public void errorMessage()
        {
            if(ucAsistantScreen.MessageText != null)
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
                //Basepage.logWrite(stringBuilder.ToString());
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Basepage.logWrite("hex format error:" + ex.ToString());
                return ex.ToString();
            }
        }

        //public static void ledWelcome()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('5', 'A');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledHelp()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('6', 'B');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledCardPayment()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('5', 'D');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledReceipt()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('3', 'C');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledClosed()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('1', 'C');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledNewCustomer()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('3', 'A');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //public static void ledItemScreen()
        //{
        //    try
        //    {
        //        new ledconnection().ledCon('5', ' ');
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
    }
}
