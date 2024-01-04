using DataModels.GeneralSCO;
using DataModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GeneralSCO.Core
{
    public class General_SCO
    {
        public static string POS_Username;
        public static string POS_Password;
        public static string LogFilePath;

        public static string ServiceIp;
        public static string TerminalId;
        public static string StoreNo;
        public static string TransactionId;


        public General_SCO()
        {
            if (ConfigurationManager.AppSettings["POS_Username"] != null)
                POS_Username = ConfigurationManager.AppSettings["POS_Username"].ToString();

            if (ConfigurationManager.AppSettings["POS_Password"] != null)
                POS_Password = ConfigurationManager.AppSettings["POS_Password"].ToString();

            if (ConfigurationManager.AppSettings["LogFile"] != null)
                LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();

            if (ConfigurationManager.AppSettings["Service_Ip"] != null)
                ServiceIp = ConfigurationManager.AppSettings["Service_Ip"].ToString();
            
            if (ConfigurationManager.AppSettings["Terminal_Id"] != null)
                TerminalId = ConfigurationManager.AppSettings["Terminal_Id"].ToString();

            if (ConfigurationManager.AppSettings["Store_No"] != null)
                StoreNo = ConfigurationManager.AppSettings["Store_No"].ToString();

            if (ConfigurationManager.AppSettings["LogFile"] != null)
                LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
        }

        public Tuple<int, string> SignTerminal(SignTerminalRequest terminalRequest)
        {
            try
            {
                var result = PostRequest<SignTerminalRequest, SignTerminalResponse>(ServiceIp + "SignTerminal", terminalRequest);
                if (result != null && result.Item2 == 200)
                {
                   var response = result.Item1 as SignTerminalResponse;
                    return new Tuple<int, string>(response.Code.Value,response.StatusMessage);
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, string>(response.Code.Value, response.Message);
                }
            }
            catch(Exception ex)
            {
                logWrite(ex.Message); 
                return new Tuple<int, string>(500, ex.Message);
            }
        }


        public Tuple<int,string> StartTransaction()
        {
            TransactionId = string.Empty;
            try
            {
                StartTransactionRequest request = new StartTransactionRequest()
                {
                    StoreNo = StoreNo,
                    TerminalNo = TerminalId
                };
                var result = PostRequest<StartTransactionRequest, StartTransactionResponse>(ServiceIp + "StartTransaction", request);
                if (result != null && result.Item2 == 200)
                {
                    var response = result.Item1 as StartTransactionResponse;
                    TransactionId = response.TransactionId;
                    return new Tuple<int, string>( response.Code.Value, response.TransactionId);
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, string>(response.Code.Value, response.Message);
                }
            }
            catch (Exception ex)
            {
                logWrite(ex.Message);
                return new Tuple<int, string>(500, ex.Message); ;
            }
        }

        public Tuple<int, ProductDetails>  ProductDetails()
        {
            try
            {
                ProductDetailsRequest request = new ProductDetailsRequest()
                {
                    BarCode = sco_data.ScannedBarcode,
                    StoreNo = StoreNo,
                    TerminalNo = TerminalId
                };
                var result = PostRequest<ProductDetailsRequest, ProductDetails>(ServiceIp + "ProductDetails", request);
                if (result != null && result.Item2 == 200)
                {
                    var response = result.Item1 as ProductDetails;
                    return new Tuple<int, ProductDetails>(result.Item2, response);
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, ProductDetails>(response.Code.Value, new ProductDetails());
                }
            }
            catch (Exception ex)
            {
                logWrite(ex.Message);
                return new Tuple<int, ProductDetails>(500, new ProductDetails());
            }
        }

        public Tuple<int, view_models> AddToCart()
        {
            view_models viewModels = new view_models();
            CartProducts cartProducts = new CartProducts();
            try
            {
                AddItemRequest request = new AddItemRequest()
                {
                    BarCode = sco_data.ScannedBarcode,
                    Qty = 1,
                    StoreNo = StoreNo,
                    TerminalNo = TerminalId,
                    TransactionId = TransactionId
                };
                var result = PostRequest<AddItemRequest, CartProducts>(ServiceIp + "AddToCart", request);
                if (result != null && result.Item2 == 200)
                {
                    cartProducts = result.Item1 as CartProducts;
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, view_models>(response.Code.Value, new view_models());
                }

                if (cartProducts != null && cartProducts.Products.Count() > 0)
                {
                    List<items> itemList = new List<items>();//sco_data.ItemList;

                    foreach (var a in cartProducts.Products)
                    {
                        itemList.Add(new items()
                        {
                            Name = a.Description,
                            Price = a.Price.ToString(),
                            Qty = a.Qty.ToString()
                        });
                    }
                    viewModels.items = itemList;

                    sco_data.TransactionTotal = cartProducts.Total.TotalAmount.ToString();
                    sco_data.TransactionVat = cartProducts.Total.TotalVat.ToString();
                }
            }
            catch (Exception ex)
            {
                logWrite(ex.Message);
            }
            return new Tuple<int, view_models>(200, viewModels);
        }

        public Tuple<int, view_models> OrderTotal()
        {
            view_models viewModels = new view_models();
            CartProducts cartProducts = new CartProducts();
            try
            {
                StartTransactionRequest request = new StartTransactionRequest() {
                    StoreNo = StoreNo,
                    TerminalNo = TerminalId
                };
                var result = PostRequest<StartTransactionRequest, CartProducts>(ServiceIp + "OrderTotal", request);
                if (result != null && result.Item2 == 200)
                {
                    cartProducts = result.Item1 as CartProducts;
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, view_models>(response.Code.Value, new view_models());
                }
                if (cartProducts != null && cartProducts.Products.Count() > 0)
                {
                    List<items> itemList = new List<items>();//sco_data.ItemList;

                    foreach (var a in cartProducts.Products)
                    {
                        itemList.Add(new items()
                        {
                            Name = a.Description,
                            Price = a.Price.ToString(),
                            Qty = a.Qty.ToString()
                        });
                    }
                    viewModels.items = itemList;

                    sco_data.TransactionTotal = cartProducts.Total.TotalAmount.ToString();
                    sco_data.TransactionVat = cartProducts.Total.TotalVat.ToString();
                }
            }
            catch (Exception ex)
            {
                logWrite(ex.Message);
            }
            return new Tuple<int, view_models>(200, viewModels);
        }


        public Tuple<int, dynamic> PrintReceipt(bool goGreen = false, string mobileNumber = "")
        {
            try
            {
                PrintReceiptRequest request = new PrintReceiptRequest()
                {
                    StoreNo = StoreNo,
                    TerminalNo = TerminalId,
                    TransactionId = TransactionId,
                    GoGreen = goGreen,
                    MobileNumber = mobileNumber
                };

                var result = PostRequest<PrintReceiptRequest, PrintReceiptResponse>(ServiceIp + "PrintReceipt", request);
                
                if (result != null && result.Item2 == 200)
                {
                    var receiptResponse = result.Item1 as PrintReceiptResponse;
                    return new Tuple<int, dynamic>(result.Item2, receiptResponse);
                }
                else
                {
                    var response = result.Item1 as Error;
                    return new Tuple<int, dynamic>(response.Code.Value, response.Message);
                }
            }
            catch (Exception ex)
            {
                logWrite(ex.Message);
                return new Tuple<int, dynamic>(500, ex.Message);
            }
        }


        private Tuple<dynamic,int> PostRequest<TIn, TOut>(string uri, TIn content) where TOut : new()
        {
            TOut @out = new TOut();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                var serialized = new StringContent(JsonConvert.SerializeObject(content), 
                                Encoding.UTF8, "application/json");

                var postResult = client.PostAsync(uri, serialized);
                postResult.Wait();

                var response = postResult.Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    @out =  JsonConvert.DeserializeObject<TOut>(result);
                    return new Tuple<dynamic, int>(@out, 200);
                }
                else
                {
                    logWrite("GeneralSCO > PostRequest encountered exception: " + response.ReasonPhrase);
                    string result = response.Content.ReadAsStringAsync().Result;
                    Error error = JsonConvert.DeserializeObject<Error>(result);
                    return new Tuple<dynamic, int>(error, ((int)response.StatusCode));
                }
            }
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
