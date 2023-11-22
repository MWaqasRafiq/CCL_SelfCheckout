using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using DataModels.ToshibaSA;
using System.IO;

namespace Toshiba_SIT.Core
{
    public class ToshibaSA
    {
        public static string HostServiceIp;
        public static string HostServiceEndpoint;
        public static string HostTerminalId;
        public static string LogFilePath;

        /// <summary>
        /// Constructor
        /// </summary>
        public ToshibaSA()
        {
            if (ConfigurationManager.AppSettings["Service_Ip"] != null)
                HostServiceIp = ConfigurationManager.AppSettings["Service_Ip"].ToString();
            if (ConfigurationManager.AppSettings["Service_EndPoint"] != null)
                HostServiceEndpoint = ConfigurationManager.AppSettings["Service_EndPoint"].ToString();
            if (ConfigurationManager.AppSettings["Terminal_Id"] != null)
                HostTerminalId = ConfigurationManager.AppSettings["Terminal_Id"].ToString();
            if (ConfigurationManager.AppSettings["LogFile"] != null)
                LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
        }
  
        /// <summary>
        /// Private method for Toshiba SA call
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        private InvoiceResponseVM PostPosRequest(InvoiceRequestVM serviceRequest)
        {
            InvoiceResponseVM serviceResponseVM = new InvoiceResponseVM();
            try
            {
                List<InvoiceRequestVM> serviceRequestVM = new List<InvoiceRequestVM>();
                serviceRequest.TerminalID = HostTerminalId;
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
                        result = result.Remove(result.Length - (HostTerminalId.Length + 2), (HostTerminalId.Length + 2));
                        serviceResponseVM = JsonConvert.DeserializeObject<InvoiceResponseVM>(result) ?? new InvoiceResponseVM();
                        
                        serviceResponseVM.Receipt = ConvertArabicText864To1256(serviceResponseVM.Receipt);
                        
                        if (result.ToLower().Contains("securemode"))
                        {
                            serviceResponseVM.IsSecured = true;
                        }
                    }
                    else
                    {
                        logWrite("ToshibaSA Display Error - Code:" + response.StatusCode + "Display Error - Message:" + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                logWrite("ToshibaSA "+ ex.Message);
            }

            return serviceResponseVM;
        }

        /// <summary>
        /// Get Receipt from server using Display2
        /// </summary>
        /// <returns></returns>
        public InvoiceResponseVM GetReceipt()
        {
            return PostPosRequest(new InvoiceRequestVM
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
        public InvoiceResponseVM VoidFromReceipt(string barCode)
        {
            return PostPosRequest(new InvoiceRequestVM()
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
        public InvoiceResponseVM AddItemToReceipt(string barCode)
        {
            return PostPosRequest(new InvoiceRequestVM()
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
        public InvoiceResponseVM SignOnRequest(bool IsSecured = false)
        {
            return PostPosRequest(new InvoiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = IsSecured ? "1<61>" : "1<78>1<61>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });
        }

        /// <summary>
        /// Close the transaction with total
        /// </summary>
        /// <returns></returns>
        public InvoiceResponseVM TotalReceipt()
        {
            return PostPosRequest(new InvoiceRequestVM()
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
        public InvoiceResponseVM CashPayment(string Amount)
        {
            if (Amount.Contains("."))
            {
                decimal amount = decimal.Parse(Amount);
                amount = amount * 100;
                Amount = Convert.ToInt32(amount).ToString();
            }

            var result = PostPosRequest(new InvoiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = Amount + "<91>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });

            return result;
        }

        /// <summary>
        /// Process payment by card and complete transaction
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public InvoiceResponseVM CardPayment(string Amount)
        {
            if (Amount.Contains("."))
            {
                decimal amount = decimal.Parse(Amount);
                amount = amount * 100;
                Amount = Convert.ToInt32(amount).ToString();
            }

            var result = PostPosRequest(new InvoiceRequestVM()
            {
                ProcessFlag = "display",
                DisplayLine = Amount + "<96>",
                IPDevice = "",
                ListenerFlag = "1",
                qty = ""
            });

            return result;
        }

        /// <summary>
        /// Convert CodeBase code into Arabic letter
        /// </summary>
        /// <param name="arabic"></param>
        /// <returns></returns>
        public string ConvertArabicText864To1256(string arabic)
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
                logWrite("Arabic Encoding Error - " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Write Logs
        /// </summary>
        /// <param name="msg"></param>
        public static void logWrite(string msg)
        {
            string format = "ddMMyyyy";
            string path = LogFilePath + "POS" + DateTime.Now.ToString(format) + ".log";
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

    }
}
