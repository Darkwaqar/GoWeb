using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Media
{
    public partial class VideoModel : BaseNopModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }

        public string VideoUrl { get; set; }
    }
}