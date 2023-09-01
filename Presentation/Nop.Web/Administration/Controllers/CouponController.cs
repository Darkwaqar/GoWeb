using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Affiliates;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Affiliates;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System.IO;
using System.Collections.Generic;
using Nop.Services.Common;
using Nop.Web.Framework.Mvc;
using Nop.Services.ExportImport;

namespace Nop.Admin.Controllers
{
    public partial class CouponController : BaseAdminController
    {
        #region Fields

        private readonly ICouponService _couponService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;

        #endregion

        #region Constructors

        public CouponController(ICouponService couponService,
            IPriceFormatter priceFormatter, IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper, LocalizationSettings localizationSettings,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            ILocalizationService localizationService, ILanguageService languageService,
            ICustomerActivityService customerActivityService, IPermissionService permissionService,
            IPdfService pdfService,IExportManager exportManager,
            IImportManager importManager)
        {
            this._couponService = couponService;
            this._priceFormatter = priceFormatter;
            this._workflowMessageService = workflowMessageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationSettings = localizationSettings;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var model = new CouponListModel();
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Coupons.List.Activated.All")
            });
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Coupons.List.Activated.ActivatedOnly")
            });
            model.ActivatedList.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Coupons.List.Activated.DeactivatedOnly")
            });

            model.GenerateCouponBulkModel.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult CouponList(DataSourceRequest command, CouponListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedKendoGridJson();

            bool? isCouponActivated = null;
            if (model.ActivatedId == 1)
                isCouponActivated = true;
            else if (model.ActivatedId == 2)
                isCouponActivated = false;
            var coupons = _couponService.GetAllCoupons(isCouponActivated: isCouponActivated,
                couponCouponCode: model.CouponCode,
                recipientName: model.RecipientName,
                pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = coupons.Select(x =>
                {
                    var m = x.ToModel();
                    m.RemainingAmountStr = _priceFormatter.FormatPrice(x.GetCouponRemainingAmount(), true, false);
                    m.AmountStr = _priceFormatter.FormatPrice(x.Amount, true, false);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return m;
                }),
                Total = coupons.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var model = new CouponModel();
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CouponModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var coupon = model.ToEntity();
                coupon.CreatedOnUtc = DateTime.UtcNow;
                _couponService.InsertCoupon(coupon);

                //activity log
                _customerActivityService.InsertActivity("AddNewCoupon", _localizationService.GetResource("ActivityLog.AddNewCoupon"), coupon.CouponCouponCode);

                SuccessNotification(_localizationService.GetResource("Admin.Coupons.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = coupon.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupon = _couponService.GetCouponById(id);
            if (coupon == null)
                //No gift card found with the specified id
                return RedirectToAction("List");

            var model = coupon.ToModel();
            model.RemainingAmountStr = _priceFormatter.FormatPrice(coupon.GetCouponRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(coupon.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(coupon.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(CouponModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupon = _couponService.GetCouponById(model.Id);

            model.RemainingAmountStr = _priceFormatter.FormatPrice(coupon.GetCouponRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(coupon.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(coupon.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            if (ModelState.IsValid)
            {
                coupon = model.ToEntity(coupon);
                _couponService.UpdateCoupon(coupon);

                //activity log
                _customerActivityService.InsertActivity("EditCoupon", _localizationService.GetResource("ActivityLog.EditCoupon"), coupon.CouponCouponCode);

                SuccessNotification(_localizationService.GetResource("Admin.Coupons.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = coupon.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _couponService.GenerateCouponCode() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual ActionResult NotifyRecipient(CouponModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupon = _couponService.GetCouponById(model.Id);

            model = coupon.ToModel();
            model.RemainingAmountStr = _priceFormatter.FormatPrice(coupon.GetCouponRemainingAmount(), true, false);
            model.AmountStr = _priceFormatter.FormatPrice(coupon.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(coupon.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            try
            {
                if (!CommonHelper.IsValidEmail(coupon.RecipientEmail))
                    throw new NopException("Recipient email is not valid");
                if (!CommonHelper.IsValidEmail(coupon.SenderEmail))
                    throw new NopException("Sender email is not valid");


                var languageId = _localizationSettings.DefaultAdminLanguageId;
                int queuedEmailId = _workflowMessageService.SendCouponNotification(coupon, languageId);
                if (queuedEmailId > 0)
                {
                    coupon.IsRecipientNotified = true;
                    _couponService.UpdateCoupon(coupon);
                    model.IsRecipientNotified = true;
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc, false);
            }

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupon = _couponService.GetCouponById(id);
            if (coupon == null)
                //No gift card found with the specified id
                return RedirectToAction("List");

            _couponService.DeleteCoupon(coupon);

            //activity log
            _customerActivityService.InsertActivity("DeleteCoupon", _localizationService.GetResource("ActivityLog.DeleteCoupon"), coupon.CouponCouponCode);

            SuccessNotification(_localizationService.GetResource("Admin.Coupons.Deleted"));
            return RedirectToAction("List");
        }

        //Gif card usage history
        [HttpPost]
        public virtual ActionResult UsageHistoryList(int couponId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedKendoGridJson();

            var coupon = _couponService.GetCouponById(couponId);
            if (coupon == null)
                throw new ArgumentException("No gift card found with the specified id");

            var usageHistoryModel = coupon.CouponUsageHistory.OrderByDescending(gcuh => gcuh.CreatedOnUtc)
                .Select(x => new CouponModel.CouponUsageHistoryModel
                {
                    Id = x.Id,
                    AffiliateId = x.AffiliateId,
                    UsedValue = _priceFormatter.FormatPrice(x.UsedValue, true, false),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                    AffiliateName = x.UsedWithAffiliate.GetFullName(),
                    FromSender = x.FromSender,
                    ToRecipient = x.ToRecipient,
                    FromCity = x.FromCity,
                    FromState = x.FromState,
                    FromZip = x.FromZip,
                    FromCountry = x.FromCountry,
                    ToCity = x.ToCity,
                    ToState = x.ToState,
                    ToZip = x.ToZip,
                    ToCountry = x.ToCountry,
                })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = usageHistoryModel.PagedForCommand(command),
                Total = usageHistoryModel.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-coupon-pdf")]
        public virtual ActionResult DownloadCatalogAsPdf(CouponListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            bool? isCouponActivated = null;
            if (model.ActivatedId == 1)
                isCouponActivated = true;
            else if (model.ActivatedId == 2)
                isCouponActivated = false;

            var coupons = _couponService.GetAllCoupons(isCouponActivated: isCouponActivated,
                couponCouponCode: model.CouponCode,
                recipientName: model.RecipientName);

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintCouponsToPdf(stream, coupons);
                    bytes = stream.ToArray();
                }
                return File(bytes, MimeTypes.ApplicationPdf, "pdfcoupons.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual ActionResult ExportXmlAll(CouponListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            bool? isCouponActivated = null;
            if (model.ActivatedId == 1)
                isCouponActivated = true;
            else if (model.ActivatedId == 2)
                isCouponActivated = false;

            var coupons = _couponService.GetAllCoupons(isCouponActivated: isCouponActivated,
                couponCouponCode: model.CouponCode,
                recipientName: model.RecipientName);

            try
            {
                var xml = _exportManager.ExportCouponsToXml(coupons);
                return new XmlDownloadResult(xml, "coupons.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupons = new List<Coupon>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                coupons.AddRange(_couponService.GetCouponsByIds(ids));
            }

            var xml = _exportManager.ExportCouponsToXml(coupons);
            return new XmlDownloadResult(xml, "coupons.xml");
        }


        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual ActionResult ExportExcelAll(CouponListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            bool? isCouponActivated = null;
            if (model.ActivatedId == 1)
                isCouponActivated = true;
            else if (model.ActivatedId == 2)
                isCouponActivated = false;

            var coupons = _couponService.GetAllCoupons(isCouponActivated: isCouponActivated,
                couponCouponCode: model.CouponCode,
                recipientName: model.RecipientName);

            try
            {
                var bytes = _exportManager.ExportCouponsToXlsx(coupons);

                return File(bytes, MimeTypes.TextXlsx, "coupons.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            var coupons = new List<Coupon>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                coupons.AddRange(_couponService.GetCouponsByIds(ids));
            }
            
            var bytes = _exportManager.ExportCouponsToXlsx(coupons);

            return File(bytes, MimeTypes.TextXlsx, "coupons.xlsx");
        }

        [HttpPost]
        public virtual ActionResult ImportExcel()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportCouponsFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Coupons.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }
        #endregion

        #region Create Coupon Bulk

        [HttpPost]
        public virtual ActionResult GenerateBulkCoupon(CouponListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCoupons))
                return AccessDeniedView();

            if (model.GenerateCouponBulkModel.NumberOfCoupon == 0 )
            {
                ErrorNotification("Please Provide how much you want to create Coupon");
                return RedirectToAction("List");
            }
            var copyModel = model.GenerateCouponBulkModel;
            try
            {
                for (int i = 0; i < model.GenerateCouponBulkModel.NumberOfCoupon; i++)
                {
                    var coupon = new Coupon();

                    coupon.CouponTypeId = model.GenerateCouponBulkModel.CouponTypeId;
                    coupon.CouponCouponCode = _couponService.GenerateCouponCode();
                    coupon.Amount = model.GenerateCouponBulkModel.Amount;
                    coupon.SenderName = model.GenerateCouponBulkModel.SenderName;
                    coupon.Message = model.GenerateCouponBulkModel.Message;
                    coupon.IsCouponActivated = true;
                    coupon.CreatedOnUtc = DateTime.UtcNow;
                    _couponService.InsertCoupon(coupon);

                    _customerActivityService.InsertActivity("AddNewCoupon", _localizationService.GetResource("ActivityLog.AddNewCoupon"), coupon.CouponCouponCode);
                }
                
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }
        #endregion
    }
}
