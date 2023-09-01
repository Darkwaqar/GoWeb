using Nop.Core.Domain.Media;

namespace Nop.Core.Domain.Banners
{
    /// <summary>
    /// Represents a banner picture mapping
    /// </summary>
    public partial class BannerPicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the banner identifier
        /// </summary>
        public int BannerId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets the banner
        /// </summary>
        public virtual Banner Banner { get; set; }
        /// <summary>
        /// Gets or sets the Url
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Gets or sets the vendro identifier
        /// </summary>
        public int VendorId { get; set; }
        /// <summary>
        /// Gets or sets the cateogory identifier
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public int Width { get; set; }
    }
}
