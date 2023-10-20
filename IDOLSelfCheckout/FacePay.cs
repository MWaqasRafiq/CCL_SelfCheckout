using esegece.sgcWebSockets;
using Newtonsoft.Json;
using POS.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using IDOLSelfCheckout.UserControls;

namespace IDOLSelfCheckout
{
    public class FacePay
    {
        private TsgcWebSocketClient socketClient;
        public FacePay()
        {
            socketClient = new TsgcWebSocketClient();
            InitializeSocketConnection();
        }
        /// <summary>
        /// Initialize the WebSocket Client
        /// </summary>
        private void InitializeSocketConnection()
        {
            try
            {
                Basepage.logWrite("FacePay - Initialization!");

                socketClient.Host = Basepage.PopId_Host;
                socketClient.Port = Basepage.PopId_Port;

                socketClient.Specifications.RFC6455 = true;
                socketClient.OnError += OnSocketErrorEvent;
                socketClient.OnException += OnSocketExceptionEvent;
                socketClient.OnConnect += OnSocketConnectEvent;
                socketClient.OnMessage += OnSocketResponseEvent;
                socketClient.Start();
                Basepage.logWrite("FacePay - Initialized!");
            }
            catch (Exception ex) 
            {
                Basepage.logWrite("FacePay - " +ex.Message);
            }
        }

        public void IdentifyPerson()
        {
            try
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
                {
                    { "type", "CREATE_SESSION" }
                };
                string mes = JsonConvert.SerializeObject(keyValuePairs);
                socketClient.WriteData(mes);
                Basepage.logWrite("FacePay - Search request sent.");
            }
            catch (Exception ex) {
                Basepage.logWrite("FacePay - " + ex.Message);
            }
        }

        private void OnSocketExceptionEvent(TsgcWSConnection Connection, Exception e)
        {
            Basepage.logWrite("FacePay - " + "Exception: " + e.Message);
        }

        private void OnSocketErrorEvent(TsgcWSConnection Connection, string Error)
        {
            Basepage.logWrite("FacePay - " + "Error: " + Error);
        }

        /// <summary>
        /// On every response we'll receive the message on this method
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Text"></param>
        private void OnSocketResponseEvent(TsgcWSConnection Connection, string Text)
        {
            try
            {
                Basepage.logWrite("FacePay - " + "Message Received from Server: " + Text);
                if (Text.Contains("CREATE_SESSION"))
                {
                    string res = CreateSession(Connection, Text);
                }
                else if (Text.Contains("IDENTIFIED") && Text.Contains("USER_FOUND"))
                {
                    PayAfterFaceDetection(Connection, Text);
                }
                else if (Text.Contains("PAYMENT") && Text.Contains("APPROVED") && Text.Contains("TRANSACTION_COMPLETED"))
                {
                    GetTransactionDetails(Text);
                }
                else
                {
                    Basepage.logWrite("FacePay - Error Received from Server: " + Text);
                    throw new Exception(Text);
                }
            }
            catch (Exception ex) 
            { 
                Basepage.logWrite("FacePay - " + ex.Message); 
            }

        }

        private string CreateSession(TsgcWSConnection Connection, string Text)
        {
            var responseJson = JsonConvert.DeserializeObject<SessionInfoVM>(Text);
            if (responseJson != null && responseJson.sessionStatus.ToUpper() == "ACTIVE")
            {
                //////
                //will create session here
                sco_data.SessionId = responseJson.sessionId;
                //////
                ///
                SessionVM session = new SessionVM()
                {
                    sessionId = responseJson.sessionId,
                    type = "IDENTIFY"
                };

                string serializedJson = JsonConvert.SerializeObject(session);
                Text = Connection.WriteAndWaitData(serializedJson);
            }
            else
            {
                //log error
                throw new Exception("Session was not created!");
            }
            return Text;
        }

        private void OnSocketConnectEvent(TsgcWSConnection Connection)
        {
            Basepage.logWrite("FacePay - " + "Socket Connected! " + Connection.IP);
        }

        private void PayAfterFaceDetection(TsgcWSConnection Connection, string Text)
        {
            var responseJson = JsonConvert.DeserializeObject<IdentificationResponse>(Text);
            if (responseJson != null && responseJson.userId != null)
            {
                PaymentRequest paymentRequest = new PaymentRequest()
                {
                    type = "PAYMENT",
                    requestedAmount = Convert.ToInt32(Convert.ToDecimal(sco_data.TransactionTotal) *100),
                    sessionId = responseJson.sessionId,
                    taxAmount = Convert.ToInt32(Convert.ToDecimal(sco_data.TransactionVat) * 100),
                    tipAmount = 0
                };

                string pay = JsonConvert.SerializeObject(paymentRequest);
                Connection.WriteData(pay);

                Basepage.logWrite("FacePay - " + "Transaction process started.");
            }
            else
            {
                Basepage.logWrite("FacePay - " + "Please verify your identity by entering last 4 digits of your phone number.");
            }
        }

        private void GetTransactionDetails(string Text)
        {
            //Log the response
            Basepage.logWrite("FacePay - " + Text);
            var responseJson = JsonConvert.DeserializeObject<ResponseJson>(Text);

            //print receipt
            ucPrintScreenOPOS printScreenOPOS = new ucPrintScreenOPOS();

            //close session
            sco_data.SessionId = string.Empty;
            Basepage.logWrite("FacePay - " + "Transaction Completed.");
        }


        public class ResponseJson
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string faceId { get; set; }
            public string pin { get; set; }
            public int? transactionId { get; set; }
            public string loyalty { get; set; }
        }
        public class ResponseJsonError
        {
            public string errorCode { get; set; }
            public string errorMessage { get; set; }
        }
        public class PaymentRequest
        {
            public string type { get; set; }
            public string sessionId { get; set; }
            public string uniqueTransactionId { get; set; }
            public int requestedAmount { get; set; }
            public int tipAmount { get; set; }
            public int taxAmount { get; set; }
        }
        public class TransactionLog
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public int transactionId { get; set; }
            public string person_id { get; set; }
            public string amount { get; set; }
            public string tip { get; set; }
        }
        public class PaymentLog
        {
            public PaymentLog()
            {
                jsonResponse = new ResponseJson();
                jsonPaymentRequest = new PaymentRequest();
                jsonError = new ResponseJsonError();
            }
            public ResponseJson jsonResponse { get; set; }
            public PaymentRequest jsonPaymentRequest { get; set; }
            public ResponseJsonError jsonError { get; set; }
        }
        public class SessionInfoVM
        {
            public string sessionId { get; set; }
            public string sessionStatus { get; set; }
            public string type { get; set; }

        }
        public class SessionVM
        {
            public string sessionId { get; set; }
            public string type { get; set; }

        }
        public class IdentificationResponse
        {
            public string sessionId { get; set; }
            public string sessionStatus { get; set; }
            public string type { get; set; }
            public string userId { get; set; }
            public string status { get; set; }
        }
    }
}
