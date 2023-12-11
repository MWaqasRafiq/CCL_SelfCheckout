//using esegece.sgcWebSockets;
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
using System.Net.WebSockets;
using System.Threading;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using Toshiba_SIT.Core;
using DataModels.Shared;

namespace IDOLSelfCheckout
{
    public class FacePay
    {
        private int count;
        private ClientWebSocket socketClient;

        /// <summary>
        /// FacePay constructor initialization
        /// </summary>
        public FacePay()
        {
            count = 0;
            socketClient = new ClientWebSocket();
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
                Uri serverUri = new Uri("ws://"+ Basepage.PopId_Host + ":"+ Basepage.PopId_Port.ToString() + ""); // Replace with your WebSocket server URL
                socketClient.ConnectAsync(serverUri, CancellationToken.None);
                Thread.Sleep(1000);
                Basepage.logWrite("FacePay - Initialized!");
            }
            catch (Exception ex) 
            {
                Basepage.logWrite("FacePay - " +ex.Message);
            }
        }

        /// <summary>
        /// Initializing the Socket Connection
        /// </summary>
        /// <returns></returns>
        public string IdentifyPerson()
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(sco_data.SessionId))
                {
                    if (socketClient.State == WebSocketState.Open)
                    {
                        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
                    {
                        { "type", "CREATE_SESSION" }
                    };
                        string mes = JsonConvert.SerializeObject(keyValuePairs);
                        byte[] sendBytes = Encoding.UTF8.GetBytes(mes);
                        socketClient.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        //if(count < 100)
                        //    IdentifyPerson();
                    }
                    Basepage.logWrite("FacePay - Search request sent.");

                    var buffer = new byte[1024];
                    var segment = new ArraySegment<byte>(buffer);
                    var receivedMessage = socketClient.ReceiveAsync(segment, CancellationToken.None);
                    result = Encoding.UTF8.GetString(buffer, 0, receivedMessage.Result.Count);
                    Basepage.logWrite("FacePay - " + receivedMessage);
                }
                else
                    result = "CREATE_SESSION";
            }
            catch (Exception ex) {
                Basepage.logWrite("FacePay - " + ex.Message);
            }
            result = ProcessFacePay(result);
            return result;
        }

        /// <summary>
        /// Create socket session creation request
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        private string ProcessFacePay(string Text)
        {
            try
            {
                if (Text.Contains("CREATE_SESSION") || string.IsNullOrEmpty(Text))
                {
                    Text = CreateSession(Text);
                }
                if (Text.Contains("IDENTIFIED") && Text.Contains("USER_FOUND"))
                {
                    if (Text.Contains("2FA_REQUIRED"))
                    {
                        // open screen with keyboard for PIN
                    }
                    else
                        Text = PayAfterFaceDetection(Text);
                }
                if (Text.Contains("PAYMENT") && Text.Contains("APPROVED") && Text.Contains("TRANSACTION_COMPLETED"))
                {
                    if (Text.Contains("2FA_REQUIRED"))
                    {
                        // open screen with keyboard for PIN
                    }
                    else
                        CompleteTransaction(Text);
                }
                else
                {
                    Basepage.logWrite("FacePay - Error Received from Server: " + Text);

                    CloseSession();

                    if (Text.Contains("DECLINED"))
                    {
                        var responseJson = JsonConvert.DeserializeObject<ResponseJson>(Text);
                        uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                    }
                    else
                    {
                        uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                    }
                }
            }
            catch(Exception ex)
            {
                Basepage.logWrite("FacePay - Message:" + ex.Message + " - Data:" + Text);
            }

            return Text;
        }

        /// <summary>
        /// Store session and send a request for face identification
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string CreateSession(string Text)
        {
            try
            {
                if(!string.IsNullOrEmpty(sco_data.SessionId))
                {
                    IdentifyVM session = new IdentifyVM()
                    {
                        sessionId = sco_data.SessionId,
                        type = "IDENTIFY",
                        identifyFields = "[FIRST_NAME]"
                    };

                    string serializedJson = JsonConvert.SerializeObject(session);
                    Text = SendAndReceiveMessage(serializedJson);
                }
                else
                {
                    var responseJson = JsonConvert.DeserializeObject<SessionInfoVM>(Text);
                    if (responseJson != null && responseJson.sessionStatus.ToUpper() == "ACTIVE")
                    {
                        //////
                        //will create session here
                        sco_data.SessionId = responseJson.sessionId;
                        //////
                        ///
                        IdentifyVM session = new IdentifyVM()
                        {
                            sessionId = responseJson.sessionId,
                            type = "IDENTIFY",
                            identifyFields = "[FIRST_NAME]"
                        };

                        string serializedJson = JsonConvert.SerializeObject(session);
                        Text = SendAndReceiveMessage(serializedJson);
                    }
                    else
                    {
                        throw new Exception("Session was not created!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Session was not created!");
            }
            return Text;
        }

        /// <summary>
        /// Closing the socket connection
        /// </summary>
        private void CloseSession()
        {
            try
            {
                socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                Basepage.logWrite("FacePay - Connection Closed.");
            }
            catch (Exception ex)
            {
                Basepage.logWrite("FacePay - " + ex.Message);
            }
        }

        /// <summary>
        /// Payment Processing after the face is deducted
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        private string PayAfterFaceDetection(string Text)
        {
            try
            {
                var responseJson = JsonConvert.DeserializeObject<IdentificationResponse>(Text);
                if (responseJson != null && responseJson.userId != null)
                {
                    PaymentRequest paymentRequest = new PaymentRequest()
                    {
                        type = "PAYMENT",
                        requestedAmount = Convert.ToInt32(Convert.ToDecimal(sco_data.TransactionTotal) * 100),
                        sessionId = responseJson.sessionId,
                        taxAmount = Convert.ToInt32(Convert.ToDecimal(sco_data.TransactionVat) * 100),
                        tipAmount = 0,
                        uniqueTransactionId = sco_data.ReceiptNumber
                    };

                    string pay = JsonConvert.SerializeObject(paymentRequest);
                    Text = SendAndReceiveMessage(pay);

                    Basepage.logWrite("FacePay - " + "Transaction process started.");
                }
                else
                {
                    Basepage.logWrite("FacePay - " + "Please verify your identity by entering last 4 digits of your phone number.");
                }
            }
            catch (Exception ex)
            {
                Basepage.logWrite("FacePay - " + "Transaction Received an error" + ex.Message);
            }
            return Text;
        }

        /// <summary>
        /// Close the transaction after payment is done.
        /// </summary>
        /// <param name="Text"></param>
        private void CompleteTransaction(string Text)
        {
            try
            {
                //Log the response
                Basepage.logWrite("FacePay - " + Text);
                var responseJson = JsonConvert.DeserializeObject<ResponseJson>(Text) ?? new ResponseJson();

                //Verify the last transaction
                Text = GetLastTransaction(responseJson.sessionId);
                var response = JsonConvert.DeserializeObject<ResponseJson>(Text) ?? new ResponseJson();

                //Card Payment
                switch (Basepage.ServerName)
                {
                    case "SA":
                        ToshibaSA toshibaSA = new ToshibaSA();
                        toshibaSA.CashPayment(responseJson.totalAmount.ToString());
                        sco_data.TransactionProcess = "FINISHED";
                        break;
                    case "LS":

                        break;
                    default:
                        // code block
                        break;
                }
                
                Thread.Sleep(300);

                //print receipt
                ucPrintScreenOPOS printScreenOPOS = new ucPrintScreenOPOS();
                printScreenOPOS.printScreenWait();

                //close session
                CloseSession();
                Basepage.logWrite("FacePay - " + "Transaction Completed.");
            }
            catch(Exception ex)
            {
                Basepage.logWrite("FacePay - " + ex.Message);
            }
            
        }

        /// <summary>
        /// Generic method to send message to a socket and return the response
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        private string SendAndReceiveMessage(string Request)
        {
            string result = string.Empty;
            if (socketClient.State == WebSocketState.Open)
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(Request);
                socketClient.SendAsync(new ArraySegment<byte>(sendBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            var buffer = new byte[1024];
            var segment = new ArraySegment<byte>(buffer);
            var message = socketClient.ReceiveAsync(segment, CancellationToken.None);
            result = Encoding.UTF8.GetString(buffer, 0, message.Result.Count);
            Basepage.logWrite("FacePay - Received message: " + result);

            return result;
        }

        /// <summary>
        /// Check the Last Transaction Status
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public string GetLastTransaction(string SessionId)
        {
            try
            {
                if (!string.IsNullOrEmpty(SessionId))
                {
                    SessionVM session = new SessionVM()
                    {
                        type = "LAST_TXN",
                        sessionId = SessionId
                    };
                    SessionId = JsonConvert.SerializeObject(session);

                    SessionId = SendAndReceiveMessage(SessionId);

                    Basepage.logWrite("FacePay - Last Transaction Found: " + SessionId);
                }
                else
                {
                    Basepage.logWrite("FacePay - " + "Last Transaction  is not found.");
                }
            }
            catch (Exception ex)
            {
                Basepage.logWrite("FacePay - " + "Last Transaction Received an error: " + ex.Message);
            }
            return SessionId;
        }

        public class ResponseJson
        {
            public string last4 { get; set; }
            public string bankName { get; set; }
            public string type { get; set; }
            public string cardBrand { get; set; }
            public string declineReason { get; set; }
            public string authCode { get; set; }
            public string sessionStatus { get; set; }
            public string sessionId { get; set; }
            public string userId { get; set; }
            public int totalAmount { get; set; }
            public int requestedAmount { get; set; }
            public int tipAmount { get; set; }
            public int taxAmount { get; set; }
            public int account { get; set; }
            public string status { get; set; }
            public string paymentMethod { get; set; }
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
        public class IdentifyVM
        {
            public string sessionId { get; set; }
            public string type { get; set; }
            public string identifyFields { get; set; }
        }
        public class IdentificationResponse
        {
            public string sessionId { get; set; }
            public string sessionStatus { get; set; }
            public string type { get; set; }
            public string userId { get; set; }
            public string status { get; set; }
        }
        //public class PaymentLog
        //{
        //    public PaymentLog()
        //    {
        //        jsonResponse = new ResponseJson();
        //        jsonPaymentRequest = new PaymentRequest();
        //        jsonError = new ResponseJsonError();
        //    }
        //    public ResponseJson jsonResponse { get; set; }
        //    public PaymentRequest jsonPaymentRequest { get; set; }
        //    public ResponseJsonError jsonError { get; set; }
        //}
        //public class TransactionLog
        //{
        //    public string firstName { get; set; }
        //    public string lastName { get; set; }
        //    public string phone { get; set; }
        //    public int transactionId { get; set; }
        //    public string person_id { get; set; }
        //    public string amount { get; set; }
        //    public string tip { get; set; }
        //}
    }
}
