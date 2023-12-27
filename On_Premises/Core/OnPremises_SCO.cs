using DataModels;
using DataModels.LsRetail;
using DataModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On_Premises.Core
{
    public class OnPremises_SCO
    {

        public static string LogFilePath;
        public OnPremises_SCO()
        {
            if (ConfigurationManager.AppSettings["LogFile"] != null)
                LogFilePath = ConfigurationManager.AppSettings["LogFile"].ToString();
        }
        public view_models AddItemOnPremises()
        {
            view_models viewModels = new view_models();
            PricesRequest masafiPricesRequest;
            using (StreamReader streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\products" + "\\products.json"))
                masafiPricesRequest = JsonConvert.DeserializeObject<PricesRequest>(streamReader.ReadToEnd());

            prices prices = new prices();
            prices product = Array.Find<prices>(masafiPricesRequest.prices, (Predicate<prices>)(element => element.barcode == sco_data.ScannedBarcode));
            if (product != null)
            {
                logWrite("sco_data.ScannedBarcode.Product=" + product.name);
                viewModels = addItem(sco_data.ReceiptNumber, sco_data.ScannedBarcode, product);
            }
            else
            {
                logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Not Found");
                return null;
            }
            return viewModels;
        }
        public view_models VoidItemOnPremises()
        {
            view_models viewModels = new view_models();
            PricesRequest masafiPricesRequest;
            using (StreamReader streamReader = new StreamReader(Directory.GetCurrentDirectory() + "\\products" + "\\products.json"))
                masafiPricesRequest = JsonConvert.DeserializeObject<PricesRequest>(streamReader.ReadToEnd());

            prices prices = new prices();
            prices product = Array.Find<prices>(masafiPricesRequest.prices, (Predicate<prices>)(element => element.barcode == sco_data.ScannedBarcode));
            if (product != null)
            {
                logWrite("sco_data.ScannedBarcode.Product=" + product.name);
                viewModels = voidItem(sco_data.ReceiptNumber, sco_data.ScannedBarcode, product);
            }
            else
            {
                logWrite("sco_data.ScannedBarcode.Product=" + sco_data.ScannedBarcode + " Not Found");
                return null;
            }
            return viewModels;
        }

        public view_models addItem(string receiptNo, string barcodeNo, prices product)
        {
            bool flag = true;
            view_models viewModels = new view_models();
            if (flag)
            {
                sco_data.TransactionTotal = (Convert.ToDouble(sco_data.TransactionTotal) + Convert.ToDouble(product.price)).ToString("0.00");
                sco_data.TransactionVat = (Convert.ToDouble(sco_data.TransactionVat) + Convert.ToDouble(product.price) / 100.0 * 5.0).ToString("0.00");
                if (flag)
                {
                    List<items> itemList = sco_data.ItemList;
                    if (itemList.Where(x => x.Name == product.name && x.Price == product.price).Any())
                    {
                        itemList.Where(w => w.Name == product.name && w.Price == product.price)
                                .ToList().ForEach(w => w.Qty = (Convert.ToInt32(w.Qty) + 1).ToString());
                    }
                    else
                    {
                        itemList.Add(new items()
                        {
                            Name = product.name,
                            Qty = "1",
                            Price = product.price
                        });
                    }
                    sco_data.LastItemDescription = product.name;
                    if (itemList.Count <= 0)
                        return viewModels;
                    viewModels = new view_models()
                    {
                        items = (IEnumerable<items>)itemList
                    };
                }
            }
            return viewModels;
        }

        public view_models voidItem(string receiptNo, string barcodeNo, prices product)
        {
            bool flag = true;
            view_models viewModels = new view_models();
            if (flag)
            {
                sco_data.TransactionTotal = (Convert.ToDouble(sco_data.TransactionTotal) - Convert.ToDouble(product.price)).ToString("0.00");
                sco_data.TransactionVat = (Convert.ToDouble(sco_data.TransactionVat) - Convert.ToDouble(product.price) / 100.0 * 5.0).ToString("0.00");
                if (flag)
                {
                    List<items> itemList = sco_data.ItemList;
                    if (itemList.Where(x => x.Name == product.name && x.Price == product.price).Any())
                    {
                        var item = itemList.Where(w => w.Name == product.name).FirstOrDefault();
                        if(item != null)
                            itemList.Remove(item);
                    }

                    if (itemList.Count <= 0)
                        return viewModels;
                    viewModels = new view_models()
                    {
                        items = (IEnumerable<items>)itemList
                    };
                }
            }
            return viewModels;
        }

        /// <summary>
        /// Write Logs
        /// </summary>
        /// <param name="msg"></param>
        public static void logWrite(string msg)
        {
            string format = "ddMMyyyy";
            string path = LogFilePath + "POS" + DateTime.Now.ToString(format) + ".log";
            string str = "1.3.0.2";
            if (!File.Exists(path))
            {
                StreamWriter streamWriter = new StreamWriter(path);
                streamWriter.WriteLine("[" + str.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                streamWriter.Close();
            }
            else
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                    {
                        streamWriter.WriteLine("[" + str.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                        streamWriter.Close();
                    }
                }
            }
        }
    }
}
