using Nop.Core.Domain.Fcm;
using System;

namespace Nop.Web.Areas.Mservices.Models.Customer
{
    public class NotificationMobileModel
    {
        public int Id { get; set; }
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
        public string VendorName { get; set; }
        public string VendorImage { get; set; }


        /// <summary>
        /// Gets or sets the extra of action 
        /// </summary>
        public string Extra { get; set; }
        public FcmActionType FcmActionType { get;  set; }
    }
}