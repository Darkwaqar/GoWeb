using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Admin.Models.Settings
{
    public partial class GeneralCommonSettingsModel : BaseNopModel
    {
        public GeneralCommonSettingsModel()
        {
            StoreInformationSettings = new StoreInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            CaptchaSettings = new CaptchaSettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel();
            FullTextSettings = new FullTextSettingsModel();
            DisplayDefaultMenuItemSettings = new DisplayDefaultMenuItemSettingsModel();
            DisplayDefaultFooterItemSettings = new DisplayDefaultFooterItemSettingsModel();
        }

        public StoreInformationSettingsModel StoreInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public CaptchaSettingsModel CaptchaSettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }
        public DisplayDefaultMenuItemSettingsModel DisplayDefaultMenuItemSettings { get; set; }
        public DisplayDefaultFooterItemSettingsModel DisplayDefaultFooterItemSettings { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }


        #region Nested classes

        public partial class StoreInformationSettingsModel : BaseNopModel
        {
            public StoreInformationSettingsModel()
            {
                this.AvailableStoreThemes = new List<ThemeConfigurationModel>();
            }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreClosed")]
            public bool StoreClosed { get; set; }
            public bool StoreClosed_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme")]
            [AllowHtml]
            public string DefaultStoreTheme { get; set; }
            public bool DefaultStoreTheme_OverrideForStore { get; set; }
            public IList<ThemeConfigurationModel> AvailableStoreThemes { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowCustomerToSelectTheme")]
            public bool AllowCustomerToSelectTheme { get; set; }
            public bool AllowCustomerToSelectTheme_OverrideForStore { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Logo")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }


            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayEuCookieLawWarning")]
            public bool DisplayEuCookieLawWarning { get; set; }
            public bool DisplayEuCookieLawWarning_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookLink")]
            public string FacebookLink { get; set; }
            public bool FacebookLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterLink")]
            public string TwitterLink { get; set; }
            public bool TwitterLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.YoutubeLink")]
            public string YoutubeLink { get; set; }
            public bool YoutubeLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GooglePlusLink")]
            public string GooglePlusLink { get; set; }
            public bool GooglePlusLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InstagramLink")]
            public string InstagramLink { get; set; }
            public bool InstagramLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LinkedInLink")]
            public string LinkedInLink { get; set; }
            public bool LinkedInLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PinterestLink")]
            public string PinterestLink { get; set; }
            public bool PinterestLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PlayStoreLink")]
            public string PlayStoreLink { get; set; }
            public bool PlayStoreLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AppStoreLink")]
            public string AppStoreLink { get; set; }
            public bool AppStoreLink_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreInDatabaseContactUsForm")]
            public bool StoreInDatabaseContactUsForm { get; set; }
            public bool StoreInDatabaseContactUsForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreInDatabaseVendorAccountForm")]
            public bool StoreInDatabaseVendorAccountForm { get; set; }
            public bool StoreInDatabaseVendorAccountForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.StoreInDatabaseAppointmentForm")]
            public bool StoreInDatabaseAppointmentForm { get; set; }
            public bool StoreInDatabaseAppointmentForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnContactUsForm")]
            public bool SubjectFieldOnContactUsForm { get; set; }
            public bool SubjectFieldOnContactUsForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SubjectFieldOnAppointmentForm")]
            public bool SubjectFieldOnAppointmentForm { get; set; }
            public bool SubjectFieldOnAppointmentForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseSystemEmailForContactUsForm")]
            public bool UseSystemEmailForContactUsForm { get; set; }
            public bool UseSystemEmailForContactUsForm_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
            public bool SitemapEnabled { get; set; }
            public bool SitemapEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeBlogPosts")]
            public bool SitemapIncludeBlogPosts { get; set; }
            public bool SitemapIncludeBlogPosts_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories")]
            public bool SitemapIncludeCategories { get; set; }
            public bool SitemapIncludeCategories_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeManufacturers")]
            public bool SitemapIncludeManufacturers { get; set; }
            public bool SitemapIncludeManufacturers_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews")]
            public bool SitemapIncludeNews { get; set; }
            public bool SitemapIncludeNews_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProducts")]
            public bool SitemapIncludeProducts { get; set; }
            public bool SitemapIncludeProducts_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeProductTags")]
            public bool SitemapIncludeProductTags { get; set; }
            public bool SitemapIncludeProductTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTopics")]
            public bool SitemapIncludeTopics { get; set; }
            public bool SitemapIncludeTopics_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapPageSize")]
            public int SitemapPageSize { get; set; }
            public bool SitemapPageSize_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PopupForTermsOfServiceLinks")]
            public bool PopupForTermsOfServiceLinks { get; set; }
            public bool PopupForTermsOfServiceLinks_OverrideForStore { get; set; }

            #region Nested classes

            public partial class ThemeConfigurationModel
            {
                public string ThemeName { get; set; }
                public string ThemeTitle { get; set; }
                public string PreviewImageUrl { get; set; }
                public string PreviewText { get; set; }
                public bool SupportRtl { get; set; }
                public bool Selected { get; set; }
            }

            #endregion
        }

        public partial class SeoSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            [NoTrim]
            public string PageTitleSeparator { get; set; }
            public bool PageTitleSeparator_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public int PageTitleSeoAdjustment { get; set; }
            public bool PageTitleSeoAdjustment_OverrideForStore { get; set; }
            public SelectList PageTitleSeoAdjustmentValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }
            public bool DefaultTitle_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }
            public bool DefaultMetaKeywords_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }
            public bool DefaultMetaDescription_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GenerateProductMetaDescription")]
            [AllowHtml]
            public bool GenerateProductMetaDescription { get; set; }
            public bool GenerateProductMetaDescription_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }
            public bool ConvertNonWesternChars_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }
            public bool CanonicalUrlsEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.WwwRequirement")]
            public int WwwRequirement { get; set; }
            public bool WwwRequirement_OverrideForStore { get; set; }
            public SelectList WwwRequirementValues { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableJsBundling")]
            public bool EnableJsBundling { get; set; }
            public bool EnableJsBundling_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling")]
            public bool EnableCssBundling { get; set; }
            public bool EnableCssBundling_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TwitterMetaTags")]
            public bool TwitterMetaTags { get; set; }
            public bool TwitterMetaTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.OpenGraphMetaTags")]
            public bool OpenGraphMetaTags { get; set; }
            public bool OpenGraphMetaTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CustomHeadTags")]
            [AllowHtml]
            public string CustomHeadTags { get; set; }
            public bool CustomHeadTags_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AppPackageIdAndroid")]
            public string AppPackageIdAndroid { get; set; }
            public bool AppPackageIdAndroid_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AppPackageIdIos")]
            public string AppPackageIdIos { get; set; }
            public bool AppPackageIdIos_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AppUrlSchemesIos")]
            public string AppUrlSchemesIos { get; set; }
            public bool AppUrlSchemesIos_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AppIdIos")]
            public string AppIdIos { get; set; }
            public bool AppIdIos_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FacebookAppId")]
            public string FacebookAppId { get; set; }
            public bool FacebookAppId_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.MobileAppPictureId")]
            [UIHint("Picture")]
            public int MobileAppPictureId { get; set; }
            public bool MobileAppPictureId_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.Microdata")]
            public bool MicrodataEnabled { get; set; }
            public bool MicrodataEnabled_OverrideForStore { get; set; }
        }

        public partial class SecuritySettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ForceSslForAllPages")]
            public bool ForceSslForAllPages { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea")]
            public bool EnableXsrfProtectionForAdminArea { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore")]
            public bool EnableXsrfProtectionForPublicStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
            public bool HoneypotEnabled { get; set; }
        }

        public partial class CaptchaSettingsModel : BaseNopModel
        {
            public CaptchaSettingsModel()
            {
                this.AvailableReCaptchaVersions = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
            public bool Enabled { get; set; }
            public bool Enabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
            public bool ShowOnLoginPage { get; set; }
            public bool ShowOnLoginPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
            public bool ShowOnRegistrationPage { get; set; }
            public bool ShowOnRegistrationPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ShowOnCheckGiftCardBalancePage")]
            public bool ShowOnCheckGiftCardBalancePage { get; set; }
            public bool ShowOnCheckGiftCardBalancePage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
            public bool ShowOnContactUsPage { get; set; }
            public bool ShowOnContactUsPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
            public bool ShowOnEmailWishlistToFriendPage { get; set; }
            public bool ShowOnEmailWishlistToFriendPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage")]
            public bool ShowOnEmailProductToFriendPage { get; set; }
            public bool ShowOnEmailProductToFriendPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
            public bool ShowOnBlogCommentPage { get; set; }
            public bool ShowOnBlogCommentPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
            public bool ShowOnNewsCommentPage { get; set; }
            public bool ShowOnNewsCommentPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductReviewPage")]
            public bool ShowOnProductReviewPage { get; set; }
            public bool ShowOnProductReviewPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnApplyVendorPage")]
            public bool ShowOnApplyVendorPage { get; set; }
            public bool ShowOnApplyVendorPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductAppointmentPage")]
            public bool ShowOnProductAppointmentPage { get; set; }
            public bool ShowOnProductAppointmentPage_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
            [AllowHtml]
            public string ReCaptchaPublicKey { get; set; }
            public bool ReCaptchaPublicKey_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
            [AllowHtml]
            public string ReCaptchaPrivateKey { get; set; }
            public bool ReCaptchaPrivateKey_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion")]
            public ReCaptchaVersion ReCaptchaVersion { get; set; }
            public bool ReCaptchaVersion_OverrideForStore { get; set; }

            public IList<SelectListItem> AvailableReCaptchaVersions { get; set; }
        }

        public partial class PdfSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
            public bool LetterPageSizeEnabled { get; set; }
            public bool LetterPageSizeEnabled_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
            [UIHint("Picture")]
            public int LogoPictureId { get; set; }
            public bool LogoPictureId_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisablePdfInvoicesForPendingOrders")]
            public bool DisablePdfInvoicesForPendingOrders { get; set; }
            public bool DisablePdfInvoicesForPendingOrders_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1")]
            [AllowHtml]
            public string InvoiceFooterTextColumn1 { get; set; }
            public bool InvoiceFooterTextColumn1_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2")]
            [AllowHtml]
            public string InvoiceFooterTextColumn2 { get; set; }
            public bool InvoiceFooterTextColumn2_OverrideForStore { get; set; }

        }

        public partial class LocalizationSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage")]
            public bool AutomaticallyDetectLanguage { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocalizedPropertiesOnStartup")]
            public bool LoadAllLocalizedPropertiesOnStartup { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllUrlRecordsOnStartup")]
            public bool LoadAllUrlRecordsOnStartup { get; set; }
        }

        public partial class FullTextSettingsModel : BaseNopModel
        {
            public bool Supported { get; set; }

            public bool Enabled { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public int SearchMode { get; set; }
            public SelectList SearchModeValues { get; set; }
        }

        public partial class DisplayDefaultMenuItemSettingsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem")]
            public bool DisplayHomePageMenuItem { get; set; }
            public bool DisplayHomePageMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem")]
            public bool DisplayNewProductsMenuItem { get; set; }
            public bool DisplayNewProductsMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem")]
            public bool DisplayProductSearchMenuItem { get; set; }
            public bool DisplayProductSearchMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem")]
            public bool DisplayCustomerInfoMenuItem { get; set; }
            public bool DisplayCustomerInfoMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
            public bool DisplayBlogMenuItem { get; set; }
            public bool DisplayBlogMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
            public bool DisplayForumsMenuItem { get; set; }
            public bool DisplayForumsMenuItem_OverrideForStore { get; set; }
            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
            public bool DisplayContactUsMenuItem { get; set; }
            public bool DisplayContactUsMenuItem_OverrideForStore { get; set; }
        }

        public partial class DisplayDefaultFooterItemSettingsModel : BaseNopModel
        {
            #region Properties

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem")]
            public bool DisplaySitemapFooterItem { get; set; }
            public bool DisplaySitemapFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem")]
            public bool DisplayContactUsFooterItem { get; set; }
            public bool DisplayContactUsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayProductSearchFooterItem")]
            public bool DisplayProductSearchFooterItem { get; set; }
            public bool DisplayProductSearchFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem")]
            public bool DisplayNewsFooterItem { get; set; }
            public bool DisplayNewsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem")]
            public bool DisplayBlogFooterItem { get; set; }
            public bool DisplayBlogFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem")]
            public bool DisplayForumsFooterItem { get; set; }
            public bool DisplayForumsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedProductsFooterItem")]
            public bool DisplayRecentlyViewedProductsFooterItem { get; set; }
            public bool DisplayRecentlyViewedProductsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareProductsFooterItem")]
            public bool DisplayCompareProductsFooterItem { get; set; }
            public bool DisplayCompareProductsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewProductsFooterItem")]
            public bool DisplayNewProductsFooterItem { get; set; }
            public bool DisplayNewProductsFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerInfoFooterItem")]
            public bool DisplayCustomerInfoFooterItem { get; set; }
            public bool DisplayCustomerInfoFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerOrdersFooterItem")]
            public bool DisplayCustomerOrdersFooterItem { get; set; }
            public bool DisplayCustomerOrdersFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCustomerAddressesFooterItem")]
            public bool DisplayCustomerAddressesFooterItem { get; set; }
            public bool DisplayCustomerAddressesFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem")]
            public bool DisplayShoppingCartFooterItem { get; set; }
            public bool DisplayShoppingCartFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem")]
            public bool DisplayWishlistFooterItem { get; set; }
            public bool DisplayWishlistFooterItem_OverrideForStore { get; set; }

            [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem")]
            public bool DisplayApplyVendorAccountFooterItem { get; set; }
            public bool DisplayApplyVendorAccountFooterItem_OverrideForStore { get; set; }

            #endregion
        }

        #endregion
    }
}