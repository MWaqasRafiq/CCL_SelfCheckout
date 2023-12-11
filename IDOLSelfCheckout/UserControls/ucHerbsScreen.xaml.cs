using DataModels;
using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;


#nullable enable
namespace IDOLSelfCheckout.UserControls
{
    public partial class ucHerbsScreen : UserControl, IComponentConnector
    {
        private PricesRequest list = new PricesRequest();
        private DataModels.prices product = new DataModels.prices();
        internal
#nullable disable
        Button _btn_herbs_item1;
        internal Label _btn_herbs_label1;
        internal Button _btn_herbs_item2;
        internal Label _btn_herbs_label2;
        internal Button _btn_herbs_item3;
        internal Label _btn_herbs_label3;
        internal Button _btn_back;

        public ucHerbsScreen()
        {
            this.InitializeComponent();
            this.list = new LSscoApi().productList();
            this.btn_herbs_label1.Content = (object)this.list.prices[9].name;
            this.btn_herbs_label2.Content = (object)this.list.prices[10].name;
            this.btn_herbs_label3.Content = (object)this.list.prices[11].name;
        }

        private void btn_herbs_item1_Click(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[9];
            this.barcodeData(this.product.barcode);
        }

        private void btn_back_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());

        private void btn_herbs_item2_Click(object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[10];
            this.barcodeData(this.product.barcode);
        }

        private void btn_herbs_item3_Click(object sender, RoutedEventArgs e)
        {
            this.product = this.list.prices[11];
            this.barcodeData(this.product.barcode);
        }

        private void barcodeData(string barcode)
        {
            Basepage basepage = new Basepage();
            this.product = this.list.prices.Where(x => x.barcode == barcode).FirstOrDefault();
            if (this.product != null && basepage.addItem("", barcode, this.product))
            {
                basepage.updateTransactionDetails(this.product);
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
            }
            else
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
        }
    }
}