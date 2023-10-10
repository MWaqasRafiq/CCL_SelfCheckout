using IDOLSelfCheckout.Classes;
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
        private readonly view_models viewModels;
        public ucMainScreen()
        {
            InitializeComponent();

            ItemListDataGrid = item_list;
            this.viewModels = new view_models
            {
                items = new List<items>()
                {
                    new items { Name = "", Price = "" }
                }
            };
            ItemListDataGrid.DataContext = this.viewModels;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TransactionDetails = transactoin_details;
            TransactionTotal = transactoin_total;
            TransactionVat = transactoin_vat;
            ItemInfo = item_info;
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
                    Basepage.ledWelcome();
                }
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
            }
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            btn_help.Focusable = false;
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
        }

        
    }
}
