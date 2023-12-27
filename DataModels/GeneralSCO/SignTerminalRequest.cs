using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.GeneralSCO
{
    public class SignTerminalRequest
    {
        public string StoreNo { get; set; }
        public string TerminalNo { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }
}
