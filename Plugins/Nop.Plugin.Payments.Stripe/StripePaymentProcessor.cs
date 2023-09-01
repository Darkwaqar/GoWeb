using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Stripe.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;
using Stripe;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Payments.Stripe
{

    /// <summary>
    /// Stripe payment processor
    /// </summary>
    public class StripePaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly StripePaymentSettings _stripePaymentSettings;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public StripePaymentProcessor(
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            StripePaymentSettings stripePaymentSettings,
            ICustomerService customerService)
        {
            _currencyService = currencyService;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _stripePaymentSettings = stripePaymentSettings;
            _customerService = customerService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Set up for a call to the Stripe API
        /// </summary>
        /// <returns></returns>
        private RequestOptions GetStripeApiRequestOptions()
        {
            return new RequestOptions
            {
                ApiKey = _stripePaymentSettings.SecretKey,
                IdempotencyKey = Guid.NewGuid().ToString()
            };
        }


        /// <summary>
        /// Perform a shallow validation of a stripe token
        /// </summary>
        /// <param name="stripeTokenObj"></param>
        /// <returns></returns>
        private bool IsPaymentIntentID(string token)
        {
            return token.StartsWith("pi_");
        }
    
        #endregion

        #region Methods

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new StripePaymentSettings
            {
                AdditionalFee = 0,
                AdditionalFeePercentage = false
            });


            //locales            
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.PaymentMethodDescription", "Pay with Credit Card");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.SecretKey", "Secret key, live or test (starts with sk_)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.PublishableKey", "Publishable key, live or test (starts with pk_)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.StripeToken.Key", "Stripe Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.Receipt.Url", "Stripe Receipt");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Stripe.Instructions", @"
                <p>
                     For plugin configuration follow these steps:<br />
                    <br />
                    1. If you haven't already, create an account on Stripe.com and sign in<br />
                    2. In the Developers menu (left), choose the API Keys option.
                    3. You will see two keys listed, a Publishable key and a Secret key. You will need both. (If you'd like, you can create and use a set of restricted keys. That topic isn't covered here.)
                    <em>Stripe supports test keys and production keys. Use whichever pair is appropraite. There's no switch between test/sandbox and proudction other than using the appropriate keys.</em>
                    4. Paste these keys into the configuration page of this plug-in. (Both keys are required.) 
                    <br />
                    <em>Note: If using production keys, the payment form will only work on sites hosted with HTTPS. (Test keys can be used on http sites.) If using test keys, 
                    use these <a href='https://stripe.com/docs/testing'>test card numbers from Stripe</a>.</em><br />
                </p>");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<StripePaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.SecretKey");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.PublishableKey");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Instructions");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.StripeToken.Key");
            this.DeletePluginLocaleResource("Plugins.Payments.Stripe.Fields.Receipt.Url");

            base.Uninstall();
        }


        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Core.Domain.Orders.Order order)
        {
            return true;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            string chargeID = refundPaymentRequest.Order.AuthorizationTransactionId;
            var orderAmtRemaining = refundPaymentRequest.Order.OrderTotal - refundPaymentRequest.AmountToRefund;
            bool isPartialRefund = orderAmtRemaining > 0;

            if (!IsPaymentIntentID(chargeID))
            {
                throw new NopException($"Refund error: {chargeID} is not a Stripe Charge ID. Refund cancelled");
            }
            var service = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                Charge = chargeID,
                Amount = (long)(refundPaymentRequest.AmountToRefund * 100),
                Reason = RefundReasons.RequestedByCustomer
            };
            var refund = service.Create(refundOptions, GetStripeApiRequestOptions());

            RefundPaymentResult result = new RefundPaymentResult();

            switch (refund.Status)
            {
                case "succeeded":
                    result.NewPaymentStatus = isPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded;
                    break;

                case "pending":
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    result.AddError($"Refund failed with status of ${ refund.Status }");
                    break;

                default:
                    throw new NopException("Refund returned a status of ${refund.Status}");
            }
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }


        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            //get customer
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new NopException("Customer cannot be loaded");

            string tokenKey = _localizationService.GetResource("Plugins.Payments.Stripe.Fields.StripeToken.Key");
            if (!processPaymentRequest.CustomValues.TryGetValue(tokenKey, out object stripeTokenObj) || !(stripeTokenObj is string) || !IsPaymentIntentID((string)stripeTokenObj))
            {
                throw new NopException("Card token not received");
            }
            string stripeToken = stripeTokenObj.ToString();

            var service = new PaymentIntentService();
            var paymentIntent = service.Get(stripeToken,null,GetStripeApiRequestOptions());
               //throw new NopException(paymentIntent.ToString());
            var options = new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                { "order_id", processPaymentRequest.OrderGuid.ToString() },
                },
                Description = string.Format(StripePaymentDefaults.PaymentNote, processPaymentRequest.OrderGuid),
                ReceiptEmail = customer.Email,
            };
            service.Update(stripeToken, options,GetStripeApiRequestOptions());

            var result = new ProcessPaymentResult();
            if (paymentIntent.Status == "succeeded")
            {
                result.NewPaymentStatus = PaymentStatus.Paid;
                result.AuthorizationTransactionId = paymentIntent.Id;
                result.AuthorizationTransactionResult = $"Transaction was processed by using {paymentIntent.Charges.Data?[0].PaymentMethodDetails.Card.Brand}. Status is {paymentIntent.Status}";
                processPaymentRequest.CustomValues.Add(_localizationService.GetResource("Plugins.Payments.Stripe.Fields.Receipt.Url"),string.Format("<a href="+ paymentIntent.Charges.Data?[0].ReceiptUrl+ ">See Stripe Receipt</a>"));
                return result;
            }
            else
            {
                throw new NopException($"Charge error: {paymentIntent.Charges.Data?[0].Outcome.SellerMessage ?? paymentIntent.Status }");
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.Stripe.PaymentMethodDescription"); }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _stripePaymentSettings.AdditionalFee, _stripePaymentSettings.AdditionalFeePercentage);
            return result;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentStripe";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.Stripe.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentStripe";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.Stripe.Controllers" }, { "area", null } };
        }


        /// <summary>
        /// Get type of controller
        /// </summary>
        /// <returns>Type</returns>
        public Type GetControllerType()
        {
            return typeof(PaymentStripeController);
        }
        #endregion
    }
}
