using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Vendors
{
    public class MobileAppSettingModel : BaseNopEntityModel
    {
        public MobileAppSettingModel()
        {

            AvailableTabAnimation = new List<SelectListItem>();
            AvailableImageAspectRatio = new List<SelectListItem>();
            AvailableShopTheLookType = new List<SelectListItem>();
            AvailableHomeTabType = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Vendors.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.TabAnimation")]
        public int TabAnimation { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ShopTheLookEnabled")]
        public bool ShopTheLookEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CatalogEnabled")]
        public bool CatalogEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.NewTabEnabled")]
        public bool NewTabEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.SalesTabEnabled")]
        public bool SalesTabEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.LoyalityEnabled")]
        public bool LoyalityEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ImageAspectRatio")]
        public decimal ImageAspectRatio { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CallToOrderEnabled")]
        public bool CallToOrderEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ShortcutEnabled")]
        public bool ShortcutEnabled { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AboutUsEnabled")]
        public bool AboutUsEnabled { get; set; }


        [NopResourceDisplayName("Admin.Vendors.Fields.ShopTheLookType")]
        public int ShopTheLookType { get; set; }



        [NopResourceDisplayName("Admin.Vendors.Fields.HomeTabType")]
        public int HomeTabType { get; set; }


        [NopResourceDisplayName("Admin.Vendors.Fields.AvailableTabAnimation")]
        public IList<SelectListItem> AvailableTabAnimation { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.AvailableImageAspectRatio")]
        public IList<SelectListItem> AvailableImageAspectRatio { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.AvailableShopTheLookType")]
        public IList<SelectListItem> AvailableShopTheLookType { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.HomeTabType")]
        public IList<SelectListItem> AvailableHomeTabType { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.AppintmentEnable")]
        public bool AppintmentEnable { get; set; }
        [NopResourceDisplayName("Admin.Vendors.Fields.AppointmentPictureId")]
        [UIHint("Picture")]
        public int AppointmentPictureId { get; set; }

    }
}