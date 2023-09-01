using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class CouponListModel : BaseNopModel
    {
        public CouponListModel()
        {
            ActivatedList = new List<SelectListItem>();
            GenerateCouponBulkModel = new GenerateCouponBulkModel();
        }

        [NopResourceDisplayName("Admin.Coupons.List.CouponCode")]
        [AllowHtml]
        public string CouponCode { get; set; }

        [NopResourceDisplayName("Admin.Coupons.List.RecipientName")]
        [AllowHtml]
        public string RecipientName { get; set; }

        [NopResourceDisplayName("Admin.Coupons.List.Activated")]
        public int ActivatedId { get; set; }
        [NopResourceDisplayName("Admin.Coupons.List.Activated")]
        public IList<SelectListItem> ActivatedList { get; set; }


        //copy all product from vendor to vendor
        public GenerateCouponBulkModel GenerateCouponBulkModel { get; set; }
    }
}