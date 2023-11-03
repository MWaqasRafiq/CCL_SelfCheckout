using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using IDOLSelfCheckout.Classes;
using POS.Devices;
using static System.Net.Mime.MediaTypeNames;
using POS.Devices;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;

namespace IDOLSelfCheckout.LSRetail
{
    public class OposPrinterCall
    {
        private string _oposDeviceName = Basepage.OPOS_PrinterName;
        //private OPOSPOSPrinter Printer = null;
        public OposPrinterCall()
        {
        }
        public Boolean print()
        {
            Boolean status = false;
            try
            {
                Basepage.logWrite("Printer print..");
                string items = "";
                List<items> itemList = sco_data.ItemList;

                if(itemList.Count == 0 ) { return status; }
                
                foreach (items item in itemList)
                {
                    string name = " " + item.Name + "                           ";
                    name = name.Substring(0, 27);
                    string price = "          " + item.Price;
                    price = price.Substring(price.Length - 10, 10);
                    items += name + price + "\n";
                }

                string receipt = " ===================================\n" +
                             " \t\t  Choithrams \n" +
                             " ===================================\n" +
                             " \t full of goodness\n" +
                             " ----------------------------------------------------\n" +
                             " Slip:\t\t " + sco_data.ReceiptNumber + "\n" +
                             " Staff:101 \t Trans: " + sco_data.TransactionNo + "\n" +
                             " Date: \t\t " + sco_data.TransactionDateTime + "\n" +
                             " ----------------------------------------------------\n\n" +
                             " Description \t\t Amount\n" +
                             " ----------------------------------------------------\n" +
                             items +
                             " ----------------------------------------------------\n" +
                             " Discount\t\t\t" + (string.IsNullOrEmpty(sco_data.TransactionDiscount) ? "0.00" : sco_data.TransactionDiscount) + "\n" +
                             " Cards \t\t\t" + (string.IsNullOrEmpty(sco_data.TransactionTotal) ? "0.00" : sco_data.TransactionTotal) + "\n\n" +
                             " ----------------------------------------------------\n" +
                             " Total       \t\t Amount\n" +
                             " "+itemList.Count.ToString() + " \t\t\t" + itemList.Sum(x => Convert.ToDecimal(x.Price)).ToString() + "\n" +
                             " ----------------------------------------------------\n\n";


                Basepage.logWrite("Printer receipt:\n" + receipt);
                receipt += "\n\t ** Happy To See You Again** \n\n\n\n\n \x1b\x0C";
                var printDlg = new PrintDialog();
                var doc = new FlowDocument(new Paragraph(new Run(receipt)));
                doc.PagePadding = new Thickness(10);
                doc.FontSize = 16;
                doc.FontFamily = new System.Windows.Media.FontFamily("Calibri");

                printDlg.PrintDocument((doc as IDocumentPaginatorSource).DocumentPaginator, "Print Receipt");
                Basepage.logWrite("printed");
                //Printer.Close();
                status = true;
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Printer Error:" + ex.Message);
                sco_data.ErrorMessage = ex.Message;
                status = false;
            }
            return status;
        }

