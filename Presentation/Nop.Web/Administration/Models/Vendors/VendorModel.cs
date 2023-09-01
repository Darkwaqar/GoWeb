using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Common;
using Nop.Admin.Validators.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Vendors
{
    [Validator(typeof(VendorValidator))]
    public partial class VendorModel : BaseNopEntityModel, ILocalizedModel<VendorLocalizedModel>
    {

        public VendorModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Address = new AddressModel();

            Locales = new List<VendorLocalizedModel>();
            AssociatedCustomers = new List<AssociatedCustomerInfo>();

            //Extended
            AddBannerModel = new VendorBanner();
            MobileAppSettingModel = new MobileAppSettingModel();
            SocialLinks = new SocialLinksModel();
            PaymentInfo = new VendorPaymentInfoModel();
            SelectedStoreIds = new List<int>();
            SelectedPlatformIds = new List<int>();

            AvailableStores = new List<SelectListItem>();
            AvailablePlatform = new List<SelectListItem>();
            AvailableCategoryTemplates = new List<SelectListItem>();
            AvailableCategories = new List<SelectListItem>();
            AvaliableThemeColor = new List<SelectListItem>();
            SelectedMappedCategoryIds = new List<int>();
        }


        #region Default Model

        [NopResourceDisplayName("Admin.Vendors.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Vendors.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        public AddressModel Address { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        [NopResourceDisplayName("Admin.Vendors.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        public IList<VendorLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AssociatedCustomerEmails")]
        public IList<AssociatedCustomerInfo> AssociatedCustomers { get; set; }


        //vendor notes
        [NopResourceDisplayName("Admin.Vendors.VendorNotes.Fields.Note")]
        [AllowHtml]
        public string AddVendorNoteMessage { get; set; }

        #endregion

        #region Extended Model

        #region Advance Options
        [NopResourceDisplayName("Admin.Vendors.Fields.IsSmsVerified")]
        public bool IsSmsVerified { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CoverPicture")]
        [UIHint("Picture")]
        public int CoverPictureId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MarketPlaceLogo")]
        [UIHint("Picture")]
        public int mpLogo { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ContactDescription")]
        [AllowHtml]
        public string ContactDescription { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ContactNumber")]
        public string ContactNumber { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.TermAndConditionsUrl")]
        public string TermAndConditionsUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ReturnAndRefundUrl")]
        public string ReturnAndRefundUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.DeliveryPolicyUrl")]
        public string DeliveryPolicyUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ShippingInformationUrl")]
        public string ShippingInformationUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ReturnAndExchangePolicyUrl")]
        public string ReturnAndExchangePolicyUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PaymentSecurityUrl")]
        public string PaymentSecurityUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.FaqUrl")]
        public string FaqUrl { get; set; }


        [NopResourceDisplayName("Admin.Vendors.Fields.ShopBanner")]
        [UIHint("Picture")]
        public int ShopBanner { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppAndroid")]
        public bool StandAloneAppAndroid { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppPackageIdAndroid")]
        public string StandAloneAppPackageIdAndroid { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppIos")]
        public bool StandAloneAppIos { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppPackageIdIos")]
        public string StandAloneAppPackageIdIos { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppUrlSchemesIos")]
        public string StandAloneAppUrlSchemesIos { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.StandAloneAppIdIos")]
        public string StandAloneAppIdIos { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.FacebookAppId")]
        public string FacebookAppId { get; set; }

        #endregion

        #region Social Links

        [NopResourceDisplayName("Admin.Vendors.Fields.WebPageUrl")]
        public string Url { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AboutUsPageUrl")]
        public string AboutUsPageUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.BlogUrl")]
        public string BlogUrl { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.FacebookPageLink")]
        public string FacebookPage { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.LinkedinLink")]
        public string LinkedInPage { get; set; }

        #endregion

        #region Vendor Specifications

        

        [NopResourceDisplayName("Admin.Vendors.Fields.IsFeatured")]
        public bool IsFeatured { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.Brand")]
        public bool Brand { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.Designer")]
        public bool Designer { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.Emerging ")]
        public bool Emerging { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.Stablish")]
        public bool Stablish { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.SheEarns")]
        public bool SheEarns { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.TopBrand")]
        public bool TopBrand { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.WomenBrand")]
        public bool WomenBrand { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.MenBrand")]
        public bool MenBrand { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.TopShoesBrand")]
        public bool TopShoesBrand { get; set; }


        [NopResourceDisplayName("Admin.Vendors.Fields.ThemeColor")]
        public string ThemeColor { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.AvalibleThemeColor")]
        public IList<SelectListItem> AvaliableThemeColor { get; set; }

        #endregion

        #region Vendor Mapping

        [NopResourceDisplayName("Admin.Vendors.Fields.AboutUs")]
        [AllowHtml]
        public string AboutUs { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        [NopResourceDisplayName("Admin.vendors.Fields.Parent")]
        public int ParentCategoryId { get; set; }

        //Category
        [NopResourceDisplayName("Admin.Categories.Fields.CategoryTemplate")]
        public int CategoryTemplateId { get; set; }
        public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

        //store
        public bool LimitedToStores { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }

        //PlatForm mapping
        [NopResourceDisplayName("Admin.Vendors.Fields.LimitedToPlatform")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedPlatformIds { get; set; }

        public IList<SelectListItem> AvailablePlatform { get; set; }

        #endregion
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        //banners
        public VendorBanner AddBannerModel { get; set; }

        //vendor Platform
        [NopResourceDisplayName("Admin.Vendors.VendorPlatForm.Fields.Platform")]
        [AllowHtml]
        public string AddVendorPlatformMessage { get; set; }
        public MobileAppSettingModel MobileAppSettingModel { get; set; }

        public SocialLinksModel SocialLinks { get; set; }

        public VendorPaymentInfoModel PaymentInfo { get; set; }

        public decimal ShippingCharge { get; set; }
        public decimal CommissionPercentage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax")]
        public bool FreeShippingOverXIncludingTax { get; set; }

        //categories
        [NopResourceDisplayName("Admin.Vendor.Fields.MappedCategories")]
        [UIHint("MultiSelect")]
        public List<int> SelectedMappedCategoryIds { get;  set; }

        #endregion

        #region Nested classes

        #region Default-Classes

        public class AssociatedCustomerInfo : BaseNopEntityModel
        {
            public string Email { get; set; }
        }

        public partial class VendorNote : BaseNopEntityModel
        {
            public int VendorId { get; set; }
            [NopResourceDisplayName("Admin.Vendors.VendorNotes.Fields.Note")]
            public string Note { get; set; }
            [NopResourceDisplayName("Admin.Vendors.VendorNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        #endregion

        #region Extended Classes

        public partial class VendorBanner : BaseNopEntityModel
        {
            public int VendorId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Vendors.Banner.Fields.Picture")]
            public int ImageId { get; set; }

            [NopResourceDisplayName("Admin.Vendors.Banner.Fields.Url")]
            public string Url { get; set; }

            [NopResourceDisplayName("Admin.Vendors.Banner.Fields.Title")]
            public string Title { get; set; }

            [NopResourceDisplayName("Admin.Vendors.Banner.Fields.Description")]
            public string Description { get; set; }

            [NopResourceDisplayName("Admin.Vendors.Banner.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }




        #endregion

        #endregion

    }

    public partial class VendorLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }

}