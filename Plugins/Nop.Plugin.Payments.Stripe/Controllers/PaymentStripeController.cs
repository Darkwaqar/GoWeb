using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Plugin.Payments.Stripe.Models;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Stripe;
using Nop.Services.Orders;
using Nop.Core.Domain.Tax;

namespace Nop.Plugin.Payments.Stripe.Controllers
{
    public class PaymentStripeController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly StripePaymentSettings _stripePaymentSettings;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSetting;
        private readonly TaxSettings _taxSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public PaymentStripeController(ILocalizationService localizationService,
            ISettingService settingService,
            StripePaymentSettings stripePaymentSettings,
            IStoreService storeService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            IOrderTotalCalculationService orderTotalCalculationService,
            IStoreContext storeContext)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._stripePaymentSettings = stripePaymentSettings;
            this._storeService = storeService;
            this._workContext = workContext;
            this._orderSetting = orderSettings;
            this._taxSettings = taxSettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._storeContext = storeContext;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Perform a shallow validation of a stripe token
        /// </summary>
        /// <param name="stripeTokenObj"></param>
        /// <returns></returns>
        private bool IsPaymentIntentID(string token)
        {
            return token.StartsWith("pi_");
        }

        private RequestOptions GetStripeApiRequestOptions()
        {
            return new RequestOptions
            {
                ApiKey = _stripePaymentSettings.SecretKey,
                IdempotencyKey = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Convert a NopCommere address to a Stripe API address
        /// </summary>
        /// <param name="nopAddress"></param>
        /// <returns></returns>
        private AddressOptions MapNopAddressToStripe(Core.Domain.Common.Address nopAddress)
        {
            return new AddressOptions
            {
                Line1 = nopAddress.Address1,
                City = nopAddress.City,
                State = nopAddress.StateProvince?.Abbreviation,
                PostalCode = nopAddress.ZipPostalCode,
                Country = nopAddress.Country.ThreeLetterIsoCode
            };
        }

        #endregion

        #region Methods

        [AdminAuthorize]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var stripePaymentSettings = _settingService.LoadSetting<StripePaymentSettings>(storeScope);
            var model = new ConfigurationModel();
            model.SecretKey = stripePaymentSettings.SecretKey;
            model.PublishableKey = stripePaymentSettings.PublishableKey;
            model.AdditionalFee = stripePaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = stripePaymentSettings.AdditionalFeePercentage;


            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.SecretKey_OverrideForStore = _settingService.SettingExists(stripePaymentSettings, x => x.SecretKey, storeScope);
                model.PublishableKey_OverrideForStore = _settingService.SettingExists(stripePaymentSettings, x => x.PublishableKey, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(stripePaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(stripePaymentSettings, x => x.AdditionalFeePercentage, storeScope);
            }

            return View("~/Plugins/Payments.Stripe/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult Configure(ConfigurationModel model)
        {

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var stripePaymentSettings = _settingService.LoadSetting<StripePaymentSettings>(storeScope);

            //save settings
            stripePaymentSettings.SecretKey = model.SecretKey;
            stripePaymentSettings.PublishableKey = model.PublishableKey;
            stripePaymentSettings.AdditionalFee = model.AdditionalFee;
            stripePaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(stripePaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(stripePaymentSettings, x => x.PublishableKey, model.PublishableKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(stripePaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(stripePaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }


        public ActionResult PaymentInfo(bool mobile = false)
        {
            //validation
            var customer = _workContext.CurrentCustomer;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                throw new Exception("Your cart is empty");


            var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                out var orderSubTotalDiscountAmountBase, out var orderSubTotalAppliedDiscounts,
                out var subTotalWithoutDiscountBase, out var subTotalWithDiscountBase);
            var total = _orderTotalCalculationService.GetShoppingCartTotal(cart)?? subTotalWithoutDiscountBase;

            var paymentIntents = new PaymentIntentService();
            var paymentIntentCreateOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)(total * 100),
                Currency = _workContext.WorkingCurrency.CurrencyCode,
            };
            if (customer.ShippingAddress != null)
            {
                paymentIntentCreateOptions.Shipping = new ChargeShippingOptions
                {
                    Address = MapNopAddressToStripe(customer.ShippingAddress),
                    Phone = customer.ShippingAddress.PhoneNumber,
                    Name = customer.ShippingAddress.FirstName + ' ' + customer.ShippingAddress.LastName
                };
            }

           var paymentIntent = paymentIntents.Create(paymentIntentCreateOptions, GetStripeApiRequestOptions());

            PaymentInfoModel model = new PaymentInfoModel
            {
                //whether current customer is guest
                IsGuest = _workContext.CurrentCustomer.IsGuest(),
                PostalCode = _workContext.CurrentCustomer.BillingAddress?.ZipPostalCode ?? _workContext.CurrentCustomer.ShippingAddress?.ZipPostalCode,
                OnePageCheckoutEnabled = _orderSetting.OnePageCheckoutEnabled,
                PublishableKey = _stripePaymentSettings.PublishableKey,
                ClientSecret = paymentIntent.ClientSecret,
            };

            if (mobile)
                return Json(model, JsonRequestBehavior.AllowGet);
            else
                return View("~/Plugins/Payments.Stripe/Views/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            IList<string> errors = new List<string>();

            var stripeToken = form.GetValues("stripeToken");
            if (stripeToken.Count() != 1 || !IsPaymentIntentID(stripeToken[0]))
            {
                errors.Add("Payment Intent Id was not supplied or invalid");
            }

            return errors;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentRequest = new ProcessPaymentRequest();
            var stripeToken = form["stripeToken"];
            if (!String.IsNullOrEmpty(stripeToken))
                paymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Stripe.Fields.StripeToken.Key"), stripeToken.ToString());
            //if (form.TryGetValue("stripeToken", out stripeToken) && !StringValues.IsNullOrEmpty(stripeToken))

            return paymentRequest;
        }


        #endregion
    }
}
