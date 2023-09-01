using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Appointments
{
    /// <summary>
    /// Represents a Appointment attribute value
    /// </summary>
    public partial class AppointmentAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int AppointmentAttributeId { get; set; }

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
        /// Gets or sets the Appointment attribute
        /// </summary>
        public virtual AppointmentAttribute AppointmentAttribute { get; set; }
    }
}
