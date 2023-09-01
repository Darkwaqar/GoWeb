using Nop.Core;
using Nop.Web.Framework;
using System;

namespace Nop.Plugin.Payments.TelenorEasyPay.Domain
{
    public class EasyPayPaymentStatus : BaseEntity
    {
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.OrderId")]
        public virtual int OrderId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaymentToken")]
        public virtual string PaymentToken { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TokenExpiryDatetime")]
        public virtual string TokenExpiryDatetime { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.StoreId")]
        public virtual int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.StoreName")]
        public virtual string StoreName { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.ResponseCode")]
        public virtual string ResponseCode { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.OrderDatetime")]
        public virtual string OrderDatetime { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaidDatetime")]
        public virtual string PaidDatetime { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionStatus")]
        public virtual string TransactionStatus { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionAmount")]
        public virtual decimal TransactionAmount { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.PaymentMethod")]
        public virtual string PaymentMethod { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.Msisdn")]
        public virtual string Msisdn { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.AccountNumber")]
        public virtual string AccountNumber { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.Description")]
        public virtual string Description { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.TransactionId")]
        public virtual string TransactionId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.UTCTime")]
        public virtual DateTime UTCTime { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TelenorEasyPay.Fields.RawData")]
        public virtual string RawData { get; set; }

    }

}
