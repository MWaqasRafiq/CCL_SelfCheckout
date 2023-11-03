using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using IDOLSelfCheckout.UserControls;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using static IDOLSelfCheckout.FacePay;
using IDOLSelfCheckout.DataModel;
using System.Threading;


#nullable enable
namespace IDOLSelfCheckout
{
    public partial class MainWindow : Window, IComponentConnector
    {
        public static Grid Main_SCO;
        public static Grid Item_SCO;
        public static Image Img_Circle;
        private string _barcode = string.Empty;
        private System.Timers.Timer _timer;
        private List<string> Images1 = new List<string>();
        private int count1;
        internal
#nullable disable
        Image _sceneriesBtn;
        internal Grid _Main_sco;
        internal TextBox _textBox1;
        internal TextBlock _textBlock1;
        internal Image _imgCircle;
        private ServerIntegration serverIntegration;

        public MainWindow()
        {
            this.InitializeComponent();
            sco_data.ItemList = new List<items>();
            string[] filter = new string[4]
            {
        "*.jpg",
        "*.png",
        "*.gif",
        "*.jpeg"
            };
            foreach (string fileName in MainWindow.GetFileNames("C:\\IDOL\\images\\advertise\\", filter))
                this.Images1.Add(fileName);
            this._timer = new System.Timers.Timer(10000.0);
            this._timer.Elapsed += new ElapsedEventHandler(this._timer_Elapsed);
            this._timer.Enabled = true;
            this._timer.Start();

            serverIntegration = new ServerIntegration();
        }

        private void Window_Loaded(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            new Basepage().loadValues();
            MainWindow.Img_Circle = this.imgCircle;
            MainWindow.Main_SCO = this.Main_sco;
            uc_call.Uc_Add(MainWindow.Main_SCO, (UserControl)new ucStartScreen());
            new ucAsistantScreen().setLabel();
            this.PreviewKeyDown += new KeyEventHandler(this.labelBarCode_PreviewKeyDown);
        }

        private void labelBarCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int vkey = KeyInterop.VirtualKeyFromKey(e.Key == Key.System ? e.SystemKey : e.Key);
            char c = (char)vkey;
            Basepage.logWrite("vkey=" + c.ToString());

            if (char.IsNumber(c))
                _barcode += c;
            Basepage.logWrite("_barcode=" + _barcode);
            Basepage.logWrite("e.Key=" + Convert.ToString(e.Key));

            if (e.Key == Key.Return)
            {
                if (Basepage.LoyaltyRequested && !Basepage.LoyaltyScaned)
                {
                    Basepage.logWrite("Loyalty Scanned: " + _barcode);

                    Basepage.LoyaltyRequested = false;
                    Basepage.LoyaltyScaned = true;

                    if (true)
                        uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucPaymentScreen());
                    return;
                }
                Basepage.logWrite("Scanned barcode=" + _barcode);
                if (sco_data.TransactionProcess == "STARTED")
                {
                    string bar = _barcode;
                    Basepage.logWrite("Scanned Barcode=" + bar);
                    if (_barcode == "1111111111116")
                    {
                        uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucAsistantScreen());
                    }
                    else
                    {
                        sco_data.ScannedBarcode = _barcode;
                        Basepage.logWrite("sco_data.ScannedBarcode=" + sco_data.ScannedBarcode);
                        if (Basepage.IsLocalConsumption)
                        {
                            masafiPricesRequest masafiPricesRequest = new LSscoApi().productList();
                            prices prices = new prices();
                            prices product = Array.Find<prices>(masafiPricesRequest.prices, (Predicate<prices>)(element => element.barcode == sco_data.ScannedBarcode));
                            if (product != null)
                            {
                                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + product.name);
                                Basepage basepage = new Basepage();
                                Basepage.logWrite("basepage reinitialized");
                                if (basepage.addItem(sco_data.ReceiptNumber, sco_data.ScannedBarcode, product))
                                {
                                    Basepage.logWrite("sco_data.ScannedBarcode.Product=" + product.name + " Added");
                                    basepage.updateTransactionDetails(product);
                                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
                                }
                                else
                                {
                                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
                                    Basepage.logWrite("sco_data.ScannedBarcode.Product=" + product.name + " Went to Help");

                                }
                            }
                            else
                            {
                                Basepage.logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Not Found");
                            }
                        }
                        else
                        {
                            serverIntegration.AddItemToReceipt(sco_data.ScannedBarcode);
                        }
                        
                    }
                }
                this._barcode = "";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
                if (e.Key == Key.Return)
                {
                    this.textBlock1.Text = "You Entered: " + this.textBox1.Text;
                    sco_data.ScannedBarcode = this.textBlock1.Text;
                    Basepage.logWrite("Scanned Barcode=" + sco_data.ScannedBarcode);
                    Basepage basepage = new Basepage();
                    if (true)
                        uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
                    else
                        uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucHelpScreen());
                }
        }

        private static string[] GetFileNames(string path, string[] filter)
        {
            string[] array = ((IEnumerable<string>)filter).SelectMany<string, string>((Func<string, IEnumerable<string>>)(f => (IEnumerable<string>)Directory.GetFiles(path, f))).ToArray<string>();
            for (int index = 0; index < array.Length; ++index)
                array[index] = Path.GetFileName(array[index]);
            return array;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e) => ((DispatcherObject)this).Dispatcher.Invoke(new Action(this.UpdateImage));

        public void UpdateImage()
        {
            if (this.Images1 == null)
                return;
            if (this.count1 < this.Images1.Count)
                ++this.count1;
            if (this.count1 >= this.Images1.Count)
                this.count1 = 0;
            this.sceneriesBtn.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\IDOL\\images\\advertise\\" + this.Images1[this.count1].ToString());
        }


    }
}