        public Boolean OPOSprint()
        {
            Boolean status = false;
            try
            {
                Basepage.logWrite("Printer print..");
                string items = "";
                List<items> itemList = sco_data.ItemList;

                if (itemList.Count == 0) { return status; }

                foreach (items item in itemList)
                {
                    string name = item.Qty + "x " + item.Name + "                           ";
                    name = name.Substring(0, 27);
                    string price = "          " + (item.Price.Contains(".") ? item.Price : item.Price + ".00");
                    price = price.Substring(price.Length - 10, 10);
                    items += name + price + "\n";
                }

                string receipt = "\n ===========================================\n" +
                             " \t\t Choithrams       \n" +
                             " ===========================================\n" +
                             " \t full of goodness\n" +
                             " --------------------------------------------\n" +
                             " Slip:\t\t " + sco_data.ReceiptNumber + "\n" +
                             " Staff:101 \t\t Trans: " + sco_data.TransactionNo + "\n" +
                             " Date: \t\t " + (string.IsNullOrEmpty(sco_data.TransactionDateTime) ? "0.00" : sco_data.TransactionDateTime) + "\n" +
                             " --------------------------------------------\n\n" +
                             " Description و\t\t Amount م\n" +
                             " --------------------------------------------\n" +
                               items +
                             " --------------------------------------------\n" +
                             " Discount\t\t\t" + (string.IsNullOrEmpty(sco_data.TransactionDiscount) ? "0.00" : sco_data.TransactionDiscount) + "\n" +
                             " Cards \t\t\t\t" + (string.IsNullOrEmpty(sco_data.TransactionTotal) ? "0.00" : sco_data.TransactionTotal) + "\n\n" +
                             " --------------------------------------------\n" +
                             " Total       \t\t\t Amount\n" +
                             " " + itemList.Count.ToString() + " \t\t\t\t" + itemList.Sum(x => Convert.ToDecimal(x.Price)).ToString() + "\n" +
                             " --------------------------------------------\n\n";


                Basepage.logWrite("Printer receipt:\n" + receipt);

                ////byte[] BinaryData = System.Text.Encoding.UTF8.GetBytes(string.IsNullOrEmpty(sco_data.ReceiptNumber) ? "1234567890" : sco_data.ReceiptNumber);
                //var codes = QRCodeWriter.CreateQrCode(BinaryData, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng("MyQR.png");
                //var res = CreateQRCode(string.IsNullOrEmpty(sco_data.ReceiptNumber) ? "1234567890" : sco_data.ReceiptNumber);
                OPOSPOSPrinter Printer = new OPOSPOSPrinterClass();

                Printer.Open(_oposDeviceName); // Check your printer class after executing this line and make sure there is no fault on the instantiated class (printer)
                Basepage.logWrite("device Opened");
                Printer.ClaimDevice(2000); //Is it enought to pool your device
                Printer.CharacterSet = 1256;
                Printer.DeviceEnabled = true;
                //Printer.SetBitmap(1, 2, "C:\\IDOL\\images\\bits\\logo122.bpm", 100, -2);
                //Printer.PrintBitmap(2, "C:\\IDOL\\images\\bits\\logo122.bpm", 100, -2);
                Printer.PrintNormal(2, receipt);
                Basepage.logWrite("Printed main part");
                Printer.PrintBarCode(2, string.IsNullOrEmpty(sco_data.ReceiptNumber) ? "98509798524383" : sco_data.ReceiptNumber, 108, 100, 200, -2, -13);

                Printer.PrintNormal(2, "\n -------------------------------------------- \n");

                //Printer.PrintBitmap(2, "C:\\IDOL\\images\\bits\\logo20.bpm", 100, -2);
                //string ArabicChars = "اللغة العربية";
                //var arabic = Encoding.GetEncoding(1256);
                //Printer.PrintNormal(2, arabic.GetString(arabic.GetBytes(ArabicChars)));
                //Printer.PrintNormal(2, "\n --------------------------------------------");
                //Printer.PrintBarCode(2,"Test",);
                Printer.PrintNormal(2, "\n     **THANK YOU, HAPPY TO SEE YOU AGAIN** \n\n\n\n\n\n");
                //////////////////Printer.PrintNormal(2, "\x1B|cA\x1B|2COPOS POSPrinter\x1B|1C\nvia Microsoft.NET\n\n");//Make sure about this line seems to be tricky

                //Printer.SetLogo(1, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");
                //// Printer.SetLogo(1, "\x1b\xa\xd");    
                //Printer.PrintNormal(2, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");

                //Printer.SetLogo(1, "\x1b|tL");
                //Printer.PrintNormal(2, "logo print." + "\n");

                //Printer.SetLogo(0, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");
                //Printer.SetLogo(1, "\x1b|tL");
                //Printer.PrintNormal(2, "logo print." + "\n");
                //Printer.PrintNormal(2, " \x1b\x0C");
                //Printer.PrintNormal(2, "arabic print." + "\n");
                //Printer.DirectIO(111, 1, "-2");
                //string arabic = "مانشلضرون";
                ////string CodeArabic1256 = ASCIIEncoding.Default.GetString(Encoding.GetEncoding(1256).GetBytes(arabic));
                ////byte[] winByte = Encoding.GetEncoding(1256).GetBytes(receipt);
                ////string result = Encoding.GetEncoding(1256).GetString(winByte);
                //Printer.PrintNormal(2, arabic);
                //Printer.CharacterSet = 864;
                //Printer.PrintNormal(2, arabic);
                //Printer.PrintNormal(2, Cutter);
                //Basepage.logWrite("printed2");

                Printer.CutPaper(99);

                Basepage.logWrite("printed");

                Printer.Close();
                status = true;
            }
            catch (Exception ex)
            {
                Basepage.logWrite("Printer Error:" + ex.Message);
                sco_data.ErrorMessage = ex.Message;
                status = false;
            }
            return status;
        }

