using DataModels.LsRetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Shared
{
    public class sco_data
    {
        public static string SessionId;
        public static string TransactionPinpadInfo;
        public static string TransactionProcess;
        public static string ScannedBarcode;
        public static string ReceiptNumber;
        public static string StoreNumber;
        public static string TerminalNumber;
        public static string StaffId;
        public static string TransactionDateTime;
        public static string TransactionTotal;
        public static string TransactionKey;
        public static string TransactionDiscount;
        public static string TransactionStatus;
        public static string TransactionNo;
        public static string MaskCardNumber;
        public static string ExpiryDate;
        public static string AuthCode;
        public static string TransactionVat;
        public static string LastItemDescription;
        public static string ErrorMessage = "";
        public static List<items> ItemList;
    }
}
