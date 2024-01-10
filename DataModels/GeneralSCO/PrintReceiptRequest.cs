using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class PrintReceiptRequest
    {
        public string TransactionId { get; set; }
        public string MobileNumber { get; set; }
        public bool? GoGreen { get; set; }

    }
}
