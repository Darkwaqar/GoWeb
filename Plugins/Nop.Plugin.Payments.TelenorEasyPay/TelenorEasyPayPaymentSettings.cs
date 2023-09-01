using Nop.Core.Configuration;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.TelenorEasyPay
{
    public class TelenorEasyPayPaymentSettings : ISettings
    {
        public string StoreId { get; set; }
        public double ExpiryDate { get; set; }
        public bool IsMerchantTokenExpiry { get; set; }
        public bool IsSandbox { get; set; }
        public bool AutoRedirect { get; set; }
        public IList<SelectList> AvaliblePaymentMethod { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string MerchantHashed { get; set; }
    }
}
