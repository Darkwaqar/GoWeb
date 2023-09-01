using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Web.Models.Vendors
{
    public partial class PublicVendorReviewDisplayModel : BaseNopModel
    {
        public IList<VendorReviewListModel> VendorReviews { get; set; }
    }
}
