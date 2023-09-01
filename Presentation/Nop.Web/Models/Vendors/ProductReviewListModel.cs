using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Web.Models.Vendors
{
    public partial class ProductReviewListModel : BaseNopModel
    {
        public int ProductId { get; set; }
        public string ProductSeName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }

    }
}