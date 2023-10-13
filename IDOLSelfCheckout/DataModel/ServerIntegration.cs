using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.UserControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace IDOLSelfCheckout.DataModel
{
    public class ServerIntegration
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static string HostServiceIp;
        public static string HostServiceEndpoint;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ServerIntegration() 
        {
            if (ConfigurationManager.AppSettings["Service_Ip"] != null)
                HostServiceIp = ConfigurationManager.AppSettings["Service_Ip"].ToString() ;
            if (ConfigurationManager.AppSettings["Service_EndPoint"] != null)
                HostServiceEndpoint = ConfigurationManager.AppSettings["Service_EndPoint"].ToString();
      
        }

        /// <summary>
        /// CheckPosResponse - This method is used for checking the current status and values of current machine from server
        /// </summary>
        /// <returns>PosServiceResponseVM</returns>
        public PosServiceResponseVM PostPosRequest(PosServiceRequestVM serviceRequest)
        {
            PosServiceResponseVM serviceResponseVM = new PosServiceResponseVM();
            try
            {
                List<PosServiceRequestVM> serviceRequestVM = new List<PosServiceRequestVM>();
                if (serviceRequest == null)
                {
                    serviceRequestVM.Add(new PosServiceRequestVM
                    {
                        ProcessFlag = "display2",
                        DisplayLine = "",
                        IPDevice = "",
                        TerminalID = Basepage.HostTerminalId,
                        ListenerFlag = "1",
                        qty = ""
                    });
                }
                else
                {
                    serviceRequest.TerminalID = Basepage.HostTerminalId;
                    serviceRequestVM.Add(serviceRequest);
                }
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
                        Basepage.logWrite("Response from server"+ result);
                        result = result.Remove(result.Length - (Basepage.HostTerminalId.Length+2), (Basepage.HostTerminalId.Length + 2));
                        serviceResponseVM = JsonConvert.DeserializeObject<PosServiceResponseVM>(result)??new PosServiceResponseVM();
                    }
                    else
                    {
                        Basepage.logWrite("Display Error - Code:" + response.StatusCode );
                        Basepage.logWrite("Display Error - Message:" + response.ReasonPhrase);
                    }

                }

            }
            catch (Exception ex)
            {
                Basepage.logWrite(ex.Message);
            }

            return serviceResponseVM;
        }

    }
}
