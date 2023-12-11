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
            //PinPadInfo.Content = "test";

            //Task.Factory.StartNew(() =>
            //{
            //    CardProcessStarted();
            //});


        }

        private void MyLoadedRoutedEventHandler(object sender, RoutedEventArgs e)
        {
            //Thread.Sleep(1000);

        }

        //private void MainWindow_OnLayoutUpdated(object sender, EventArgs eventArgs)
        //{
        //    Basepage.ledCardPayment();
        //    Basepage.logWrite("card process started");
        //    Basepage bp = new Basepage();
        //    Boolean status = bp.tenderPayment(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
        //    if (status)
        //    {
        //        uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
        //        Basepage.ledReceipt();
        //    }
        //    else
        //    {
        //        uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());

        //    }
        //}

        private void CardProcessStarted()
        {
            //loop = true;
            Thread.Sleep(200);
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    while (loop)
            //    {
            //        UpdateLabelContent(PinpadInfo, "Status: " + sco_data.TransactionPinpadInfo);
            //        Thread.Sleep(1000);
            //    }

            //});

            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //string dsd = "";
            Dispatcher.Invoke(() =>
            {
                //PinPadInfo.Content = "test";

                Basepage bp = new Basepage();
                Boolean status = bp.tenderPayment(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
                if (status)
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreenOPOS());
                    //loop = false;

                }
                else
                {

                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                    //loop = false;
                }

            });

            //  });

        }

        private delegate void UpdateLabelDelegate(DependencyProperty dp, object value);

        public void UpdateLabelContent(Label label, string newContent)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                PinpadInfo.Dispatcher.Invoke(new UpdateLabelDelegate(label.SetValue), DispatcherPriority.Background, ContentProperty, newContent);
                Basepage.logWrite("UpdateLabel=" + newContent);
            });
        }

    }
}