        //public string CreateQRCode(string qRCode)
        //{
        //    QRCodeGenerator QrGenerator = new QRCodeGenerator();
        //    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qRCode, QRCodeGenerator.ECCLevel.Q);
        //    QRCode QrCode = new QRCode(QrCodeInfo);
        //    Bitmap QrBitmap = QrCode.GetGraphic(60);
        //    byte[] BitmapArray = BitmapToByteArray(QrBitmap);
        //    string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
        //    Basepage.logWrite("QR Code = " + QrUri);
        //    return QrUri;
        //}
        //public static byte[] BitmapToByteArray(Bitmap bitmap)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        bitmap.Save(ms, ImageFormat.Png);
        //        return ms.ToArray();
        //    }
        //}
        //public Boolean print1()
        //{
        //    Boolean status=false;
        //    try
        //    {
        //        string items = "";
        //        List<items> itemList = sco_data.ItemList;
        //        foreach (items item in itemList)
        //        {
        //            string name = item.Name+ "                                    ";
        //            name = name.Substring(0, 27);
        //            string price ="            " +item.Price;
        //            price = price.Substring(price.Length-10,10);
        //            items += name  + price + "\n";

        //        }
        //        string receipt = "==========================================\n" +
        //                     "                   NESTO       \n" +
        //                     "==========================================\n" +
        //                     "              All that you need.\n" +
        //                     "-----------------------------------------\n" +
        //                     "Slip:          " + sco_data.ReceiptNumber + "\n" +
        //                     "Staff:101      Trans: " + sco_data.TransactionNo + "\n" +
        //                     "Date:          " + sco_data.TransactionDateTime + "\n" +
        //                     "-----------------------------------------\n\n" +
        //                     "Description                     Amount\n" +
        //                     "-----------------------------------------\n" +
        //                     items +
        //                     "-----------------------------------------\n" +
        //                     "Discount                        " + sco_data.TransactionDiscount + "\n" +
        //                     "Cards                          " + sco_data.TransactionTotal + "\n\n" +
        //                     "      \n\n";


        //        Basepage.logWrite("Printer receipt:\n" + receipt);
        //        OposPOSPrinter_CCO.OPOSPOSPrinter Printer = new OposPOSPrinter_CCO.OPOSPOSPrinter(); //Make sure you don`t need to initialize anything and check overloaded constructors
        //        int res = Printer.Open(Basepage.OPOS_PrinterName); // Check your printer class after executing this line and make sure there is no fault on the instantiated class (printer)
        //        //Basepage.logWrite("device Opened");
        //        //Printer.ClaimDevice(2000); //Is it enought to pool your device
        //        //Printer.CharacterSet = 1256;
        //        //Printer.DeviceEnabled = true;
        //        int res2=  Printer.PrintNormal(2, receipt);
        //        //Printer.PrintBarCode(2, sco_data.ReceiptNumber, 108, 100, 200, -2, -13);
        //        //Printer.PrintNormal(2, "\n        Happy To See You Again \n\n\n\n\n\n \x1b\x0C");
        //        //string GS = Convert.ToString((char)29);
        //        //string ESC = Convert.ToString((char)27);
        //        //string Cutter = "";
        //        //Cutter = ESC + "@";
        //        //Cutter += GS + "V" + (char)48;
        //        //Printer.PrintNormal(2, Cutter);
        //        //Basepage.logWrite("printed");
        //        //Printer.PrintNormal(2, "\x1B|cA\x1B|2COPOS POSPrinter\x1B|1C\nvia Microsoft.NET\n\n");//Make sure about this line seems to be tricky

        //        //Printer.SetLogo(1, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");
        //        //// Printer.SetLogo(1, "\x1b\xa\xd");    
        //        //Printer.PrintNormal(2, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");

        //        //Printer.SetLogo(1, "\x1b|tL");
        //        //Printer.PrintNormal(2, "logo print." + "\n");

