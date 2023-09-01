using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security.Captcha;
using Nop.Services.Messages;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework.Security;
using Nop.Core.Domain.Appointments;
using Nop.Services.Appointments;
using Nop.Web.Models.Appointment;
using Nop.Web.Factories;

namespace Nop.Web.Controllers
{
    public partial class AppointmentController : BasePublicController
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAppointmentService _appointmentService;
        private readonly IAppointmentModelFactory _appointmentModelFactory;
        private readonly CustomerSettings _customerSettings;

        #endregion

        #region Constructors

        public AppointmentController(IProductService productService,
            CaptchaSettings captchaSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IStoreContext storeContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IAppointmentService appointmentService,
            IAppointmentModelFactory appointmentModelFactory,
            CustomerSettings customerSettings)
        {
            this._productService = productService;
            this._captchaSettings = captchaSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._storeContext = storeContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._appointmentService = appointmentService;
            this._appointmentModelFactory = appointmentModelFactory;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region ProductAppointmentsAdd

        [HttpPost, ActionName("ProductAppointments")]
        [PublicAntiForgery]
        [FormValueRequired("add-Appointment")]
        [CaptchaValidator]
        public virtual ActionResult ProductAppointmentsAdd(int productId, ProductAppointmentsModel model, bool captchaValid)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnProductAppointmentPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToAppointmentProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Appointments.OnlyRegisteredUsersCanWriteAppointments"));
            }

            if (ModelState.IsValid)
            {
                //save Appointment
                bool isApproved = !_catalogSettings.ProductAppointmentsMustBeApproved;

                var productAppointment = new ProductAppointment
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    AppointmentText = model.AddProductAppointment.AppointmentText,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };

                _appointmentService.InsertProductAppointment(productAppointment);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductAppointments)
                    _workflowMessageService.SendProductAppointmentNotificationMessage(productAppointment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductAppointment", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddProductAppointment"), product.Name);

                //raise event
                if (productAppointment.IsApproved)
                    _eventPublisher.Publish(new ProductAppointmentApprovedEvent(productAppointment));

                model = _appointmentModelFactory.PrepareProductAppointmentsModel(model, product);
                model.AddProductAppointment.AppointmentText = null;

                model.AddProductAppointment.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductAppointment.Result = _localizationService.GetResource("Appointments.SeeAfterApproving");
                else
                    model.AddProductAppointment.Result = _localizationService.GetResource("Appointments.SuccessfullyAdded");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = _appointmentModelFactory.PrepareProductAppointmentsModel(model, product);
            return View(model);
        }

        public virtual ActionResult CustomerProductAppointments(int? page)
        {
            if (_workContext.CurrentCustomer.IsGuest())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.HideProductAppointmentsTab)
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _appointmentModelFactory.PrepareCustomerProductAppointmentsModel(page);
            return View(model);
        }
        #endregion
    }
}