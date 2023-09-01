using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value
    /// </summary>
    public partial class VendorAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int VendorAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the vendor attribute
        /// </summary>
        public virtual VendorAttribute VendorAttribute { get; set; }
    }
}
