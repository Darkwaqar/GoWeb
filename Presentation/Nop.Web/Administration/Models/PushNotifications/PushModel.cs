using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models.PushNotifications
{
    public partial class PushModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.PushNotifications.Fields.PushTitle")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.PushNotifications.Fields.PushMessageText")]
        public string MessageText { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.PushNotifications.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.PushNotifications.Fields.ClickUrl")]
        public string ClickUrl { get; set; }
    }
}