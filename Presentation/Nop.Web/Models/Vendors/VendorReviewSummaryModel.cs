using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Vendors
{
    public partial class VendorReviewSummaryModel : BaseNopModel
    {
        public VendorReviewSummaryModel()
        {
            VendorRatingCounts = new int[5];
        }
        public int TotalVendorReviews { get; set; }
        public int[] VendorRatingCounts { get; set; }
        public string VendorName { get; set; }
        public PublicVendorReviewDisplayModel VendorReviewDisplayModel { get; set; }
    }
}