using Nop.Core.Plugins;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Tax;
using System.Web;
using Nop.Plugin.Payments.TelenorEasyPay.Controllers;
using Nop.Plugin.Payments.TelenorEasyPay.Data;
using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;
using System.Linq;
using WebAppEasyPay.Models;

namespace Nop.Plugin.Payments.TelenorEasyPay
{
    class TelenorEasyPayPaymentProcessor : BasePlugin, IPaymentMethod
    {

        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        private readonly EasyPayStatusObjectContext _dbContext;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly TelenorEasyPayPaymentSettings _easyPayPaymentSettings;



        #endregion

        #region Ctor

        public TelenorEasyPayPaymentProcessor(CurrencySettings currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeParser productAttributeParser,
            ISettingService settingService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            HttpContextBase httpContext,
            EasyPayStatusObjectContext dbContext,
            IScheduleTaskService scheduleTaskService,
            TelenorEasyPayPaymentSettings easyPayPaymentSettings)
        {
            this._currencySettings = currencySettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._paymentService = paymentService;
            this._priceCalculationService = priceCalculationService;
            this._productAttributeParser = productAttributeParser;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            this._dbContext = dbContext;
            this._scheduleTaskService = scheduleTaskService;
            this._easyPayPaymentSettings = easyPayPaymentSettings;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets EasyPay Post URL
        /// </summary>
        /// <returns></returns>
        private string GetPostUrl()
        {
            return _easyPayPaymentSettings.IsSandbox ? "https://easypaystg.easypaisa.com.pk/easypay/Index.jsf" :
                "https://easypay.easypaisa.com.pk/easypay/Index.jsf";
        }

        /// <summary>
        /// Gets EasyPay Receipt URL
        /// </summary>
        /// <returns></returns>
        private string GetReceiptUrl()
        {
            return _webHelper.GetStoreLocation(true) + "PaymentEasyPay/EasyPayReceipt";
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.Automatic; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Redirection; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.TelenorEasyPay.PaymentMethodDescription"); }
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentEasyPay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentEasyPayController);
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentEasyPay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.TelenorEasyPay.Controllers" }, { "area", null } };
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var urlToRedirect = GenerationRedirectionUrl(postProcessPaymentRequest,false);
            if (urlToRedirect == null)
                throw new Exception("EasyPay URL cannot be generated");

            if (HttpContext.Current.Session["ServicesRedirection"] != null)
            {
                urlToRedirect = GenerationRedirectionUrl(postProcessPaymentRequest,true);
                _httpContext.Session["RediredURL"] = urlToRedirect;
            }
            else
            {
                _httpContext.Response.Redirect(urlToRedirect);
            }
        }

