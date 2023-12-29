using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class ProductDetailsRequest
    {
        public string StoreNo { get; set; }
        public string TerminalNo { get; set; }
        public string BarCode { get; set; }
    }
}
