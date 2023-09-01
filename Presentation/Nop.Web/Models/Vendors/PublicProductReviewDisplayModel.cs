using Nop.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Web.Models.Vendors
{
    public partial class PublicProductReviewDisplayModel : BaseNopModel
    {
        public PublicProductReviewDisplayModel()
        {
            ProductImageUrl = new Dictionary<int, string>();
        }
        public IList<ProductReviewListModel> ProductReviews { get; set; }
        public IDictionary<int, string> ProductImageUrl { get; set; }
    }
}