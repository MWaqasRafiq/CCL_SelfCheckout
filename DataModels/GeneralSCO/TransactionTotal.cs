using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class TransactionTotal
    {
        public string TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalDiscount { get; set; }
    }
}
