using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class CartProducts
    {
        public List<ProductDetails> Products { get; set; }
        public TransactionTotal Total { get; set; }
        public CartProducts()
        {
            Products = new List<ProductDetails>();
            Total = new TransactionTotal();
        }
    }
}
