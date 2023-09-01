using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;
using Nop.Web.Models.Vendors;
using System.Text;
using System.Web;
using Nop.Services.Media;
using Nop.Services.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Services.Seo;
using System.Linq;
using Nop.Core.Domain.Seo;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using System.IO;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using System.Text.RegularExpressions;
using Nop.Web.Models.Appointment;
using Nop.Services.Appointments;

namespace Nop.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IThemeContext _themeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly IWorkflowMessageService _workflowMessageService;

        private readonly TaxSettings _taxSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IContactAttributeParser _contactAttributeParser;
        private readonly IContactAttributeService _contactAttributeService;
        private readonly IContactAttributeFormatter _contactAttributeFormatter;

        private readonly IAppointmentAttributeParser _appointmentAttributeParser;
        private readonly IAppointmentAttributeService _appointmentAttributeService;
        private readonly IAppointmentAttributeFormatter _appointmentAttributeFormatter;

        private readonly IPopupService _popupService;
        private readonly IInteractiveFormService _interactiveFormService;


        #endregion

        #region Constructors

        public CommonController(ICommonModelFactory commonModelFactory,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            IThemeContext themeContext,
            IGenericAttributeService genericAttributeService,
            ICustomerActivityService customerActivityService,
            IVendorService vendorService,
            IWorkflowMessageService workflowMessageService,
            TaxSettings taxSettings,
            StoreInformationSettings storeInformationSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            VendorSettings vendorSettings,
            IPictureService pictureService,
            IProductService productService,
            IProductModelFactory productModelFactory,
            ISettingService settingService,
            IStoreService storeService,
            IContactAttributeParser contactAttributeParser,
            IContactAttributeService contactAttributeService,
            IContactAttributeFormatter contactAttributeFormatter,
            IAppointmentAttributeParser appointmentAttributeParser,
            IAppointmentAttributeService appointmentAttributeService,
            IAppointmentAttributeFormatter appointmentAttributeFormatter,
            IPopupService popupService,
            IInteractiveFormService interactiveFormService)
        {
            this._commonModelFactory = commonModelFactory;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._themeContext = themeContext;
            this._genericAttributeService = genericAttributeService;
            this._customerActivityService = customerActivityService;
            this._vendorService = vendorService;
            this._workflowMessageService = workflowMessageService;

            this._taxSettings = taxSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._vendorSettings = vendorSettings;
            this._pictureService = pictureService;
            this._productService = productService;
            this._productModelFactory = productModelFactory;
            this._settingService = settingService;
            this._storeService = storeService;
            this._contactAttributeParser = contactAttributeParser;
            this._contactAttributeService = contactAttributeService;
            this._contactAttributeFormatter = contactAttributeFormatter;

            this._appointmentAttributeParser = appointmentAttributeParser;
            this._appointmentAttributeService = appointmentAttributeService;
            this._appointmentAttributeFormatter = appointmentAttributeFormatter;

            this._popupService = popupService;
            this._interactiveFormService = interactiveFormService;
        }

        #endregion

        #region Methods

        //page not found
        public virtual ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;
            this.Response.ContentType = "text/html";

            return View();
        }

        //logo
        [ChildActionOnly]
        public virtual ActionResult Logo()
        {
            var model = _commonModelFactory.PrepareLogoModel();
            return PartialView(model);
        }

        //language
        [ChildActionOnly]
        public virtual ActionResult LanguageSelector()
        {
            var model = _commonModelFactory.PrepareLanguageSelectorModel();

            if (model.AvailableLanguages.Count == 1)
                Content("");

            return PartialView(model);
        }
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //language part in URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Request.ApplicationPath;
                if (returnUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //already localized URL
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                }
                returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
            }
            return Redirect(returnUrl);
        }

        //currency
        [ChildActionOnly]
        public virtual ActionResult CurrencySelector()
        {
            var model = _commonModelFactory.PrepareCurrencySelectorModel();
            if (model.AvailableCurrencies.Count == 1)
                Content("");

            return PartialView(model);
        }
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SetCurrency(int customerCurrency, string returnUrl = "")
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //tax type
        [ChildActionOnly]
        public virtual ActionResult TaxTypeSelector()
        {
            if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                return Content("");

            var model = _commonModelFactory.PrepareTaxTypeSelectorModel();
            return PartialView(model);
        }
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SetTaxType(int customerTaxType, string returnUrl = "")
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //footer
        [ChildActionOnly]
        public virtual ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }

        //header links
        [ChildActionOnly]
        public virtual ActionResult HeaderLinks()
        {
            var model = _commonModelFactory.PrepareHeaderLinksModel();
            return PartialView(model);
        }
        [ChildActionOnly]
        public virtual ActionResult AdminHeaderLinks()
        {
            var model = _commonModelFactory.PrepareAdminHeaderLinksModel();
            return PartialView(model);
        }


        //social
        [ChildActionOnly]
        public virtual ActionResult Social()
        {
            var model = _commonModelFactory.PrepareSocialModel();
            return PartialView(model);
        }


        //footer
        [ChildActionOnly]
        public virtual ActionResult Footer()
        {
            var model = _commonModelFactory.PrepareFooterModel();
            return PartialView(model);
        }


        //contact us page
        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult ContactUs()
        {
            var model = new ContactUsModel();
            model = _commonModelFactory.PrepareContactUsModel(model, false);
            return View(model);
        }
        [HttpPost, ActionName("ContactUs")]
        [PublicAntiForgery]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult ContactUsSend(ContactUsModel model, FormCollection form, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //parse contact attributes
            var attributeXml = _commonModelFactory.ParseContactAttributes(form);
            var contactAttributeWarnings = _commonModelFactory.GetContactAttributesWarnings(attributeXml);
            if (contactAttributeWarnings.Any())
            {
                foreach (var item in contactAttributeWarnings)
                {
                    ModelState.AddModelError("", item);
                }
            }

            if (ModelState.IsValid)
            {
                model.ContactAttributeXml = attributeXml;
                model.ContactAttributeInfo = _contactAttributeFormatter.FormatAttributes(attributeXml, _workContext.CurrentCustomer);

                string subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactUsMessage(_workContext.CurrentCustomer, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body, model.ContactAttributeInfo, model.ContactAttributeXml);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return View(model);
            }
            model = _commonModelFactory.PrepareContactUsModel(model, true);
            return View(model);
        }


        //contact us page
        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult BookAppointment()
        {
            var model = new AppointmentModel();
            model = _commonModelFactory.PrepareAppointmentModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("BookAppointment")]
        [PublicAntiForgery]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult BookAppointmentSend(AppointmentModel model, FormCollection form, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }
            //parse contact attributes
            var attributeXml = _commonModelFactory.ParseAppointmentAttributes(form);
            var appointmentAttributeWarnings = _commonModelFactory.GetAppointmentAttributesWarnings(attributeXml);
            if (appointmentAttributeWarnings.Any())
            {
                foreach (var item in appointmentAttributeWarnings)
                {
                    ModelState.AddModelError("", item);
                }
            }

            if (ModelState.IsValid)
            {
                model.AppointmentAttributeXml = attributeXml;
                model.AppointmentAttributeInfo = _appointmentAttributeFormatter.FormatAttributes(attributeXml, _workContext.CurrentCustomer);

                string subject = _commonSettings.SubjectFieldOnAppointmentForm ? model.Subject : null;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendAppointmentMessage(_workContext.CurrentCustomer, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body, model.AppointmentAttributeInfo, model.AppointmentAttributeXml);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Appointment.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.Appointment", _localizationService.GetResource("ActivityLog.PublicStore.Appointment"));

                return View(model);
            }

            model = _commonModelFactory.PrepareAppointmentModel(model, true);
            return View(model);
        }
        //contact vendor page
        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ContactVendor(int vendorId)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            var model = new ContactVendorModel();
            model = _commonModelFactory.PrepareContactVendorModel(model, vendor, false);
            return View(model);
        }
        [HttpPost, ActionName("ContactVendor")]
        [PublicAntiForgery]
        [CaptchaValidator]
        public virtual ActionResult ContactVendorSend(ContactVendorModel model, bool captchaValid)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return RedirectToRoute("HomePage");

            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            model = _commonModelFactory.PrepareContactVendorModel(model, vendor, true);

            if (ModelState.IsValid)
            {
                string subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactVendorMessage(_workContext.CurrentCustomer, vendor, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        //sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Sitemap(SitemapPageModel pageModel)
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var model = _commonModelFactory.PrepareSitemapModel(this.Url, pageModel);
            return View(model);
        }

        //SEO sitemap page
        [NopHttpsRequirement(SslRequirement.No)]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult SitemapXml(int? id)
        {
            if (!_commonSettings.SitemapEnabled)
                return RedirectToRoute("HomePage");

            var siteMap = _commonModelFactory.PrepareSitemapXml(this.Url, id);
            return Content(siteMap, "text/xml");
        }

        //store theme
        [ChildActionOnly]
        public virtual ActionResult StoreThemeSelector()
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = _commonModelFactory.PrepareStoreThemeSelectorModel();
            return PartialView(model);
        }
        public virtual ActionResult SetStoreTheme(string themeName, string returnUrl = "")
        {
            _themeContext.WorkingThemeName = themeName;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        //favicon
        [ChildActionOnly]
        public virtual ActionResult Favicon()
        {
            var model = _commonModelFactory.PrepareFaviconModel();
            if (String.IsNullOrEmpty(model.FaviconUrl))
                return Content("");

            return PartialView(model);
        }

        //EU Cookie law
        [ChildActionOnly]
        public virtual ActionResult EuCookieLaw()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if (_workContext.CurrentCustomer.IsSearchEngineAccount())
                return Content("");

            if (_workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.EuCookieLawAccepted, _storeContext.CurrentStore.Id))
                //already accepted
                return Content("");

            //ignore notification?
            //right now it's used during logout so popup window is not displayed twice
            if (TempData["nop.IgnoreEuCookieLawWarning"] != null && Convert.ToBoolean(TempData["nop.IgnoreEuCookieLawWarning"]))
                return Content("");

            return PartialView();
        }
        [HttpPost]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.EuCookieLawAccepted, true, _storeContext.CurrentStore.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult RobotsTextFile()
        {
            var content = _commonModelFactory.PrepareRobotsTextFile();
            Response.ContentType = MimeTypes.TextPlain;
            Response.Write(content);
            return null;
        }

        public virtual ActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult StoreClosed()
        {
            return View();
        }

        #endregion

        #region Contactvendor
        [HttpPost, ActionName("VendorContact")]
        [CaptchaValidator]

        public ActionResult VendorContact(ContactUsModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (true)
            {
                string email = model.Email.Trim();
                string fullName = model.FullName;
                string subject = model.Subject;

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts()[0];
                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                string from;
                string fromName;
                string body = Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;

                }
                else
                {
                    from = email;
                    fromName = fullName;

                }
                string address = "";
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = emailAccount.Email,
                    ToName = emailAccount.DisplayName,
                    ReplyTo = email,
                    ReplyToName = fullName,
                    Priority = QueuedEmailPriority.High,
                    Subject = subject,
                    Body = body + address,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs", _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return Json(model);
            }
        }
        #endregion

        #region FaqMobile
        public ActionResult FaqMobile()
        {
            return PartialView();
        }
        #endregion

        #region LoyaltyMobile
        public ActionResult LoyaltyMobile()
        {
            return PartialView();
        }
        #endregion

        #region OfferSubscribe
        [HttpPost]
        public ActionResult OfferSubscribe(string email)
        {
            string result;
            bool success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            }
            else
            {
                string Enquiry = "";
                string subject = "Offer SubScribed";
                string body = Core.Html.HtmlHelper.FormatText(Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactUsMessage(_workContext.CurrentCustomer, _workContext.WorkingLanguage.Id,
                    email, "", subject, body, null, null);

                success = true;
                result = _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent");
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }
        #endregion

        #region Wife

        //contact vendor page
        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult WifeApply()
        {
            var model = new WifeApplyModel();
            model = _commonModelFactory.PrepareWifeApplyModel(model);
            return View(model);
        }
        [HttpPost, ActionName("WifeApply")]
        [PublicAntiForgery]
        [CaptchaValidator]
        public virtual ActionResult WifeApply(WifeApplyModel model, bool captchaValid, HttpPostedFileBase uploadedFile)
        {

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = uploadedFile.GetPictureBits();
                    var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);

                    if (picture != null)
                        model.ProductImage = picture.Id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("WifeApply.Name.Picture.ErrorMessage"));
                }
            }
            else
            {
                ModelState.AddModelError("", _localizationService.GetResource("WifeApply.Picture.Required"));
            }

            if (ModelState.IsValid)
            {
                string subject = "New Wife Form Filled";
                var sbDescription = new StringBuilder();

                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.Name"), model.Name);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.BusinessName"), model.BusinessName);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.ContactNumber"), model.ContactNumber);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.Email"), model.Email);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.CountryId"), model.CountryId);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.StateId"), model.StateProvinceId);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.City"), model.City);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.SellOnline"), model.SellOnline);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.Website"), model.Website);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.FacebookLink"), model.FacebookLink);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.Physically"), model.Physically);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.ShopAddress"), model.ShopAddress);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.TypeOfProducts"), model.TypeOfProducts);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.ProductImage"), model.ProductImage);
                sbDescription.AppendFormat("{0}: {1} \n<br>", _localizationService.GetResource("WifeApply.Enquiry"), model.Enquiry);
                string body = Core.Html.HtmlHelper.FormatText(sbDescription.ToString(), false, true, true, false, false, false);

                _workflowMessageService.SendContactUsMessage(_workContext.CurrentCustomer, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.Name, subject, body, null, null);

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("WifeApply.YourEnquiryHasBeenSent");

                return View(model);
            }
            model = _commonModelFactory.PrepareWifeApplyModel(model);
            return View(model);
        }
        #endregion

        #region Share
        public ActionResult Share(int sid = 0, int pid = 0)
        {
            var product = _productService.GetProductById(pid);
            sid = product != null ? product.VendorId : sid;
            var vendor = _vendorService.GetVendorById(sid);

            var store = _storeContext.CurrentStore;
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var SeoSetting = _settingService.LoadSetting<SeoSettings>(storeScope);

            var model = new ProductShareModel();
            model.CurrentStoreName = _storeContext.CurrentStore.Name;
            model.AndroidWebUrl = "https://play.google.com/store/apps/details?id=";
            model.AndroidMobileUrl = "market://details?id=";
            model.IosWebUrl = "https://itunes.apple.com/us/app/";
            model.IosMobileUrl = "MShop://share?sid=";

            if (vendor == null)
            {
                model.Title = SeoSetting.DefaultTitle;
                model.MetaTitle = store.Name;
                model.MetaDescription = !String.IsNullOrEmpty(SeoSetting.DefaultMetaDescription) ? SeoSetting.DefaultMetaDescription : store.Name;
                model.MetaKeywords = !String.IsNullOrEmpty(SeoSetting.DefaultMetaKeywords) ? SeoSetting.DefaultMetaKeywords : store.Name;
                model.Type = "Store";
                model.SeName = store.SslEnabled ? store.SecureUrl : store.Url;
                model.ImageUrl = SeoSetting.MobileAppPictureId != 0 ? _pictureService.GetPictureUrl(SeoSetting.MobileAppPictureId) : "";
                model.FacebookAppId = SeoSetting.FacebookAppId;
                model.AndroidWebUrl += !String.IsNullOrEmpty(SeoSetting.AppPackageIdAndroid) ? SeoSetting.AppPackageIdAndroid : "com.growonline.gomobishop";
                model.AndroidMobileUrl += !String.IsNullOrEmpty(SeoSetting.AppPackageIdIos) ? SeoSetting.AppPackageIdIos : "com.growonline.gomobishop";
                model.IosWebUrl += !String.IsNullOrEmpty(SeoSetting.AppPackageIdIos) ? SeoSetting.AppPackageIdIos + "/id" + SeoSetting.AppIdIos : "mobishop-all-in-one-shopping/id1441632028?mt=8";
                model.IosMobileUrl += "0";
                model.FacebookAppId = "0";
            }
            else
            {
                model.Title = vendor.Name;
                model.MetaTitle = !String.IsNullOrEmpty(vendor.MetaTitle) ? vendor.MetaTitle : vendor.Name;
                model.MetaDescription = vendor.MetaDescription;
                model.MetaKeywords = vendor.MetaKeywords;
                model.Type = "Vendor";
                model.SeName = vendor.GetSeName();
                model.ImageUrl = _pictureService.GetPictureUrl(vendor.PictureId);
                model.FacebookAppId = vendor.FacebookAppId;

                if (vendor.StandAloneAppAndroid)
                {
                    model.AndroidWebUrl += vendor.StandAloneAppPackageIdAndroid + "&referrer=sid:" + vendor.Id;
                    model.AndroidMobileUrl += vendor.StandAloneAppPackageIdAndroid + "&referrer=sid:" + vendor.Id;
                }
                else
                {
                    model.AndroidWebUrl += SeoSetting.AppPackageIdAndroid + "&referrer=sid:" + vendor.Id;
                    model.AndroidMobileUrl += SeoSetting.AppPackageIdAndroid + "&referrer=sid:" + vendor.Id;
                }

                if (vendor.StandAloneAppIos)
                {
                    model.IosWebUrl += vendor.StandAloneAppPackageIdIos + "/id" + vendor.StandAloneAppIdIos + "?mt=8";
                    model.IosMobileUrl = vendor.StandAloneAppUrlSchemesIos + "://share?sid=" + vendor.Id;
                }
                else
                {
                    model.IosWebUrl += !String.IsNullOrEmpty(SeoSetting.AppPackageIdIos) ? SeoSetting.AppPackageIdIos + "/id" + SeoSetting.AppIdIos : "mobishop-all-in-one-shopping/id1441632028?mt=8";
                    model.IosMobileUrl += vendor.Id;
                }


                if (product != null)
                {
                    model.Title = model.Title + " | " + product.Name;
                    model.MetaTitle = !String.IsNullOrEmpty(product.MetaTitle) ? product.MetaTitle : product.Name;
                    model.MetaDescription = product.MetaDescription;
                    model.MetaKeywords = product.MetaKeywords;
                    model.Type = "Product";
                    model.SeName = product.GetSeName();
                    var pictures = _pictureService.GetPicturesByProductId(product.Id);
                    var defaultPicture = pictures.FirstOrDefault();
                    model.ImageUrl = _pictureService.GetPictureUrl(defaultPicture);
                    model.AndroidWebUrl += "_pid:" + product.Id;
                    model.AndroidMobileUrl += "_pid:" + product.Id;
                    model.IosWebUrl += "_pid:" + product.Id;
                    model.IosMobileUrl += "_pid:" + product.Id;

                }
            }
            return View(model);
        }
        #endregion

        #region Contact Form

        [HttpPost]
        public virtual ActionResult ContactAttributeChange(FormCollection form)
        {
            var attributeXml = _commonModelFactory.ParseContactAttributes(form);

            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var attributes = _contactAttributeService.GetAllContactAttributes(_storeContext.CurrentStore.Id);
            foreach (var attribute in attributes)
            {
                var conditionMet = _contactAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual ActionResult UploadFileContactAttribute(int attributeId)
        {
            var attribute = _contactAttributeService.GetContactAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, MimeTypes.TextPlain);
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("Contact.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    }, MimeTypes.TextPlain);
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            EngineContext.Current.Resolve<IDownloadService>().InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("Contact.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            }, MimeTypes.TextPlain);
        }
        #endregion

        #region Appointment Form

        [HttpPost]
        public virtual ActionResult AppointmentAttributeChange(FormCollection form)
        {
            var attributeXml = _commonModelFactory.ParseAppointmentAttributes(form);

            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var attributes = _appointmentAttributeService.GetAllAppointmentAttributes(_storeContext.CurrentStore.Id);
            foreach (var attribute in attributes)
            {
                var conditionMet = _appointmentAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual ActionResult UploadFileAppointmentAttribute(int attributeId)
        {
            var attribute = _appointmentAttributeService.GetAppointmentAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, MimeTypes.TextPlain);
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("Appointment.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    }, MimeTypes.TextPlain);
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            EngineContext.Current.Resolve<IDownloadService>().InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("Appointment.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            }, MimeTypes.TextPlain);
        }
        #endregion

        #region PopupAction

        public virtual ActionResult PopupAction()
        {
            var result = _popupService.GetActivePopupByCustomerId(_workContext.CurrentCustomer.Id);
            if (result == null)
                return Content("");

            var model = new PopupModel
            {
                Id = result.Id,
                Body = result.Body,
                Name = result.Name,
                PopupType = (PopupType)result.PopupTypeId,
                CustomerActionId = result.CustomerActionId
            };

            _popupService.MovepopupToArchive(result.Id, _workContext.CurrentCustomer.Id);

            return PartialView(model);
        }
        #endregion

        #region PopupInteractiveForm

        [HttpPost, ActionName("PopupInteractiveForm")]
        public virtual ActionResult PopupInteractiveForm(FormCollection formCollection)
        {

            var errors = new List<string>();
            var formid = Request.Form["Id"];
            var form = _interactiveFormService.GetFormById(int.Parse(formid));
            if (form == null)
                return Json(new
                {
                    success = errors.Any(),
                    errors = errors
                });

            string enquiry = "";

            foreach (var item in form.InteractiveFormAttributes)
            {
                enquiry += string.Format("{0}: {1} <br />", item.Name, Request.Form[item.SystemName]);

                if (!string.IsNullOrEmpty(item.RegexValidation))
                {
                    var valuesStr = Request.Form[item.SystemName];
                    Regex regex = new Regex(item.RegexValidation);
                    Match match = regex.Match(valuesStr);
                    if (!match.Success)
                    {
                        errors.Add(string.Format(_localizationService.GetResource("PopupInteractiveForm.Fields.Regex"), item.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id)));
                    }
                }
                if (item.IsRequired)
                {
                    var valuesStr = Request.Form[item.SystemName];
                    if (string.IsNullOrEmpty(valuesStr))
                        errors.Add(string.Format(_localizationService.GetResource("PopupInteractiveForm.Fields.IsRequired"), item.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id)));
                }
                if (item.ValidationMinLength.HasValue)
                {
                    if (item.AttributeControlType == AttributeControlType.TextBox ||
                        item.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = Request.Form[item.SystemName].ToString();
                        int enteredTextLength = String.IsNullOrEmpty(valuesStr) ? 0 : valuesStr.Length;
                        if (item.ValidationMinLength.Value > enteredTextLength)
                        {
                            errors.Add(string.Format(_localizationService.GetResource("PopupInteractiveForm.Fields.TextboxMinimumLength"), item.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), item.ValidationMinLength.Value));
                        }
                    }
                }
                if (item.ValidationMaxLength.HasValue)
                {
                    if (item.AttributeControlType == AttributeControlType.TextBox ||
                        item.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        var valuesStr = Request.Form[item.SystemName].ToString();
                        int enteredTextLength = String.IsNullOrEmpty(valuesStr) ? 0 : valuesStr.Length;
                        if (item.ValidationMaxLength.Value < enteredTextLength)
                        {
                            errors.Add(string.Format(_localizationService.GetResource("PopupInteractiveForm.Fields.TextboxMaximumLength"), item.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), item.ValidationMaxLength.Value));
                        }
                    }
                }

            }

            if (!errors.Any())
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(form.EmailAccountId);
                if (emailAccount == null)
                    emailAccount = (_emailAccountService.GetAllEmailAccounts()).FirstOrDefault();
                if (emailAccount == null)
                    throw new Exception("No email account could be loaded");

                string from;
                string fromName;
                string subject = string.Format(_localizationService.GetResource("PopupInteractiveForm.EmailForm"), form.Name);
                from = emailAccount.Email;
                fromName = emailAccount.DisplayName;

                _queuedEmailService.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = emailAccount.Email,
                    ToName = emailAccount.DisplayName,
                    Priority = QueuedEmailPriority.High,
                    Subject = subject,
                    Body = enquiry,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                //activity log
                _customerActivityService.InsertActivity("PublicStore.InteractiveForm", form.Id, string.Format(_localizationService.GetResource("ActivityLog.PublicStore.InteractiveForm"), form.Name));
            }

            return Json(new
            {
                success = errors.Any(),
                errors = errors
            });
        }
        #endregion
    }
}
