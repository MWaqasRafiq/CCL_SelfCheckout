using IDOLSelfCheckout.Classes;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;


#nullable enable
namespace IDOLSelfCheckout.UserControls
{
    public partial class ucItemScreen : UserControl, IComponentConnector
    {
        internal
#nullable disable
        Button _btn_donation_add;
        internal Image _donationimg;
        internal Button _btn_bakery_add;
        internal Button _btn_herbs_add;
        internal Button _btn_operators_add;
        internal Button _btn_pay;
        CCL_Lamp lamp;
        public ucItemScreen()
        {
            this.InitializeComponent();
            lamp = new CCL_Lamp();
            //Basepage.ledItemScreen();
            lamp.BlueOpen();

            btn_pay.SetResourceReference(ContentProperty, "pay");
            btn_bakery_add.SetResourceReference(ContentProperty, "firstAid");
            btn_herbs_add.SetResourceReference(ContentProperty, "homeCare");
            btn_donation_add.SetResourceReference(ContentProperty, "donate");
            //lbl
            //lblScanItems.SetResourceReference(ContentProperty, "firstAid");
            //lblScanItemsDtl.SetResourceReference(ContentProperty, "firstAid");
        }

        private void btn_pay_Click(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            Basepage basepage = new Basepage();
            if (true)
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucPaymentScreen());
            else
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
        }

        private void btn_donation_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucDonationScreen());

        private void btn_bakery_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucBakeryScreen());

        private void btn_herbs_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHerbsScreen());

        private void btn_operators_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucOperatorsScreen());

        private void Window_Loaded(object sender, RoutedEventArgs e) => ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber);

    }
}
