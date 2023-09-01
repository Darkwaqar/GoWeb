using System;

namespace Nop.Core.Domain.Fcm
{
    public partial class Notification : BaseEntity
    {

        public FcmType FcmType { get; set; }
        public FcmApplicationType FcmApplicationType { get; set; }
        public int ApplicationId { get; set; }
        public int ActionId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int Image { get; set; }
        public string DirectImageLink { get; set; }
        public string GotoLink { get; set; }

        public int Icon { get; set; }

        public string DirectIconLink { get; set; }

        public bool IsReaded { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of notification update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }


        public DateTime? ReadedOnUtc { get; set; }

        public int VendorId { get; set; }
    }
}
