using DataModels.GeneralSCO;
using DataModels.Shared;
using IDOLSelfCheckout.Classes;
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
        }

        bool IsAllDigits(string s) => s.Replace(",", "").Replace(".", "").All(char.IsDigit);
#nullable enable
        private void btn_pay_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sco_data.TransactionTotal) && IsAllDigits(sco_data.TransactionTotal)
                && Convert.ToDecimal(sco_data.TransactionTotal) > 0)
            {
                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucPaymentScreen());
            }
            else
            {
                SignTerminalRequest terminalRequest = new SignTerminalRequest() {
                    Type = "on",
                    Password = "",
                    StoreNo ="",
                    TerminalNo = "",
                    UserId = ""
                };
                GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();
                var res = general_SCO.SignTerminal(terminalRequest);
            }
        }

        private void btn_donation_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucDonationScreen());

        private void btn_bakery_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucBakeryScreen());

        private void btn_herbs_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHerbsScreen());

        private void btn_operators_add_Click(object sender, RoutedEventArgs e) => uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucOperatorsScreen());

        private void Window_Loaded(object sender, RoutedEventArgs e) => ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber);

    }
}
