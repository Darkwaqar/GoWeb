using System;

namespace Nop.Core.Domain.PushNotifications
{
    public partial class PushRegistration : BaseEntity
    {
        public int CustomerId { get; set; }

        public bool Allowed { get; set; }

        public string Token { get; set; }

        public DateTime RegisteredOn { get; set; }
    }
}
