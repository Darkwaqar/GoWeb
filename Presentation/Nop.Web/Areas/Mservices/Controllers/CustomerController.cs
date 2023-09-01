using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Security.Honeypot;
using Nop.Web.Models.Customer;
using Nop.Services.Catalog;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Web.Factories;
using Nop.Web.MServices.Models.Common;
using Nop.Web.Areas.Mservices.Models.Customer;
using Nop.Admin.Models.Common;
using Nop.Admin.Extensions;
using Nop.Web.Models.Vendors;
using Nop.Web.Extensions;
using Nop.Services.Vendors;
using Nop.Core.Domain.Payments;
using Nop.Services.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Fcm;
using Nop.Services.Fcm;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Gdpr;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class CustomerController : BaseController
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRewardPointService _rewardPointService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly INotificationService _notificationService;
        private readonly IFcmActionService _fcmActionService;
        private readonly GdprSettings _gdprSettings;
        private readonly IGdprService _gdprService;

        #endregion

        #region Ctor

        public CustomerController(IAuthenticationService authenticationService,
            ICustomerModelFactory customerModelFactory,
            IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            OrderSettings orderSettings,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IOrderService orderService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IOpenAuthenticationService openAuthenticationService,
            IDownloadService downloadService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IReturnRequestService returnRequestService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            SecuritySettings securitySettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            StoreInformationSettings storeInformationSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            IRewardPointService rewardPointService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ILogger logger,
            IDeviceService deviceService,
            IProductService productService,
            IVendorService vendorService,
            INotificationService notificationService,
            IFcmActionService fcmActionService,
            GdprSettings gdprSettings,
            IGdprService gdprService)
        {
            this._customerModelFactory = customerModelFactory;
            this._authenticationService = authenticationService;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._customerService = customerService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._taxService = taxService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
            this._addressSettings = addressSettings;
            this._forumSettings = forumSettings;
            this._orderSettings = orderSettings;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderService = orderService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._openAuthenticationService = openAuthenticationService;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._returnRequestService = returnRequestService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._securitySettings = securitySettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._rewardPointService = rewardPointService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._logger = logger;
            this._deviceService = deviceService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._notificationService = notificationService;
            this._fcmActionService = fcmActionService;
            this._gdprSettings = gdprSettings;
            this._gdprService = gdprService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void TryAssociateAccountWithExternalAccount(Customer customer)
        {
            var parameters = ExternalAuthorizerHelper.RetrieveParametersFromRoundTrip(true);
            if (parameters == null)
                return;

            if (_openAuthenticationService.AccountExists(parameters))
                return;

            _openAuthenticationService.AssociateExternalAccountWithUser(customer, parameters);
        }
        #endregion

        #region Login /register

        #region LoginUser
        [HttpPost]
        public ActionResult LoginUser(LoginModel model, string returnUrl = "")
        {

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult =
                    _customerRegistrationService.ValidateCustomer(
                        _customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password, _storeContext.CurrentStore.Id);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username, _storeContext.CurrentStore.Id)
                                : _customerService.GetCustomerByEmail(model.Email, _storeContext.CurrentStore.Id);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));


                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));
                            var customerApi = new CustomerApi
                            {
                                UserID = customer.CustomerGuid,
                                Email = customer.Email,
                                FullName = customer.GetFullName(),
                                NumberOfCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).LimitPerStore(_storeContext.CurrentStore.Id).Count(),
                                NumberOfWishlistItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).LimitPerStore(_storeContext.CurrentStore.Id).Count()
                            };

                            return View(customerApi, true, 200, "Successfully login");
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                    case CustomerLoginResults.Deleted:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                    case CustomerLoginResults.NotActive:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                    case CustomerLoginResults.NotRegistered:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                    case CustomerLoginResults.LockedOut:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                    case CustomerLoginResults.WrongPassword:
                    default:
                        return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials"));
                }
            }

            //If we got this far, something failed, redisplay form
            return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials"));

        }

        #endregion

        #region RegisterUser
        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [ValidateInput(false)]
        public ActionResult RegisterUser(FormCollection form)
        {
            string returnUrl = string.Empty;
            //check whether registration is allowed
            RegisterModel model = new RegisterModel();
            model.Email = string.IsNullOrEmpty(Convert.ToString(form["Email"])) ? "" : Convert.ToString(form["Email"]);
            model.FirstName = string.IsNullOrEmpty(Convert.ToString(form["FirstName"])) ? "" : Convert.ToString(form["FirstName"]);
            model.LastName = string.IsNullOrEmpty(Convert.ToString(form["LastName"])) ? "" : Convert.ToString(form["LastName"]);
            model.Password = string.IsNullOrEmpty(Convert.ToString(form["Password"])) ? "" : Convert.ToString(form["Password"]);
            model.ConfirmPassword = string.IsNullOrEmpty(Convert.ToString(form["ConfirmPassword"])) ? "" : Convert.ToString(form["ConfirmPassword"]);
            model.Phone = string.IsNullOrEmpty(Convert.ToString(form["Phone"])) ? "" : Convert.ToString(form["Phone"]);
            model.Fax = string.IsNullOrEmpty(Convert.ToString(form["Fax"])) ? "" : Convert.ToString(form["Fax"]);

            if (model.Email == null || model.FirstName == null || model.LastName == null
                || model.Password == null || model.ConfirmPassword == null)
                return InvokeHttp400("All params are require.");

            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return InvokeHttp400("User Disabled");

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            //custom customer attributes
            var customerAttributesXml = form.ParseCustomCustomerAttributes(_customerAttributeParser, _customerAttributeService);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Email,
                    _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);

                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                            SystemCustomerAttributeNames.VatNumberStatusId,
                            (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    TryAssociateAccountWithExternalAccount(customer);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                        LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                        Email = customer.Email,
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) : null,
                        StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0 ?
                            (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) : null,
                        City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                        Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                        Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        customer.Addresses.Add(defaultAddress);
                        customer.BillingAddress = defaultAddress;
                        customer.ShippingAddress = defaultAddress;
                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return View("", true, 200, "Email is Send for Verification");
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return View("", true, 200, "Wait for Admin Approval");
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                var customerApi = new CustomerApi
                                {
                                    UserID = customer.CustomerGuid,
                                    Email = customer.Email,
                                    FullName = customer.GetFullName(),
                                    NumberOfCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).LimitPerStore(_storeContext.CurrentStore.Id).Count(),
                                    NumberOfWishlistItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).LimitPerStore(_storeContext.CurrentStore.Id).Count()
                                };
                                return View(customerApi, true, 200, "Successfully registered and login");

                            }
                        default:
                            {
                                var customerApi = new CustomerApi
                                {
                                    UserID = customer.CustomerGuid,
                                    Email = customer.Email,
                                    FullName = customer.GetFullName(),
                                    NumberOfCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).LimitPerStore(_storeContext.CurrentStore.Id).Count(),
                                    NumberOfWishlistItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).LimitPerStore(_storeContext.CurrentStore.Id).Count()
                                };
                                return View(customerApi, true, 200, "Successfully registered");
                            }
                    }
                }
                else
                {
                    string message = "Unknown error occured.";
                    if (registrationResult.Errors != null)
                    {
                        message = registrationResult.Errors[0].ToString();
                    }
                    return InvokeHttp400(message);
                }
            }

            //If we got this far, something failed, redisplay form
            return InvokeHttp400("Something Went Wrong");
        }
        #endregion

        #endregion

        #region LoginFacebookUser

        [HttpPost]
        public ActionResult LoginFacebookUser(FormCollection form)
        {
            string email = form["Email"];

            var loginResult =
                _customerRegistrationService.ValidateCustomerByEmail(email, _storeContext.CurrentStore.Id);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);

                        //migrate shopping cart
                        _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                        //sign in new customer
                        _authenticationService.SignIn(customer, false);

                        //raise event       
                        _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                        //activity log
                        _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));
                        var customerApi = new CustomerApi
                        {
                            UserID = customer.CustomerGuid,
                            Email = customer.Email,
                            FullName = customer.GetFullName(),
                            NumberOfCartItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).LimitPerStore(_storeContext.CurrentStore.Id).Count(),
                            NumberOfWishlistItems = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist).LimitPerStore(_storeContext.CurrentStore.Id).Count()
                        };

                        return View(customerApi, true, 200, "Successfully login");
                    }
                case CustomerLoginResults.CustomerNotExist:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                case CustomerLoginResults.Deleted:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                case CustomerLoginResults.NotActive:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                case CustomerLoginResults.NotRegistered:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                case CustomerLoginResults.LockedOut:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                case CustomerLoginResults.WrongPassword:
                default:
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.WrongCredentials"));
            }
        }
        #endregion

        #region RecoverPassword

        public ActionResult RecoverPassword(string email)
        {

            var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryTokenDateGenerated, generatedDateTime);

                //send email
                _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer, _workContext.WorkingLanguage.Id);

                //model.Result = ;
                return View("", true, 200, _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent"));
            }
            else
            {
                return InvokeHttp400(_localizationService.GetResource("Account.PasswordRecovery.EmailNotFound"));
            }
        }
        #endregion

        #region GetRewardPoints

        public ActionResult GetRewardPoints()
        {
            var customer = _workContext.CurrentCustomer;
            var StoreId = _storeContext.CurrentStore;
            var RewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, StoreId.Id);
            var rewardPoints = _rewardPointService.GetRewardPointsHistory(customer.Id, true).ToList();
            List<RewardPointsHistory> rewardPointsHistory = new List<RewardPointsHistory>();


            int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);

            int MinimumRewardPointsToUse = _rewardPointsSettings.MinimumRewardPointsToUse;
            decimal minRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(MinimumRewardPointsToUse);
            decimal minRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minRewardPointsAmountBase, _workContext.WorkingCurrency);
            foreach (var reward in rewardPoints)
            {
                rewardPointsHistory.Add(new RewardPointsHistory
                {
                    CustomerId = customer.Id,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(reward.CreatedOnUtc, DateTimeKind.Utc),
                    Message = reward.Message,
                    Points = reward.Points,
                    PointsBalance = reward.PointsBalance,
                    Id = reward.Id,
                    UsedAmount = reward.UsedAmount
                });
            }
            if (rewardPointsHistory.Any())
            {
                var model = new
                {
                    RewardPointsHistory = rewardPointsHistory,
                    RewardPointsBalance = RewardPointsBalance,
                    rewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false),
                    MinimumRewardPointsToUse = MinimumRewardPointsToUse,
                    MinimumRewardPointsToUseAmount = _priceFormatter.FormatPrice(minRewardPointsAmount, true, false)

                };
                return View(model);
            }
            else
            {
                return InvokeHttp400("No Reward Points found.");
            }
        }
        #endregion

        #region ChangePassword

        [HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("User not Registered");

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email, _storeContext.CurrentStore.Id,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest, null);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model, true, 200, _localizationService.GetResource("Account.ChangePassword.Success"));
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return InvokeHttp400("Change Password failed");
        }
        #endregion

        #region Info / Edit

        #region EditInfo
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditInfo(CustomerInfoModel model, FormCollection form)
        {
            try
            {
                if (model.Email==null)
                {
                    return InvokeHttp400(_localizationService.GetResource("Account.Login.Fields.Email.Required"));
                }

                if (!_workContext.CurrentCustomer.IsRegistered())
                {
                    return InvokeHttp400("Not a valid customer");
                }

                var customer = _workContext.CurrentCustomer;
                //custom customer attributes
                var customerAttributesXml = form.ParseCustomCustomerAttributes(_customerAttributeParser, _customerAttributeService);
                var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }


                //if (ModelState.IsValid)
                //{
                //username 
                if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
                {
                    if (!customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change username
                        _customerRegistrationService.SetUsername(customer, model.Username.Trim(), _storeContext.CurrentStore.Id);
                        //re-authenticate
                        _authenticationService.SignIn(customer, true);
                    }
                }
                //email
                if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    //change email
                    _customerRegistrationService.SetEmail(customer, model.Email.Trim(), false, _storeContext.CurrentStore.Id);
                    //re-authenticate (if usernames are disabled)
                    if (!_customerSettings.UsernamesEnabled)
                    {
                        _authenticationService.SignIn(customer, true);
                    }
                }

                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                }
                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    var prevVatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.VatNumber);
                    if (prevVatNumber != model.VatNumber)
                    {
                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                                SystemCustomerAttributeNames.VatNumberStatusId,
                                (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }
                }

                //form fields
                if (_customerSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                {
                    DateTime? dateOfBirth = model.ParseDateOfBirth();
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                }
                //if (_customerSettings.CompanyEnabled)
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                if (_customerSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                if (_customerSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                if (_customerSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                if (_customerSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                if (_customerSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                if (_customerSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                if (_customerSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                //newsletter
                if (_customerSettings.NewsletterEnabled)
                {
                    //save newsletter value
                    var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                    if (newsletter != null)
                    {
                        if (model.Newsletter)
                        {
                            newsletter.Active = true;
                            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                        }
                        else
                            _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                    }
                    else
                    {
                        if (model.Newsletter)
                        {
                            _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                            {
                                NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                Email = customer.Email,
                                Active = true,
                                StoreId = _storeContext.CurrentStore.Id,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }
                    }
                    // }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Signature, model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);


                    // return RedirectToRoute("CustomerInfo");
                }

                //If we got this far, something failed, redisplay form
                _customerModelFactory.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
                return View(model, true, 200, "Info Updated Successfully");
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.Message);
            }
        }

        #endregion

        #endregion

        #region Addresses / Delete / Add / Edit

        #region Addresses
        public virtual ActionResult Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            var model = _customerModelFactory.PrepareCustomerAddressListModel();
            return View(model);
        }
        #endregion

        #region DeleteAddress

        public virtual ActionResult DeleteAddress(int addressId = 0)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Please Login");

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
                return View("", true, 200, "Address Delete successfully.");

            }
            else
            {
                return InvokeHttp400("Address Not Found.");
            }

        }

        #endregion

        #region AddAddress
        [HttpPost]
        public virtual ActionResult AddAddress(AddressMobileModel model, FormCollection form)
        {
            var customer = _workContext.CurrentCustomer;


            if (!customer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            if (model.FirstName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.FirstName.Required"));
            if (model.LastName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.LastName.Required"));
            if (model.Email == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Email.Required"));
            if (_addressSettings.CountryEnabled)
            {
                if (model.CountryId == null || model.CountryId == 0) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (_addressSettings.CountryEnabled && _addressSettings.StateProvinceEnabled)
            {
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = _stateProvinceService.GetStateProvincesByCountryId(countryId).Any();

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (_addressSettings.StreetAddressRequired && _addressSettings.StreetAddressEnabled)
            {
                if (model.Address1 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (_addressSettings.StreetAddress2Required && _addressSettings.StreetAddress2Required)
            {
                if (model.Address2 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (_addressSettings.ZipPostalCodeRequired && _addressSettings.ZipPostalCodeEnabled)
            {
                if (model.ZipPostalCode == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (_addressSettings.CityRequired && _addressSettings.CityEnabled)
            {
                if (model.City == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (_addressSettings.PhoneRequired && _addressSettings.PhoneEnabled)
            {
                if (model.PhoneNumber == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.Phone.Required"));
            }

            Address address = ReturnAddressModel(model);
            address.CreatedOnUtc = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);
                return View("", true, 200, "Address added successfully.");
            }
            else
                return InvokeHttp400(ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);

        }
        #endregion

        #region EditAddress
        [HttpPost]
        public virtual ActionResult EditAddress(AddressMobileModel model, FormCollection form)
        {
            var customer = _workContext.CurrentCustomer;


            if (!customer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            if (model.Id == 0)
                return InvokeHttp400("Please Provide Address ID");

            if (model.FirstName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.FirstName.Required"));
            if (model.LastName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.LastName.Required"));
            if (model.Email == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Email.Required"));
            if (_addressSettings.CountryEnabled)
            {
                if (model.CountryId == null || model.CountryId == 0) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (_addressSettings.CountryEnabled && _addressSettings.StateProvinceEnabled)
            {
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = _stateProvinceService.GetStateProvincesByCountryId(countryId).Any();

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (_addressSettings.StreetAddressRequired && _addressSettings.StreetAddressEnabled)
            {
                if (model.Address1 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (_addressSettings.StreetAddress2Required && _addressSettings.StreetAddress2Required)
            {
                if (model.Address2 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (_addressSettings.ZipPostalCodeRequired && _addressSettings.ZipPostalCodeEnabled)
            {
                if (model.ZipPostalCode == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (_addressSettings.CityRequired && _addressSettings.CityEnabled)
            {
                if (model.City == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (_addressSettings.PhoneRequired && _addressSettings.PhoneEnabled)
            {
                if (model.PhoneNumber == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.Phone.Required"));
            }


            //custom address attributes

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == model.Id);
            if (address == null)
                //address is not found
                return InvokeHttp400("address is not found");

            if (ModelState.IsValid)
            {
                address.Id = model.Id;
                address.FirstName = model.FirstName;
                address.LastName = model.LastName;
                address.Email = model.Email;
                address.Company = model.Company;
                address.Address1 = model.Address1;
                address.Address2 = model.Address2;
                address.City = model.City;
                address.ZipPostalCode = model.ZipPostalCode;
                address.PhoneNumber = model.PhoneNumber;
                if (model.CountryId != null)
                    address.CountryId = Convert.ToInt32(model.CountryId);
                if (model.StateProvinceId != null)
                    address.StateProvinceId = Convert.ToInt32(model.StateProvinceId);

                _addressService.UpdateAddress(address);
                return View(null, true, 200, "Address Updated successfully.");
            }

            return InvokeHttp400(ModelState.LastOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);
        }
        #endregion

        #region AddAddress | ReturnAddressModel

        private Address ReturnAddressModel(AddressMobileModel model)
        {
            Address address = new Address();
            address.FirstName = model.FirstName;
            address.LastName = model.LastName;
            address.Email = model.Email;
            address.Company = model.Company;
            address.Address1 = model.Address1;
            address.Address2 = model.Address2;
            address.City = model.City;
            address.ZipPostalCode = model.ZipPostalCode;
            address.PhoneNumber = model.PhoneNumber;
            if (model.CountryId != null)
                address.CountryId = Convert.ToInt32(model.CountryId);
            if (model.StateProvinceId != null)
                address.StateProvinceId = Convert.ToInt32(model.StateProvinceId);

            return address;
        }

        #endregion

        #endregion

        #region Devices

        [HttpPost]
        public virtual ActionResult AddDevice(DeviceModel model, FormCollection form)
        {
            if (String.IsNullOrEmpty(model.DeviceId))
            {
                return InvokeHttp400("Please add Device Id");
            }

            if (ModelState.IsValid)
            {
                var Device = model.ToEntity();

                Device.Active = true;

                var device = _deviceService.GetAllDevices(SearchId: model.DeviceId).FirstOrDefault();
                if (device == null)
                {
                    Device.UpdatedOnUtc = DateTime.Now;
                    _deviceService.InsertDevice(Device);
                }
                else
                {
                    Device.CreatedOnUtc = DateTime.Now;
                    _deviceService.UpdateDevice(device);
                }

                var customer = _workContext.CurrentCustomer;
                if (customer.IsRegistered())
                {
                    foreach (var dev in customer.Devices.Where(x => x.DeviceId == model.DeviceId).ToList())
                    {
                        customer.Devices.Remove(dev);
                    }
                    Device.CustomerId = customer.Id;
                    Device.CreatedOnUtc = DateTime.Now;
                    Device.UpdatedOnUtc = DateTime.Now;
                    customer.Devices.Add(Device);
                    _customerService.UpdateCustomer(customer);

                }

                return View("", true, 200, "Device added successfully.");
            }
            else
                return InvokeHttp400(ModelState.Where(x => x.Value.Errors.Count > 0).FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);
        }
        #endregion

        #region CheckLogin

        public virtual ActionResult CheckLogin()
        {
            Object model = new object();
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                model = new
                {
                    Login = false
                };

            }
            else
            {
                model = new
                {
                    Login = true
                };
            }
            return View(model);
        }
        #endregion

        #region ReviewCenter

        public ActionResult UserReviewsData(string Type = "")
        {
            Type = Type.ToLower();
            var customer = _workContext.CurrentCustomer;
            if (customer != null && customer.IsRegistered())
            {
                if (Type == "product")
                {
                    var productReviews = _productService.GetAllProductReviews(customer.Id, null).ToList();
                    productReviews = productReviews.OrderByDescending(pr => pr.CreatedOnUtc).ToList();
                    var model = new PublicProductReviewDisplayModel()
                    {
                        ProductReviews = productReviews.ToModel(_pictureService)
                    };

                    foreach (var pr in productReviews)
                    {
                        if (model.ProductImageUrl.ContainsKey(pr.ProductId))
                            continue;
                        var imageUrl = pr.Product.ProductPictures.Any() ? _pictureService.GetPictureUrl(pr.Product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl();
                        model.ProductImageUrl.Add(pr.ProductId, imageUrl);

                    }
                    return View(model.ProductReviews);
                }
                if (Type == "vendor")
                {
                    var vendorReviews = _vendorService.GetVendorReviews(null, customer.Id, null, 1, int.MaxValue).ToList();


                    var model = new PublicVendorReviewDisplayModel()
                    {
                        VendorReviews = vendorReviews.ToListModel(_pictureService, _productService, _vendorService, _customerService)
                    };
                    return View(model);
                }
                if (Type == "pending")
                {
                    var customerOrders = _orderService.SearchOrders(customerId: customer.Id).Where(x => x.OrderStatus == OrderStatus.Complete || x.PaymentStatus == PaymentStatus.Paid || x.ShippingStatus == ShippingStatus.Delivered).ToList();

                    var pendingReviewProducts = _vendorService.GetProductsWithPendingReviews(customerOrders, customer.Id);

                    var model = new PublicPendingReviewDisplayModel();
                    var vendorList = new Dictionary<int, Vendor>(); //storing vendors for performance

                    foreach (var prp in pendingReviewProducts)
                    {

                        var order = prp.Key;
                        var orderModel = new PendingOrderModel()
                        {
                            OrderId = order.Id,
                        };
                        var reviewModelList = new List<PendingReviewModel>();

                        foreach (var product in prp.Value)
                        {
                            Vendor v = null;
                            if (vendorList.ContainsKey(product.VendorId))
                                v = vendorList[product.VendorId];
                            else
                            {
                                v = _vendorService.GetVendorById(product.VendorId);
                                vendorList.Add(v.Id, v);//add it to a dictionary for avoiding next time database query for same vendor
                            }

                            var prModel = new PendingReviewModel()
                            {
                                OrderId = order.Id,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                ProductImageUrl = product.ProductPictures.Any() ? _pictureService.GetPictureUrl(product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl(),
                                ProductSeName = product.GetSeName(),
                                VendorName = v.Name,
                                VendorSeName = v.GetSeName(),
                                VendorImageUrl = _pictureService.GetPictureUrl(v.mpLogo)
                            };
                            reviewModelList.Add(prModel);
                        }
                        model.PendingReviews.Add(orderModel, reviewModelList);
                    }
                    return View(model.PendingReviews.Values);
                }
                return InvokeHttp400("Type Cannot Be Empty");
            }
            return InvokeHttp400("Customer not registed");
        }
        #endregion

        #region Notifications
        public virtual ActionResult Notifications()
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Customer is not registered");


            var notifications = _workContext.CurrentCustomer.Notifications.OrderByDescending(x => x.Id);

            List<NotificationMobileModel> Notification = new List<NotificationMobileModel>();
            foreach (var notification in notifications)
            {
                NotificationMobileModel notificationMobileModel = new NotificationMobileModel();
                notificationMobileModel.Id = notification.Id;
                notificationMobileModel.FcmType = notification.FcmType;
                notificationMobileModel.FcmApplicationType = notification.FcmApplicationType;
                notificationMobileModel.ApplicationId = notification.ApplicationId;
                notificationMobileModel.ActionId = notification.ActionId;
                notificationMobileModel.Title = notification.Title;
                notificationMobileModel.Message = notification.Message;
                notificationMobileModel.Image = notification.Image;
                notificationMobileModel.DirectImageLink = notification.DirectImageLink;
                notificationMobileModel.GotoLink = notification.GotoLink;
                notificationMobileModel.Icon = notification.Icon;
                notificationMobileModel.DirectIconLink = notification.DirectIconLink;
                notificationMobileModel.IsReaded = notification.IsReaded;
                notificationMobileModel.CreatedOnUtc = notification.CreatedOnUtc;
                notificationMobileModel.UpdatedOnUtc = notification.UpdatedOnUtc;
                notificationMobileModel.ReadedOnUtc = notification.ReadedOnUtc;
                notificationMobileModel.VendorId = notification.VendorId;

                if (notification.VendorId > 0)
                {
                    var vendor = _vendorService.GetVendorById(notification.VendorId);
                    if (vendor != null)
                    {
                        notificationMobileModel.VendorName = vendor.Name;
                        notificationMobileModel.VendorImage = _pictureService.GetPictureUrl(vendor.mpLogo);
                    }
                }

                if (notification.ActionId > 0)
                {
                    var action = _fcmActionService.GetFcmActionById(notification.ActionId);
                    if (action != null)
                    {
                        notificationMobileModel.FcmActionType = action.FcmActionType;
                        notificationMobileModel.Extra = action.Extra;
                    }

                }

                Notification.Add(notificationMobileModel);
            }


            if (Notification.Count > 0)
                return View(Notification);
            else
                return InvokeHttp400("No Notification Found");
        }

        public virtual ActionResult ReadNotification(int id = 0)
        {
            var customer = _workContext.CurrentCustomer;
            if (!customer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            if (id == 0)
                return InvokeHttp400("Please Provide Notification Id");
            var notification = _notificationService.GetNotificationById(id);

            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Please Login");

            //find notification (ensure that it belongs to the current customer)
            var customerNotification = customer.Notifications.FirstOrDefault(a => a.Id == id);
            if (customerNotification == null)
                //address is not found
                return InvokeHttp400("Notification is not found");

            notification.IsReaded = true;
            notification.ReadedOnUtc = DateTime.UtcNow;
            _notificationService.UpdateNotification(notification);
            return View("Notification Updated");

        }
        #endregion

        //New Services 

        #region Login / logout

        public virtual ActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        public virtual ActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult =
                    _customerRegistrationService.ValidateCustomer(
                        _customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password, _storeContext.CurrentStore.Id);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username, _storeContext.CurrentStore.Id)
                                : _customerService.GetCustomerByEmail(model.Email, _storeContext.CurrentStore.Id);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

                            return View("", true, 200, "Successfully login");
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        public virtual ActionResult Logout()
        {
            //external authentication
            ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated,
                    "Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                    _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id);
                _customerActivityService.InsertActivity("Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id);

                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return this.RedirectToAction("Edit", "Customer",
                    new { id = _workContext.CurrentCustomer.Id, area = "Admin" });

            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["nop.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToRoute("HomePage");
        }

        #endregion

        #region My account / Info

        public virtual ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerInfoModel();
            model = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Info(CustomerInfoModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var oldCustomerModel = new CustomerInfoModel();

            var customer = _workContext.CurrentCustomer;

            //get customer info model before changes for gdpr log
            if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
                oldCustomerModel = _customerModelFactory.PrepareCustomerInfoModel(oldCustomerModel, customer, false);

            //custom customer attributes
            var customerAttributesXml = form.ParseCustomCustomerAttributes(_customerAttributeParser, _customerAttributeService);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _customerRegistrationService.SetUsername(customer, model.Username.Trim(), _storeContext.CurrentStore.Id);

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalCustomerIfImpersonated == null)
                                _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _customerSettings.UserRegistrationType ==
                                                UserRegistrationType.EmailValidation;
                        _customerRegistrationService.SetEmail(customer, model.Email.Trim(), requireValidation, _storeContext.CurrentStore.Id);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_customerSettings.UsernamesEnabled && !requireValidation)
                                _authenticationService.SignIn(customer, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId,
                            model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);

                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            string vatName;
                            string vatAddress;
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName,
                                out vatAddress);
                            _genericAttributeService.SaveAttribute(customer,
                                SystemCustomerAttributeNames.VatNumberStatusId, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!String.IsNullOrEmpty(model.VatNumber) &&
                                _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender,
                            model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth,
                            dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company,
                            model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress,
                            model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2,
                            model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode,
                            model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.County, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId,
                            model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId,
                            model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter =
                            _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email,
                                _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Signature,
                            model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributesXml);

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                        form.LogGdpr(customer, oldCustomerModel, model, _gdprService, _gdprSettings, _workContext, _localizationService, _countryService, _stateProvinceService, _logger);


                }
            }
            catch (Exception exc)
            {
                return InvokeHttp400(exc.Message);
            }


            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return View(model);
        }
        
        public virtual ActionResult EmailRevalidation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.EmailRevalidationToken);
            if (string.IsNullOrEmpty(cToken))
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged")
                });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            if (String.IsNullOrEmpty(customer.EmailToRevalidate))
                return RedirectToRoute("HomePage");

            if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("HomePage");

            //change email
            try
            {
                _customerRegistrationService.SetEmail(customer, customer.EmailToRevalidate, false, _storeContext.CurrentStore.Id);
            }
            catch (Exception exc)
            {
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource(exc.Message)
                });
            }
            customer.EmailToRevalidate = null;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.EmailRevalidationToken, "");

            //re-authenticate (if usernames are disabled)
            if (!_customerSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(customer, true);
            }

            var model = new EmailRevalidationModel()
            {
                Result = _localizationService.GetResource("Account.EmailRevalidation.Changed")
            };
            return View(model);
        }

        #endregion
    }
}