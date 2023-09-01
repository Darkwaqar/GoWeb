using Nop.Core;
using Nop.Web.Framework;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.TelenorEasyPay.Domain
{
    public class EasyPayPaymentStatusList : BaseEntity
    {
        public EasyPayPaymentStatusList()
        {
            AvailableStores=new List<SelectListItem>();
            AvailablePaymentMethod = new List<SelectListItem>();
            AvailableTransactionStatus = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.AccountNumber")]
        public virtual string AccountNumber { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.Msisdn")]
        public virtual string Msisdn { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.OrderId")]
        public virtual int OrderId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaymentMethod")]
        public virtual string PaymentMethod { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaymentToken")]
        public virtual string PaymentToken { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.ResponseCode")]
        public virtual string ResponseCode { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionAmount")]

        public virtual decimal TransactionAmount { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.StoreId")]
        public virtual int StoreId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionId")]
        public virtual string TransactionId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionStatus")]
        public virtual string TransactionStatus { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailablePaymentMethod { get; set; }

        public IList<SelectListItem> AvailableTransactionStatus { get; set; }

    }

}
