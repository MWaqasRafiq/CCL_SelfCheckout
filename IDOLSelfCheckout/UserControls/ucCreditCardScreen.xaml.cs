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
using System.Windows.Threading;

namespace IDOLSelfCheckout.UserControls
{
    /// <summary>
    /// Interaction logic for ucCreditCardScreen.xaml
    /// </summary>
    public partial class ucCreditCardScreen : UserControl
    {
        Boolean loop = false;
        public static Label PinPadInfo;
        public ucCreditCardScreen()
        {
            InitializeComponent();
            PinPadInfo = PinpadInfo;
        }

        private void CardProcessStarted()
        {
            Thread.Sleep(200);
            Dispatcher.Invoke(() =>
            {
                Basepage bp = new Basepage();
                Boolean status = bp.tenderPayment(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
                if (status)
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreenOPOS());
                }
                else
                {
                    Basepage.logWrite("CardProcessStarted sco_data.ScannedBarcode=" + sco_data.ScannedBarcode + "Went to help");
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                }

            });
        }

        private delegate void UpdateLabelDelegate(DependencyProperty dp, object value);

        public void UpdateLabelContent(Label label, string newContent)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                PinpadInfo.Dispatcher.Invoke(new UpdateLabelDelegate(label.SetValue), DispatcherPriority.Background, ContentProperty, newContent);
                //Basepage.logWrite("UpdateLabel=" + newContent);
            });
        }

    }
}

