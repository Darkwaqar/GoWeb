using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Common;
using Nop.Admin.Models.Fcm;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Fcm;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Fcm;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class FcmController : BaseAdminController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IDeviceService _deviceService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedFcmService _queuedFcmService;
        private readonly IFcmApplicationService _fcmApplicationService;
        private readonly IFcmActionService _fcmActionService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IVendorService _vendorService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        public FcmController(IWorkContext workContext,
            IPermissionService permissionService,
            IDeviceService deviceService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IQueuedFcmService queuedFcmService,
            IFcmApplicationService fcmApplicationService,
            IFcmActionService fcmActionService,
            IPictureService pictureService,
            IStoreContext storeContext,
            ICustomerService customerService,
            IVendorService vendorService,
            ICacheManager cacheManager)
        {
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._deviceService = deviceService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._queuedFcmService = queuedFcmService;
            this._fcmApplicationService = fcmApplicationService;
            this._fcmActionService = fcmActionService;
            this._pictureService = pictureService;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._vendorService = vendorService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual DeviceModel PrepareDeviceModelForList(Device device)
        {
            return new DeviceModel
            {
                CustomerId = device.CustomerId,
                Id = device.Id,
                DeviceId = device.DeviceId,
                Brand = device.Brand,
                OSVersion = device.OSVersion,
                Carrier = device.Carrier,
                DeviceOS = device.DeviceOS,
                DeviceRam = device.DeviceRam,
                Longitude = device.Longitude,
                Latitude = device.Latitude,
                Active = device.Active,
                Error = device.Error,
                CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc),
                UpdatedOnUtc = _dateTimeHelper.ConvertToUserTime(device.UpdatedOnUtc, DateTimeKind.Utc),
            };
        }



        [NonAction]
        protected virtual FcmActionModel PrepareFcmActionListModel(FcmAction fcmAction)
        {
            return new FcmActionModel
            {
                Id = fcmAction.Id,
                Name = fcmAction.Name,
                Extra = fcmAction.Extra,
                Active = fcmAction.Active,
                CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(fcmAction.CreatedOnUtc, DateTimeKind.Utc),
            };
        }

        [NonAction]
        protected virtual void PrepareFcmActionModel(FcmActionModel model)
        {
            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = currentVendor.Name,
                    Value = currentVendor.Id.ToString()
                });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"),
                    Value = "0"
                });

                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }

        }




        [NonAction]
        protected virtual FcmApplicationModel PrepareApplicationListModelForList(FcmApplication fcmApplication)
        {
            return new FcmApplicationModel
            {
                Id = fcmApplication.Id,
                Name = fcmApplication.Name,
                Package = fcmApplication.Package,
                AppKey = fcmApplication.AppKey,
                CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(fcmApplication.CreatedOnUtc, DateTimeKind.Utc),
            };
        }

        [NonAction]
        protected virtual void PrepareApplicationModel(FcmApplicationModel model)
        {
            model.CreatedOnUtc = DateTime.Now;

            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = currentVendor.Name,
                    Value = currentVendor.Id.ToString()
                });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"), Value = "0" });
                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }

        }

        [NonAction]
        protected virtual void PrepareNotificationModel(NotificationModel model)
        {

            var vendorId = _workContext.CurrentVendor != null ? _workContext.CurrentVendor.Id : 0;
            model.AvailableApplications.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Fcm.Application.SelectApplication"), Value = "0" });
            foreach (var c in _fcmApplicationService.SearchApplications(vendorId))
                model.AvailableApplications.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.ApplicationId) });

            model.AvailableActions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Fcm.Application.SelectAction"), Value = "0" });
            foreach (var c in _fcmActionService.SearchFcmActions(vendorId))
                model.AvailableActions.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.ActionId) });

            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem { Text = currentVendor.Name, Value = currentVendor.Id.ToString() });
                model.Icon = _workContext.CurrentVendor.mpLogo;
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"), Value = "0" });
                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }

            model.SendImmediately = true;
        }


        private double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = (dist * 60 * 1.1515) / 0.6213711922;          //miles to kms
            return (dist);
        }
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }


        private void AddFcmToQueued(IWorkContext workContext, FcmApplication application, Device device, NotificationModel model)
        {
            if (model.Image != 0)
            {
                model.DirectImageLink = _pictureService.GetPictureUrl(model.Image);
            }

            if (model.Icon != 0)
            {
                model.DirectIconLink = _pictureService.GetPictureUrl(model.Icon, 128);
            }

            int prevPictureId = model.Image;
            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != model.Image)
            {
                var prevPicture = _pictureService.GetPictureById(prevPictureId);
                if (prevPicture != null)
                    _pictureService.DeletePicture(prevPicture);
            }

            int prevIconId = model.Icon;
            //delete an old picture (if deleted or updated)
            if (prevIconId > 0 && prevIconId != model.Icon)
            {
                var prevPicture = _pictureService.GetPictureById(prevIconId);
                if (prevPicture != null)
                    _pictureService.DeletePicture(prevPicture);
            }

            var queuedFcm = new QueuedFcm
            {
                Priority = QueuedEmailPriority.High,
                FromName = _workContext.CurrentCustomer.Username ?? _workContext.CurrentCustomer.GetFullName(),
                From = _workContext.CurrentCustomer.Email,
                AppKey = application.AppKey,
                Package = application.Package,
                FcmType = model.FcmType,
                FcmApplicationType = model.FcmApplicationType,
                DeviceId = device.DeviceId,
                DevicePId = device.Id,
                Title = model.Title,
                Message = model.Message,
                Image = model.DirectImageLink,
                Icon = model.DirectIconLink,
                GotoLink = model.GotoLink,
                Intent = model.ActionId,
                VendorId = model.VendorId,
                StoreId = _storeContext.CurrentStore.Id,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = (model.SendImmediately || !model.DontSendBeforeDate.HasValue) ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value)
            };
            //little heck to speedup
            if (device.CustomerId > 0)
            {
                var customer = _customerService.GetCustomerById(device.CustomerId);
                queuedFcm.ToName = customer.Username ?? customer.GetFullName();
                var notification = model.ToEntity();
                foreach (var noti in customer.Notifications.Where(x => x.Message == notification.Message || x.Title == notification.Title).ToList())
                {
                    customer.Notifications.Remove(noti);
                }
                notification.CreatedOnUtc = DateTime.Now;
                notification.UpdatedOnUtc = DateTime.Now;
                customer.Notifications.Add(notification);
            }

            _queuedFcmService.InsertQueuedFcm(queuedFcm);


        }
        #endregion

        #region Methods

        public ActionResult Index()
        {
            var model = new DashboardModel();
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult FcmStatistics()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = new FcmStatisticsModel();
            var totalDevices = _deviceService.GetAllDevices(ShowHidden: true);
            model.NumberOfDevices = totalDevices.Count;
            model.NumberOfAndroidDevices = totalDevices.Where(x => x.DeviceOS == FcmApplicationType.Android).Count();
            model.NumberOfIosDevices = totalDevices.Where(x => x.DeviceOS == FcmApplicationType.Ios).Count();
            model.NumberOfWebDevices = totalDevices.Where(x => x.DeviceOS == FcmApplicationType.Web).Count(); ;
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult FcmUserStatistics()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = new FcmUserStatisticsModel();

            model.NumberOfApplications = _fcmApplicationService.GetAllFcmApplication().Count;

            model.NumberOfNotificationSend = _queuedFcmService.SearchFcms(null, null, null, null, 0, 0, null, null,
                false, false, 3, false, 0, int.MaxValue).Count;

            model.NumberOfActions = _fcmActionService.GetAllFcmAction().Count;

            model.NumberOfLastMonth = 212;

            return PartialView(model);
        }

        #endregion

        #region FcmDevice

        public virtual ActionResult FcmDeviceList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new DeviceListModel();
            var vendorId = _workContext.CurrentVendor != null ? _workContext.CurrentVendor.Id : 0;

            model.AvailableApplications.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Fcm.Application.All"), Value = "0" });
            foreach (var c in _fcmApplicationService.SearchApplications(vendorId))
                model.AvailableApplications.Add(new SelectListItem { Text = c.Name, Value = c.Package });

            model.SearchHidden = true;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult FcmDeviceList(DataSourceRequest command, DeviceListModel model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedKendoGridJson();

            if (model.SearchPackage.Equals("0"))
                model.SearchPackage = null;

            var fcmApplications = _deviceService.GetAllDevices(
                Package: model.SearchPackage,
                SearchBrand: model.SearchBrand,
                SearchCarrier: model.SearchCarrier,
                SearchFcmApplicationType: model.SearchFcmApplicationType,
                SearchLongitude: model.SearchLongitude,
                SearchLatitude: model.SearchLatitude,
                ShowHidden: model.SearchHidden,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = fcmApplications.Select(PrepareDeviceModelForList),
                Total = fcmApplications.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult FcmDeviceCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new DeviceModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult FcmDeviceCreate(DeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var device = model.ToEntity();
                _deviceService.InsertDevice(device);

                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Device.Added"));
                if (continueEditing)
                {
                    return RedirectToAction("FcmDeviceEdit", new { id = device.Id });
                }
                return RedirectToAction("FcmDeviceList");
            }

            //If we got this far, something failed, redisplay form
            model = new DeviceModel();
            return View(model);
        }

        public virtual ActionResult FcmDeviceEdit(int Id)
        {
            var device = _deviceService.GetDeviceById(Id);
            if (device == null)
                //No address found with the specified id
                return RedirectToAction("FcmDeviceList");

            var model = device.ToModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult FcmDeviceEdit(DeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(model.Id);
            if (device == null)
                //No address found with the specified id
                return RedirectToAction("FcmDeviceList");

            if (ModelState.IsValid)
            {
                device = model.ToEntity(device);
                _deviceService.UpdateDevice(device);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Devices.Updated"));
                if (continueEditing)
                {
                    return RedirectToAction("FcmDeviceEdit", new { Id = model.Id });
                }
                return RedirectToAction("FcmDeviceList");
            }

            //If we got this far, something failed, redisplay form
            model = new DeviceModel();
            return View(model);
        }

        #endregion

        #region Application

        public virtual ActionResult ApplicationList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new FcmApplicationListModel();
            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = currentVendor.Name,
                    Value = currentVendor.Id.ToString()
                });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });

                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ApplicationList(DataSourceRequest command, FcmApplicationListModel model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedKendoGridJson();

            if (_workContext.CurrentVendor != null)
            {
                var currentvendor = _workContext.CurrentVendor;
                model.SearchVendorId = currentvendor.Id;
            }

            var fcmApplication = _fcmApplicationService.SearchApplications(
                SearchVendorId: model.SearchVendorId,
                SearchId: model.SearchId,
                SearchPackage: model.SearchPackage,
                SearchAppKey: model.SearchAppKey,
                SearchName: model.SearchName,
                SearchFcmApplicationType: model.SearchFcmApplicationType,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = fcmApplication.Select(PrepareApplicationListModelForList),
                Total = fcmApplication.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult ApplicationCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new FcmApplicationModel();
            PrepareApplicationModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult ApplicationCreate(FcmApplicationModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var fcmApplication = model.ToEntity();
                _fcmApplicationService.InsertFcmApplication(fcmApplication);

                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Application.Added"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ApplicationEdit", new { id = fcmApplication.Id });
                }
                return RedirectToAction("ApplicationList");
            }

            //If we got this far, something failed, redisplay form
            PrepareApplicationModel(model);
            return View(model);
        }

        public virtual ActionResult ApplicationEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var fcmApplication = _fcmApplicationService.GetFcmApplicationById(id);
            if (fcmApplication == null)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var model = fcmApplication.ToModel();
            PrepareApplicationModel(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult ApplicationEdit(FcmApplicationModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var fcmapplication = _fcmApplicationService.GetFcmApplicationById(model.Id);
            if (fcmapplication == null)
                //No Application found with the specified id
                return RedirectToAction("ApplicationList");

            if (ModelState.IsValid)
            {
                fcmapplication = model.ToEntity(fcmapplication);
                _fcmApplicationService.UpdateFcmApplication(fcmapplication);

                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Application.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("ApplicationEdit", new { id = model.Id });
                }
                return RedirectToAction("ApplicationList");
            }

            //If we got this far, something failed, redisplay form
            model = new FcmApplicationModel();
            PrepareApplicationModel(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var fcmApplication = _fcmApplicationService.GetFcmApplicationById(id);
            if (fcmApplication == null)
                //No fcmApplication found with the specified id
                return RedirectToAction("ApplicationList");

            //delete a fcmApplication
            _fcmApplicationService.DeleteFcmApplication(fcmApplication);

            SuccessNotification(_localizationService.GetResource("Admin.Fcm.Application.Deleted"));
            return RedirectToAction("ApplicationList");
        }
        #endregion

        #region Settings

        public ActionResult Settings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new FcmSettingsModel();
            model.GoogleMapKey = _localizationService.GetLocaleStringResourceByName("Admin.Fcm.Settings.GoogleMapKey.Value").ResourceValue;
            return View(model);
        }

        [HttpPost]
        public ActionResult Settings(FcmSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var resource = _localizationService.GetLocaleStringResourceByName("Admin.Fcm.Settings.GoogleMapKey.Value");
            resource.ResourceValue = model.GoogleMapKey;
            _localizationService.UpdateLocaleStringResource(resource);
            SuccessNotification(_localizationService.GetResource("Admin.Fcm.Setting.Updated"));
            return View(model);
        }
        #endregion

        #region FcmAction

        public virtual ActionResult FcmActionList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new FcmActionListModel();
            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = currentVendor.Name,
                    Value = currentVendor.Id.ToString()
                });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });

                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }
            model.ShowHidden = true;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult FcmActionList(DataSourceRequest command, FcmActionListModel model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedKendoGridJson();

            if (_workContext.CurrentVendor != null)
            {
                var currentvendor = _workContext.CurrentVendor;
                model.SearchVendorId = currentvendor.Id;
            }

            var fcmApplication = _fcmActionService.SearchFcmActions(
                SearchVendorId: model.SearchVendorId,
                SearchName: model.SearchName,
                showHidden: model.ShowHidden,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = fcmApplication.Select(PrepareFcmActionListModel),
                Total = fcmApplication.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult FcmActionCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new FcmActionModel();
            PrepareFcmActionModel(model);
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult FcmActionCreate(FcmActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var fcmAction = model.ToEntity();
                _fcmActionService.InsertFcmAction(fcmAction);

                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Action.Added"));
                if (continueEditing)
                {
                    return RedirectToAction("FcmActionEdit", new { id = fcmAction.Id });
                }
                return RedirectToAction("FcmActionList");
            }

            //If we got this far, something failed, redisplay form
            model = new FcmActionModel();
            return View(model);
        }

        public virtual ActionResult FcmActionEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var fcmAction = _fcmActionService.GetFcmActionById(id);
            if (fcmAction == null)
                //No vendor found with the specified id
                return RedirectToAction("FcmActionList");

            var model = fcmAction.ToModel();
            PrepareFcmActionModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult FcmActionEdit(FcmActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var fcmAction = _fcmActionService.GetFcmActionById(model.Id);
            if (fcmAction == null)
                //No Application found with the specified id
                return RedirectToAction("FcmActionList");

            if (ModelState.IsValid)
            {
                fcmAction = model.ToEntity(fcmAction);
                _fcmActionService.UpdateFcmAction(fcmAction);

                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Action.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("FcmActionEdit", new { id = model.Id });
                }
                return RedirectToAction("FcmActionList");
            }

            //If we got this far, something failed, redisplay form
            model = new FcmActionModel();
            return View(model);
        }

        #endregion

        #region SendFcmToSingleDevice

        public virtual ActionResult SendFcm(int Id)
        {
            var device = _deviceService.GetDeviceById(Id);
            if (device == null)
                //No address found with the specified id
                return RedirectToAction("FcmDeviceList");

            var model = new SendDevicesFcmModel();
            model.DeviceId = Id;
            PrepareNotificationModel(model.Notification);
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult SendFcm(SendDevicesFcmModel model)
        {
            var device = _deviceService.GetDeviceById(model.DeviceId);
            if (device == null)
                //No address found with the specified id
                return RedirectToAction("FcmDeviceList");

            var application = _fcmApplicationService.SearchApplications(SearchPackage: device.Package);
            if (!application.Any())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Fcm.Device.SendFcm.Package.Error"));
                return RedirectToAction("FcmDeviceList");
            }
            if (ModelState.IsValid)
            {
                model.Notification.FcmApplicationType = application.First().FcmApplicationType;
                AddFcmToQueued(_workContext, application.First(), device, model.Notification);
                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Device.SendFcm.Queued"));
            }
            else
            {
                foreach (var item in ModelState.Where(x => x.Value.Errors.Count > 0))
                {
                    ErrorNotification(item.Value.Errors.First().ErrorMessage);
                }
            }

            //If we got this far, something failed, redisplay form
            PrepareNotificationModel(model.Notification);
            return View(model);
        }



        #endregion

        #region BulkFcmSend

        public virtual ActionResult TopicNotification()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new NotificationModel();
            PrepareNotificationModel(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult TopicNotification(NotificationModel model)
        {
            var application = _fcmApplicationService.GetFcmApplicationById(model.ApplicationId);
            if (application == null)
            {
                //No application found with the specified id
                ErrorNotification(_localizationService.GetResource("Admin.Fcm.Application.NotFound"));
                return RedirectToAction("TopicNotification");
            }

            var devices = _deviceService.GetAllDevices(Package: application.Package);
            if (devices == null)
            {
                //No devices found with the specified id
                ErrorNotification(_localizationService.GetResource("Admin.Fcm.Device.NotFound"));
                return RedirectToAction("TopicNotification");
            }

            if (ModelState.IsValid)
            {
                foreach (var device in devices)
                {
                    AddFcmToQueued(_workContext, application, device, model);
                }
                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Device.SendFcm.Queued"));
            }
            else
            {
                foreach (var item in ModelState.Where(x => x.Value.Errors.Count > 0))
                {
                    ErrorNotification(item.Value.Errors.First().ErrorMessage);
                }
            }

            //If we got this far, something failed, redisplay form
            PrepareNotificationModel(model);
            return View(model);
        }


        #endregion

        #region LocationFcm

        public virtual ActionResult LocationNotification()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageFcms))
                return AccessDeniedView();

            var model = new LocationNotificationModel();
            model.Radius = 10;
            PrepareNotificationModel(model.Notification);
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult LocationNotification(LocationNotificationModel model)
        {
            List<Device> devicelist = new List<Device>();
            var devices = _deviceService.GetAllDevices();
            foreach (var device in devices)
            {
                double d = Distance((double)model.Latitude, (double)model.Longitude, (double)device.Latitude, (double)device.Longitude);
                if (d < model.Radius)
                {
                    devicelist.Add(device);
                }
            }

            var application = _fcmApplicationService.GetFcmApplicationById(model.Notification.ApplicationId);

            if (application == null)
            {
                //No application found with the specified id
                ErrorNotification(_localizationService.GetResource("Admin.Fcm.Application.NotFound"));
                return RedirectToAction("LocationNotification");
            }
            devicelist.Where(x => x.Package == application.Package);

            if (ModelState.IsValid)
            {
                foreach (var device in devicelist)
                {
                    AddFcmToQueued(_workContext, application, device, model.Notification);
                }
                SuccessNotification(_localizationService.GetResource("Admin.Fcm.Device.SendFcm.Queued"));
            }
            else
            {
                foreach (var item in ModelState.Where(x => x.Value.Errors.Count > 0))
                {
                    ErrorNotification(item.Value.Errors.First().ErrorMessage);
                }
            }

            //If we got this far, something failed, redisplay form
            PrepareNotificationModel(model.Notification);
            return View(model);
        }

        #endregion


        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult GetApplicationByFcmApplicationType(int type)
        {
            //permission validation is not required here


            var vendorId = _workContext.CurrentVendor != null ? _workContext.CurrentVendor.Id : 0;

            // This action method gets called via an ajax request
            if (type == 0)
                throw new ArgumentNullException("Type");

            var application = _fcmApplicationService.SearchApplications(SearchVendorId: vendorId, SearchFcmApplicationType: (FcmApplicationType)type).ToList();
            var result = (from s in application
                          select new { id = s.Id, name = s.Name }).ToList();


            //some country is selected
            if (!result.Any())
            {
                //country does not have states
                result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Fcm.Application.NotFound") });
            }
            else
            {
                result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Fcm.Application.SelectApplication") });
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult GetCustomerInDistance(double Latitude, double Longitude, double distance)
        {
            //permission validation is not required here


            // This action method gets called via an ajax request
            if (Longitude == 0)
                throw new ArgumentNullException("Longitude");

            if (Latitude == 0)
                throw new ArgumentNullException("Latitude");

            if (distance == 0)
                throw new ArgumentNullException("distance");

            var devices = _deviceService.GetAllDevices();
            List<DeviceDistanceModel> Marker = new List<DeviceDistanceModel>();
            foreach (var device in devices)
            {
                double d = Distance(Latitude, Longitude, (double)device.Latitude, (double)device.Longitude);
                if (d < distance)          //nearbyplaces which are within 25 kms 
                {
                    DeviceDistanceModel dist = new DeviceDistanceModel();
                    dist.Id = device.Id;
                    dist.Latitute = device.Latitude;
                    dist.Longitude = device.Longitude;
                    dist.Distance = d;
                    Marker.Add(dist);
                }
            }

            return Json(Marker, JsonRequestBehavior.AllowGet);
        }


    }
}