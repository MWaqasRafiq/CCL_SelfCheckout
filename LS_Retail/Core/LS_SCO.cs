using DataModels;
using DataModels.LsRetail;
using DataModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LS_Retail.Core
{
    public class LS_SCO
    {
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
        public static string LogFilePath;

        public LS_SCO()
        {
            if (ConfigurationManager.AppSettings["LSRetail_Header_Username"] != null)
                LSRetail_Header_Username = ConfigurationManager.AppSettings["LSRetail_Header_Username"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_Header_Password"] != null)
                LSRetail_Header_Password = ConfigurationManager.AppSettings["LSRetail_Header_Password"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_UserID"] != null)
                LSRetail_UserID = ConfigurationManager.AppSettings["LSRetail_UserID"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_TerminalDetails"] != null)
                LSRetail_TerminalDetails = ConfigurationManager.AppSettings["LSRetail_TerminalDetails"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_CreateNewTransactionNo"] != null)
                LSRetail_CreateNewTransactionNo = ConfigurationManager.AppSettings["LSRetail_CreateNewTransactionNo"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_AddTransaction"] != null)
                LSRetail_AddTransaction = ConfigurationManager.AppSettings["LSRetail_AddTransaction"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_TotalPressed"] != null)
                LSRetail_TotalPressed = ConfigurationManager.AppSettings["LSRetail_TotalPressed"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_TenderKeyPressed"] != null)
                LSRetail_TenderKeyPressed = ConfigurationManager.AppSettings["LSRetail_TenderKeyPressed"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_FinishPosTransaction"] != null)
                LSRetail_FinishPosTransaction = ConfigurationManager.AppSettings["LSRetail_FinishPosTransaction"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_ItemDetails"] != null)
                LSRetail_ItemDetails = ConfigurationManager.AppSettings["LSRetail_ItemDetails"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_ReceiptDetails"] != null)
                LSRetail_ReceiptDetails = ConfigurationManager.AppSettings["LSRetail_ReceiptDetails"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_PrintReceipt"] != null)
                LSRetail_PrintReceipt = ConfigurationManager.AppSettings["LSRetail_PrintReceipt"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_PaymentDeviceComPort"] != null)
                LSRetail_PaymentDeviceComPort = ConfigurationManager.AppSettings["LSRetail_PaymentDeviceComPort"].ToString();
            if (ConfigurationManager.AppSettings["LSRetail_LedComPort"] != null)
                LSRetail_LedComPort = ConfigurationManager.AppSettings["LSRetail_LedComPort"].ToString();
            if (ConfigurationManager.AppSettings["LogFile"] != null)
                LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
        }

        public PricesRequest productList()
        {
            try
            {
                PricesRequest masafiPricesRequest;
                using (StreamReader streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\products" + "\\products.json"))
                    masafiPricesRequest = JsonConvert.DeserializeObject<PricesRequest>(streamReader.ReadToEnd());
                string requestUriString = "https://api.ommasign.com/v1/datasource/2118/force";
                Console.WriteLine("baseurl: {0}", (object)requestUriString);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
                string s = JsonConvert.SerializeObject((object)masafiPricesRequest).ToString();
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                Console.WriteLine("productList request: {0}", (object)s);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.Headers.Add("Authorization", "3956e4248611923d479c83ed05d050448a827c0baa149fe04e5bac1a693a89aa");
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                    requestStream.Write(bytes, 0, bytes.Length);
                Console.WriteLine("productList responseString: {0}", (object)new StreamReader(httpWebRequest.GetResponse().GetResponseStream()).ReadToEnd());
                return masafiPricesRequest;
            }
            catch (WebException ex)
            {
                WebResponse response;
                using (response = ex.Response)
                {
                    Console.WriteLine("Error code: {0}", (object)((HttpWebResponse)response).StatusCode.ToString());
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            Console.WriteLine("ErrorResponse: {0}", (object)streamReader.ReadToEnd());
                            return new PricesRequest();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new PricesRequest();
            }
        }

        public Boolean getTerminalDetails()
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ret=\"urn:microsoft-dynamics-schemas/page/retailusers\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <ret:Read>\n" +
                "         <ret:ID>" + LSRetail_UserID + "</ret:ID>\n" +
                "      </ret:Read>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_TerminalDetails;// "http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Page/Retailusers";
            string soapAction = "urn:microsoft-dynamics-schemas/page/retailusers";
            string res = lsConnection(xml, url, soapAction);

            List<string> list = new List<string>();

            list.Add("SelfCheckout_Staff_ID");
            list.Add("Store_No");
            list.Add("POS_Terminal");

            List<string> a = xPathReader(res, list);
            if (a.Count > 2)
            {
                sco_data.StaffId = a[0].ToString();
                sco_data.StoreNumber = a[1].ToString();
                sco_data.TerminalNumber = a[2].ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean createNewTransactionNo(string storeNo, string posTerminalNo, string staffId)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
               "   <soapenv:Header/>\n" +
               "   <soapenv:Body>\n" +
               "      <self:InsertPosTransaction>\n" +
               "         <self:storeNo>" + storeNo + "</self:storeNo>\n" +
               "         <self:posTerminalNo>" + posTerminalNo + "</self:posTerminalNo>\n" +
               "         <self:staffId>" + staffId + "</self:staffId>\n" +
               "      </self:InsertPosTransaction>\n" +
               "   </soapenv:Body>\n" +
               "</soapenv:Envelope>";

            string url = LSRetail_CreateNewTransactionNo;// "http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";

            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout:InsertPosTransactionLine";
            string res = lsConnection(xml, url, soapAction);


            List<string> list = new List<string>();

            list.Add("return_value");

            List<string> a = xPathReader(res, list);
            if (a.Count > 0)
            {
                sco_data.ReceiptNumber = a[0].ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean addTransaction(string receiptNo, string barcodeNo)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <self:InsertPosTransactionLine>\n" +
                "         <self:receiptNo>" + receiptNo + "</self:receiptNo>\n" +
                "         <self:barcodeNo>" + barcodeNo + "</self:barcodeNo>\n" +
                "         <self:decQty>1</self:decQty>\n" +
                "      </self:InsertPosTransactionLine>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_AddTransaction; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout:InsertPosTransactionLine";
            string res = lsConnection(xml, url, soapAction);

            List<string> list = new List<string>();

            list.Add("return_value");

            List<string> a = xPathReader(res, list);
            if (a.Count > 0)
            {
                string returnVal = a[0].ToString();
                if (returnVal == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Boolean totalPressed(string receiptNo)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <self:TotalPressed>\n" +
                "         <self:receiptNo>" + receiptNo + "</self:receiptNo>\n" +
                "      </self:TotalPressed>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_TotalPressed; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout";
            string res = lsConnection(xml, url, soapAction);

            List<string> list = new List<string>();

            list.Add("TotalPressed_Result");

            List<string> a = xPathReader(res, list);
            if (a.Count > 0)
            {
                return true;
                //string returnVal = a[0].ToString();
                //if (returnVal == "SUCCESS")
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            else
            {
                return false;
            }
        }

        public bool tenderKeyPressed(string receiptNo, string tenderedAmount, string creditCardNo, string creditCardExpriyDate)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <self:TenderKeyPressed>\n" +
                "         <self:receiptNo>" + receiptNo + "</self:receiptNo>\n" +
                "         <self:tenderTypeCode>3</self:tenderTypeCode>\n" +
                "         <self:creditCardNo>" + creditCardNo + "</self:creditCardNo>\n" +
                "         <self:creditCardExpriyDate>" + creditCardExpriyDate + "</self:creditCardExpriyDate>\n" +
                "         <self:tenderedAmount>" + tenderedAmount + "</self:tenderedAmount>\n" +
                "      </self:TenderKeyPressed>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_TenderKeyPressed; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout";
            string res = lsConnection(xml, url, soapAction);
            List<string> list = new List<string>();

            list.Add("return_value");

            List<string> a = xPathReader(res, list);
            if (a.Count > 0)
            {
                string returnVal = a[0].ToString();
                if (returnVal == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Boolean FinishPosTransaction(string receiptNo)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <self:PostTransaction>\n" +
                "         <self:receiptNo>" + receiptNo + "</self:receiptNo>\n" +
                "      </self:PostTransaction>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_FinishPosTransaction; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout";
            string res = lsConnection(xml, url, soapAction);
            List<string> list = new List<string>();

            list.Add("return_value");

            List<string> a = xPathReader(res, list);
            if (a.Count > 0)
            {
                string returnVal = a[0].ToString();
                sco_data.TransactionNo = returnVal;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean getItemDetails(string receiptNo)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:item=\"urn:microsoft-dynamics-schemas/page/itemwisedetails\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <item:ReadMultiple>\n" +
                "         <!--1 or more repetitions:-->\n" +
                "         <item:filter>\n" +
                "            <item:Field>Receipt_No</item:Field>\n" +
                "            <item:Criteria>" + receiptNo + "</item:Criteria>\n" +
                "         </item:filter>\n" +
                "         <!--Optional:-->\n" +
                "         <item:bookmarkKey></item:bookmarkKey>\n" +
                "         <item:setSize></item:setSize>\n" +
                "      </item:ReadMultiple>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_ItemDetails; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Page/ItemWiseDetails";
            string soapAction = "urn:microsoft-dynamics-schemas/page/itemwisedetails";
            string res = lsConnection(xml, url, soapAction);

            //List<string> list = new List<string>();

            //list.Add("Description");
            //list.Add("Price");
            //list.Add("Quantity");

            List<items> itm = XPathMultiReader2(res);
            sco_data.ItemList = itm;



            //List<string> a2 = xPathMultiReader(res, "Price");
            //List<string> a3 = xPathMultiReader(res, "Quantity");

            if (itm.Count > 0)
            {
                view_models viewModel = new view_models
                {
                    items = itm
                };

                //ucMainScreen.ItemListDataGrid.DataContext = viewModel;//code to update
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean getReceiptDetails(string receiptNo)
        {
            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:rec=\"urn:microsoft-dynamics-schemas/page/receiptdetails\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <rec:Read>\n" +
                "         <rec:Receipt_No>" + receiptNo + "</rec:Receipt_No>\n" +
                "      </rec:Read>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_ReceiptDetails; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Page/ReceiptDetails";
            string soapAction = "urn:microsoft-dynamics-schemas/page/receiptdetails";
            string res = lsConnection(xml, url, soapAction);

            List<string> list = new List<string>();

            list.Add("Key");
            list.Add("Gross_Amount");
            list.Add("Line_Discount");
            list.Add("Trans_Date");
            list.Add("Trans_Time");
            list.Add("Net_Amount");

            List<string> a = xPathReader(res, list);
            if (a.Count > 2)
            {
                sco_data.TransactionKey = a[0].ToString();
                sco_data.TransactionTotal = Convert.ToDecimal(a[1]).ToString("0.00");
                sco_data.TransactionDiscount = Convert.ToDecimal(a[2]).ToString("0.00");
                sco_data.TransactionDateTime = a[3].ToString() + " " + a[4].ToString().Substring(0, 5);
                sco_data.TransactionVat = (Convert.ToDecimal(a[1]) - Convert.ToDecimal(a[5])).ToString("0.00");
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean printReceipt(string storeNo, string posTerminalNo, string transactionNo)
        {

            string xml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:self=\"urn:microsoft-dynamics-schemas/codeunit/Self_Checkout\">\n" +
                "   <soapenv:Header/>\n" +
                "   <soapenv:Body>\n" +
                "      <self:PrintReceipt>\n" +
                "         <self:storeNo>" + storeNo + "</self:storeNo>\n" +
                "         <self:posTerminalNo>" + posTerminalNo + "</self:posTerminalNo>\n" +
                "         <self:transactionNo>" + transactionNo + "</self:transactionNo>\n" + // this is trx number from finishpostrasanction response.
                "         <self:print>true</self:print>\n" +
                "      </self:PrintReceipt>\n" +
                "   </soapenv:Body>\n" +
                "</soapenv:Envelope>";

            string url = LSRetail_PrintReceipt; //"http://192.168.2.118:8047/WS/WS/CRONUS LS 1101 W1 Demo/Codeunit/Self_Checkout";
            string soapAction = "urn:microsoft-dynamics-schemas/codeunit/Self_Checkout";
            string res = lsConnection(xml, url, soapAction);

            List<string> list = new List<string>();

            list.Add("return_value");

            List<string> a = xPathReader(res, list);
            if (a[0].ToString() != "")
            {
                return true;
            }
            else
            {
                sco_data.ErrorMessage = res;
                return false;
            }

        }

        public string lsConnection(string xml, string url, string SoapAction)
        {
            WebResponse response;
            string responseString;
            try
            {
                logWrite("url:" + url);
                logWrite("RequestData ==>:" + xml);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                var data = Encoding.ASCII.GetBytes(xml);
                request.Method = "POST";
                request.ContentType = "text/xml;charset=\"utf-8\"";
                request.Accept = "text/xml";
                request.Headers.Add("SOAPAction", SoapAction);
                request.ContentLength = data.Length;
                var username = LSRetail_Header_Username; // "test";
                var password = LSRetail_Header_Password; // "Idol@123";
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                               .GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                logWrite("ResponseData <==:" + responseString);
                string xmlString = responseString;

            }
            catch (WebException e)
            {
                using (response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    sco_data.ErrorMessage = httpResponse.StatusCode.ToString();
                    logWrite("Error code:" + sco_data.ErrorMessage);

                    using (Stream datares = response.GetResponseStream())
                    using (var reader = new StreamReader(datares))
                    {
                        string text = reader.ReadToEnd();
                        sco_data.ErrorMessage = sco_data.ErrorMessage + " " + text;
                        logWrite("ErrorResponse:" + text);
                        responseString = text;
                    }
                }
            }
            catch (Exception ex)
            {
                responseString = ex.Message;
            }
            return responseString;
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

        public List<string> xPathMultiReader(string xml, string xpathChild)
        {
            List<string> result = new List<string>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XmlNodeList childNodeList = document.DocumentElement.GetElementsByTagName(xpathChild);
            string value = string.Empty;
            foreach (XmlNode childNode in childNodeList)
            {
                value = childNode.InnerText;
                result.Add(value);
            }
            return result;
        }

        public List<items> XPathMultiReader2(string xml)
        {
            //items itm = new items();
            string xpathChild1 = "Description";
            string xpathChild2 = "Amount";
            string xpathChild3 = "Quantity";
            List<items> result = new List<items>();

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XmlNodeList childNodeList = document.DocumentElement.GetElementsByTagName(xpathChild1);
            XmlNodeList childNodeList2 = document.DocumentElement.GetElementsByTagName(xpathChild2);
            XmlNodeList childNodeList3 = document.DocumentElement.GetElementsByTagName(xpathChild3);
            string value = string.Empty;
            for (int i = 0; i < childNodeList.Count; i++)
            {
                XmlNode childNodeName = childNodeList[i];
                XmlNode childNodePrice = childNodeList2[i];
                XmlNode childNodeQty = childNodeList3[i];
                string name = childNodeName.InnerText;
                string price = childNodePrice.InnerText;
                string qty = childNodeQty.InnerText;
                if (name.Length > 18)
                {
                    name = name.Substring(0, 18);
                }
                sco_data.LastItemDescription = name;
                result.Add(new items
                {
                    Name = name,
                    Qty = qty,
                    Price = Convert.ToDecimal(price).ToString("0.00")
                });
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
