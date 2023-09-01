using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using System.Collections.Generic;

namespace Nop.Core.Domain.Interactive
{
    /// <summary>
    /// Represents a contact attribute
    /// </summary>
    public partial class InteractiveFormAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<InteractiveFormAttributeValue> _interactiveFormAttributeValues;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public int InteractiveFormId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SystemName
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the regex validation
        /// </summary>
        public string RegexValidation { get; set; }

        /// <summary>
        /// Gets or sets the css style
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets the css class
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }


        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }


        //validation fields

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        public string ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        public int? ValidationFileMaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value;
            }
        }
        /// <summary>
        /// Gets the checkout attribute values
        /// </summary>
        public virtual ICollection<InteractiveFormAttributeValue> InteractiveFormAttributeValues
        {
            get { return _interactiveFormAttributeValues ?? (_interactiveFormAttributeValues = new List<InteractiveFormAttributeValue>()); }
            protected set { _interactiveFormAttributeValues = value; }
        }
    }
}
