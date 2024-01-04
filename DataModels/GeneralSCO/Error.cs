using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class Error
    {
        public int? Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
