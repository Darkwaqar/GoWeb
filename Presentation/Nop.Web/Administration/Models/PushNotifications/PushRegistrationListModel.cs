using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Admin.Models.PushNotifications
{
    public partial class PushRegistrationListModel :BaseNopModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public bool Allowed { get; set; }

        public string Token { get; set; }

        public DateTime RegisteredOn { get; set; }

        public string CustomerEmail { get; set; }
       
    }
}