using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDOLSelfCheckout.Classes
{
    public class PosServiceRequestVM
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ProcessFlag { get; set; }
        public string DisplayLine { get; set; }
        public string IPDevice { get; set; }
        public string TerminalID { get; set; }
        public string ListenerFlag { get; set; }
        public string qty { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
    public class PosServiceListRequestVM
    {
        public PosServiceListRequestVM()
        {
            RequestVMs = new List<PosServiceRequestVM>();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public List<PosServiceRequestVM> RequestVMs { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
