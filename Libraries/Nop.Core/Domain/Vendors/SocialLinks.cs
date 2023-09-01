namespace Nop.Core.Domain.Vendors
{
    public partial class SocialLinks : BaseEntity
    {
        public int VendorId { get; set; }

        public string InstagramMobile { get; set; }

        public string InstagramWebURL { get; set; }

        public string FaceboolMobile { get; set; }

        public string FaceboolWebURL { get; set; }

        public string TwitterMobile { get; set; }

        public string TwitterWebURL { get; set; }

        public string LinkedInMobile { get; set; }

        public string LinkedWebURL { get; set; }

        public string YoutubeMobile { get; set; }

        public string YoutubeWebURL { get; set; }

        public string WhatsappMobile { get; set; }

        public string WhatsappWebURL { get; set; }
    }
}
