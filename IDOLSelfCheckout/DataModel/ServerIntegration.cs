using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.UserControls;
using Newtonsoft.Json;
//using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using static IDOLSelfCheckout.FacePay;

namespace IDOLSelfCheckout.DataModel
{
    public class ServerIntegration
    {
        public static string HostServiceIp;
        public static string HostServiceEndpoint;
        
        /// <summary>
        /// Constructor initialization
        /// </summary>
        public ServerIntegration() 
        {
            if (ConfigurationManager.AppSettings["Service_Ip"] != null)
                HostServiceIp = ConfigurationManager.AppSettings["Service_Ip"].ToString() ;
            if (ConfigurationManager.AppSettings["Service_EndPoint"] != null)
                HostServiceEndpoint = ConfigurationManager.AppSettings["Service_EndPoint"].ToString();
      
        }

        #region Transactional Private Methods

        /// <summary>
        /// CheckPosResponse - This method is used for checking the current status and values of current machine from server
        /// </summary>
        /// <returns>PosServiceResponseVM</returns>
        private PosServiceResponseVM PostPosRequest(PosServiceRequestVM serviceRequest)
        {
            PosServiceResponseVM serviceResponseVM = new PosServiceResponseVM();
            try
            {
                List<PosServiceRequestVM> serviceRequestVM = new List<PosServiceRequestVM>();
                serviceRequest.TerminalID = Basepage.HostTerminalId;
                serviceRequestVM.Add(serviceRequest);
                    
                using (var httpClient = new HttpClient())
                {
                    var requestJson = JsonConvert.SerializeObject(serviceRequestVM);
                    httpClient.BaseAddress = new Uri(HostServiceIp);
                    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    var postResult = httpClient.PostAsync(HostServiceEndpoint, content);
                    postResult.Wait();

                    var response = postResult.Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        result = result.Remove(result.Length - (Basepage.HostTerminalId.Length+2), (Basepage.HostTerminalId.Length + 2));
                        serviceResponseVM = JsonConvert.DeserializeObject<PosServiceResponseVM>(result)??new PosServiceResponseVM();
                        serviceResponseVM.Receipt = ConvertArabicText864To1256(serviceResponseVM.Receipt);
                        if (result.ToLower().Contains("securemode"))
                        {
                            serviceResponseVM.IsSecured = true;
                        }
                    }
                    else
                    {
                        Basepage.logWrite("Display Error - Code:" + response.StatusCode + "Display Error - Message:" + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                Basepage.logWrite(ex.Message);
            }

            return serviceResponseVM;
        }

        /// <summary>
        /// Convert CodeBase code into Arabic letter
        /// </summary>
        /// <param name="arabic"></param>
        /// <returns></returns>
        private string ConvertArabicText864To1256(string arabic)
        {
            List<byte> Arabic864 = new List<byte>();
            List<byte> revArabic = new List<byte>();
            string result = string.Empty;
            bool flag = false;


            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                for (int i = 0; i < arabic.Length; i++)
                {
                    if (arabic[i] == '[' && arabic[i + 4] == ']')
                    {
                        flag = true;
                        i++;
                        string hh = arabic[i++].ToString() + arabic[i++].ToString() + arabic[i++].ToString();
                        revArabic.Add((byte)int.Parse(hh));
                    }
                    else
                    {
                        if (flag)
                        {
                            if (arabic[i] == ' ' || arabic[i] == ',' || arabic[i] == '.' || arabic[i] == ':')
                            {
                                revArabic.Add((byte)arabic[i]);
                            }
                            else
                            {
                                byte[] B1 = revArabic.ToArray();
                                for (int j = 0; j < B1.Length; j++)
                                {
                                    Arabic864.Add(B1[B1.Length - j - 1]);
                                }
                                revArabic.Clear();
                                flag = false;
                            }
                        }
                        Arabic864.Add((byte)arabic[i]);
                    }
                }
                Arabic864.AddRange(revArabic);
                byte[] B = Arabic864.ToArray();
                result = Encoding.GetEncoding("windows-1256").GetString(B);
            }
            catch (Exception e)
            {
                Basepage.logWrite("Arabic Encoding Error - " + e.Message);
            }
            return result;
        }
        
        #endregion

        #region Transactional Public Methods

        /// <summary>
        /// Get Receipt from server using Display2
        /// </summary>
        /// <returns></returns>
        public PosServiceResponseVM GetReceipt()
        {
            return PostPosRequest(new PosServiceRequestVM
            {
                ProcessFlag = "display2",
                DisplayLine = "",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Void/Remove an item from receipt
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public PosServiceResponseVM VoidFromReceipt(string barCode)
        {
            return PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = "<70>" + barCode + "<80>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Add item to receipt
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public PosServiceResponseVM AddItemToReceipt(string barCode)
        {
            return PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = barCode + "<80>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Sign on to the server
        /// </summary>
        /// <param name="IsSecured"></param>
        /// <returns></returns>
        public PosServiceResponseVM SignOnRequest(bool IsSecured = false)
        {
            return PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = IsSecured ? "1<61>": "1<78>1<61>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Close the transaction with total
        /// </summary>
        /// <returns></returns>
        public PosServiceResponseVM TotalReceipt()
        {
            LedDisplay ledDisplay = new LedDisplay();
            ledDisplay.ChangeLedColor();

            return PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = "<81>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Process payment in cash and complete transaction
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public PosServiceResponseVM CashPayment(string Amount)
        {
            if (Amount.Contains("."))
            {
                decimal amount = decimal.Parse(Amount);
                amount = amount * 100;
                Amount = Convert.ToInt32(amount).ToString();
            }

            LedDisplay ledDisplay = new LedDisplay();
            ledDisplay.ChangeLedColor();

            var result = PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = Amount + "<91>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });

            sco_data.TransactionProcess = "FINISHED";

            return result;
        }

        /// <summary>
        /// Process payment by card and complete transaction
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public PosServiceResponseVM CardPayment(string Amount)
        {
            if (Amount.Contains("."))
            {
                decimal amount = decimal.Parse(Amount);
                amount = amount * 100;
                Amount = Convert.ToInt32(amount).ToString();
            }

            LedDisplay ledDisplay = new LedDisplay();
            ledDisplay.ChangeLedColor();

            var result = PostPosRequest(new PosServiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = Amount + "<96>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
            
            sco_data.TransactionProcess = "FINISHED";

            return result;
        }

        #endregion
    }
}
