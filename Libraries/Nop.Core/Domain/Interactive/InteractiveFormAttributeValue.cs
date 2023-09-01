using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Interactive
{
    /// <summary>
    /// Represents a contact attribute value
    /// </summary>
    public partial class InteractiveFormAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int InteractiveFormAttributeId { get; set; }

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
        /// Gets or sets the contact attribute
        /// </summary>
        public virtual InteractiveFormAttribute InteractiveFormAttribute { get; set; }
    }
}
