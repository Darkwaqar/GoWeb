using System.Web.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Common;
using Nop.Web.Models.Vendors;
using System.Collections.Generic;
using Nop.Web.Models.Banner;
using Nop.Core.Domain.Banners;
using Nop.Web.Models.Appointment;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the common models factory
    /// </summary>
    public partial interface ICommonModelFactory
    {
        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        LogoModel PrepareLogoModel();

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        LanguageSelectorModel PrepareLanguageSelectorModel();

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>Currency selector model</returns>
        CurrencySelectorModel PrepareCurrencySelectorModel();

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>Tax type selector model</returns>
        TaxTypeSelectorModel PrepareTaxTypeSelectorModel();

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        HeaderLinksModel PrepareHeaderLinksModel();

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        AdminHeaderLinksModel PrepareAdminHeaderLinksModel();

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>Social model</returns>
        SocialModel PrepareSocialModel();

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        FooterModel PrepareFooterModel();

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties, string attributeXml = "");

        /// <summary>
        /// Prepare the appointment us model
        /// </summary>
        /// <param name="model">Appointment us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>appointment model</returns>
        AppointmentModel PrepareAppointmentModel(AppointmentModel model, bool excludeProperties, string attributeXml = "");

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact vendor model</returns>
        ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor,
            bool excludeProperties);

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>Sitemap model</returns>
        SitemapModel PrepareSitemapModel(UrlHelper urlHelper,SitemapPageModel pageModel);

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="url">URL helper</param>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        string PrepareSitemapXml(UrlHelper url, int? id);

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        StoreThemeSelectorModel PrepareStoreThemeSelectorModel();

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        FaviconModel PrepareFaviconModel();

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        string PrepareRobotsTextFile();

        /// <summary>
        /// Prepare the Wife Apply model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <returns>Wife Apply model</returns>
        WifeApplyModel PrepareWifeApplyModel(WifeApplyModel model);


        string ParseContactAttributes(FormCollection form);
        IList<string> GetContactAttributesWarnings(string contactAttributesXml);
        IList<ContactUsModel.ContactAttributeModel> PrepareContactAttributeModel(string selectedContactAttributes);

        /// <summary>
        /// Prepare Banner models
        /// </summary>
        /// <returns>Banner model</returns>
        BannerModel PrepareBannerModel(Banner banner);


        string ParseAppointmentAttributes(FormCollection form);
        IList<string> GetAppointmentAttributesWarnings(string contactAttributesXml);
        IList<AppointmentModel.AppointmentAttributeModel> PrepareAppointmentAttributeModel(string selectedAppointmentAttributes);
    }
}
