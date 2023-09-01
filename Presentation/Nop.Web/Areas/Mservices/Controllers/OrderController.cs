using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using System.IO;
using Nop.Services.Localization;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Web.Areas.Mservices.Models.Order;
using Nop.Services.Media;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class OrderController : MServices.Controllers.BaseController
    {
        #region Fields
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IWebHelper _webHelper;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;

        #endregion

        #region Constructors

        public OrderController(IOrderModelFactory orderModelFactory,
            IOrderService orderService,
            IShipmentService shipmentService,
            IWorkContext workContext,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IWebHelper webHelper,
            RewardPointsSettings rewardPointsSettings,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            IPictureService pictureService)

        {
            this._orderModelFactory = orderModelFactory;
            this._orderService = orderService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._orderProcessingService = orderProcessingService;
            this._paymentService = paymentService;
            this._pdfService = pdfService;
            this._webHelper = webHelper;
            this._rewardPointsSettings = rewardPointsSettings;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._priceFormatter = priceFormatter;
            this._pictureService = pictureService;
        }

        #endregion

        #region Depricited CustomerOrders / Orders

        public virtual ActionResult CustomerOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Customer not registered");

            var model = _orderModelFactory.PrepareCustomerOrderListModel();
            return View(model);
        }

        public virtual ActionResult Orders(int pageNumber = 1, int pageSize = 10)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Customer not registered");

            var model = new List<OrderListingModel>();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageIndex: pageNumber, pageSize: pageSize);
            foreach (var order in orders)
            {
                var orderModel = new OrderListingModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                    CustomOrderNumber = order.CustomOrderNumber
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                var product = order.OrderItems.FirstOrDefault().Product;
                if (product != null)
                {
                    var productpictures = _pictureService.GetPicturesByProductId(product.Id);
                    orderModel.Image = productpictures.Any() ? _pictureService.GetPictureUrl(productpictures.FirstOrDefault().Id) : _pictureService.GetDefaultPictureUrl();
                }


                model.Add(orderModel);
            }

            int pagenumber = orders.PageIndex;
            int availableTotalPages = orders.TotalPages;

            var OrderListModel = new OrderListing
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Orders = model,
            };
            return View(OrderListModel);
        }

        #endregion

        #region OrderDetails

        public ActionResult OrderDetails(int orderId = 0)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return InvokeHttp400("Order not exists or deleted");

            var model = _orderModelFactory.PrepareOrderDetailsModel(order);
            return View(model);
        }
        #endregion

        #region GetPdfInvoice

        //My account / Order details page / PDF invoice
        public ActionResult GetPdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return InvokeHttp400("Order not exists or  deleted");

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, string.Format("order_{0}.pdf", order.Id));
        }
        #endregion
    }
}