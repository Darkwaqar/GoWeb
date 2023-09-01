using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TelenorEasyPay.Models
{
    public class EasyPayIpnRawResult : BaseNopModel
    {
        public string payment_token { get; set; }
        public string token_expiry_datetime { get; set; }
        public string store_name { get; set; }
        public string response_code { get; set; }
        public string order_datetime { get; set; }
        public string order_id { get; set; }
        public string paid_datetime { get; set; }
        public string transaction_status { get; set; }
        public string store_id { get; set; }
        public string transaction_amount { get; set; }
        public string payment_method { get; set; }
        public string msisdn { get; set; }
        public string account_number { get; set; }
        public string description { get; set; }
        public string transaction_id { get; set; }
        
    }

}
