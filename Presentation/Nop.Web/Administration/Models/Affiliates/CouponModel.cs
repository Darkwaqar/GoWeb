using System;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class CouponModel: BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Coupons.Fields.CouponType")]
        public int CouponTypeId { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.Amount")]
        public decimal Amount { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.Amount")]
        public string AmountStr { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.RemainingAmount")]
        public string RemainingAmountStr { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.IsCouponActivated")]
        public bool IsCouponActivated { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.CouponCouponCode")]
        [AllowHtml]
        public string CouponCouponCode { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.RecipientName")]
        [AllowHtml]
        public string RecipientName { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.RecipientEmail")]
        [AllowHtml]
        public string RecipientEmail { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.SenderName")]
        [AllowHtml]
        public string SenderName { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.SenderEmail")]
        [AllowHtml]
        public string SenderEmail { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.IsRecipientNotified")]
        public bool IsRecipientNotified { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #region Nested classes

        public partial class CouponUsageHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Coupons.History.UsedValue")]
            public string UsedValue { get; set; }
            
            public int AffiliateId { get; set; }

            [NopResourceDisplayName("Admin.Coupons.History.CreatedOn")]
            public DateTime CreatedOn { get; set; }

            [NopResourceDisplayName("Admin.Coupons.History.AffiliateName")]
            public string AffiliateName { get; set; }
            [NopResourceDisplayName("Admin.Coupons.History.FromSender")]
            public string FromSender { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.ToRecipient")]
            public string ToRecipient { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.FromCity")]
            public string FromCity { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.FromZip")]
            public string FromZip { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.FromState")]
            public string FromState { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.FromCountry")]
            public string FromCountry { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.ToCity")]
            public string ToCity { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.ToState")]
            public string ToState { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.ToZip")]
            public string ToZip { get;  set; }
            [NopResourceDisplayName("Admin.Coupons.History.ToCountry")]
            public string ToCountry { get;  set; }
        }

        #endregion
    }
}