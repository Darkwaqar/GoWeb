namespace Nop.Core.Domain.Vendors
{
    public partial class VendorBanner : BaseEntity
    {
      
        /// <summary>
        /// Gets or sets the vendor identifier
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int ImageId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }



    }
}
