using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Core;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.SMS;
using Nop.Core.Domain.SMS;
using Nop.Admin.Models.SMS;

namespace Nop.Admin.Controllers
{
	public partial class QueuedSMSController : BaseAdminController
	{
		private readonly IQueuedSMSService _queuedSMSService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

		public QueuedSMSController(IQueuedSMSService queuedSMSService,
            IDateTimeHelper dateTimeHelper, 
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext)
		{
            this._queuedSMSService = queuedSMSService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
		}

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

		    var model = new QueuedSMSListModel
		    {
                //default value
		        SearchMaxSentTries = 10
		    };
            return View(model);
		}

		[HttpPost]
		public virtual ActionResult QueuedSMSList(DataSourceRequest command, QueuedSMSListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedKendoGridJson();

            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var queuedSMSs = _queuedSMSService.SearchSMSs(model.SearchFromSMS, model.SearchToSMS, 
                startDateValue, endDateValue, 
                model.SearchLoadNotSent, false, model.SearchMaxSentTries, true,
                command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = queuedSMSs.Select(x => {
                    var m = x.ToModel();
                    m.PriorityName = x.Priority.GetLocalizedEnum(_localizationService, _workContext);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    if (x.DontSendBeforeDateUtc.HasValue)
                        m.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(x.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    if (x.SentOnUtc.HasValue)
                        m.SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOnUtc.Value, DateTimeKind.Utc);

                    //little performance optimization: ensure that "Body" is not returned
                    m.Body = "";

                    return m;
                }),
                Total = queuedSMSs.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-email-by-number")]
        public virtual ActionResult GoToSMSByNumber(QueuedSMSListModel model)
        {
            var queuedSMS = _queuedSMSService.GetQueuedSMSById(model.GoDirectlyToNumber);
            if (queuedSMS == null)
                return List();
            
            return RedirectToAction("Edit", "QueuedSMS", new { id = queuedSMS.Id });
        }

		public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

			var email = _queuedSMSService.GetQueuedSMSById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            var model = email.ToModel();
            model.PriorityName = email.Priority.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(email.CreatedOnUtc, DateTimeKind.Utc);
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            if (email.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(email.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            else model.SendImmediately = true;
            return View(model);
		}

        [HttpPost, ActionName("Edit")]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(QueuedSMSModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var email = _queuedSMSService.GetQueuedSMSById(model.Id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                email.DontSendBeforeDateUtc = (model.SendImmediately || !model.DontSendBeforeDate.HasValue) ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value);
                _queuedSMSService.UpdateQueuedSMS(email);

                SuccessNotification(_localizationService.GetResource("Admin.System.QueuedSMSs.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.PriorityName = email.Priority.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(email.CreatedOnUtc, DateTimeKind.Utc);
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            if (email.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(email.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            return View(model);
		}

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public virtual ActionResult Requeue(QueuedSMSModel queuedSMSModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var queuedSMS = _queuedSMSService.GetQueuedSMSById(queuedSMSModel.Id);
            if (queuedSMS == null)
                //No email found with the specified id
                return RedirectToAction("List");

            var requeuedSMS = new QueuedSMS
            {
                PriorityId = queuedSMS.PriorityId,
                From = queuedSMS.From,
                FromName = queuedSMS.FromName,
                To = queuedSMS.To,
                ToName = queuedSMS.ToName,
                ReplyTo = queuedSMS.ReplyTo,
                ReplyToName = queuedSMS.ReplyToName,
                CC = queuedSMS.CC,
                Bcc = queuedSMS.Bcc,
                Subject = queuedSMS.Subject,
                Body = queuedSMS.Body,
                AttachmentFilePath = queuedSMS.AttachmentFilePath,
                AttachmentFileName = queuedSMS.AttachmentFileName,
                AttachedDownloadId = queuedSMS.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                NumberAccountId = queuedSMS.NumberAccountId,
                DontSendBeforeDateUtc = (queuedSMSModel.SendImmediately || !queuedSMSModel.DontSendBeforeDate.HasValue) ? 
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(queuedSMSModel.DontSendBeforeDate.Value)
            };
            _queuedSMSService.InsertQueuedSMS(requeuedSMS);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedSMSs.Requeued"));
            return RedirectToAction("Edit", new { id = requeuedSMS.Id });
        }

	    [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

			var email = _queuedSMSService.GetQueuedSMSById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("List");

            _queuedSMSService.DeleteQueuedSMS(email);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedSMSs.Deleted"));
			return RedirectToAction("List");
		}

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _queuedSMSService.DeleteQueuedSMSs(_queuedSMSService.GetQueuedSMSsByIds(selectedIds.ToArray()));
            }

            return Json(new { Result = true });
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("delete-all")]
        public virtual ActionResult DeleteAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            _queuedSMSService.DeleteAllSMSs();

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedSMSs.DeletedAll"));
            return RedirectToAction("List");
        }

	}
}
