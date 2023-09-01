using Nop.Admin.Extensions;
using Nop.Admin.Models.Magazines;
using Nop.Core;
using Nop.Core.Domain.Magazines;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Magazines;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class MagazineController : BaseAdminController
    {

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IMagazineService _magazineService;
        private readonly IPictureService _pictureService;
        private readonly IDownloadService _downloadService;
        private readonly ICustomerActivityService _customerActivityService;
        #endregion

        #region Ctor
        public MagazineController(IWorkContext workContext,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IMagazineService queuedFcmService,
            IPictureService pictureService,
            IDownloadService downloadService,
            ICustomerActivityService customerActivityService)
        {
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._magazineService = queuedFcmService;
            this._pictureService = pictureService;
            this._downloadService = downloadService;
            this._customerActivityService = customerActivityService;
        }

        #endregion

        #region Utilities
        [NonAction]
        protected virtual MagazineModel PrepareMagazineModelForList(Magazine magazine)
        {
            return new MagazineModel
            {
                Id = magazine.Id,
                Name = magazine.Name,
                Description = magazine.Description,
                PictureThumbnailUrl = _pictureService.GetPictureUrl(magazine.PictureId, 200, true),
                Published = magazine.Published,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(magazine.CreatedOnUtc, DateTimeKind.Utc),
                UpdatedOn = _dateTimeHelper.ConvertToUserTime(magazine.UpdatedOnUtc, DateTimeKind.Utc),
            };
        }
        #endregion

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var model = new MagazineListModel();
            model.SearchActive = true;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, MagazineListModel model)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            var magazines = _magazineService.SearchMagazines(
                SearchName: model.SearchName,
                SearchDescription: model.SearchDescription,
                SearchActive: model.SearchActive,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = magazines.Select(PrepareMagazineModelForList),
                Total = magazines.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var model = new MagazineModel();
            model.UnlimitedDownloads = true;
            model.MaxNumberOfDownloads = 10;
            model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Create(MagazineModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var magazine = model.ToEntity();
                magazine.CreatedOnUtc = DateTime.UtcNow;
                magazine.UpdatedOnUtc = DateTime.UtcNow;
                _magazineService.InsertMagazine(magazine);

                //activity log
                _customerActivityService.InsertActivity("CreateMagazine", _localizationService.GetResource("ActivityLog.CreateMagazine"), magazine.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Magazines.Magazine.Added"));
                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = magazine.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model = new MagazineModel();
            return View(model);
        }

        public virtual ActionResult Edit(int Id)
        {
            var magazine = _magazineService.GetMagazineById(Id);
            if (magazine == null)
                //No magazine found with the specified id
                return RedirectToAction("List");

            var model = magazine.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(magazine.CreatedOnUtc, DateTimeKind.Utc);
            model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(magazine.UpdatedOnUtc, DateTimeKind.Utc);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(MagazineModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var magazine = _magazineService.GetMagazineById(model.Id);
            if (magazine == null)
                //No address found with the specified id
                return RedirectToAction("List");



            if (ModelState.IsValid)
            {
                var prevDownloadId = magazine.DownloadId;
                var prevSampleDownloadId = magazine.SampleDownloadId;

                //magazine
                magazine = model.ToEntity(magazine);
                magazine.UpdatedOnUtc = DateTime.UtcNow;

                _magazineService.UpdateMagazine(magazine);

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != magazine.DownloadId)
                {
                    var prevDownload = _downloadService.GetDownloadById(prevDownloadId);
                    if (prevDownload != null)
                        _downloadService.DeleteDownload(prevDownload);
                }
                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != magazine.SampleDownloadId)
                {
                    var prevSampleDownload = _downloadService.GetDownloadById(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        _downloadService.DeleteDownload(prevSampleDownload);
                }

                //activity log
                _customerActivityService.InsertActivity("EditMagazine", _localizationService.GetResource("ActivityLog.EditMagazine"), magazine.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Magazines.Magazine.Updated"));
                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { Id = model.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model = new MagazineModel();
            return View(model);
        }


        //delete product
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var magazine = _magazineService.GetMagazineById(id);
            if (magazine == null)
                //No product found with the specified id
                return RedirectToAction("List");

            _magazineService.DeleteMagazine(magazine);

            //activity log
            _customerActivityService.InsertActivity("DeleteMagazine", _localizationService.GetResource("ActivityLog.DeletedMagazine"), magazine.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Magazine.Deleted"));
            return RedirectToAction("List");
        }


        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _magazineService.DeleteMagazines(_magazineService.GetMagazinesByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }
    }
}