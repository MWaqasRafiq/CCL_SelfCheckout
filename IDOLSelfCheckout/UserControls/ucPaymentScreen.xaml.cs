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
using POS.Devices;
using IDOLSelfCheckout.DataModel;

namespace IDOLSelfCheckout.UserControls
{
    /// <summary>
    /// Interaction logic for ucPaymentScreen.xaml
    /// </summary>
    public partial class ucPaymentScreen : UserControl
    {
        public ucPaymentScreen()
        {
            InitializeComponent();

        }

        private void btn_pay_back_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucItemScreen());
        }

        private void btn_loyalty_Click(object sender, RoutedEventArgs e)
        {
            Basepage.LoyaltyRequested = true;
            Basepage.LoyaltyScaned = false;
            uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucLoyalty());
        }
        private void btn_creditcard_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucCreditCardScreen());
            //Thread.Sleep(2000);
            Basepage bp = new Basepage();
            Boolean status = bp.tenderPayment(sco_data.ReceiptNumber, sco_data.TransactionTotal, "1234", "1222");
            if (status)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());

            }


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

        bool IsAllDigits(string s) => s.Replace(",","").Replace(".","").All(char.IsDigit);

        private void btnFacePay_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sco_data.TransactionTotal) && IsAllDigits(sco_data.TransactionTotal))
            {
                //int total = Convert.ToInt32(decimal.Parse(sco_data.TransactionTotal));
                //if (total > 0)
                //{
                //    ServerIntegration serverIntegration = new ServerIntegration();
                //    serverIntegration.TotalReceipt();
                //    FacePay facePay = new FacePay();
                //    facePay.IdentifyPerson();
                //}
            }
            else
                Basepage.logWrite("FacePay - Invalid Transaction amount requested: " + sco_data.TransactionTotal);
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
