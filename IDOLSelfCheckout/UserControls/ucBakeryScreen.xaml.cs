using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDOLSelfCheckout.UserControls
{
    /// <summary>
    /// Interaction logic for ucBakeryScreen.xaml
    /// </summary>
    public partial class ucBakeryScreen : UserControl, IComponentConnector
    {
        private masafiPricesRequest list = new masafiPricesRequest();
        private prices product = new prices();
        internal
#nullable disable
        Button _btn_bakery_item1;
        internal Label _btn_bakery_label1;
        internal Button _btn_bakery_item2;
        internal Label _btn_bakery_label2;
        internal Button _btn_bakery_item3;
        internal Label _btn_bakery_label3;
        internal Button _btn_back;
        public ucBakeryScreen()
        {
            Basepage.logWrite("ucBakeryScreen >> 1");
            this.InitializeComponent();
            Basepage.logWrite("ucBakeryScreen >> 2");
            LSscoApi lsscoApi = new LSscoApi();
            Basepage.logWrite("ucBakeryScreen >> 3");
            this.list = lsscoApi.productList();
            Basepage.logWrite("ucBakeryScreen >> 4");
            this.btn_bakery_label1.Content = (object)this.list.prices[6].name;
            this.btn_bakery_label2.Content = (object)this.list.prices[7].name;
            this.btn_bakery_label3.Content = (object)this.list.prices[8].name;
            Basepage.logWrite("ucBakeryScreen >> 5");
        }

        private void btn_bakery_item1_Click(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[6];
            this.barcodeData(this.product.barcode, this.product);
        }

        private void btn_back_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());

        private void btn_bakery_item2_Click(object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[7];
            this.barcodeData(this.product.barcode, this.product);
        }

        private void btn_bakery_item3_Click(object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[8];
            this.barcodeData(this.product.barcode, this.product);
        }

        private void barcodeData(string barcode, prices product)
        {
            Basepage basepage = new Basepage();
            if (basepage.addItem(sco_data.ReceiptNumber, barcode, product))
            {
                basepage.updateTransactionDetails(product);
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
            }
            else
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
        }
    }
}
