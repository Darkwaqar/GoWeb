using Nop.Admin.Models.Fcm;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Customers
{
    public partial class CustomerNotificationModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        public NotificationModel Notification { get; set; }
    }
}