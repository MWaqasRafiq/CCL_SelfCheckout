using IDOLSelfCheckout.Classes;
using Newtonsoft.Json;
using OposPrinter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.IO;
using esegece.sgcWebSockets;
using POS.Devices;

namespace IDOLSelfCheckout.UserControls
{
    /// <summary>
    /// Interaction logic for ucPaymentScreen.xaml
    /// </summary>
    public partial class ucPaymentScreen : UserControl
    {
        private TsgcWebSocketClient socketClient;
        public ucPaymentScreen()
        {
            InitializeComponent();

            socketClient = new TsgcWebSocketClient();
        }

        private void btn_pay_back_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucItemScreen());
        }

        private void btn_creditcard_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucCreditCardScreen());
            //Thread.Sleep(200);
            //Basepage bp = new Basepage();
            //Boolean status = bp.tenderPayment(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
            //if (status)
            //{
            //    uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
            //}
            //else
            //{
            //    uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
               
            //}
        

        }

        private void btnCreditcard2_Click(object sender, RoutedEventArgs e)
        {
            Basepage bp = new Basepage();
            Boolean status = bp.tenderPaymentOffline(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
            if (status)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());

            }
        }

        private void btnCreditcardOpos_Click(object sender, RoutedEventArgs e)
        {
            Basepage bp = new Basepage();
            Boolean status = bp.tenderPaymentOffline(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
            if (status)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreenOPOS());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
            }
        }

        private void btnFacePay_Click(object sender, RoutedEventArgs e)
        {
            FacePay facePay = new FacePay();
            facePay.IdentifyPerson();
        }
        //BackgroundWorker worker;

       
        //private void Worker_RunWorkerCompleted()
        //{
        //    //imgCircle.Visibility = Visibility.Collapsed;
        //    MainWindow.Main_SCO.IsEnabled = true;

        //}

        //private void PerformTaskProcess()
        //{
        //    //imgCircle.Visibility = Visibility.Visible;
        //    MainWindow.Main_SCO.IsEnabled = false; //Disabling the button

        //}



    }
}
