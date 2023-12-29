using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class ProductDetails
    {
        public string Description { get; set; }
        public string Description2 { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Vat { get; set; }
        public string DiscountDesc { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public bool Weighted { get; set; }
        public bool AgeRestriction { get; set; }
        public string BCD { get; set; }
    }
}
