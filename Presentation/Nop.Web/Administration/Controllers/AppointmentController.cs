using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Admin.Models.Appointments;
using Nop.Core.Domain.Appointments;
using Nop.Services.Appointments;

namespace Nop.Admin.Controllers
{
    public class AppointmentController : BaseAdminController
    {

        #region Fields

        private readonly IProductService _productService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreService _storeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;
        private readonly IAppointmentService _appointmentService;

        #endregion Fields

        #region Constructors

        public AppointmentController(IProductService productService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IEventPublisher eventPublisher,
            IStoreService storeService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext,
            IAppointmentService appointmentService)
        {
            this._productService = productService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;
            this._storeService = storeService;
            this._customerActivityService = customerActivityService;
            this._workContext = workContext;
            this._appointmentService = appointmentService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareProductAppointmentModel(ProductAppointmentModel model,
           ProductAppointment productAppointment, bool excludeProperties, bool formatAppointmentAndReplyText)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (productAppointment == null)
                throw new ArgumentNullException("productAppointment");

            model.Id = productAppointment.Id;
            model.StoreName = productAppointment.Store.Name;
            model.ProductId = productAppointment.ProductId;
            model.ProductName = productAppointment.Product.Name;
            model.CustomerId = productAppointment.CustomerId;
            var customer = productAppointment.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(productAppointment.CreatedOnUtc, DateTimeKind.Utc);
            if (!excludeProperties)
            {
                if (formatAppointmentAndReplyText)
                {
                    model.AppointmentText = Core.Html.HtmlHelper.FormatText(productAppointment.AppointmentText, false, true, false, false, false, false);
                    model.ReplyText = Core.Html.HtmlHelper.FormatText(productAppointment.ReplyText, false, true, false, false, false, false);
                }
                else
                {
                    model.AppointmentText = productAppointment.AppointmentText;
                    model.ReplyText = productAppointment.ReplyText;
                }
                model.IsApproved = productAppointment.IsApproved;
            }
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
        }
        #endregion

        #region Methods

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            var model = new ProductAppointmentListModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var stores = _storeService.GetAllStores().Select(st => new SelectListItem() { Text = st.Name, Value = st.Id.ToString() });
            foreach (var selectListItem in stores)
                model.AvailableStores.Add(selectListItem);

            //"approved" property
            //0 - all
            //1 - approved only
            //2 - disapproved only
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductAppointments.List.SearchApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductAppointments.List.SearchApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.ProductAppointments.List.SearchApproved.DisapprovedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, ProductAppointmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            bool? approved = null;
            if (model.SearchApprovedId > 0)
                approved = model.SearchApprovedId == 1;

            var productAppointments = _appointmentService.GetAllProductAppointments(0, approved,
                createdOnFromValue, createdToFromValue, model.SearchText,
                model.SearchStoreId, model.SearchProductId, vendorId,
                command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = productAppointments.Select(x =>
                {
                    var m = new ProductAppointmentModel();
                    PrepareProductAppointmentModel(m, x, false, true);
                    return m;
                }),
                Total = productAppointments.TotalCount
            };

            return Json(gridModel);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            var productAppointment = _appointmentService.GetProductAppointmentById(id);
            if (productAppointment == null)
                //No product Appointment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && productAppointment.Product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = new ProductAppointmentModel();
            PrepareProductAppointmentModel(model, productAppointment, false, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ProductAppointmentModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            var productAppointment = _appointmentService.GetProductAppointmentById(model.Id);
            if (productAppointment == null)
                //No product Appointment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && productAppointment.Product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var isLoggedInAsVendor = _workContext.CurrentVendor != null;

                var previousIsApproved = productAppointment.IsApproved;
                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    productAppointment.AppointmentText = model.AppointmentText;
                    productAppointment.IsApproved = model.IsApproved;
                }

                productAppointment.ReplyText = model.ReplyText;
                _appointmentService.UpdateProductAppointment(productAppointment);

                //activity log
                _customerActivityService.InsertActivity("EditProductAppointment", _localizationService.GetResource("ActivityLog.EditProductAppointment"), productAppointment.Id);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && productAppointment.IsApproved)
                        _eventPublisher.Publish(new ProductAppointmentApprovedEvent(productAppointment));

                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductAppointments.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = productAppointment.Id }) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareProductAppointmentModel(model, productAppointment, true, false);
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            var productAppointment = _appointmentService.GetProductAppointmentById(id);
            if (productAppointment == null)
                //No product Appointment found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            var product = productAppointment.Product;
            _appointmentService.DeleteProductAppointment(productAppointment);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductAppointment", _localizationService.GetResource("ActivityLog.DeleteProductAppointment"), productAppointment.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.ProductAppointments.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter not approved Appointments
                var productAppointments = _appointmentService.GetProducAppointmentsByIds(selectedIds.ToArray()).Where(Appointment => !Appointment.IsApproved);
                foreach (var productAppointment in productAppointments)
                {
                    productAppointment.IsApproved = true;
                    _appointmentService.UpdateProductAppointment(productAppointment);

                    //raise event 
                    _eventPublisher.Publish(new ProductAppointmentApprovedEvent(productAppointment));
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                //filter approved Appointments
                var productAppointments = _appointmentService.GetProducAppointmentsByIds(selectedIds.ToArray()).Where(Appointment => Appointment.IsApproved);
                foreach (var productAppointment in productAppointments)
                {
                    productAppointment.IsApproved = false;
                    _appointmentService.UpdateProductAppointment(productAppointment);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds != null)
            {
                var productAppointments = _appointmentService.GetProducAppointmentsByIds(selectedIds.ToArray());
                _appointmentService.DeleteProductAppointments(productAppointments);

                //update product totals
                foreach (var productAppointment in productAppointments)
                {
                    //activity log
                    _customerActivityService.InsertActivity("DeleteProductAppointment", _localizationService.GetResource("ActivityLog.DeleteProductAppointment"), productAppointment.Id);
                }
            }

            return Json(new { Result = true });
        }

        public virtual ActionResult ProductSearchAutoComplete(string term)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductAppointments))
                return Content("");

            const int searchTermMinimumLength = 3;
            if (String.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = _productService.SearchProducts(
                keywords: term,
                vendorId: vendorId,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                          select new
                          {
                              label = p.Name,
                              productid = p.Id
                          })
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}