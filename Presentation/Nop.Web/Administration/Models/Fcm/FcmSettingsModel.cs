using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    public class FcmSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Fcm.Settings.Fields.GoogleMapKey")]
        [AllowHtml]
        public string GoogleMapKey { get; set; }
    }
}