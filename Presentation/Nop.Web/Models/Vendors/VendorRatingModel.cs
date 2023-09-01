using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Vendors
{
    public partial class VendorRatingModel : BaseNopModel
    {
        public int TotalRatings { get; set; }

        public decimal AverageRating { get; set; }

        public int VendorId { get; set; }

        public string VendorSeName { get; set; }
    }
}
