using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public partial class SendDevicesFcmModel : BaseNopEntityModel
    {
        public SendDevicesFcmModel()
        {
            Notification = new NotificationModel();
        }

        public int DeviceId { get; set; }

        public NotificationModel Notification { get; set; }
    }
}