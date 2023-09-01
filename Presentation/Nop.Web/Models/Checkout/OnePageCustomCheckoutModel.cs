using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public partial class OnePageCustomCheckoutModel : BaseNopModel
    {
        public OnePageCustomCheckoutModel()
        {
            JsonRequest = true;
        }

        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public string BillingAddressModel { get; set; }
        public string ShippingAddressModel { get; set; }
        public string ShippingMethodModel { get; set; }
        public string PaymentMethodModel { get; set; }
        public string PaymenInfoModel { get; set; }
        public string ConfirmOrderModel { get; set; }
        public bool JsonRequest { get; set; }
        public bool PaymentRequired { get; set; }
    }
}