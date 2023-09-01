using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Web.Models.Vendors
{
    public partial class PublicPendingReviewDisplayModel : BaseNopModel
    {
        public PublicPendingReviewDisplayModel()
        {
            PendingReviews = new Dictionary<PendingOrderModel, List<PendingReviewModel>>();
        }
        public Dictionary<PendingOrderModel, List<PendingReviewModel>> PendingReviews { get; set; }
        
    }
}