namespace Nop.Core.Domain.Vendors
{
    public partial class MobileAppSetting : BaseEntity
    {
        public int VendorId { get; set; }
        public bool CallToOrderEnabled { get; set; }
        public bool CatalogEnabled { get; set; }
        public decimal ImageAspectRatio { get; set; }
        public bool LoyalityEnabled { get; set; }
        public bool NewTabEnabled { get; set; }
        public bool SalesTabEnabled { get; set; }
        public bool ShopTheLookEnabled { get; set; }
        public bool ShortcutEnabled { get; set; }
        public int TabAnimation { get; set; }
        public bool AboutUsEnabled { get; set; }
        public int ShopTheLookType { get; set; }
        public int HomeTabType { get; set; }
        public bool AppintmentEnable { get; set; }
        public int AppointmentPictureId { get; set; }
    }
}
