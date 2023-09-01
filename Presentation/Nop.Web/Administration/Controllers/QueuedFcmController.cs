using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Fcm;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Services.Fcm;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class QueuedFcmController : BaseAdminController
    {

        private readonly IQueuedFcmService _queuedFcmService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IVendorService _vendorService;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreService _storeService;

        public QueuedFcmController(IQueuedFcmService queuedFcmService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IVendorService vendorService,
            ICacheManager cacheManager,
            IFcmApplicationService fcmApplicationService,
            IStoreService storeService)
        {
            this._queuedFcmService = queuedFcmService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._vendorService = vendorService;
            this._cacheManager = cacheManager;
            this._storeService = storeService;

        }

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var model = new QueuedFcmListModel
            {
                //default value
                SearchMaxSentTries = 10
            };

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });


            model.AvailableVendors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Products.Fields.Vendor.None"),
                Value = "0"
            });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult QueuedFcmList(DataSourceRequest command, QueuedFcmListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedKendoGridJson();

            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var queuedFcms = _queuedFcmService.SearchFcms(model.SearchFromFcm, model.SearchToFcm, model.SearchPackage, model.SearchAppKey, model.SearchVendorId, model.SearchStoreId,
                startDateValue, endDateValue,
                model.SearchLoadNotSent, false, model.SearchMaxSentTries, true,
                command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = queuedFcms.Select(x =>
                {
                    var m = x.ToModel();
                    m.PriorityName = x.Priority.GetLocalizedEnum(_localizationService, _workContext);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    if (x.DontSendBeforeDateUtc.HasValue)
                        m.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(x.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    if (x.SentOnUtc.HasValue)
                        m.SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOnUtc.Value, DateTimeKind.Utc);

                    //little performance optimization: ensure that "Body" is not returned
                    m.Message = "";

                    return m;
                }),
                Total = queuedFcms.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-fcm-by-number")]
        public virtual ActionResult GoToFcmByNumber(QueuedFcmListModel model)
        {
            var queuedFcm = _queuedFcmService.GetQueuedFcmById(model.GoDirectlyToNumber);
            if (queuedFcm == null)
                return List();

            return RedirectToAction("Edit", "QueuedFcm", new { id = queuedFcm.Id });
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var fcm = _queuedFcmService.GetQueuedFcmById(id);
            if (fcm == null)
                //No fcm found with the specified id
                return RedirectToAction("List");

            var model = fcm.ToModel();
            model.PriorityName = fcm.Priority.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(fcm.CreatedOnUtc, DateTimeKind.Utc);
            if (fcm.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(fcm.SentOnUtc.Value, DateTimeKind.Utc);
            if (fcm.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(fcm.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            else model.SendImmediately = true;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(QueuedFcmModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var fcm = _queuedFcmService.GetQueuedFcmById(model.Id);
            if (fcm == null)
                //No fcm found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                fcm = model.ToEntity(fcm);
                fcm.DontSendBeforeDateUtc = (model.SendImmediately || !model.DontSendBeforeDate.HasValue) ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value);
                _queuedFcmService.UpdateQueuedFcm(fcm);

                SuccessNotification(_localizationService.GetResource("Admin.System.QueuedFcms.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = fcm.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.PriorityName = fcm.Priority.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(fcm.CreatedOnUtc, DateTimeKind.Utc);
            if (fcm.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(fcm.SentOnUtc.Value, DateTimeKind.Utc);
            if (fcm.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(fcm.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            return View(model);
        }

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public virtual ActionResult Requeue(QueuedFcmModel queuedFcmModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var queuedFcm = _queuedFcmService.GetQueuedFcmById(queuedFcmModel.Id);
            if (queuedFcm == null)
                //No fcm found with the specified id
                return RedirectToAction("List");

            var requeuedFcm = new QueuedFcm
            {
                VendorId = queuedFcm.VendorId,
                PriorityId = queuedFcm.PriorityId,
                From = queuedFcm.From,
                FromName = queuedFcm.FromName,
                DeviceId = queuedFcm.DeviceId,
                DevicePId = queuedFcm.Id,
                Title = queuedFcm.Title,
                Message = queuedFcm.Message,
                Image = queuedFcm.Image,
                Icon = queuedFcm.Icon,
                Intent = queuedFcm.Intent,
                Package = queuedFcm.Package,
                AppKey = queuedFcm.AppKey,
                Action = queuedFcm.Action,
                FcmApplicationType = queuedFcm.FcmApplicationType,
                FcmType = queuedFcm.FcmType,
                GotoLink = queuedFcm.GotoLink,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = (queuedFcmModel.SendImmediately || !queuedFcmModel.DontSendBeforeDate.HasValue) ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(queuedFcmModel.DontSendBeforeDate.Value)
            };
            _queuedFcmService.InsertQueuedFcm(requeuedFcm);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedFcms.Requeued"));
            return RedirectToAction("Edit", new { id = requeuedFcm.Id });
        }

    }
}