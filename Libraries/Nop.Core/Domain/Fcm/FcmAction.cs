using System;

namespace Nop.Core.Domain.Fcm
{
    public partial class FcmAction : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of action 
        /// </summary>
        public string Name { get; set; }


        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the extra of action 
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// Gets or sets the status of FcmActionType 
        /// </summary>
        public FcmActionType FcmActionType { get; set; }
        /// <summary>
        /// Gets or sets the status of action 
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
