using System;

namespace Nop.Core.Domain.PushNotifications
{
    public partial class PushMessage : BaseEntity
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime SentOn { get; set; }

        public int NumberOfReceivers { get; set; }
    }
}
