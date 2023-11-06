using IDOLSelfCheckout.BankDevice;
using IDOLSelfCheckout.Classes;
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
    /// Interaction logic for ucAsistantScreen.xaml
    /// </summary>
    public partial class ucAsistantScreen : UserControl
    {
        public static TextBox MessageText;
        //WpfLamp.MainWindow _Lamp;
        public ucAsistantScreen()
        {
            InitializeComponent();
            //MessageText = messageTxt;
            messageTxt.Text = sco_data.ErrorMessage;
            //_Lamp = new WpfLamp.MainWindow();
        }

        private void btn_assistant_sco_Click(object sender, RoutedEventArgs e)
        {
            sco_data.ItemList = new List<items>();
            List<items> itemList = sco_data.ItemList;
            sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
            sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
            ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
            ucMainScreen.TransactionTotal.Content = (object)("TOTAL AED " + sco_data.TransactionTotal);
            ucMainScreen.TransactionVat.Content =   (object)("VAT   AED " + sco_data.TransactionVat);
            view_models viewModels = new view_models()
            {
                items = (IEnumerable<items>)itemList
            };
            ucMainScreen.ItemListDataGrid.DataContext = (object)viewModels;
            uc_call.Uc_Add(MainWindow.Main_SCO, new ucStartScreen());
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (sco_data.TransactionProcess == "FINISHED")
            {
                uc_call.Uc_Add(MainWindow.Main_SCO, new ucStartScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucItemScreen());
            }
        }

        private void btn_assistant_reconsulation_Click(object sender, RoutedEventArgs e)
        {
            Oma880 oma = new Oma880();
            oma.PaymentDeviceCom("112","");
        }
        public void setLabel()
        {
            MessageText = messageTxt;
        }

        private void btn_closed_screen_Click(object sender, RoutedEventArgs e)
        {
            sco_data.TransactionProcess = "CLOSED";
            uc_call.Uc_Add(MainWindow.Main_SCO, new ucClosedScreen());
        }

        private void btn_reprint_Click(object sender, RoutedEventArgs e)
        {
            Basepage bp = new Basepage();
            Boolean status = bp.reprintLastTrnsaction();
            if (status)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());

            }
        }

        private void btn_led_Click(object sender, RoutedEventArgs e)
        {
            //WpfLamp.MainWindow lamp = new WpfLamp.MainWindow();
            //Basepage.logWrite("Lamp Opening Red.");
            //Thread.Sleep(500);
            //lamp.btnOpen_Click();
            //Thread.Sleep(500);
            //lamp.BlueOpen_Click();
            //Basepage.logWrite("Lamp Opened Red.");
            //_Lamp.GreenOpen();
            //Thread.Sleep(500);
            //_Lamp.YellowOpen();
            //Thread.Sleep(500);
            //_Lamp.BlueOpen();
            //Thread.Sleep(500);
            //Basepage.logWrite("Lamp closing.");
            //_Lamp.Close();

            //Basepage.ledWelcome();
            //Basepage.ledHelp();
            //Basepage.ledCardPayment();
            //Basepage.ledReceipt();
            //Basepage.ledClosed();
        }
    }
}
