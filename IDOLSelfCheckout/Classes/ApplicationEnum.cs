using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDOLSelfCheckout.Classes
{
    enum PosSubState
    {
        Ready = 1008,
        Pay = 1010
    }
    public enum ServersEnum
    {
        OP = 0, //On Premises // Offline
        SA = 1, //Toshiba SA SIT
        LS = 2 // LS Retails
    }
}
