using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.LsRetail
{
    public class LsResponseGet
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string code { get; set; }
        public PricesRequest data { get; set; }
    }
}
