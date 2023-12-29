using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class AddItemRequest
    {
        public string TransactionId { get; set; }
        public string StoreNo { get; set; }
        public string TerminalNo { get; set; }
        public string BarCode { get; set; }
        /// <summary>
        /// weight could be here for weighted items in grams
        /// </summary>
        public int Qty { get; set; }
    }
}
