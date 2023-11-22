using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.ToshibaSA
{
    public class InvoiceRequestVM
    {
        public string ProcessFlag { get; set; }
        public string DisplayLine { get; set; }
        public string IPDevice { get; set; }
        public string TerminalID { get; set; }
        public string ListenerFlag { get; set; }
        public string qty { get; set; }
    }
}
