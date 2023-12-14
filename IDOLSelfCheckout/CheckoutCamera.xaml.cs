using IDOLSelfCheckout.Classes;
using IDOLSelfCheckout.LSRetail;
using IDOLSelfCheckout.UserControls;
using IDOLSelfCheckout;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace OposPrinter
{
    /// <summary>
    /// Interaction logic for CheckoutCamera.xaml
    /// </summary>
    public partial class CheckoutCamera : System.Windows.Window
    {
        private readonly VideoCapture capture;
        private readonly CascadeClassifier cascadeClassifier;

        private readonly BackgroundWorker bkgWorker;
        public CheckoutCamera()
        {
            InitializeComponent();

            capture = new VideoCapture();
            cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

            bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            bkgWorker.DoWork += Worker_DoWork;

            Loaded += CheckoutCamera_Loaded;
            Closing += MainWindow_Closing;

            Basepage.ledReceipt();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
            printScreenWait();
        }
        private void CheckoutCamera_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            capture.Open(0, VideoCaptureAPIs.ANY);
            if (!capture.IsOpened())
            {
                Close();
                return;
            }

            bkgWorker.RunWorkerAsync();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bkgWorker.CancelAsync();

            capture.Dispose();
            cascadeClassifier.Dispose();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                using (var frameMat = capture.RetrieveMat())
                {
                    var rects = cascadeClassifier.DetectMultiScale(frameMat, 1.1, 5, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

                    foreach (var rect in rects)
                    {
                        Cv2.Rectangle(frameMat, rect, Scalar.Red);
                    }

                    // Must create and use WriteableBitmap in the same thread(UI Thread).
                    Dispatcher.Invoke(() =>
                    {
                        FrameImage.Source = frameMat.ToWriteableBitmap();
                    });
                }

                Thread.Sleep(30);
            }
        }


        private void printScreenWait()
        {

            Thread.Sleep(500);
            Dispatcher.Invoke(() =>
            {
                sco_data.TransactionProcess = "FINISHED";
                OposPrinterCall pp = new OposPrinterCall();
                Boolean statu = pp.OPOSprint();
                if (statu)
                {
                    uc_call.Uc_Add(MainWindow.Main_SCO, new ucStartScreen());
                    sco_data.ItemList = new List<items>();
                    sco_data.TransactionTotal = Convert.ToDecimal("0").ToString("0.00");
                    sco_data.TransactionVat = Convert.ToDecimal("0").ToString("0.00"); ;
                    ucMainScreen.TransactionDetails.Content = (object)("StoreNo:" + sco_data.StoreNumber + "  Terminal:" + sco_data.TerminalNumber + "  \r\nReceiptNumber:" + sco_data.ReceiptNumber);
                    ucMainScreen.TransactionTotal.Content = (object)("TOTAL SAR " + sco_data.TransactionTotal);
                    ucMainScreen.TransactionVat.Content = (object)("VAT   SAR " + sco_data.TransactionVat);
                }
                else
                {
                    uc_call.Uc_Add(MainWindow.Item_SCO, new ucHelpScreen());
                }
                Thread.Sleep(2000);
            });
        }
    }
}
