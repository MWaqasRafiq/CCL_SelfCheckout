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
    public partial class ucDuScreen : UserControl, IComponentConnector
    {
        internal
#nullable disable
        Grid _buttons;
        internal Button _btn_du_item1;
        internal Button _btn_du_item2;
        internal Button _btn_du_item3;
        internal Button _btn_du_item4;
        internal Button _btn_back;
        private masafiPricesRequest list = new masafiPricesRequest();
        private prices product = new prices();

        public ucDuScreen() { 
            this.InitializeComponent();
            LSscoApi lsscoApi = new LSscoApi();
            this.list = lsscoApi.productList();
        }

        private void btn_du_item1_Click(
#nullable enable
        object sender, RoutedEventArgs e) => this.barcodeData("1166544800655");

        private void btn_back_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucOperatorsScreen());

        private void btn_du_item2_Click(object sender, RoutedEventArgs e) => this.barcodeData("5561155400565");

        private void btn_du_item3_Click(object sender, RoutedEventArgs e) => this.barcodeData("1115506444551");

        private void btn_du_item4_Click(object sender, RoutedEventArgs e) => this.barcodeData("1220066554456");

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
