using DataModels;
using DataModels.Shared;
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
    public partial class ucDonationScreen : UserControl, IComponentConnector
    {
        internal
#nullable disable
        Button _btn_donation_item1;
        internal Button _btn_donation_item2;
        internal Button _btn_donation_item3;
        internal Button _btn_donation_item4;
        internal Button _btn_back;
        private PricesRequest list = new PricesRequest();
        private DataModels.prices product = new DataModels.prices();
        public ucDonationScreen() { 
            this.InitializeComponent();
            LSscoApi lsscoApi = new LSscoApi();
            this.list = lsscoApi.productList();
        } 

        private void btn_donation_item1_Click(
#nullable enable
        object sender, RoutedEventArgs e) => this.barcodeData("9880000000726");

        private void btn_donation_item2_Click(object sender, RoutedEventArgs e) => this.barcodeData("9880000000733");

        private void btn_donation_item3_Click(object sender, RoutedEventArgs e) => this.barcodeData("9880000000764");

        private void btn_donation_item4_Click(object sender, RoutedEventArgs e) => this.barcodeData("9880000000764");

        private void btn_back_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());

        private void barcodeData(string barcode)
        {
            Basepage basepage = new Basepage();
            this.product = this.list.prices.Where(x=> x.barcode == barcode).FirstOrDefault();
            if (this.product != null && basepage.addItem(sco_data.ReceiptNumber, barcode, this.product))
            {
                basepage.updateTransactionDetails(this.product);
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
            }
            else
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
        }
    }
}
