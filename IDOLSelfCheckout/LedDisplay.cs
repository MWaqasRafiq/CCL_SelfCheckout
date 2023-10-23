using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDOLSelfCheckout
{
    public class LedDisplay
    {
        public void ChangeLedColor()
        {
            Basepage.ledWelcome();
            Thread.Sleep(1000);
            Basepage.ledHelp();
            Thread.Sleep(1000);
            Basepage.ledCardPayment();
            Thread.Sleep(1000);
            Basepage.ledReceipt();
            Thread.Sleep(1000);
            Basepage.ledClosed();
            Thread.Sleep(1000);
        }
    }
}
