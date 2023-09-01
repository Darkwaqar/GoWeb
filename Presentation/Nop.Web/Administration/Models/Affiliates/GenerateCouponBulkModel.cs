using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class GenerateCouponBulkModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Coupons.Fields.NumberOfCoupon")]
        public int NumberOfCoupon { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.CouponType")]
        public int CouponTypeId { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.Amount")]
        public decimal Amount { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.SenderName")]
        [AllowHtml]
        public string SenderName { get; set; }

        [NopResourceDisplayName("Admin.Coupons.Fields.Message")]
        [AllowHtml]
        public string Message { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

    }
}