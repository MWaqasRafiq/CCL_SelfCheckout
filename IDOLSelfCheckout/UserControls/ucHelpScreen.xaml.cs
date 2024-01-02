using IDOLSelfCheckout.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ucHelpScreen.xaml
    /// </summary>
    public partial class ucHelpScreen : UserControl
    {
        CCL_Lamp lamp;
        public ucHelpScreen()
        {
            InitializeComponent();
            //Basepage.ledHelp();
            lamp = new CCL_Lamp();
            lamp.RedOpen();
        }
        private void btn_help_sco_Click(object sender, RoutedEventArgs e)
        {
            btn_help_sco.Focusable = true;
            uc_call.Uc_Add(MainWindow.Item_SCO, new ucAsistantScreen());
        }
     
    }
}
