using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.ToshibaSA
{
    public class InvoiceResponseVM
    {
        public string TerminalID { get; set; }
        public string BalanceDue { get; set; }
        public string PosState { get; set; }
        public string PosSubState { get; set; }
        public string NumOfItems { get; set; }
        public string SignOnStatus { get; set; }
        public string ScoTsDisp1 { get; set; }
        public string ScoTsDisp2 { get; set; }
        public string ScoMemeberName { get; set; }
        public string ScoMemeberBalance { get; set; }
        public string ResponseMessage { get; set; }
        public string Receipt { get; set; }
        public string Display { get; set; }
        public string IdolPrintLine1 { get; set; }
        public bool IsSecured { get; set; }
    }
}
