using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper.UIHelper
{
    public static class SystemConfig
    {
        private static string BaseAddressUri { get { return "http://172.16.61.12:9011/Transaction"; } }
        public static string MakeTokenUri { get { return $"{BaseAddressUri}/GetToken"; } }
        public static string ValidateTokenUri { get { return $"{BaseAddressUri}/ValidateToken"; } }
        //public static string IpgUri { get { return "https://ikc.shaparak.ir/TPayment/Payment/index"; } }
        public static string IpgUri { get { return "https://ikc.shaparak.ir/iuiv3/IPG/Index/"; } }


        //public static string RevertUrl { get { return "http://localhost:1270/qrc/Payment/DoResponse"; } }
        public static string RevertUrl { get { return "https://ikc.shaparak.ir/qrc/Payment/DoResponse"; } }
        public static string GetMerchantUri { get { return "http://172.16.67.34:8686/Merchant.svc/getMerchantInfo"; } }
        public static string GetInvoiceNumber()
        {
            var invoice = Guid.NewGuid().ToString();
            invoice = Regex.Replace(invoice, "[A-Za-z\\-]", string.Empty);
            return invoice.Length > 12 ? invoice.Substring(0, 12) : invoice;
        }


    }
}
