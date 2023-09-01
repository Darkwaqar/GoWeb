using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Vendors
{
    public partial class CustomerReviewSummaryModel : BaseNopModel
    {
        public CustomerReviewSummaryModel()
        {
            ProductRatingCounts = new int[5];
            VendorRatingCounts = new int[5];
        }
        public int TotalProductReviews { get; set; }
        public int TotalVendorReviews { get; set; }
        public int[] ProductRatingCounts { get; set; }
        public int[] VendorRatingCounts { get; set; }
        public ProductReview LastRatedProductReview { get; set; }
        public VendorReview LastRatedVendorReview { get; set; }
        public string CustomerName { get; set; }
        public bool IsRedirection { get; set; }
    }
}