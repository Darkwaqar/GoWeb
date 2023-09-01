using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using Nop.Services.Payments;
using System.Web.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Plugin.Payments.TelenorEasyPay.Models;
using Nop.Services.Orders;
using System;
using Nop.Plugin.Payments.TelenorEasyPay.Domain;
using Nop.Plugin.Payments.TelenorEasyPay.Services;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Orders;
using System.Linq;
using Nop.Services.Catalog;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Helpers;

namespace Nop.Plugin.Payments.TelenorEasyPay.Controllers
{
    public class PaymentEasyPayController : BasePaymentController
    {

        #region Fields
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly TelenorEasyPayPaymentSettings _easyPayPaymentSettings;
        private readonly IEasyPayResponseService _easyPayResponseService;
        private readonly IEasyPayPaymentStatusUrlService _easyPayPaymentStatusUrlService;
        private readonly IEasyPayPaymentStatusService _easyPayPaymentStatusService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        #endregion

        #region Constructor

        public PaymentEasyPayController(
          IWorkContext workContext,
          IStoreService storeService,
          ISettingService settingService,
          ILocalizationService localizationService,
          IOrderService orderService,
          TelenorEasyPayPaymentSettings easyPayPaymentSettings,
          IEasyPayResponseService easyPayResponseService,
          IEasyPayPaymentStatusUrlService easyPayStatusUrlService,
          IEasyPayPaymentStatusService easyPayStatusService,
          IOrderProcessingService orderProcessingService,
          IProductService productService,
          ICategoryService categoryService,
          ILogger logger,
          IWebHelper webHelper,
          IPermissionService permissionService,
          IDateTimeHelper dateTimeHelper)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._orderService = orderService;
            this._easyPayPaymentSettings = easyPayPaymentSettings;
            this._easyPayResponseService = easyPayResponseService;
            this._easyPayPaymentStatusUrlService = easyPayStatusUrlService;
            this._easyPayPaymentStatusService = easyPayStatusService;
            this._orderProcessingService = orderProcessingService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Configure

        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var easyPayPaymentSettings = _settingService.LoadSetting<TelenorEasyPayPaymentSettings>(storeScope);
            var model = new ConfigurationModel();
            AddPaymentMethods(model);
            model.StoreId = easyPayPaymentSettings.StoreId;
            model.ExpiryDate = easyPayPaymentSettings.ExpiryDate;
            model.IsMerchantTokenExpiry = easyPayPaymentSettings.IsMerchantTokenExpiry;
            model.IsSandbox = easyPayPaymentSettings.IsSandbox;
            model.AutoRedirect = easyPayPaymentSettings.AutoRedirect;
            model.SelectedPaymentMethod = easyPayPaymentSettings.SelectedPaymentMethod;
            model.MerchantHashed = easyPayPaymentSettings.MerchantHashed;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.StoreId_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.StoreId, storeScope);
                model.ExpiryDate_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.ExpiryDate, storeScope);
                model.IsMerchantTokenExpiry_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.IsMerchantTokenExpiry, storeScope);
                model.IsSandbox_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.IsSandbox, storeScope);
                model.AutoRedirect_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.AutoRedirect, storeScope);
                model.SelectedPaymentMethod_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.SelectedPaymentMethod, storeScope);
                model.MerchantHashed_OverrideForStore = _settingService.SettingExists(easyPayPaymentSettings, x => x.MerchantHashed, storeScope);
            }
            return View("~/Plugins/Payments.TelenorEasyPay/Views/PaymentEasyPay/Configure.cshtml", model);
        }


        [AdminAuthorize]
        [ChildActionOnly]
        [HttpPost]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var easyPayPaymentSettings = _settingService.LoadSetting<TelenorEasyPayPaymentSettings>(storeScope);
            AddPaymentMethods(model);

            //save settings
            easyPayPaymentSettings.StoreId = model.StoreId;
            easyPayPaymentSettings.ExpiryDate = model.ExpiryDate;
            easyPayPaymentSettings.IsMerchantTokenExpiry = model.IsMerchantTokenExpiry;
            easyPayPaymentSettings.IsSandbox = model.IsSandbox;
            easyPayPaymentSettings.AutoRedirect = model.AutoRedirect;
            easyPayPaymentSettings.SelectedPaymentMethod = model.SelectedPaymentMethod;
            easyPayPaymentSettings.MerchantHashed = model.MerchantHashed;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.StoreId, model.StoreId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.ExpiryDate, model.ExpiryDate_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.IsMerchantTokenExpiry, model.IsMerchantTokenExpiry_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.IsSandbox, model.IsSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.AutoRedirect, model.AutoRedirect_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.SelectedPaymentMethod, model.SelectedPaymentMethod_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.AvaliblePaymentMethod, model.SelectedPaymentMethod_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(easyPayPaymentSettings, x => x.MerchantHashed, model.MerchantHashed_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        [AdminAuthorize]
        [ChildActionOnly]
        private void AddPaymentMethods(ConfigurationModel model)
        {
            model.AvaliblePaymentMethod.Add(new SelectListItem
            {
                Text = "All",
                Value = "",
            });
            model.AvaliblePaymentMethod.Add(new SelectListItem
            {
                Text = "Credit Card",
                Value = "CC_PAYMENT_METHOD",
            });
            model.AvaliblePaymentMethod.Add(new SelectListItem
            {
                Text = "Mobile Account",
                Value = "MA_PAYMENT_METHOD",
            });
            model.AvaliblePaymentMethod.Add(new SelectListItem
            {
                Text = "Over the counter",
                Value = "OTC_PAYMENT_METHOD",
            });

            var SelectedPaymentMethod = model.AvaliblePaymentMethod.FirstOrDefault(x => x.Value.Equals(model.SelectedPaymentMethod, StringComparison.InvariantCultureIgnoreCase));
            if (SelectedPaymentMethod != null)
                SelectedPaymentMethod.Selected = true;
        }


        #endregion

        #region PaymentInfo
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            return new List<string>();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.TelenorEasyPay/Views/PaymentEasyPay/PaymentInfo.cshtml");
        }

        #endregion

        #region EasyPayConfirmation
        public ActionResult EasyPayConfirmation()
        {
            ViewBag.auth_token = Request.Params["auth_token"];
            ViewBag.ReceiptUrl = _webHelper.GetStoreLocation(true) + "PaymentEasyPay/EasyPayReceipt";
            ViewBag.ConfirmUrl = "https://easypay.easypaisa.com.pk/easypay/Confirm.jsf";
            return View("~/Plugins/Payments.TelenorEasyPay/Views/PaymentEasyPay/EasyPayConfirmation.cshtml");
        }
        #endregion

        #region EasyPayReceipt
        public ActionResult EasyPayReceipt()
        {
            var rawRequest = Request.Url.AbsoluteUri;
            EasyPayResponse response = new EasyPayResponse();
            response.OrderId = Convert.ToInt32(Request["orderRefNumber"]);
            response.RawData = rawRequest;
            response.UTCTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(Request["paymentToken"]))
                response.IsOtcPayment = true;
            _easyPayResponseService.InsertEasyPayResponse(response);
            return RedirectToRoute("CheckoutCompleted", new { orderId = response.OrderId });
        }
        #endregion

        #region EasyPayIPNListener

        public string EasyPayIPNListener(string url)
        {
            Boolean isOk = false;

            string orderId = "0", xurl = "no-url";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    xurl = url;
                    string[] splitUrl = url.Split(new char[] { '/' });
                    orderId = splitUrl[splitUrl.Length - 1];

                    isOk = true;
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc.Message);
                    _logger.Error(exc.Message, exc);
                }
            }

            EasyPayPaymentStatusUrl ipnResponse = new EasyPayPaymentStatusUrl();
            ipnResponse.OrderId = int.Parse(orderId);
            ipnResponse.Url = xurl;
            ipnResponse.UTCTime = DateTime.UtcNow;

            _easyPayPaymentStatusUrlService.Log(ipnResponse);

            if (isOk)
            {
                GetIpnResponse(xurl);
            }

            return "1";
        }

        #endregion

        #region GetIpnResponse

        [NonAction]
        private void GetIpnResponse(String url)
        {
            Boolean isOk = false;

            EasyPayPaymentStatus result = new EasyPayPaymentStatus();
            result.UTCTime = DateTime.UtcNow;

            var response = RequestWeb(url);
            result.RawData = response;

            try
            {
                EasyPayIpnRawResult parsedResult = JsonConvert.DeserializeObject<EasyPayIpnRawResult>(response);

                if (!string.IsNullOrWhiteSpace(parsedResult.account_number))
                    result.AccountNumber = parsedResult.account_number;
                if (!string.IsNullOrWhiteSpace(parsedResult.description))
                    result.Description = parsedResult.description;
                if (!string.IsNullOrWhiteSpace(parsedResult.msisdn))
                    result.Msisdn = parsedResult.msisdn;
                if (!string.IsNullOrWhiteSpace(parsedResult.order_datetime))
                    result.OrderDatetime = parsedResult.order_datetime;
                if (!string.IsNullOrWhiteSpace(parsedResult.order_id))
                    result.OrderId = int.Parse(parsedResult.order_id);
                if (!string.IsNullOrWhiteSpace(parsedResult.paid_datetime))
                    result.PaidDatetime = parsedResult.paid_datetime;
                if (!string.IsNullOrWhiteSpace(parsedResult.payment_method))
                    result.PaymentMethod = parsedResult.payment_method;
                if (!string.IsNullOrWhiteSpace(parsedResult.payment_token))
                    result.PaymentToken = parsedResult.payment_token;
                if (!string.IsNullOrWhiteSpace(parsedResult.response_code))
                    result.ResponseCode = parsedResult.response_code;
                if (!string.IsNullOrWhiteSpace(parsedResult.store_id))
                    result.StoreId = int.Parse(parsedResult.store_id);
                if (!string.IsNullOrWhiteSpace(parsedResult.store_name))
                    result.StoreName = parsedResult.store_name;
                if (!string.IsNullOrWhiteSpace(parsedResult.token_expiry_datetime))
                    result.TokenExpiryDatetime = parsedResult.token_expiry_datetime;
                if (!string.IsNullOrWhiteSpace(parsedResult.transaction_amount))
                    result.TransactionAmount = decimal.Parse(parsedResult.transaction_amount);
                if (!string.IsNullOrWhiteSpace(parsedResult.transaction_id))
                    result.TransactionId = parsedResult.transaction_id;
                if (!string.IsNullOrWhiteSpace(parsedResult.transaction_status))
                    result.TransactionStatus = parsedResult.transaction_status;

                isOk = true;

            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                _logger.Error(exc.Message, exc);
            }

            _easyPayPaymentStatusService.InsertEasyPayPaymentStatus(result);


            if (isOk)
            {
                //mark order paid
                if (result.ResponseCode == "0000" && result.TransactionStatus.ToLower() == "paid")
                {
                    var order = _orderService.GetOrderById(result.OrderId);

                    if (order != null)
                    {
                        try
                        {
                            if (order.PaymentStatus != PaymentStatus.Paid)
                                _orderProcessingService.MarkOrderAsPaid(order);
                        }
                        catch (Exception exc)
                        {
                            ErrorNotification(exc.Message);
                            _logger.Error(exc.Message, exc);
                        }
                    }
                }
            }


        }

        #endregion

        #region callEasyPayIpnApiInLoop

        [NonAction]
        private void callEasyPayIpnApiInLoop()
        {
            try
            {
                //last 4 days payments
                var resp = _easyPayResponseService.GetOTCPayments(4);
                if (resp != null && resp.Count > 0)
                {
                    foreach (EasyPayResponse item in resp)
                    {
                        int oid = item.OrderId;

                        Order tempOrder = _orderService.GetOrderById(oid);
                        if (tempOrder.PaymentStatus == PaymentStatus.Pending)
                        {
                            var statusUrlObj = _easyPayPaymentStatusUrlService.GetEasyPayPaymentStatusUrlByOrderId(oid);
                            if (statusUrlObj != null)
                            {
                                GetIpnResponse(statusUrlObj.Url);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                _logger.Error(exc.Message, exc);
            }
            finally
            {

            }
        }

        #endregion

        #region "helperMethods"

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }

        public string GetLanIPAddress()
        {
            //The X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a 
            //client connecting to a web server through an HTTP proxy or load balancer
            var ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

        public string GetProductNames(Order order)
        {
            var productIds = (from item in order.OrderItems
                              select item.ProductId).ToArray();
            var products = _productService.GetProductsByIds(productIds);
            var productsNames = from p in products
                                select p.Name;
            var strProductNames = string.Join(",", productsNames.ToList());

            return strProductNames;
        }

        public int NoOfItemsSold(Order order)
        {
            var totalQuantity = (from item in order.OrderItems
                                 select item.Quantity).Sum();


            return totalQuantity;
        }

        public string GetProductCategories(Order order)
        {
            var productIds = (from item in order.OrderItems
                              select item.ProductId).ToList();

            var categoryNames = new List<string>();
            foreach (var productId in productIds)
            {
                categoryNames.AddRange(
                    _categoryService.GetProductCategoriesByProductId(productId).Select(x => x.Category.Name).ToList());
            }


            return string.Join(",", categoryNames);
        }

        public String IsPreviousCustomer(int customerId)
        {

            var psIds = new List<int> { (int)PaymentStatus.Paid };
            var allPaidOrders = _orderService.SearchOrders(customerId: customerId, psIds: psIds);
            if (allPaidOrders.Count > 0)
            {
                return "YES";
            }
            return "NO";
        }


        public static string RequestWeb(string url)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            System.Web.HttpContext.Current.Trace.Warn("responseFromServer", responseFromServer);

            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return responseFromServer;
        }

        #endregion

        #region EasyPayPaymentStatus

        public virtual ActionResult EasyPayPaymentStatusList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new EasyPayPaymentStatusList();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            model.AvailablePaymentMethod.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.AvailablePaymentMethod.Add(new SelectListItem { Text = "Credit Card", Value = "CC" });
            model.AvailablePaymentMethod.Add(new SelectListItem { Text = "Mobile Account", Value = "MA" });
            model.AvailablePaymentMethod.Add(new SelectListItem { Text = "Over The Counter", Value = "OTC" });

            model.AvailableTransactionStatus.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "DROPPED", Value = "DROPPED" });
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "INITIATED", Value = "INITIATED" });
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "PAID", Value = "PAID" });
            model.AvailableTransactionStatus.Add(new SelectListItem { Text = "FAILED", Value = "FAILED" });
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult EasyPayPaymentStatusList(DataSourceRequest command, EasyPayPaymentStatus model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.AccessDenied.Description") });

            var fcmApplications = _easyPayPaymentStatusService.SearchEasyPayPaymentStatus(
                AccountNumber: model.AccountNumber,
                Msisdn: model.Msisdn,
                OrderId: model.OrderId,
                PaymentMethod: model.PaymentMethod,
                PaymentToken: model.PaymentToken,
                ResponseCode: model.ResponseCode,
                TransactionAmount: model.TransactionAmount,
                StoreId: model.StoreId,
                TransactionId: model.TransactionId,
                TransactionStatus: model.TransactionStatus,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = fcmApplications.Select(PrepareEasyPaymentStatusForList),
                Total = fcmApplications.TotalCount
            };

            return Json(gridModel);
        }

        [NonAction]
        protected virtual EasyPayPaymentStatusList PrepareEasyPaymentStatusForList(EasyPayPaymentStatus easyPayPaymentStatus)
        {
            return new EasyPayPaymentStatusList
            {
                Id = easyPayPaymentStatus.Id,
                AccountNumber= easyPayPaymentStatus.AccountNumber,
                Msisdn= easyPayPaymentStatus.Msisdn,
                OrderId= easyPayPaymentStatus.OrderId,
                PaymentMethod= easyPayPaymentStatus.PaymentMethod,
                PaymentToken= easyPayPaymentStatus.PaymentToken,
                ResponseCode= easyPayPaymentStatus.ResponseCode,
                TransactionAmount= easyPayPaymentStatus.TransactionAmount,
                StoreId= easyPayPaymentStatus.StoreId,
                TransactionId= easyPayPaymentStatus.TransactionId,
                TransactionStatus= easyPayPaymentStatus.TransactionStatus,
            };
        }

        public virtual ActionResult EasyPayPaymentStatusCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new EasyPayPaymentStatus();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult EasyPayPaymentStatusCreate(EasyPayPaymentStatus model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var fcmAction = model;
                _easyPayPaymentStatusService.InsertEasyPayPaymentStatus(fcmAction);

                SuccessNotification(_localizationService.GetResource("Plugins.Payments.TelenorEasyPay.Added"));
                if (continueEditing)
                {
                    return RedirectToAction("EasyPayPaymentStatusEdit", new { id = fcmAction.Id });
                }
                return RedirectToAction("EasyPayPaymentStatusList");
            }

            //If we got this far, something failed, redisplay form
            model = new EasyPayPaymentStatus();
            return View(model);
        }

        public virtual ActionResult EasyPayPaymentStatusEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var fcmAction = _easyPayPaymentStatusService.GetEasyPayPaymentStatusById(id);
            if (fcmAction == null)
                //No vendor found with the specified id
                return RedirectToAction("EasyPayPaymentStatusList");

            var model = fcmAction;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult EasyPayPaymentStatusEdit(EasyPayPaymentStatus model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var fcmAction = _easyPayPaymentStatusService.GetEasyPayPaymentStatusById(model.Id);
            if (fcmAction == null)
                //No Application found with the specified id
                return RedirectToAction("EasyPayPaymentStatusList");

            if (ModelState.IsValid)
            {
                fcmAction = model;
                _easyPayPaymentStatusService.UpdateEasyPayPaymentStatus(fcmAction);

                SuccessNotification(_localizationService.GetResource("Plugins.Payments.TelenorEasyPay.Updated"));
                if (continueEditing)
                {
                    return RedirectToAction("EasyPayPaymentStatusEdit", new { id = model.Id });
                }
                return RedirectToAction("EasyPayPaymentStatusList");
            }

            //If we got this far, something failed, redisplay form
            model = new EasyPayPaymentStatus();
            return View(model);
        }

        #endregion
    }
}
