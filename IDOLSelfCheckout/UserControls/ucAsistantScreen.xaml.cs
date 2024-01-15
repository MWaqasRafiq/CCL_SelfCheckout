using DataModels.GeneralSCO;
using DataModels.Shared;
using IDOLSelfCheckout.BankDevice;
using IDOLSelfCheckout.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for ucAsistantScreen.xaml
    /// </summary>
    public partial class ucAsistantScreen : UserControl
    {
        private System.Timers.Timer _timer;
        public static TextBox MessageText;
        CCL_Lamp lamp;
        public ucAsistantScreen()
        {
            InitializeComponent();
            
            //MessageText = messageTxt;
            messageTxt.Text = sco_data.ErrorMessage;
            lamp = new CCL_Lamp();
            lamp.RedOpen();
            setLabel();
            ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Visible;
        }
        private void btn_new_transaction_Click(object sender, RoutedEventArgs e)
        {
            sco_data.ItemList = new List<DataModels.Shared.items>();
            List<DataModels.Shared.items> itemList = sco_data.ItemList;
            sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
            sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); 
            ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
            ucMainScreen.TransactionTotal.Content = (object)("TOTAL " + sco_data.TransactionCurrency + " " + sco_data.TransactionTotal);
            ucMainScreen.TransactionVat.Content =   (object)("VAT  " + sco_data.TransactionCurrency + " " + sco_data.TransactionVat);
            DataModels.Shared.view_models viewModels = new DataModels.Shared.view_models()
            {
                items = (IEnumerable<DataModels.Shared.items>)itemList
            };
            ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Hidden;
            ucMainScreen.ItemListDataGrid.DataContext = (object)viewModels;
            uc_call.Uc_Add(MainWindow.Main_SCO, new ucStartScreen());
        }

        private void btn_void_item_Click(object sender, RoutedEventArgs e)
        {
            Basepage.VoidRequested = true;
            Basepage.VoidScaned = false;
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
            Basepage.VoidRequested = false;
            ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Hidden;
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
            if (Basepage.ServerName == "GP")
            {
                SignTerminalRequest terminalRequest = new SignTerminalRequest()
                {
                    Type = "off",
                    Password = "",
                    StoreNo = Basepage.StoreNumber,
                    TerminalNo = Basepage.TerminalId,
                    UserId = ""
                };
                GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();
                var res = general_SCO.SignTerminal(terminalRequest);
            }
            
            sco_data.TransactionProcess = "CLOSED";
            uc_call.Uc_Add(MainWindow.Main_SCO, new ucClosedScreen());
            ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Hidden;
        }

        private void btn_reprint_Click(object sender, RoutedEventArgs e)
        {
         
            Basepage bp = new Basepage();
            bool status = bp.reprintLastTrnsaction();
            if (status)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucPrintScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());

            }
            ucMainScreen.ItemListDataGrid.Columns[5].Visibility = Visibility.Hidden;
        }

        private async void btn_led_Click(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => Led_Test());
        }

        private async void Led_Test()
        {
            try
            {
                lamp.RedOpen();
                Thread.Sleep(1000);
                lamp.RedClose();
                lamp.BlueOpen();
                Thread.Sleep(1000);
                lamp.BlueClose();
                lamp.GreenOpen();
                Thread.Sleep(1000);
                lamp.GreenClose();
                await lamp.BlueBlinkOpen(10);
                lamp.Close();

                lamp.RedOpen();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
