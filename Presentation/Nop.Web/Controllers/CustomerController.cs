﻿using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Security.Honeypot;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public partial class CustomerController : BasePublicController
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly CustomerSettings _customerSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IStoreService _storeService;
        private readonly IEventPublisher _eventPublisher;

        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IGdprService _gdprService;
        private readonly IExportManager _exportManager;
        private readonly IGiftCardService _giftCardService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILogger _logger;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITwoFactorAuthenticationService _twoFactorAuthenticationService;
        private readonly HttpContextBase _httpContext;
        private readonly ICustomerActionEventService _customerActionEventService;

        #endregion

        #region Ctor

        public CustomerController(IAddressModelFactory addressModelFactory,
            ICustomerModelFactory customerModelFactory,
            IAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            CustomerSettings customerSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            IAddressService addressService,
            ICountryService countryService,
            IOrderService orderService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IOpenAuthenticationService openAuthenticationService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IStoreService storeService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            StoreInformationSettings storeInformationSettings,
            GdprSettings gdprSettings,
            IGdprService gdprService,
            IExportManager exportManager,
            IGiftCardService giftCardService,
            IStateProvinceService stateProvinceService,
            ILogger logger,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            ITwoFactorAuthenticationService twoFactorAuthenticationService,
            HttpContextBase httpContext,
            ICustomerActionEventService customerActionEventService)
        {
            this._addressModelFactory = addressModelFactory;
            this._customerModelFactory = customerModelFactory;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._taxService = taxService;
            this._customerSettings = customerSettings;
            this._addressSettings = addressSettings;
            this._forumSettings = forumSettings;
            this._addressService = addressService;
            this._countryService = countryService;
            this._orderService = orderService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._openAuthenticationService = openAuthenticationService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._storeService = storeService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._gdprSettings = gdprSettings;
            this._gdprService = gdprService;
            this._exportManager = exportManager;
            this._giftCardService = giftCardService;
            this._stateProvinceService = stateProvinceService;
            this._logger = logger;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._twoFactorAuthenticationService = twoFactorAuthenticationService;
            this._httpContext = httpContext;
            _customerActionEventService = customerActionEventService;
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

        protected virtual void ValidateRequiredConsents(List<GdprConsent> consents, FormCollection form)
        {
            foreach (var consent in consents)
            {
                var controlId = $"consent{consent.Id}";
                var cbConsent = form[controlId];
                if (string.IsNullOrEmpty(cbConsent) || !cbConsent.ToString().Equals("on"))
                {
                    ModelState.AddModelError("", consent.RequiredMessage);
                }
            }
        }

        #endregion

        #region Login / logout

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
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

                            //sign in
                            return SignInAction(customer,model.RememberMe, returnUrl);
                           
                        }
                    case CustomerLoginResults.RequiresTwoFactor:
                        {
                            var userName = _customerSettings.UsernamesEnabled ? model.Username : model.Email;
                            _httpContext.Session.Add("RequiresTwoFactor", userName);
                            return RedirectToRoute("TwoFactorAuthorization");
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

        public ActionResult TwoFactorAuthorization()
        {
            if (!_customerSettings.TwoFactorAuthenticationEnabled)
                return RedirectToRoute("Login");

            var username = _httpContext.Session["RequiresTwoFactor"] as string;
            if (string.IsNullOrEmpty(username))
                return RedirectToRoute("HomePage");

            var customer = _customerSettings.UsernamesEnabled ?  _customerService.GetCustomerByUsername(username) :  _customerService.GetCustomerByEmail(username);
            if (customer == null)
                return RedirectToRoute("HomePage");

            if (!customer.GetAttribute<bool>(SystemCustomerAttributeNames.TwoFactorEnabled))
                return RedirectToRoute("HomePage");

            if (_customerSettings.TwoFactorAuthenticationType != TwoFactorAuthenticationType.AppVerification)
            {
                 _twoFactorAuthenticationService.GenerateCodeSetup("", customer, _workContext.WorkingLanguage, _customerSettings.TwoFactorAuthenticationType);
            }

            return View();
        }

        [HttpPost]
        public ActionResult TwoFactorAuthorization(string token)
        {
            if (!_customerSettings.TwoFactorAuthenticationEnabled)
                return RedirectToRoute("Login");

            var username = _httpContext.Session["RequiresTwoFactor"] as string;
            if (string.IsNullOrEmpty(username))
                return RedirectToRoute("HomePage");

            var customer = _customerSettings.UsernamesEnabled ?  _customerService.GetCustomerByUsername(username,_storeContext.CurrentStore.Id) :  _customerService.GetCustomerByEmail(username, _storeContext.CurrentStore.Id);
            if (customer == null)
                return RedirectToRoute("Login");

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Account.TwoFactorAuth.SecurityCodeIsRequired"));
            }
            else
            {
                var secretKey = customer.GetAttribute<string>(SystemCustomerAttributeNames.TwoFactorSecretKey);
                if ( _twoFactorAuthenticationService.AuthenticateTwoFactor(secretKey, token, customer, _customerSettings.TwoFactorAuthenticationType))
                {
                    //remove session
                    HttpContext.Session.Remove("RequiresTwoFactor");

                    //sign in
                    return SignInAction(customer);
                }
                ModelState.AddModelError("", _localizationService.GetResource("Account.TwoFactorAuth.WrongSecurityCode"));
            }

            return View();
        }

        protected ActionResult SignInAction(Customer customer,bool rememberMe = false, string returnUrl = null)
        {
            //migrate shopping cart
            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

            //sign in new customer
            _authenticationService.SignIn(customer, rememberMe);

            //raise event       
            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

            //activity log
            _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(returnUrl);
        }

        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
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

        #region Password recovery

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecovery()
        {
            var model = _customerModelFactory.PreparePasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerService.GetCustomerByEmail(model.Email, _storeContext.CurrentStore.Id);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(customer,
                        SystemCustomerAttributeNames.PasswordRecoveryTokenDateGenerated, generatedDateTime);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                        _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);
            if (customer == null)
                return RedirectToRoute("HomePage");

            if (string.IsNullOrEmpty(customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken)))
            {
                return View(new PasswordRecoveryConfirmModel
                {
                    DisablePasswordChanging = true,
                    Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged")
                });
            }

            var model = _customerModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [PublicAntiForgery]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);
            if (customer == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(email, _storeContext.CurrentStore.Id,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword), null);
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken,
                        "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Register

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Register()
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model = _customerModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, FormCollection form)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;

            //custom customer attributes
            var customerAttributesXml = form.ParseCustomCustomerAttributes(_customerAttributeParser,_customerAttributeService);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService
                    .GetAllConsents().Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
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
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName,
                            out vatAddress);
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumberStatusId, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
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
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.County, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
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
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);


                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
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

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (_customerSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        {
                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!string.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
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
                        CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0
                            ? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)
                            : null,
                        StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0
                            ? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId)
                            : null,
                        County = customer.GetAttribute<string>(SystemCustomerAttributeNames.County),
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
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);

                    _customerActionEventService.Registration(customer);

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
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareRegisterModel(model, true, customerAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult RegisterResult(int resultId)
        {
            var model = _customerModelFactory.PrepareRegisterResultModel(resultId);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        [HttpPost]
        public virtual ActionResult RegisterResult(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator.IsValid(username, _customerSettings))
            {
                statusText = _localizationService.GetResource("Account.Fields.Username.NotValid");
            }
            else if (_customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentCustomer != null &&
                    _workContext.CurrentCustomer.Username != null &&
                    _workContext.CurrentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = _customerService.GetCustomerByUsername(username);
                    if (customer == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult AccountActivation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email, _storeContext.CurrentStore.Id);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            var model = new AccountActivationModel();
            model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account / Info

        [ChildActionOnly]
        public virtual ActionResult CustomerNavigation(int selectedTabId = 0)
        {
            var model = _customerModelFactory.PrepareCustomerNavigationModel(selectedTabId);
            return PartialView(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerInfoModel();
            model = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
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

                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }


            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //ensure it's our record
            var ear = _openAuthenticationService.GetExternalIdentifiersFor(_workContext.CurrentCustomer)
                .FirstOrDefault(x => x.Id == id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            _openAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
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

        #region My account / Addresses

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Addresses(int? page)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _customerModelFactory.PrepareCustomerAddressListModel(page);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerAddresses"),
            });
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public virtual ActionResult AddressAdd(CustomerAddressEditModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public virtual ActionResult AddressEdit(CustomerAddressEditModel model, int addressId, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_customerSettings.HideDownloadableProductsTab)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareCustomerDownloadableProductsModel();
            return View(model);
        }

        public virtual ActionResult UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("HomePage");

            var product = orderItem.Product;
            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("HomePage");

            var model = _customerModelFactory.PrepareUserAgreementModel(orderItem, product);
            return View(model);
        }

        #endregion

        #region My account / Change password

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ChangePassword()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _customerModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_workContext.CurrentCustomer.PasswordIsExpired())
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email, _storeContext.CurrentStore.Id,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest, null);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Avatar()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerAvatarModel();
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("upload-avatar")]
        public virtual ActionResult UploadAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        byte[] customerPictureBinary = uploadedFile.GetPictureBits();
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
                    }

                    int customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("remove-avatar")]
        public virtual ActionResult RemoveAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

            return RedirectToRoute("CustomerAvatar");
        }

        #endregion

        #region My account / Auctions

        public virtual ActionResult Auctions()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_customerSettings.HideAuctionsTab)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareCustomerAuctionsModel(_workContext.CurrentCustomer.Id, _workContext.WorkingLanguage.Id);

            return View(model);
        }

        #endregion

        #region My account / TwoFactorAuth

        public virtual ActionResult EnableTwoFactorAuthenticator()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.TwoFactorAuthenticationEnabled)
                return RedirectToRoute("CustomerInfo");

            if (_workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.TwoFactorEnabled))
                return RedirectToRoute("CustomerInfo");

            var secretkey = Guid.NewGuid().ToString();
            var setupInfo = _twoFactorAuthenticationService.GenerateCodeSetup(secretkey, _workContext.CurrentCustomer, _workContext.WorkingLanguage, _customerSettings.TwoFactorAuthenticationType);
            var model = new CustomerInfoModel.TwoFactorAuthenticationModel
            {
                CustomValues = setupInfo.CustomValues,
                SecretKey = secretkey,
                TwoFactorAuthenticationType = _customerSettings.TwoFactorAuthenticationType
            };
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult EnableTwoFactorAuthenticator(CustomerInfoModel.TwoFactorAuthenticationModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.TwoFactorAuthenticationEnabled)
                return RedirectToRoute("CustomerInfo");

            if (_workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.TwoFactorEnabled))
                return RedirectToRoute("CustomerInfo");

            if (string.IsNullOrEmpty(model.Code))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Account.TwoFactorAuth.SecurityCodeIsRequired"));
            }
            else
            {
                if ( _twoFactorAuthenticationService.AuthenticateTwoFactor(model.SecretKey, model.Code, _workContext.CurrentCustomer, _customerSettings.TwoFactorAuthenticationType))
                {
                     _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.TwoFactorEnabled, true);
                     _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.TwoFactorSecretKey, model.SecretKey);

                    SuccessNotification(_localizationService.GetResource("Account.TwoFactorAuth.Enabled"));

                    return RedirectToRoute("CustomerInfo");
                }
                ModelState.AddModelError("", _localizationService.GetResource("Account.TwoFactorAuth.WrongSecurityCode"));
            }

            return View(model);
        }


        public virtual ActionResult DisableTwoFactorAuthenticator()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.TwoFactorAuthenticationEnabled)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;
            if (customer != null)
            {
                 _genericAttributeService.SaveAttribute<bool>(customer, SystemCustomerAttributeNames.TwoFactorEnabled, false);
                 _genericAttributeService.SaveAttribute<string>(customer, SystemCustomerAttributeNames.TwoFactorSecretKey, null);

                SuccessNotification(_localizationService.GetResource("Account.TwoFactorAuth.Disabled"));

                return RedirectToRoute("CustomerInfo");
            }
            return View();
        }


        #endregion

        #region Login/Register


        public virtual ActionResult LoginRegister(bool? checkoutAsGuest)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new LoginRegisterModel();
            var Register = new RegisterModel();
            var Login = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            model.RegisterModel = _customerModelFactory.PrepareRegisterModel(Register, false, setDefaultValues: true);
            model.LoginModel = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }


        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult LoginRegister(LoginRegisterModel model, string returnUrl, bool captchaValid, FormCollection form, bool? checkoutAsGuest)
        {
            var Register = new RegisterModel();
            var customer = _workContext.CurrentCustomer;
            if (model.Type == 1)
            {
                //check whether registration is allowed
                if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                    return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

                if (_workContext.CurrentCustomer.IsRegistered())
                {
                    //Already registered customer. 
                    _authenticationService.SignOut();

                    //raise logged out event       
                    _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                    //Save a new record
                    _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
                }

                //custom customer attributes
                var customerAttributesXml = form.ParseCustomCustomerAttributes(_customerAttributeParser, _customerAttributeService);
                var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }

                //validate CAPTCHA
                if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
                {
                    ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
                }

                //GDPR
                if (_gdprSettings.GdprEnabled)
                {
                    var consents = _gdprService
                        .GetAllConsents().Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                    ValidateRequiredConsents(consents, form);
                }

                if (ModelState.IsValid)
                {
                    if (_customerSettings.UsernamesEnabled && model.RegisterModel.Username != null)
                    {
                        model.RegisterModel.Username = model.RegisterModel.Username.Trim();
                    }

                    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                    var registrationRequest = new CustomerRegistrationRequest(customer,
                        model.RegisterModel.Email,
                        _customerSettings.UsernamesEnabled ? model.RegisterModel.Username : model.RegisterModel.Email,
                        model.RegisterModel.Password,
                        _customerSettings.DefaultPasswordFormat,
                        _storeContext.CurrentStore.Id,
                        isApproved);
                    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        //properties
                        if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                        {
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.RegisterModel.TimeZoneId);
                        }
                        //VAT number
                        if (_taxSettings.EuVatEnabled)
                        {
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumber, model.RegisterModel.VatNumber);

                            string vatName;
                            string vatAddress;
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.RegisterModel.VatNumber, out vatName,
                                out vatAddress);
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.VatNumberStatusId, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!String.IsNullOrEmpty(model.RegisterModel.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.RegisterModel.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                        }

                        //form fields
                        if (_customerSettings.GenderEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.RegisterModel.Gender);
                        if (_customerSettings.FirstNameEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.RegisterModel.FirstName);
                        if (_customerSettings.LastNameEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.RegisterModel.LastName);
                        if (_customerSettings.DateOfBirthEnabled)
                        {
                            DateTime? dateOfBirth = model.RegisterModel.ParseDateOfBirth();
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                        }
                        if (_customerSettings.CompanyEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.RegisterModel.Company);
                        if (_customerSettings.StreetAddressEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.RegisterModel.StreetAddress);
                        if (_customerSettings.StreetAddress2Enabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.RegisterModel.StreetAddress2);
                        if (_customerSettings.ZipPostalCodeEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.RegisterModel.ZipPostalCode);
                        if (_customerSettings.CityEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.RegisterModel.City);
                        if (_customerSettings.CountyEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.County, model.RegisterModel.County);
                        if (_customerSettings.CountryEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.RegisterModel.CountryId);
                        if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId,
                                model.RegisterModel.StateProvinceId);
                        if (_customerSettings.PhoneEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.RegisterModel.Phone);
                        if (_customerSettings.FaxEnabled)
                            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.RegisterModel.Fax);

                        //newsletter
                        if (_customerSettings.NewsletterEnabled)
                        {
                            //save newsletter value
                            var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.RegisterModel.Email, _storeContext.CurrentStore.Id);
                            if (newsletter != null)
                            {
                                if (model.RegisterModel.Newsletter)
                                {
                                    newsletter.Active = true;
                                    _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);


                                    //GDPR
                                    if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                    {
                                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                    }
                                }
                                //else
                                //{
                                //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                                //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                                //}
                            }
                            else
                            {
                                if (model.RegisterModel.Newsletter)
                                {
                                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                    {
                                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                        Email = model.RegisterModel.Email,
                                        Active = true,
                                        StoreId = _storeContext.CurrentStore.Id,
                                        CreatedOnUtc = DateTime.UtcNow
                                    });

                                    //GDPR
                                    if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                    {
                                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                    }
                                }
                            }
                        }

                        if (_customerSettings.AcceptPrivacyPolicyEnabled)
                        {
                            //privacy policy is required
                            //GDPR
                            if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                            {
                                _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                            }
                        }

                        //GDPR
                        if (_gdprSettings.GdprEnabled)
                        {
                            var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                            foreach (var consent in consents)
                            {
                                var controlId = $"consent{consent.Id}";
                                var cbConsent = form[controlId];
                                if (!string.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                                {
                                    //agree
                                    _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                                }
                                else
                                {
                                    //disagree
                                    _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
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
                            CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId) > 0
                                ? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId)
                                : null,
                            StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId) > 0
                                ? (int?)customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId)
                                : null,
                            County = customer.GetAttribute<string>(SystemCustomerAttributeNames.County),
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
                            _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                                _localizationSettings.DefaultAdminLanguageId);

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
                                    return RedirectToRoute("RegisterResult",
                                        new { resultId = (int)UserRegistrationType.EmailValidation });
                                }
                            case UserRegistrationType.AdminApproval:
                                {
                                    return RedirectToRoute("RegisterResult",
                                        new { resultId = (int)UserRegistrationType.AdminApproval });
                                }
                            case UserRegistrationType.Standard:
                                {
                                    //send customer welcome message
                                    _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                    var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                        redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                    return Redirect(redirectUrl);
                                }
                            default:
                                {
                                    return RedirectToRoute("HomePage");
                                }
                        }
                    }

                    //errors
                    foreach (var error in registrationResult.Errors)
                        ModelState.AddModelError("", error);

                    model.RegisterModel = _customerModelFactory.PrepareRegisterModel(Register, true);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (_customerSettings.UsernamesEnabled && model.LoginModel.Username != null)
                    {
                        model.LoginModel.Username = model.LoginModel.Username.Trim();
                    }
                    var loginResult =
                        _customerRegistrationService.ValidateCustomer(
                            _customerSettings.UsernamesEnabled ? model.LoginModel.Username : model.LoginModel.Email, model.LoginModel.Password, _storeContext.CurrentStore.Id);
                    switch (loginResult)
                    {
                        case CustomerLoginResults.Successful:
                            {
                                var customerlogin = _customerSettings.UsernamesEnabled
                                    ? _customerService.GetCustomerByUsername(model.LoginModel.Username, _storeContext.CurrentStore.Id)
                                    : _customerService.GetCustomerByEmail(model.LoginModel.Email, _storeContext.CurrentStore.Id);

                                //migrate shopping cart
                                _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customerlogin, true);

                                //sign in new customer
                                _authenticationService.SignIn(customerlogin, model.LoginModel.RememberMe);

                                //raise event       
                                _eventPublisher.Publish(new CustomerLoggedinEvent(customerlogin));

                                //activity log
                                _customerActivityService.InsertActivity(customerlogin, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

                                if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                    return RedirectToRoute("HomePage");

                                return Redirect(returnUrl);
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
                model.LoginModel = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
                model.RegisterModel = _customerModelFactory.PrepareRegisterModel(Register, false, setDefaultValues: true);
                return View(model);
            }


            //If we got this far, something failed, redisplay form
            var Login = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            model.RegisterModel = _customerModelFactory.PrepareRegisterModel(Register, false, setDefaultValues: true);
            model.LoginModel = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        #endregion

        #region SellerLogin

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SellerLogin()
        {
            var model = _customerModelFactory.PrepareSellerLoginModel();
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SellerLogin(LoginModel model, bool captchaValid)
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

                            return Redirect("ApplyVendorAccount");
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
            model = _customerModelFactory.PrepareSellerLoginModel();
            return View(model);
        }
        #endregion

        #region SellerRegister

        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SellerRegister()
        {

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                return RedirectToRoute("ApplyVendorAccount");
            }
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new SellerRegisterModel();
            model = _customerModelFactory.PrepareSellerRegisterModel(model);

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult SellerRegister(SellerRegisterModel model, bool captchaValid, FormCollection form)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

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

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
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
                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    TryAssociateAccountWithExternalAccount(customer);

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);

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
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                return RedirectToRoute("ApplyVendorAccount");

                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareSellerRegisterModel(model);
            return View(model);
        }
        #endregion

        #region GDPR tools

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult GdprTools()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareGdprToolsModel();
            return View(model);
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("export-data")]
        public virtual ActionResult GdprToolsExport()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.ExportData, _localizationService.GetResource("Gdpr.Exported"));

            //export
            var bytes = _exportManager.ExportCustomerGdprInfoToXlsx(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            return File(bytes, MimeTypes.TextXlsx, "customerdata.xlsx");
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("delete-account")]
        public virtual ActionResult GdprToolsDelete()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.DeleteCustomer, _localizationService.GetResource("Gdpr.DeleteRequested"));

            var model = _customerModelFactory.PrepareGdprToolsModel();
            model.Result = _localizationService.GetResource("Gdpr.DeleteRequested.Success");
            return View(model);
        }

        #endregion

        #region Check gift card balance

        //check gift card balance page
        [NopHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult CheckGiftCardBalance()
        {
            if (!(_customerSettings.AllowCustomersToCheckGiftCardBalance))
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _customerModelFactory.PrepareCheckGiftCardBalanceModel();
            return View(model);
        }

        [HttpPost, ActionName("CheckGiftCardBalance")]
        [FormValueRequired("checkbalancegiftcard")]
        [CaptchaValidator]
        public virtual ActionResult CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: model.GiftCardCode).FirstOrDefault();
                if (giftCard != null && _giftCardService.IsGiftCardValid(giftCard))
                {
                    var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_giftCardService.GetGiftCardRemainingAmount(giftCard), _workContext.WorkingCurrency);
                    model.Result = _priceFormatter.FormatPrice(remainingAmount, true, false);
                }
                else
                {
                    model.Message = _localizationService.GetResource("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
                }
            }

            return View(model);
        }

        #endregion
    }
}
