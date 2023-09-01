using System;
using System.Collections.Generic;

namespace Nop.Web.Areas.Mservices.Models.Product
{
    public partial class ReviewSummaryModel
    {
        public ReviewSummaryModel()
        {
            RatingCounts = new int[5];
            Reviews = new List<ReviewListModel>();
        }
        public int TotalReviews { get; set; }
        public int[] RatingCounts { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public IList<ReviewListModel> Reviews { get; set; }
    }

    public partial class ReviewListModel
    {
        public int Id { get; set; }
        public string ProductImage { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}