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
    /// Interaction logic for ucOperatorsScreen.xaml
    /// </summary>
    public partial class ucOperatorsScreen : UserControl
    {
        public ucOperatorsScreen()
        {
            InitializeComponent();
        }

        private void btn_etisalat_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucEtisalatScreen());
        }

        private void btn_du_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucDuScreen());
        }

        private void btn_virgin_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucVirginScreen());
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucItemScreen());
        }
    }
}
