//using IDOLSelfCheckout.Classes;
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
using System.Threading;
using System.Runtime.InteropServices;
//using Toshiba_SIT.Core;
//using GeneralSCO.Core;
using DataModels.Shared;
using IDOLSelfCheckout.Classes;
using DataModels.GeneralSCO;
//using On_Premises.Core;


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
        CCL_Lamp lamp;

        public MainWindow()
        {
            this.InitializeComponent();
            sco_data.ItemList = new List<DataModels.Shared.items>();
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

            lamp = new CCL_Lamp();
            lamp.GreenOpen();

            
        }

        private void Window_Loaded(
#nullable enable
        object sender, RoutedEventArgs e)
        {
            new Basepage().loadValues();
            MainWindow.Img_Circle = this.imgCircle;
            MainWindow.Main_SCO = this.Main_sco;

            SignTerminalRequest terminalRequest = new SignTerminalRequest()
            {
                Type = "on",
                Password = "",
                StoreNo = Basepage.StoreNumber,
                TerminalNo = Basepage.TerminalId,
                UserId = ""
            };
            GeneralSCO.Core.General_SCO general_SCO = new GeneralSCO.Core.General_SCO();
            var res = general_SCO.SignTerminal(terminalRequest);
            if (res.Item1 == 1)
            {
                uc_call.Uc_Add(MainWindow.Main_SCO, (UserControl)new ucStartScreen());
            }
            else
            {
                uc_call.Uc_Add(MainWindow.Main_SCO, (UserControl)new ucClosedScreen());
            }

            this.PreviewKeyDown += new KeyEventHandler(this.labelBarCode_PreviewKeyDown);
            VideoControl.Source = new Uri(Basepage.VideoControlSource);
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

                    uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucPaymentScreen());
                    return;
                }
                else if (Basepage.VoidRequested && !Basepage.VoidScaned)
                {
                    Basepage.logWrite("VOID Scanned: " + _barcode);
                    sco_data.ScannedBarcode = _barcode;
                    switch (Basepage.ServerName)
                    {
                        case "D3":
                            break;
                        default:
                            new Basepage().VoidItemOnPremises();
                            break;
                    }
                    Basepage.VoidRequested = false;
                    Basepage.VoidScaned = true;

                    return;
                }
                else
                {
                    Basepage.logWrite("Scanned barcode=" + _barcode);

                    if (sco_data.TransactionProcess == "STARTED")
                    {
                        Basepage.logWrite("Scanned Barcode=" + _barcode);

                        if (_barcode == "1111111111116")
                        {
                            uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucAsistantScreen());
                        }
                        else
                        {
                            sco_data.ScannedBarcode = _barcode;
                            Basepage.logWrite("sco_data.ScannedBarcode=" + sco_data.ScannedBarcode);
                            // here we will choose the server that we want to integrate
                            switch (Basepage.ServerName)
                            {
                                case "SA":
                                    Toshiba_SIT.Core.ToshibaSA toshibaSA = new Toshiba_SIT.Core.ToshibaSA();
                                    toshibaSA.AddItemToReceipt(sco_data.ScannedBarcode);
                                    break;
                                case "GP":
                                    new Basepage().GeneralPosAddItem();
                                    break;
                                case "D3":
                                    break;
                                default:
                                    new Basepage().AddItemOnPremises();
                                    break;
                            }
                        }
                    }
                }
                this._barcode = string.Empty;
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

                uc_call.Uc_Add(MainWindow.Item_SCO, (UserControl)new ucItemScreen());
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

        private void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoControl.Position = TimeSpan.FromSeconds(0);
            VideoControl.Play();
        }

        public void UpdateImage()
        {
            if (this.Images1 == null)
                return;
            if (this.count1 < this.Images1.Count)
                ++this.count1;
            if (this.count1 >= this.Images1.Count)
                this.count1 = 0;
            //this.sceneriesBtn.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\IDOL\\images\\advertise\\" + this.Images1[this.count1].ToString());
        }

    }
}
