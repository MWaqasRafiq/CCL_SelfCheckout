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

namespace IDOLSelfCheckout
{
    public class FacePay
    {
        //private TsgcWebSocketClient socketClient;
        public FacePay()
        {
            //socketClient = new TsgcWebSocketClient();
        }

        //public void InitializeSocketConnection()
        //{
        //    try
        //    {
        //        Basepage.logWrite("FacePay - Initialization!");

        //        //socketClient = new TsgcWebSocketClient();
        //        socketClient.Host = Basepage.PopId_Host;
        //        socketClient.Port = Basepage.PopId_Port;

        //        socketClient.OnConnect += OnSocketConnectEvent;
        //        socketClient.OnMessage += OnSocketResponseEvent;
        //        socketClient.HeartBeat.Interval = 1;
        //        socketClient.HeartBeat.Timeout = 0;
        //        socketClient.HeartBeat.Enabled = true;
        //        socketClient.Active = true;

        //        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
        //        {
        //            { "type", "search" }
        //        };
        //        string mes = JsonConvert.SerializeObject(keyValuePairs);
        //        //socketClient.WriteTimeOut = 5;
        //        socketClient.WriteData(mes);
        //        Basepage.logWrite("FacePay - Search request sent.");
        //    }
        //    catch (Exception ex) { Basepage.logWrite("FacePay - InitializeSocketConnection - " + ex.Message); }
        //}

        //private void OnSocketResponseEvent(TsgcWSConnection Connection, string Text)
        //{
        //    try
        //    {
        //        if (Text.Contains("faceId"))
        //        {
        //            PayAfterFaceDetection(Connection, Text);
        //        }
        //        else if (Text.Contains("transactionId"))
        //        {
        //            bool flag = GetTransactionDetails(Connection, Text);
        //            if (flag)
        //            {
        //                //PrintReceipt("1.01");
        //            }
        //        }
        //        else
        //        {
        //            var responseJson = JsonConvert.DeserializeObject<ResponseJsonError>(Text);
        //        }
        //        Basepage.logWrite("Message Received from Server: " + Text);
        //    }
        //    catch (Exception ex) { Basepage.logWrite("FacePay - OnSocketResponseEvent - " + ex.Message); }

        //}

        //private void OnSocketConnectEvent(TsgcWSConnection Connection)
        //{
        //    Basepage.logWrite("Socket Connected!");
        //}
        //private void PayAfterFaceDetection(TsgcWSConnection Connection, string Text)
        //{
        //    var responseJson = JsonConvert.DeserializeObject<List<ResponseJson>>(Text);
        //    if (responseJson != null && responseJson.Count == 1)
        //    {
        //        PaymentRequest paymentRequest = new PaymentRequest()
        //        {
        //            amount = sco_data.TransactionTotal,
        //            person_id = responseJson[0].faceId,
        //            type = "pay",
        //            tip = "0.00"
        //        };

        //        string pay = JsonConvert.SerializeObject(paymentRequest);
        //        Connection.WriteData(pay);

        //        if (!Directory.Exists("C:\\IDOL\\JsonData"))
        //        {
        //            Directory.CreateDirectory("C:\\IDOL\\JsonData");
        //        }
        //        if (!File.Exists("C:\\IDOL\\JsonData\\pendingTrJsondata.json"))
        //        {
        //            using (var a = File.Create("C:\\IDOL\\JsonData\\pendingTrJsondata.json"))
        //            {
        //                a.Close();
        //            }
        //        }
        //        List<PaymentLog> paymentLog = new List<PaymentLog>();
        //        paymentLog.Add(new PaymentLog()
        //        {
        //            jsonResponse = responseJson[0],
        //            jsonPaymentRequest = paymentRequest
        //        });

        //        using (StreamWriter file = File.CreateText("C:\\IDOL\\JsonData\\pendingTrJsondata.json"))
        //        {
        //            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //            serializer.Serialize(file, paymentLog);
        //        }
        //    }
        //    else
        //    {
        //        Basepage.logWrite("Please verify your identity by entering last 4 digits of your phone number.");
        //    }
        //}

        //private bool GetTransactionDetails(TsgcWSConnection Connection, string Text)
        //{
        //    try
        //    {
        //        var responseJson = JsonConvert.DeserializeObject<ResponseJson>(Text);
        //        List<PaymentLog> items = new List<PaymentLog>();
        //        List<TransactionLog> transactions = new List<TransactionLog>();
        //        PaymentLog payment = new PaymentLog();
        //        using (StreamReader r = new StreamReader("C:\\IDOL\\JsonData\\pendingTrJsondata.json"))
        //        {
        //            string json = r.ReadToEnd();
        //            items = JsonConvert.DeserializeObject<List<PaymentLog>>(json);
        //            payment = items.Where(x => x.jsonResponse.firstName == responseJson.firstName && x.jsonResponse.lastName == responseJson.lastName).FirstOrDefault();
        //            if (payment != null)
        //            {
        //                TransactionLog transaction = new TransactionLog()
        //                {
        //                    firstName = payment.jsonResponse.firstName,
        //                    lastName = payment.jsonResponse.lastName,
        //                    amount = payment.jsonPaymentRequest.amount,
        //                    person_id = payment.jsonPaymentRequest.person_id,
        //                    phone = payment.jsonResponse.phone,
        //                    tip = payment.jsonPaymentRequest.tip,
        //                    transactionId = responseJson.transactionId ?? 0
        //                };
        //                transactions.Add(transaction);
        //                items.Remove(payment);
        //            }
        //        }
        //        using (StreamReader r = new StreamReader("C:\\IDOL\\JsonData\\TransactionJsondata.json"))
        //        {
        //            string json = r.ReadToEnd();
        //            var tem = JsonConvert.DeserializeObject<List<TransactionLog>>(json);
        //            if (tem != null)
        //                transactions.AddRange(JsonConvert.DeserializeObject<List<TransactionLog>>(json));
        //        }

        //        if (!File.Exists("C:\\IDOL\\JsonData\\TransactionJsondata.json"))
        //        {
        //            using (var a = File.Create("C:\\IDOL\\JsonData\\TransactionJsondata.json"))
        //            {
        //                a.Close();
        //            }
        //        }
        //        using (StreamWriter file = File.CreateText("C:\\IDOL\\JsonData\\TransactionJsondata.json"))
        //        {
        //            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //            serializer.Serialize(file, transactions);
        //        }
        //        using (StreamWriter file = File.CreateText("C:\\IDOL\\JsonData\\pendingTrJsondata.json"))
        //        {
        //            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //            serializer.Serialize(file, items);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Basepage.logWrite("FacePay - OnSocketResponseEvent - " + ex.Message);
        //        return false;
        //    }

        //}

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
            public string error { get; set; }
        }
        public class PaymentRequest
        {
            public string type { get; set; }
            public string person_id { get; set; }
            public string amount { get; set; }
            public string tip { get; set; }
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
    }
}
