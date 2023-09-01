
using System;

namespace Nop.Core.Domain.Fcm
{
    public partial class FcmApplication : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of application 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier 
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the package of application
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// Gets or sets the AppKey of application
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// Gets or sets the AppKey of application
        /// </summary>
        public FcmApplicationType FcmApplicationType { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
