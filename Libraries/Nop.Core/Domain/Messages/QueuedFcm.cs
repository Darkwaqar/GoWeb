
using System;
using Nop.Core.Domain.Fcm;

namespace Nop.Core.Domain.Messages
{
    public partial class QueuedFcm : BaseEntity
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int PriorityId { get; set; }

        /// <summary>
        /// Gets or sets the From property (email address)
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the DeviceId property
        /// </summary>
        public string DeviceId { get; set; }


        /// <summary>
        /// Gets or sets the Title property
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// Gets or sets the Message property
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Gets or sets the Image property
        /// </summary>
        public string Image { get; set; }


        /// <summary>
        /// Gets or sets the Action property
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the Package property
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// Gets or sets the AppKey property
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time in UTC before which this email should not be sent
        /// </summary>
        public DateTime? DontSendBeforeDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }


        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public QueuedEmailPriority Priority
        {
            get
            {
                return (QueuedEmailPriority)this.PriorityId;
            }
            set
            {
                this.PriorityId = (int)value;
            }
        }

        public string GotoLink { get; set; }

        public int Intent { get; set; }

        public FcmType FcmType { get; set; }

        public FcmApplicationType FcmApplicationType { get; set; }

        public string Icon { get; set; }
        public int DevicePId { get; set; }
        public int VendorId { get; set; }
        public int StoreId { get; set; }
    }
}