        //        //Printer.SetLogo(0, (char)0x1B + (char)0x7C + (char)0x74 + (char)0x4C + "");
        //        //Printer.SetLogo(1, "\x1b|tL");
        //        //Printer.PrintNormal(2, "logo print." + "\n");
        //        //Printer.PrintNormal(2, " \x1b\x0C");
        //        //Printer.PrintNormal(2, "arabic print." + "\n");
        //        //Printer.DirectIO(111, 1, "-2");
        //        //string arabic = "مانشلضرون";
        //        ////string CodeArabic1256 = ASCIIEncoding.Default.GetString(Encoding.GetEncoding(1256).GetBytes(arabic));
        //        ////byte[] winByte = Encoding.GetEncoding(1256).GetBytes(receipt);
        //        ////string result = Encoding.GetEncoding(1256).GetString(winByte);
        //        //Printer.PrintNormal(2, arabic);
        //        //Printer.CharacterSet = 864;
        //        //Printer.PrintNormal(2, arabic);
        //        //Printer.PrintNormal(2, Cutter);
        //        //Basepage.logWrite("printed2");

        //        //Printer.CutPaper(2);
        //        int res3 = Printer.Close();
        //        status = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Basepage.logWrite("Printer Error:" + ex.Message);
        //        sco_data.ErrorMessage = ex.Message;
        //        status = false;
        //    }
        //    return status;
        //}
        //public void open()
        //{
        //    Basepage.logWrite("+Open");
        //    int nRC;
        //    // Open the printer.
        //    nRC = Printer.Open(txtPrinter.Text);
        //    Basepage.logWrite("  Open: RC = " + nRC);
        //    // If succeeded, then claim.
        //    if (nRC == (int)OPOS _Constants.OPOS_SUCCESS)
        //    {
        //        Basepage.logWrite("  SOD <" + Printer.ServiceObjectDescription + ">");
        //        Basepage.logWrite("  SOV " + Printer.ServiceObjectVersion);
        //        nRC = Printer.ClaimDevice(1000);
        //        Basepage.logWrite("  Claim: RC = " + nRC);
        //        // If succeeded, then enable.
        //        if (nRC == (int)OPOS_Constants.OPOS_SUCCESS)
        //        {
        //            Printer.DeviceEnabled = true;
        //            nRC = Printer.ResultCode;
        //            Basepage.logWrite("  Enable: RC = " + nRC);
        //        }
        //    }
        //    Basepage.logWrite("-Open: " + ((nRC == (int)OPOS_Constants.OPOS_SUCCESS) ? "Succeeded" : "Failed"));
        //}

        //private void CloseButton_Click(object sender, System.EventArgs e)
        //{
        //    Basepage.logWrite("+Close");
        //    int nRC = Printer.Close();
        //    Basepage.logWrite("  Close: RC = " + nRC);
        //    Basepage.logWrite("-Close: " + ((nRC == (int)OPOS_Constants.OPOS_SUCCESS) ? "Succeeded" : "Failed"));
        //}

        //private void PrintButton_Click(object sender, System.EventArgs e)
        //{

        //    string q = txtArabic.Text;
        //    UTF7Encoding utf = new UTF7Encoding();
        //    // Convert UTF16 to ANSI codepage 1256. winByte[] will be ANSI codepage 1256.
        //    byte[] winByte1 = Encoding.GetEncoding(1256).GetBytes(q);
        //    string ff = "";
        //    for (int l = 0; l < winByte1.Length; l++)
        //    {
        //        ff = ff + winByte1[l] + ":" + (char)winByte1[l] + "\n";
        //    }
        //    // Convert UTF7 to UTF16.
        //    // But this is WRONG because winByte is ANSI codepage 1256, NOT UTF7!


        //    string text = File.ReadAllText("c:\\PRINTERTEST\\a.txt", Encoding.Default);

        //    string CodeFile1256 = ASCIIEncoding.Default.GetString(Encoding.GetEncoding(1256).GetBytes(text));
        //    //Debug.Assert(result1 != q); // So result doesn't equal q

        //    //// The CORRECT way to convert the ANSI string back:
        //    //// Convert ANSI codepage 1256 string to UTF16

        //    //result1= Encoding.GetEncoding(1256).GetString(winByte);

