using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Appointments;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Appointment;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the Appointment model factory
    /// </summary>
    public partial class AppointmentModelFactory : IAppointmentModelFactory
    {
        #region Fields
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IAppointmentService _appointmentService;
        private readonly ILocalizationService _localizationService;

        #endregion
        public AppointmentModelFactory(CatalogSettings catalogSettings,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            CaptchaSettings captchaSettings,
            IAppointmentService appointmentService,
            ILocalizationService localizationService)
        {
            this._catalogSettings = catalogSettings;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;
            this._captchaSettings = captchaSettings;
            this._appointmentService = appointmentService;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Prepare the product appointments model
        /// </summary>
        /// <param name="model">Product appointments model</param>
        /// <param name="product">Product</param>
        /// <returns>Product appointments model</returns>
        public virtual ProductAppointmentsModel PrepareProductAppointmentsModel(ProductAppointmentsModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product == null)
                throw new ArgumentNullException("product");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.AddProductAppointment.CanCurrentCustomerLeaveAppointment = _catalogSettings.AllowAnonymousUsersToAppointmentProduct || !_workContext.CurrentCustomer.IsGuest();
            model.AddProductAppointment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductAppointmentPage;

            return model;
        }

        /// <summary>
        /// Prepare the customer product appointments model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer product appointments model</returns>
        public virtual CustomerProductAppointmentsModel PrepareCustomerProductAppointmentsModel(int? page)
        {
            var pageSize = _catalogSettings.ProductAppointmentsPageSizeOnAccountPage;
            int pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var list = _appointmentService.GetAllProductAppointments(customerId: _workContext.CurrentCustomer.Id,
                approved: null,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var productAppointments = new List<CustomerProductAppointmentModel>();

            foreach (var appointment in list)
            {
                var product = appointment.Product;
                var productAppointmentModel = new CustomerProductAppointmentModel
                {
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(p => p.Name),
                    ProductSeName = product.GetSeName(),
                    AppointmentText = appointment.AppointmentText,
                    ReplyText = appointment.ReplyText,
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(product.CreatedOnUtc, DateTimeKind.Utc).ToString("g")
                };

                if (_catalogSettings.ProductAppointmentsMustBeApproved)
                {
                    productAppointmentModel.ApprovalStatus = appointment.IsApproved
                        ? _localizationService.GetResource("Account.CustomerProductAppointments.ApprovalStatus.Approved")
                        : _localizationService.GetResource("Account.CustomerProductAppointments.ApprovalStatus.Pending");
                }
                productAppointments.Add(productAppointmentModel);
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProductAppointmentsPaged",
                UseRouteLinks = true,
                RouteValues = new CustomerProductAppointmentsModel.CustomerProductAppointmentsRouteValues { page = pageIndex }
            };

            var model = new CustomerProductAppointmentsModel
            {
                ProductAppointments = productAppointments,
                PagerModel = pagerModel
            };

            return model;
        }
    }
}