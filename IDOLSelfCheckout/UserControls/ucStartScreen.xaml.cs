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
    /// Interaction logic for ucStartScreen.xaml
    /// </summary>
    public partial class ucStartScreen : UserControl
    {
        public ucStartScreen()
        {
            InitializeComponent();
            Basepage.ledNewCustomer();
        }

        private void btn_start_sco_Click(object sender, RoutedEventArgs e)
        {
           uc_call.Uc_Add(MainWindow.Main_SCO, new ucMainScreen());

        }
    }
}
