using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class AddPaymentRequest
    {
        public string TransactionId { get; set; }
        public string PaymentType { get; set; }
        public string BankTransactionId { get; set; }
        public string CardMaskNo { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