        //    // Debug.Assert(result1 == q); // Now result DOES equal q
        //    //byte[] winByte12 = Encoding.GetEncoding(1256).GetBytes(q);
        //    string arabic = "مانشلضرون";
        //    string CodeText1256 = ASCIIEncoding.Default.GetString(Encoding.GetEncoding(1256).GetBytes(q));
        //    string CodeArabic1256 = ASCIIEncoding.Default.GetString(Encoding.GetEncoding(1256).GetBytes(arabic));
        //    string UTF7 = new UTF7Encoding().GetString(Encoding.GetEncoding(1256).GetBytes(arabic));


        //    string result = "\x1b\x0C";

        //    if (checkBoxDll.Checked == true)
        //    {

        //        Encoding864 encode = new Encoding864();
        //        result = encode.printIBM864(txtArabic.Text);


        //    }
        //    else if (txtCodePage.Text == "1256")
        //    {
        //        byte[] winByte = Encoding.GetEncoding(1256).GetBytes(txtArabic.Text);
        //        result = result + Encoding.GetEncoding(1256).GetString(winByte);

        //    }
        //    else
        //    {

        //        result = result + txtArabic.Text;

        //        result = result + "  \x0C";

        //    }

        //    if (txtCodePage.Text != "")
        //    {
        //        Printer.CharacterSet = Convert.ToInt32(txtCodePage.Text);
        //    }
        //    Basepage.logWrite("+Print CharSet:" + Printer.CharacterSet);
        //    int nRC = Printer.PrintNormal(2, "stating to print.." + "\n");
        //    Basepage.logWrite("  Print: RC = " + nRC);
        //    Basepage.logWrite("-Print: " + ((nRC == (int)OPOS_Constants.OPOS_SUCCESS) ? "Succeeded" : "Failed"));
        //    nRC = Printer.PrintNormal(2, "PrintNormal:" + txtArabic.Text + "\n");

        //    nRC = Printer.PrintNormal(2, "PrintBarCode code 39  \n");
        //    Printer.PrintBarCode((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, "99070010609700010003", (int)OPOSPOSPrinterConstants.PTR_BCS_Code39, 100, 200, (int)OPOSPOSPrinterConstants.PTR_BC_CENTER, (int)OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW);

        //    nRC = Printer.PrintNormal(2, "PrintBarCode code 128  \n");
        //    Printer.PrintBarCode((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, "99070010609700010003", (int)OPOSPOSPrinterConstants.PTR_BCS_Code128, 100, 200, (int)OPOSPOSPrinterConstants.PTR_BC_CENTER, (int)OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW);

        //    nRC = Printer.PrintNormal(2, "PrintBarCode EAN 13  \n");
        //    Printer.PrintBarCode((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, "1234567890128", (int)OPOSPOSPrinterConstants.PTR_BCS_EAN13, 100, 200, (int)OPOSPOSPrinterConstants.PTR_BC_CENTER, (int)OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW);


        //    nRC = Printer.PrintNormal(2, "UTF7:" + UTF7 + "\n");
        //    nRC = Printer.PrintNormal(2, "1256CodeText:" + CodeText1256 + "\n");
        //    nRC = Printer.PrintNormal(2, "1256CodeFile:" + CodeFile1256 + "\n");
        //    nRC = Printer.PrintNormal(2, "CodeArabic1256:" + CodeArabic1256 + "\n");
        //    nRC = Printer.PrintNormal(2, "file" + text + "\n");
        //    nRC = Printer.PrintNormal(2, "\x1b\x0C" + ff + "\n");
        //    nRC = Printer.PrintNormal(2, "217:" + (char)217 + "\n");
        //    nRC = Printer.PrintNormal(2, "218:" + (char)218 + "\n");
        //    Basepage.logWrite("  Print: RC = " + nRC);
        //    Basepage.logWrite("-Print: " + ((nRC == (int)OPOS_Constants.OPOS_SUCCESS) ? "Succeeded" : "Failed"));

        //    for (int i = 0; i < 5; i++)
        //    {
        //        nRC = Printer.PrintNormal(2, "" + result + "\n");
        //        Basepage.logWrite("  Print: RC = " + nRC);
        //        Basepage.logWrite("-Print: " + ((nRC == (int)OPOS_Constants.OPOS_SUCCESS) ? "Succeeded" : "Failed"));
        //    }
        //}

    }
}
