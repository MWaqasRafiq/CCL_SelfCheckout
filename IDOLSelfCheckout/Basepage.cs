using DataModels.GeneralSCO;
using DataModels.Shared;
using IDOLSelfCheckout.BankDevice;
using IDOLSelfCheckout.Classes;
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
using System.Windows;
using System.Windows.Controls;

namespace IDOLSelfCheckout
{
    public class Basepage
    {
        private readonly DataModels.Shared.view_models viewModels;
        public static string LogFilePath;
        public static string TerminalId;
        public static string StoreNumber;
        public static string ServerName;
        public static uint CCL_Lamp_Type;
        public static string UserName;
        public static string PassWord;
        public static uint LedComPort;
        public static string PaymentDevicePort;
        public static string OposPrinterName;
        public static int PopIdSecondaryPort;
        public static string PopIdHost;
        public static int PopIdPort;
        public static bool LoyaltyRequested;
        public static bool VoidRequested;
        public static bool VoidScaned;
        public static bool LoyaltyScaned;
        public static string TransactionId;
        public static string VideoControlSource;
        public static string AdditionalLanguage;

        public void loadValues()
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");

                if (ConfigurationManager.AppSettings["LogFile"] != null)
                    Basepage.LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();

                if (ConfigurationManager.AppSettings["VideoControlSource"] != null)
                    Basepage.VideoControlSource = ConfigurationManager.AppSettings["VideoControlSource"].ToString();

                if (ConfigurationManager.AppSettings["UserName"] != null)
                    Basepage.UserName = ConfigurationManager.AppSettings["UserName"].ToString();
                
                if (ConfigurationManager.AppSettings["PassWord"] != null)
                    Basepage.PassWord = ConfigurationManager.AppSettings["PassWord"].ToString();
                
                if (ConfigurationManager.AppSettings["PaymentDevicePort"] != null)
                    Basepage.PaymentDevicePort = ConfigurationManager.AppSettings["PaymentDevicePort"].ToString();
                
                if (ConfigurationManager.AppSettings["OposPrinterName"] != null)
                    Basepage.OposPrinterName = ConfigurationManager.AppSettings["OposPrinterName"].ToString();

                //FacePay
                if (ConfigurationManager.AppSettings["PopIdHost"] != null)
                    Basepage.PopIdHost = ConfigurationManager.AppSettings["PopIdHost"].ToString();

                if (ConfigurationManager.AppSettings["PopIdPort"] != null)
                    Basepage.PopIdPort = Convert.ToInt32(ConfigurationManager.AppSettings["PopIdPort"]);

                if (ConfigurationManager.AppSettings["PopIdSecondaryPort"] != null)
                    Basepage.PopIdSecondaryPort = Convert.ToInt32(ConfigurationManager.AppSettings["PopIdSecondaryPort"]);

                //Service API
                if (ConfigurationManager.AppSettings["TerminalNo"] != null)
                    Basepage.TerminalId = ConfigurationManager.AppSettings["TerminalNo"].ToString();

                if (ConfigurationManager.AppSettings["StoreNo"] != null)
                    Basepage.StoreNumber = ConfigurationManager.AppSettings["StoreNo"].ToString();

                if (ConfigurationManager.AppSettings["ServerName"] != null)
                    Basepage.ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();

                if (ConfigurationManager.AppSettings["LedComPort"] != null)
                    Basepage.LedComPort = Convert.ToUInt32(ConfigurationManager.AppSettings["LedComPort"]);

                if (ConfigurationManager.AppSettings["cclLampType"] != null)
                    Basepage.CCL_Lamp_Type = Convert.ToUInt32(ConfigurationManager.AppSettings["cclLampType"]);

                if (ConfigurationManager.AppSettings["AdditionalLanguage"] != null)
                    Basepage.AdditionalLanguage = ConfigurationManager.AppSettings["AdditionalLanguage"].ToString();

                if (ConfigurationManager.AppSettings["Currency"] != null)
                    sco_data.TransactionCurrency = ConfigurationManager.AppSettings["Currency"].ToString();

