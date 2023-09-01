using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Web.Models.Vendors
{
    public partial class VendorReviewListModel : BaseNopModel
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Vendor")]
        public string VendorName { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Customer")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Product")]
        public string ProductName { get; set; }

        [NopResourceDisplayName("Vendor.Fields.IsApproved")]
        public bool IsApproved { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Vendor.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Rating")]
        public int Rating { get; set; }

        [NopResourceDisplayName("Vendor.Fields.HelpfulnessYesTotal")]
        public int HelpfulnessYesTotal { get; set; }

        [NopResourceDisplayName("Vendor.Fields.HelpfulnessNoTotal")]
        public int HelpfulnessNoTotal { get; set; }

        [NopResourceDisplayName("Vendor.Fields.CreatedOnUTC")]
        public DateTime CreatedOnUTC { get; set; }

        [NopResourceDisplayName("Vendor.Fields.Order")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Vendor.Fields.CertifiedBuyerReview")]
        public bool CertifiedBuyerReview { get; set; }

        [NopResourceDisplayName("Vendor.Fields.DisplayCertifiedBadge")]
        public bool DisplayCertifiedBadge { get; set; }

        public string VendorSeName { get; set; }

        public string ProductSeName { get; set; }

        public string VendorImageUrl { get; set; }

        public string ProductImageUrl { get; set; }
        
    }
}