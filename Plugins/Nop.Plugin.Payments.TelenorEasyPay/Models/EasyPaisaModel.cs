using Nop.Web.Framework.Mvc;
namespace Nop.Plugin.Payments.TelenorEasyPay.Models
{
    public class EasyPaisaModel :BaseNopModel
    {
        public string Amount { get; set; }
        public string AutoRedirect { get; set; }
        public string EmailAddr { get; set; }
        public string StoreId { get; set; }
        public string ExpiryDate { get; set; }
        public string MobileNum { get; set; }
        public string PaymentMethod { get; set; }
        public string PostBackURL { get; set; }
        public string OrderRefNum { get; set; }
        public string MerchantHashedReq { get; set; }
        public string PostUrl { get; set; }

    }
}
