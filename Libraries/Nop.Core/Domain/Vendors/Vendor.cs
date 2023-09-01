using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor
    /// </summary>
    public partial class Vendor : BaseEntity, ILocalizedEntity, ISlugSupported, IStoreMappingSupported
    {

        #region Vendor

        private ICollection<VendorNote> _vendorNotes;
        private ICollection<VendorBanner> _vendorBanners;
        private ICollection<VendorMappedCategory> _vendorMappedCategories;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }


        #endregion

        #region Advance Option

        public bool IsSmsVerified { get; set; }
        public string SmsVerificationCode { get; set; }
        public int CoverPictureId { get; set; }
        public int mpLogo { get; set; }
        public string ContactDescription { get; set; }
        public string ContactNumber { get; set; }
        public int ShopBanner { get; set; }

        public string TermAndConditionsUrl { get; set; }
        public string ReturnAndRefundUrl { get; set; }
        public string DeliveryPolicyUrl { get; set; }
        public string ShippingInformationUrl { get; set; }
        public string ReturnAndExchangePolicyUrl { get; set; }
        public string PaymentSecurityUrl { get; set; }
        public string FaqUrl { get; set; }
        public bool StandAloneAppAndroid { get; set; }
        public string StandAloneAppPackageIdAndroid { get; set; }
        public bool StandAloneAppIos { get; set; }
        public string StandAloneAppPackageIdIos { get; set; }
        public string StandAloneAppUrlSchemesIos { get; set; }
        public string StandAloneAppIdIos { get; set; }
        public string FacebookAppId { get; set; }


        #endregion

        #region Social Links
        public string Url { get; set; }
        public string AboutUsPageUrl { get; set; }
        public string BlogUrl { get; set; }
        public string FacebookPage { get; set; }
        public string LinkedInPage { get; set; }

        #endregion

        #region Vendor Specifications

        public string ThemeColor { get; set; }
        public bool IsFeatured { get; set; }
        public bool Brand { get; set; }
        public bool Designer { get; set; }
        public bool Emerging { get; set; }
        public bool Stablish { get; set; }
        public bool SheEarns { get; set; }
        public bool TopBrand { get; set; }
        public bool WomenBrand { get; set; }
        public bool MenBrand { get; set; }
        public bool TopShoesBrand { get; set; }

        #endregion

        #region Vendor Mapping

        public string AboutUs { get; set; }
        //public int MapedCatagory { get; set; }
        public bool LimitedToStores { get; set; }


        #endregion

        /// <summary>
        /// Gets or sets vendor notes
        /// </summary>
        public virtual ICollection<VendorNote> VendorNotes
        {
            get { return _vendorNotes ?? (_vendorNotes = new List<VendorNote>()); }
            protected set { _vendorNotes = value; }
        }

        public virtual ICollection<VendorBanner> VendorBanners
        {
            get { return _vendorBanners ?? (_vendorBanners = new List<VendorBanner>()); }
            protected set { _vendorBanners = value; }
        }

        /// <summary>
        /// Gets or sets the collection of VendorMappedCategory
        /// </summary>
        public virtual ICollection<VendorMappedCategory> VendorMappedCategories
        {
            get { return _vendorMappedCategories ?? (_vendorMappedCategories = new List<VendorMappedCategory>()); }
            protected set { _vendorMappedCategories = value; }
        }

        public decimal ShippingCharge { get; set; }
        public decimal CommissionPercentage { get; set; }

        public bool FreeShippingOverXEnabled { get; set; }

        public decimal FreeShippingOverXValue { get; set; }

        public bool FreeShippingOverXIncludingTax { get; set; }
    }


}
