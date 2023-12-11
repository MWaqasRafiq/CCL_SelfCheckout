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
    public partial class ucVirginScreen : UserControl, IComponentConnector
    {
        internal
#nullable disable
        Button _btn_virgin_item1;
        internal Button _btn_virgin_item2;
        internal Button _btn_virgin_item3;
        internal Button _btn_virgin_item4;
        internal Button _btn_back;
        private DataModels.prices product = new DataModels.prices();
        private PricesRequest list = new PricesRequest();
        public ucVirginScreen()
        {
            this.InitializeComponent();
            LSscoApi lsscoApi = new LSscoApi();
            this.list = lsscoApi.productList();
        } 

        private void btn_back_Click(
#nullable enable
        object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucOperatorsScreen());

        private void btn_virgin_item1_Click(object sender, RoutedEventArgs e) => this.barcodeData("5699100006965");

        private void btn_virgin_item2_Click(object sender, RoutedEventArgs e) => this.barcodeData("5699100006613");

        private void btn_virgin_item3_Click(object sender, RoutedEventArgs e) => this.barcodeData("5699100006873");

        private void btn_virgin_item4_Click(object sender, RoutedEventArgs e) => this.barcodeData("5699100006668");

        private void barcodeData(string barcode)
        {
            Basepage basepage = new Basepage();
            this.product = this.list.prices.Where(x => x.barcode == barcode).FirstOrDefault();
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
