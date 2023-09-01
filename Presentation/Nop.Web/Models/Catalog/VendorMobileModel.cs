using System.Collections.Generic;
using Nop.Admin.Models.Vendors;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Vendors;
using Nop.Web.Areas.Mservices.Models.Vendors;

namespace Nop.Web.Models.Catalog
{
    public partial class VendorMobileModel : BaseNopEntityModel
    {
        public VendorMobileModel()
        {
            VendorNotes = new List<string>();
            Banners = new List<VendorMobileBanner>();
            MobileAppSetting = new MobileAppSettingModel();
            SocialLinks = new SocialLinksModel();
            Rating = new VendorRatingModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ThemeColor { get; set; }
        public string ContactDescription { get; set; }
        public string CoverPictureURL { get; set; }
        public string MarketPlaceLogoURL { get; set; }
        public string LogoUrl { get; set; }
        public string AboutUsPageURL { get; set; }
        public string ContactNumber { get; set; }
        public string AboutUsPageUrl { get; set; }
        public string BlogUrl { get; set; }
        public string FacebookPageURL { get; set; }
        public string LinkedInPageURL { get; set; }
        public IList<VendorMobileBanner> Banners { get; set; }
        public MobileAppSettingModel MobileAppSetting { get; set; }
        public SocialLinksModel SocialLinks { get; set; }
        public VendorRatingModel Rating { get; set; }
        public int NumberOfCartItems { get; set; }
        public int NumberOfWishlistItems { get; set; }
        public string TermAndConditionsUrl { get; set; }
        public string ReturnAndRefundUrl { get; set; }
        public string DeliveryPolicyUrl { get; set; }
        public string ShippingInformationUrl { get; set; }
        public string ReturnAndExchangePolicyUrl { get; set; }
        public string PaymentSecurityUrl { get; set; }
        public string FaqUrl { get; set; }
        public bool StandAloneApp { get; set; }
        public string StandAloneAppPackageIdAndroid { get; set; }
        public bool StandAloneAppIos { get; set; }
        public string StandAloneAppPackageIdIos { get; set; }
        public string StandAloneAppUrlSchemesIos { get; set; }
        public string FacebookAppId { get; set; }
        public ICollection<string> VendorNotes { get; set; }
        public List<CategoryModel> FeaturedCategories { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public bool HaveShoptheLook { get; set; }
        public string AboutUs { get; set; }
        public string AppointmentPictureUrl { get;  set; }
    }
}