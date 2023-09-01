using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.TelenorEasyPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvaliblePaymentMethod = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.StoreId")]
        public string StoreId { get; set; }
        public bool StoreId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.ExpiryDate")]
        public double ExpiryDate { get; set; }
        public bool ExpiryDate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.IsMerchantTokenExpiry")]
        public bool IsMerchantTokenExpiry { get; set; }
        public bool IsMerchantTokenExpiry_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.IsSandbox")]
        public bool IsSandbox { get; set; }
        public bool IsSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.AutoRedirect")]
        public bool AutoRedirect { get; set; }
        public bool AutoRedirect_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaymentMethod")]

        public string SelectedPaymentMethod { get; set; }
        public IList<SelectListItem> AvaliblePaymentMethod { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.MerchantHashed")]
        public string MerchantHashed { get; set; }
        public bool MerchantHashed_OverrideForStore { get; set; }
        public bool SelectedPaymentMethod_OverrideForStore { get; set; }
    }
}