using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.MServices.Models.Vendors;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Web.Models.Common;
using Nop.Web.Factories;
using Nop.Web.Areas.Mservices.Models.Common;
using static Nop.Web.Areas.Mservices.Models.Common.CountryandStatesModel;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Banner;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class CommonController : BaseController
    {
        #region Fields
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IForumService _forumservice;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPictureService _pictureService;

        private readonly CustomerSettings _customerSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CommonSettings _commonSettings;
        private readonly BlogSettings _blogSettings;
        private readonly NewsSettings _newsSettings;
        private readonly ForumSettings _forumSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IBannerService _bannerService;

        #endregion

        #region Constructors

        public CommonController(ICommonModelFactory commonModelFactory,
            ICategoryService categoryService,
            IProductService productService,
            IManufacturerService manufacturerService,
            ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IQueuedEmailService queuedEmailService,
            IEmailAccountService emailAccountService,
            ISitemapGenerator sitemapGenerator,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ICacheManager cacheManager,
            ICustomerActivityService customerActivityService,
            IVendorService vendorService,
            IPageHeadBuilder pageHeadBuilder,
            IPictureService pictureService,
            CustomerSettings customerSettings,
            TaxSettings taxSettings,
            CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings,
            EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings,
            BlogSettings blogSettings,
            NewsSettings newsSettings,
            ForumSettings forumSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            VendorSettings vendorSettings,
            ICountryService countryService,
            IWorkflowMessageService workflowMessageService,
        IStateProvinceService stateProvinceService,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        IBannerService bannerService)
        {
            this._commonModelFactory = commonModelFactory;
            this._categoryService = categoryService;
            this._productService = productService;
            this._manufacturerService = manufacturerService;
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountService = emailAccountService;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._forumservice = forumService;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;
            this._vendorService = vendorService;
            this._pageHeadBuilder = pageHeadBuilder;
            this._pictureService = pictureService;


            this._customerSettings = customerSettings;
            this._taxSettings = taxSettings;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._commonSettings = commonSettings;
            this._blogSettings = blogSettings;
            this._newsSettings = newsSettings;
            this._forumSettings = forumSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._vendorSettings = vendorSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._workflowMessageService = workflowMessageService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._bannerService = bannerService;

        }

        #endregion

        #region SendEnquiryToVendor
        [HttpPost]
        public ActionResult SendEnquiryToVendor(ContactVendorModelAPI model)
        {
            try
            {
                if (!_vendorSettings.AllowCustomersToContactVendors)
                    return InvokeHttp400("Customers are not allowed to contact vendor");
                // return Json("Customers are not allowed to contact vendor", JsonRequestBehavior.AllowGet);

                var vendor = _vendorService.GetVendorById(model.vendorId);
                if (vendor == null || !vendor.Active || vendor.Deleted)
                    return InvokeHttp400("Not a valid vendor");

                model.vendorName = vendor.GetLocalized(x => x.Name);

                string email = model.email.Trim();
                string fullName = model.fullName;

                string subject = "Enquiry Sent From Customer";


                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    return InvokeHttp400("No email account could be loaded");

                string from;
                string fromName;
                string body = Core.Html.HtmlHelper.FormatText(model.enquiry, false, true, false, false, false, false);
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                        Server.HtmlEncode(fullName),
                        Server.HtmlEncode(email), body);
                }
                else
                {
                    return InvokeHttp400("Not allowed to use system email for contact vendor");
                }
                _queuedEmailService.InsertQueuedEmail(new QueuedEmail
                {
                    From = from,
                    FromName = fromName,
                    To = vendor.Email,
                    ToName = vendor.Name,
                    ReplyTo = email,
                    ReplyToName = fullName,
                    Priority = QueuedEmailPriority.High,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id
                });

                return View(null, true, 200, _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent"));

            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }

        #endregion

        #region GetAvailableCountriesAndStates

        public ActionResult GetAvailableCountriesAndStates()
        {

            var model = new CountryandStatesModel();
            foreach (var country in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                var c = new Country
                {
                    Name = country.Name,
                    CountryId = country.Id
                };
                foreach (var Provinces in country.StateProvinces)
                {
                    var p = new StateProvince
                    {
                        Name = Provinces.Name,
                        ProvinceId = Provinces.Id
                    };
                    c.States.Add(p);
                };
                model.Countries.Add(c);
            }
            return View(model);
        }
        #endregion

        #region ContactUsSend
        public virtual ActionResult ContactUsSend(ContactUsModel model)
        {

            model = _commonModelFactory.PrepareContactUsModel(model, true);

            if (ModelState.IsValid)
            {
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

            return View(model);
        }
        #endregion

        #region GetCurrency
        //currency

        public ActionResult GetCurrency()
        {
            var model = _commonModelFactory.PrepareCurrencySelectorModel();
            return View(model);
        }
        #endregion

        #region SetCurrency
        public virtual ActionResult SetCurrency(int customerCurrency)
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null) _workContext.WorkingCurrency = currency;

            return View("", true, 200, "Currency Select");
        }
        #endregion

        #region SubscribeBackInStock


        // Product details page > back in stock subscribe
        public virtual ActionResult SubscribePopup(int productId = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp400("No product found with the specified id");

            var model = new BackInStockSubscribeModel();
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered();
            model.MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions;
            model.CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService
                .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                .TotalCount;
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id) != null;
            }
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult SubscribePopupPOST(int productId = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp400("No product found with the specified id");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService
                    .FindSubscription(_workContext.CurrentCustomer.Id, product.Id, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //subscription already exists
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);

                    return Json(new
                    {
                        result = "Unsubscribed"
                    });
                }

                //subscription does not exist
                //subscribe
                if (_backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, 0, 1)
                    .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                {
                    return Json(new
                    {
                        result = string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                    });
                }
                subscription = new BackInStockSubscription
                {
                    Customer = _workContext.CurrentCustomer,
                    Product = product,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _backInStockSubscriptionService.InsertSubscription(subscription);

                return Json(new
                {
                    result = "Subscribed"
                });
            }

            //subscription not possible
            return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
        }
        #endregion

        #region Banners

        public ActionResult GetBanner(string SearchBannerName = "", int StoreId = 0, int VendorId = 0, bool ShowHidden = false)
        {
            if (StoreId == 0)
            {
                StoreId = _storeContext.CurrentStore.Id;
            }

            //prepare models
            var BannerAllCacheKey = string.Format(ModelCacheEventConsumer.ALL_BANNER_MODEL_KEY, SearchBannerName, StoreId, VendorId, ShowHidden);
            var model = _cacheManager.Get(BannerAllCacheKey, () =>
            {
                var banners = _bannerService.GetAllBanners(bannerName: SearchBannerName, storeId: StoreId, vendorId: VendorId, showHidden: ShowHidden);
                var bannerList = new List<BannerModel>();
                foreach (var banner in banners)
                {
                    bannerList.Add(_commonModelFactory.PrepareBannerModel(banner));
                }

                return bannerList;
            });

            return View(model);
        }

        #endregion

        #region GetLanguage
        //language
        public virtual ActionResult GetLanguage()
        {
            var model = _commonModelFactory.PrepareLanguageSelectorModel();
            return View(model);
        }

        #endregion

        #region SetLanguage

        public virtual ActionResult SetLanguage(int langid)
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
            }

            return View("", true, 200, "Language Changed");
        }

        #endregion

    }
}