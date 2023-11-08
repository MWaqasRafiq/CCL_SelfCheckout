using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ucMainScreen.xaml
    /// </summary>
    public partial class ucMainScreen : UserControl
    {
        public static Label TransactionDetails;
        public static DataGrid ItemListDataGrid;
        public static Label TransactionTotal;
        public static Label TransactionVat;
        public static Label ItemInfo;
        public static TextBlock ReceiptText;
        private readonly view_models viewModels;
        private ServerIntegration serverIntegration;
        CCL_Lamp lamp;
        public ucMainScreen()
        {
            InitializeComponent();
            lamp = new CCL_Lamp();
            lamp.BlueOpen();
            ItemListDataGrid = item_list;
            this.viewModels = new view_models
            {
                items = new List<items>()
                {
                    new items { Name = "", Price = "" }
                }
            };
            ItemListDataGrid.DataContext = this.viewModels;

            //Server API
            serverIntegration = new ServerIntegration();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += ServiceCallWorker;
            timer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TransactionDetails = transactoin_details;
            TransactionTotal = transactoin_total;
            TransactionVat = transactoin_vat;
            ItemInfo = item_info;
            ReceiptText = Receipt_Text;
            MainWindow.Item_SCO = Item_sco;

            Basepage bp = new Basepage();

            Boolean status = true;//bp.startNewTransaction();
            sco_data.TransactionProcess = "STARTED";
            if (status)
            {
                if (sco_data.TransactionProcess == "CLOSED")
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucAsistantScreen());
                }
                else
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucItemScreen());
                }
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
            }
            sco_data.StoreNumber = Basepage.StoreNumber;
            if (Basepage.IsLocalConsumption)
            {
                item_list_parent.Visibility = Visibility.Visible;
                Receipt_Text_parent.Visibility = Visibility.Hidden;
            }
            else
            {
                item_list_parent.Visibility = Visibility.Hidden;
                Receipt_Text_parent.Visibility = Visibility.Visible;
            }
            ucMainScreen.TransactionDetails.Content = (object)("Store No: " + sco_data.StoreNumber + "  Terminal: " + sco_data.TerminalNumber );
        }

        public void UpdateReceipt(PosServiceResponseVM responseVM)
        {
            if (responseVM != null && responseVM.Receipt != null && ReceiptText != null 
                )//&& responseVM.PosSubState == "1008")
            {
                ReceiptText.Text = responseVM.Receipt;
                
                sco_data.TerminalNumber = responseVM.TerminalID;
                
                sco_data.TransactionTotal = responseVM.BalanceDue;

                if (responseVM.Receipt.Contains("TRN:"))
                    sco_data.ReceiptNumber = responseVM.Receipt.Split("TRN:")[1].Split("\r")[0];
                else
                    sco_data.ReceiptNumber = "";

                if (!string.IsNullOrEmpty(sco_data.TransactionTotal) && sco_data.TransactionTotal != "null")
                    sco_data.TransactionVat = ((Convert.ToDecimal(responseVM.BalanceDue.Replace(",", "")) / 100) * 5).ToString();
                else
                    sco_data.TransactionVat = "0.00";

                ucMainScreen.TransactionDetails.Content = (object)("Store No: " + sco_data.StoreNumber + "  Terminal: " + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED: " + ((!string.IsNullOrEmpty(sco_data.TransactionTotal) && sco_data.TransactionTotal != "null") ?  sco_data.TransactionTotal : "0.00"));
                ucMainScreen.TransactionVat.Content =   (object)("VAT   AED: " + sco_data.TransactionVat);
            }
            else
            {
                sco_data.TransactionVat = "0.00";
                sco_data.TransactionTotal = "0.00";
                if(ucMainScreen.TransactionTotal != null)
                {
                    if (ReceiptText != null) 
                        { ReceiptText.Text = ""; }
                    ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED: " + sco_data.TransactionTotal);
                    ucMainScreen.TransactionVat.Content = (object)("VAT   AED: " + sco_data.TransactionVat);
                }
            }
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            btn_help.Focusable = false;
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
        }

        /// <summary>
        /// Service for receipt to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServiceCallWorker(object sender, EventArgs e)
        {
            if (Receipt_Text != null && !Basepage.IsLocalConsumption)
            {
                PosServiceResponseVM responseVM = serverIntegration.GetReceipt();
                UpdateReceipt(responseVM);
            }
        }

    }
}
