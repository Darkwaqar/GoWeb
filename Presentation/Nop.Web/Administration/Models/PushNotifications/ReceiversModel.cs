using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.PushNotifications
{
    public partial class ReceiversModel :BaseNopModel
    {
        public int Allowed { get; set; }

        public int Denied { get; set; }
    }
}