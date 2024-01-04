using DataModels.Shared;
using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDOLSelfCheckout.UserControls
{
    /// <summary>
    /// Interaction logic for ucPrintScreenOPOS.xaml
    /// </summary>
    public partial class ucPrintScreenOPOS : UserControl
    {
        public ucPrintScreenOPOS()
        {
            InitializeComponent();
            //Basepage.ledReceipt();
            Task.Factory.StartNew(() =>
            {
                printScreenWait();
            });
        }

        public async void printScreenWait()
        {

            Thread.Sleep(500);
            Dispatcher.Invoke(() =>
            {
                OposPrinterCall pp = new OposPrinterCall();
                bool status = false;
                if (Basepage.ServerName == "GP")
                {
                    string receipt = new Basepage().GeneralPosPrintReceipt();
                    status = pp.OposGeneralprint(receipt);
                }
                else
                {
                    status = pp.OPOSprint();
                }
                if (status)
                {
                    sco_data.TransactionProcess = "FINISHED";
                    uc_call.Uc_Add(MainWindow.Main_SCO, new ucStartScreen());
                    sco_data.ItemList = new List<DataModels.Shared.items>();
                    sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                    sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                    ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                    ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
                    ucMainScreen.TransactionVat.Content = (object)("VAT   AED " + sco_data.TransactionVat);
                }
                else
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                }
                Thread.Sleep(2000);
            });
        }
    }
}
