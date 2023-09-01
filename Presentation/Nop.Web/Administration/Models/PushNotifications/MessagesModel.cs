using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.PushNotifications
{
    public partial class MessagesModel :BaseNopModel 
    {
        public int Allowed { get; set; }

        public int Denied { get; set; }
    }
}