using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Interactive
{
    /// <summary>
    /// Represents a interactive forms
    /// </summary>
    public partial class InteractiveForm : BaseEntity, ILocalizedEntity
    {
        private ICollection<InteractiveFormAttribute> _interactiveFormAttribute;
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }
       
        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }


        public virtual ICollection<InteractiveFormAttribute> InteractiveFormAttributes
        {
            get { return _interactiveFormAttribute ?? (_interactiveFormAttribute = new List<InteractiveFormAttribute>()); }
            protected set { _interactiveFormAttribute = value; }
        }
    }
}
