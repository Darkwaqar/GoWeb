using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class MobileAppSettingMap : NopEntityTypeConfiguration<MobileAppSetting>
    {
        public MobileAppSettingMap()
        {
            this.ToTable("MobileAppSetting");
            this.HasKey(v => v.Id);
            this.Property(v => v.VendorId);
            this.Property(v => v.CallToOrderEnabled);
            this.Property(v => v.CatalogEnabled);
            this.Property(v => v.ImageAspectRatio);
            this.Property(v => v.LoyalityEnabled);
            this.Property(v => v.NewTabEnabled);
            this.Property(v => v.SalesTabEnabled);
            this.Property(v => v.ShopTheLookEnabled);
            this.Property(v => v.ShortcutEnabled);
            this.Property(v => v.TabAnimation);
            this.Property(v => v.AboutUsEnabled);
            this.Property(v => v.ShopTheLookType);
        }
    }
}
