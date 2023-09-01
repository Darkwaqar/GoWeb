using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public class SocialLinksMap : NopEntityTypeConfiguration<SocialLinks>
    {
        public SocialLinksMap()
        {
            this.ToTable("SocialLinks");
            this.Property(v => v.Id).IsRequired();
            this.Property(v => v.VendorId).IsRequired();
            this.Property(v => v.FaceboolMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.FaceboolWebURL).IsOptional().HasMaxLength(200);
            this.Property(v => v.InstagramMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.InstagramWebURL).IsOptional().HasMaxLength(200);
            this.Property(v => v.LinkedInMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.LinkedWebURL).IsOptional().HasMaxLength(200);
            this.Property(v => v.TwitterMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.TwitterWebURL).IsOptional().HasMaxLength(200);
            this.Property(v => v.WhatsappMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.WhatsappWebURL).IsOptional().HasMaxLength(200);
            this.Property(v => v.YoutubeMobile).IsOptional().HasMaxLength(200);
            this.Property(v => v.YoutubeWebURL).IsOptional().HasMaxLength(200);
        }
    }
}
