using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class PrintReceiptResponse
    {
        public decimal Amount { get; set; }
        public decimal Vat { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Currency { get; set; }
        public string Receipt { get; set; }
    }
}