        private string GenerationRedirectionUrl(PostProcessPaymentRequest postProcessPaymentRequest,bool RedirectToMobile)
        {
            string MerchantHashedkey = _easyPayPaymentSettings.MerchantHashed;
            var param = new Dictionary<string, string>();
            param.Add("amount", postProcessPaymentRequest.Order.OrderTotal.ToString("F1"));
            param.Add("autoRedirect", _easyPayPaymentSettings.AutoRedirect ? "1" : "0");
            param.Add("emailAddr", HttpUtility.UrlEncode(postProcessPaymentRequest.Order.Customer.Email));
            if (_easyPayPaymentSettings.IsMerchantTokenExpiry)
                param.Add("expiryDate", HttpUtility.UrlEncode(DateTime.UtcNow.AddDays(_easyPayPaymentSettings.ExpiryDate).ToString("yyyyMMdd HHMMss")));
            else
                param.Add("expiryDate", HttpUtility.UrlEncode(DateTime.UtcNow.AddDays(2).ToString("yyyyMMdd HHMMss")));
            param.Add("mobileNum", postProcessPaymentRequest.Order.BillingAddress.PhoneNumber);
            param.Add("orderRefNum", postProcessPaymentRequest.Order.Id.ToString());
            if (!_easyPayPaymentSettings.SelectedPaymentMethod.Equals(""))
            {
                param.Add("paymentMethod", _easyPayPaymentSettings.SelectedPaymentMethod);
            }
            if (RedirectToMobile)
                param.Add("postBackURL", _webHelper.GetStoreLocation(true) + "PaymentEasyPay/EasyPayConfirmation");
            else 
                param.Add("postBackURL", _webHelper.GetStoreLocation(true) + "PaymentEasyPay/EasyPayConfirmation");

            param.Add("storeId", _easyPayPaymentSettings.StoreId);

            string value= GetPostUrl();

            value = String.Concat(value,"?",string.Join("&", param.Select(x => x.Key + "=" + x.Value).ToArray()));

            string merchantHashed = EncryptData(value, MerchantHashedkey);

            return value= String.Concat(value,"&merchantHashedReq=",merchantHashed);
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        #endregion

        #region Install
        public override void Install()
        {
            //settings
            var settings = new TelenorEasyPayPaymentSettings
            {

            };
            _settingService.SaveSetting(settings);

            //locales

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId", "Store ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId.hint", "The Easypay Merchant Store Id that is provided to you by Telenor POC, this will be configured based on the environment you are using.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ExpiryDate", "Expiry Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ExpiryDate.hint", "Either the expiry is set by merchant or the default OPS expiry is used");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsMerchantTokenExpiry", "Merchant’s Token Expiry Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsMerchantTokenExpiry.hint", "Either the expiry is set by merchant or the default OPS expiry is used");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsSandbox.hint", "No to test Easypay, orders will be routed to staging environment and select Yes for production environment.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AutoRedirect", "Auto Redirect");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AutoRedirect.hint", "Either the merchant wants to redirect its customer to it's website at the end of order or not. If yes, then the customer placing the order will be redirected after 5 seconds to the final checkout screen.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaymentMethod", "Payment Method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaymentMethod.hint", "The method of payment (Credit Card, Mobile Account, Over the Counter or all three) the merchant wants to use for payments");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PostUrl", "Post Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PostBackUrl", "Post Back Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ConfirmUrl", "Confirm Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReceiptUrl", "Receipt Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.PaymentMethodDescription", "Easy Pay Transfer Money");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.MerchantHashed", "Merchant Hashed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.RedirectionTip", "You will be redirected to EasyPay site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.BusinessEmail", "Business Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.BusinessEmail.Hint", "Specify your PayPal business email.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTToken", "PDT Identity Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTToken.Hint", "Specify PDT identity token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTValidateOrderTotal", "PDT. Validate order total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTValidateOrderTotal.Hint", "Check if PDT handler should validate order totals.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn", "Enable IPN (Instant Payment Notification)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn.Hint", "Check if IPN is enabled.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn.Hint2", "Leave blank to use the default IPN handler URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IpnUrl", "IPN Handler");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IpnUrl.Hint", "Specify IPN Handler.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AddressOverride", "Address override");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AddressOverride.Hint", "For people who already have PayPal accounts and whom you already prompted for a shipping address before they choose to pay with PayPal, you can use the entered address instead of the address the person has stored with PayPal.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage", "Return to order details page");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on PayPal site WITHOUT completing a payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.OrderId", "OrderId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaymentToken", "Payment Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TokenExpiryDatetime", "TokenExpiry Datetime");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId", "StoreId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreName", "Store Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ResponseCode", "Response Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.OrderDatetime", "Order Datetime");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaidDatetime", "Paid Datetime");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionStatus", "Transaction Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionAmount", "Transaction Amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.Msisdn", "Msisdn");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AccountNumber", "Account Number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.Description", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionId", "Transaction Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UTCTime", "UTCTime");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.List", "List");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Updated", "Updated");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Added", "Added");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.ViewAllTransfers", "View All Transfers");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TelenorEasyPay.List.Payments", "List of Payments");

            _dbContext.Install();


            ScheduleTask easyPayTask = new ScheduleTask();
            easyPayTask.Enabled = true;
            easyPayTask.Name = "Easy Pay Get Payment Status";
            easyPayTask.Seconds = 3600;
            easyPayTask.Type = "Nop.Plugin.Payments.TelenorEasyPay.EasyPayScheduleTask, Nop.Plugin.Payments.TelenorEasyPay";
            easyPayTask.StopOnError = false;

            _scheduleTaskService.InsertTask(easyPayTask);

            base.Install();
        }

        #endregion

        #region Uninstall
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<TelenorEasyPayPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId.hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ExpiryDate");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ExpiryDate.hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsMerchantTokenExpiry");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsMerchantTokenExpiry.hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IsSandbox.hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AutoRedirect");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AutoRedirect.hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PostUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PostBackUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ConfirmUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReceiptUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.MerchantHashed");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.BusinessEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.BusinessEmail.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTToken");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTToken.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTValidateOrderTotal");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PDTValidateOrderTotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PassProductNamesAndTotals");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PassProductNamesAndTotals.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.EnableIpn.Hint2");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IpnUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.IpnUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AddressOverride");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AddressOverride.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.OrderId");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaymentToken");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TokenExpiryDatetime");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreId");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.StoreName");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.ResponseCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.OrderDatetime");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.PaidDatetime");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionStatus");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionAmount");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.Msisdn");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.AccountNumber");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.Description");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.TransactionId");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Fields.UTCTime");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.List");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Updated");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.Added");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.ViewAllTransfers");
            this.DeletePluginLocaleResource("Plugins.Payments.TelenorEasyPay.List.Payments");

            _dbContext.Uninstall();


            ScheduleTask easyPayTask = _scheduleTaskService.GetTaskByType("Nop.Plugin.Payments.TelenorEasyPay.EasyPayScheduleTask, Nop.Plugin.Payments.TelenorEasyPay");
            _scheduleTaskService.DeleteTask(easyPayTask);

            base.Uninstall();
        }
        #endregion

        #region EncryptData

        public string EncryptData(string textData, string Encryptionkey)
        {

            Cryptography Crypt = new Cryptography();
            string RequestparametersEncripted = "";
            RequestparametersEncripted = (string)Crypt.Crypt(CryptType.ENCRYPT, CryptTechnique.RIJ, textData, Encryptionkey);
            return RequestparametersEncripted;
        }

        #endregion
    }
}
