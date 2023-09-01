using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Vendors
{
    public class SocialLinksModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Vendors.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.InstagramMobile")]
        public string InstagramMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.InstagramWebURL")]
        public string InstagramWebURL { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.FaceboolMobile")]
        public string FaceboolMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.FaceboolWebURL")]
        public string FaceboolWebURL { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.TwitterMobile")]
        public string TwitterMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.TwitterWebURL")]
        public string TwitterWebURL { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.LinkedInMobile")]
        public string LinkedInMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.LinkedWebURL")]
        public string LinkedWebURL { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.YoutubeMobile")]
        public string YoutubeMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.YoutubeWebURL")]
        public string YoutubeWebURL { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.WhatsappMobile")]
        public string WhatsappMobile { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.WhatsappWebURL")]
        public string WhatsappWebURL { get; set; }

    }
}