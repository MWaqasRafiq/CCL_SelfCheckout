using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDOLSelfCheckout.DataModel
{
    public class GeneralServiceIntegration
    {

        public static string ServerName;

        public GeneralServiceIntegration()
        {
            if (ConfigurationManager.AppSettings["ServerName"] != null)
                ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
        }

        public dynamic SendAndReceiveData()
        {
            dynamic result = null;

            if(ServerName == null)
            {
                return result;
            }
            else if(ServerName == "SA")
            {

            }

            return result;
        }
    }
}
