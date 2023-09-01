using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Localization;
using Nop.Admin.Validators.Messages;
using System.Web.Mvc;

namespace Nop.Admin.Models.Banner
{
    [Validator(typeof(BannerValidator))]
    public partial class BannerModel : BaseNopEntityModel, ILocalizedModel<BannerLocalizedModel>
    {
        public BannerModel()
        {
            Locales = new List<BannerLocalizedModel>();
            BannerPictureModels = new List<BannerPictureModel>();
            AddPictureModel = new BannerPictureModel();
            AvailableVendors = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        public IList<BannerLocalizedModel> Locales { get; set; }

        //pictures
        public BannerPictureModel AddPictureModel { get; set; }
        public IList<BannerPictureModel> BannerPictureModels { get; set; }


        //vendors
        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Vendor")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        //vendor
        public bool IsLoggedInAsVendor { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public partial class BannerPictureModel : BaseNopEntityModel
        {
            public int BannerId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.OverrideAltAttribute")]
            [AllowHtml]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.OverrideTitleAttribute")]
            [AllowHtml]
            public string OverrideTitleAttribute { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.Url")]
            [AllowHtml]
            public string URL { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.VendorId")]
            public int VendorId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.CategoryId")]
            public int CategoryId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.ProductId")]
            public int ProductId { get; set; }

            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.Height")]
            public int Height { get; set; }
            [NopResourceDisplayName("Admin.Promotions.Banners.Pictures.Fields.Width")]
            public int Width { get; set; }
        }
    }

    public partial class BannerLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Banners.Fields.Body")]

        public string Body { get; set; }

    }

   
}