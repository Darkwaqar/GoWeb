using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Admin.Models.Vendors
{
    public class VendorReviewModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Vendors.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ProductId")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Rating")]
        public int Rating { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.HelpfulnessYesTotal")]
        public int HelpfulnessYesTotal { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.HelpfulnessNoTotal")]
        public int HelpfulnessNoTotal { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.CreatedOnUTC")]
        public DateTime CreatedOnUTC { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ProductName")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CertifiedBuyerReview")]
        public bool CertifiedBuyerReview { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.DisplayCertifiedBadge")]
        public bool DisplayCertifiedBadge { get; set; }
    }
}