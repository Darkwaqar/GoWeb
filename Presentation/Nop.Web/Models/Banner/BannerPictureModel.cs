using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Banner
{
    public partial class BannerPictureModel : BaseNopModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }

        public string URL { get; set; }

        public int VendorId { get; set; }

        public int CategoryId { get; set; }

        public int ProductId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}