                sco_data.TerminalNumber = TerminalId;
                Basepage.logWrite("--------------------- Application Started -----------------------");
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Error loadValues:" + ex.ToString());
            }
        }

        public bool addItem(string receiptNo, string barcodeNo, DataModels.prices product)
        {
            sco_data.ScannedBarcode = barcodeNo;
            sco_data.TransactionTotal = sco_data.TransactionTotal == "null" ? "0.00" : sco_data.TransactionTotal;
            sco_data.TransactionTotal = (Convert.ToDouble(sco_data.TransactionTotal) + Convert.ToDouble(product.price)).ToString("0.00");
            sco_data.TransactionVat = (Convert.ToDouble(sco_data.TransactionVat) + Convert.ToDouble(product.price) / 100.0 * 5.0).ToString("0.00");

            switch (ServerName)
            {
                case "SA":
                    //Toshiba_SIT.Core.ToshibaSA toshibaSA = new Toshiba_SIT.Core.ToshibaSA();
                    //toshibaSA.AddItemToReceipt(sco_data.ScannedBarcode);
                    break;
                case "GP":
                    GeneralPosAddItem();
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
            //On_Premises.Core.OnPremises_SCO onPremises = new On_Premises.Core.OnPremises_SCO();
            //Basepage basepage = new Basepage();
            //var result = onPremises.AddItemOnPremises();

            //if (result != null && result.items != null)
            //{
            //    ucMainScreen.ItemListDataGrid.DataContext = (object)null;
            //    ucMainScreen.ItemListDataGrid.DataContext = (object)result;
            //    basepage.updateTransactionDetails();
            //    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
            //}
            //else
            //{
            //    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
            //    Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
            //}
        }

        public string GeneralPosPrintReceipt(bool goGreen = false, string mobileNumber = "")
        {
            PrintReceiptResponse receiptResponse = new PrintReceiptResponse();
            var tuple = new GeneralSCO.Core.General_SCO().PrintReceipt(goGreen, mobileNumber);
            if (tuple.Item1 == 200 && tuple.Item2 != null)
            {
                receiptResponse = tuple.Item2 as PrintReceiptResponse;
            }
            else
            {
                var err = tuple.Item2 as Error ?? new Error();
                throw new Exception(err.Message);
            }

            return receiptResponse.Receipt;
        }

        public void AddItemMain()
        {
            if (sco_data.TransactionProcess == "STARTED")
            {
                if (sco_data.ScannedBarcode == "1111111111116")
                {
                    ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Visible;
                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
                }
                else
                {
                    // here we will choose the server that we want to integrate
                    switch (Basepage.ServerName)
                    {
                        case "SA":
                            //Toshiba_SIT.Core.ToshibaSA toshibaSA = new Toshiba_SIT.Core.ToshibaSA();
                            //toshibaSA.AddItemToReceipt(sco_data.ScannedBarcode);
                            break;
                        case "GP":
                            GeneralPosAddItem();
                            break;
                        case "D3":
                            break;
                        default:
                            AddItemOnPremises();
                            break;
                    }
                }
            }
        }

        public void GeneralPosAddItem()
        {
            GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();
            ProductDetails product = new ProductDetails();
            var tuple = general_SCO.ProductDetails();
            if(tuple.Item1 == 200)
            {
                product = tuple.Item2;
            }
            //((DataModels.Shared.view_models)ucMainScreen.ItemListDataGrid.DataContext).items
            if (product != null && !string.IsNullOrEmpty(product.Description))
            {
                var result = general_SCO.AddToCart();
                if(result.Item1 == 200)
                {
                    ucMainScreen.ItemListDataGrid.DataContext = (object)null;
                    if (result != null && result.Item2.items != null)
                    {
                        sco_data.ItemList = result.Item2.items.ToList();

                        sco_data.LastItemDescription = product.Description;
                        ucMainScreen.ItemListDataGrid.DataContext = (object)result.Item2;
                        updateTransactionDetails();
                        uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
                    }
                }
                else
                {
                    Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
                }
            }
            else
            {
                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
            }
        }

        public void VoidItemMain()
        {
            switch (Basepage.ServerName)
            {
                case "GP":
                    new Basepage().GeneralPosVoidCart();
                    break;
                default:
                    new Basepage().VoidItemOnPremises();
                    break;
            }
            Basepage.VoidRequested = false;
            Basepage.VoidScaned = true;
        }

        public void GeneralPosVoidCart()
        {
            GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();
            var result = general_SCO.VoidFromCart();
            if (result.Item1 == 200)
            {
                ucMainScreen.ItemListDataGrid.DataContext = (object)null;
                if (result != null && result.Item2.items != null)
                {
                    sco_data.ItemList = result.Item2.items.ToList();

                    sco_data.LastItemDescription = string.Empty;
                    ucMainScreen.ItemListDataGrid.DataContext = (object)result.Item2;
                    updateTransactionDetails();
                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
                }
            }
            else
            {
                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
            }
        }

        public void OrderTotal()
        {
            GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();

            var result = general_SCO.OrderTotal();
            if (result.Item1 == 200)
            {
                ucMainScreen.ItemListDataGrid.DataContext = (object)null;
                if (result != null && result.Item2.items != null)
                {
                    sco_data.ItemList = result.Item2.items.ToList();

                    ucMainScreen.ItemListDataGrid.DataContext = (object)result.Item2;
                    updateTransactionDetails();
                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
                }
            }
            else
            {
                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Went to Help");
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
            }
        }

        public void VoidItemOnPremises()
        {
            //On_Premises.Core.OnPremises_SCO onPremises = new On_Premises.Core.OnPremises_SCO();
            //Basepage basepage = new Basepage();
            //var result = onPremises.VoidItemOnPremises();

            //ucMainScreen.ItemListDataGrid.DataContext = (object)null;
            //if (result != null && result.items != null)
            //{
            //    ucMainScreen.ItemListDataGrid.DataContext = (object)result;
            //}
            //basepage.updateTransactionDetails();
            //uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
        }

        public bool tenderPayment(string receiptNo, string tenderedAmount, string creditCardNo, string creditCardExpriyDate)
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
                        {
                            sco_data.TransactionProcess = "FINISHED";
                            if (Basepage.ServerName == "GP")
                            {
                                AddPaymentRequest request = new AddPaymentRequest()
                                {
                                    Amount = Convert.ToDecimal(tenderedAmount),
                                    PaymentType = "card",
                                    TransactionId = TransactionId,
                                    Currency = sco_data.TransactionCurrency,
                                    BankTransactionId = string.Empty,
                                    CardMaskNo = sco_data.MaskCardNumber
                                };
                                var tuple = new GeneralSCO.Core.General_SCO().AddPayment(request);
                                if (tuple.Item1 == 200 && tuple.Item2 != null)
                                {
                                    if (tuple.Item2 as AddPaymentResponse != null)
                                    {
                                        sco_data.MaskCardNumber = string.Empty;
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                        sco_data.ErrorMessage = "Cash Payment: No data has found.";
                                        this.errorMessage();
                                    }
                                }
                                else
                                {
                                    var err = tuple.Item2 as Error ?? new Error() {Message = "Error Cash Payment: No data has found." };
                                    sco_data.ErrorMessage = err.Message;
                                }
                            }
                        }
                        sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                        sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                        ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                        ucMainScreen.TransactionTotal.Content = (object)("TOTAL " + sco_data.TransactionCurrency + " " + sco_data.TransactionTotal);
                        ucMainScreen.TransactionVat.Content = (object)("VAT  " + sco_data.TransactionCurrency + " " + sco_data.TransactionVat);

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

        public bool tenderPaymentOffline(string receiptNo, string tenderedAmount, string creditCardNo, string creditCardExpriyDate)
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
                    {
                        sco_data.TransactionProcess = "FINISHED";
                        if (Basepage.ServerName == "GP")
                        {
                            AddPaymentRequest request = new AddPaymentRequest() 
                            { 
                                Amount = Convert.ToDecimal(tenderedAmount),
                                PaymentType = "cash",
                                TransactionId = TransactionId,
                                Currency = sco_data.TransactionCurrency,
                                BankTransactionId = string.Empty,
                                CardMaskNo = string.Empty
                            };
                            var tuple = new GeneralSCO.Core.General_SCO().AddPayment(request);
                            if (tuple.Item1 == 200 && tuple.Item2 != null)
                            {
                                if (tuple.Item2 as AddPaymentResponse != null)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag = false;
                                    sco_data.ErrorMessage = "Cash Payment: No data has found.";
                                    this.errorMessage();
                                }
                            }
                            else
                            {
                                var err = tuple.Item2 as Error;
                                sco_data.ErrorMessage = err.Message;
                            }
                        }
                    }
                    sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                    sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                    ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                    ucMainScreen.TransactionTotal.Content = (object)("TOTAL " + sco_data.TransactionCurrency + " " + sco_data.TransactionTotal);
                    ucMainScreen.TransactionVat.Content = (object)("VAT  " + sco_data.TransactionCurrency + " " + sco_data.TransactionVat);
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
                if (Basepage.ServerName == "GP")
                {
                    PrintReceiptResponse receiptResponse = new PrintReceiptResponse();
                    var tuple = new GeneralSCO.Core.General_SCO().PrintLastReceipt();
                    if (tuple.Item1 == 200 && tuple.Item2 != null)
                    {
                        receiptResponse = tuple.Item2 as PrintReceiptResponse;
                        if(receiptResponse != null)
                        {
                            flag = new OposPrinterCall().OposGeneralprint(receiptResponse.Receipt);
                        }
                        else
                        {
                            sco_data.ErrorMessage = "Receipt not found";
                            this.errorMessage();
                        }
                    }
                    else
                    {
                        var err = tuple.Item2 as Error;
                        sco_data.ErrorMessage = err.Message;
                    }

                }
                else 
                    flag = new OposPrinterCall().OPOSprint();
            }
            catch (Exception ex)
            {
                sco_data.ErrorMessage = ex.Message;
                this.errorMessage();
            }
            return flag;
        }

        public void updateTransactionDetails()
        {
            ucMainScreen.TransactionDetails.Content = (object)("StoreNo: " + sco_data.StoreNumber + "       Terminal: " + sco_data.TerminalNumber + "  \r\nReceiptNumber: " + sco_data.ReceiptNumber);
            ucMainScreen.TransactionTotal.Content = (object)("TOTAL " + sco_data.TransactionCurrency + " " + (sco_data.TransactionTotal == null ? "0.00": sco_data.TransactionTotal));
            ucMainScreen.TransactionVat.Content = (object)("VAT  " + sco_data.TransactionCurrency + " " + (sco_data.TransactionVat == null ? "0.00" : sco_data.TransactionVat));
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
