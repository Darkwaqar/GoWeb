using Nop.Services.Localization;
using Nop.Services.Messages;
using System.Web.Mvc;
using Nop.Web.Framework.Kendoui;
using Nop.Admin.Models.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using Nop.Services.Helpers;
using Nop.Core;
using System.Collections.Generic;
using Nop.Web.Framework.Controllers;
using Nop.Admin.Extensions;
using System.Linq;
using Nop.Admin.Models.Contact;
using Nop.Core.Domain.Messages;

namespace Nop.Admin.Controllers
{

    public partial class ContactFormController : BaseAdminController
    {
        private readonly IContactUsService _contactUsService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IQueuedEmailService _queuedEmailService;

        public ContactFormController(IContactUsService contactUsService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IQueuedEmailService queuedEmailService)
        {
            this._contactUsService = contactUsService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;
            this._emailAccountService = emailAccountService;
            this._emailAccountSettings = emailAccountSettings;
            this._queuedEmailService = queuedEmailService;
        }


        public virtual ActionResult Index() => RedirectToAction("List");

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContactForm))
                return AccessDeniedView();

            var model = new ContactFormListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ContactFormList(DataSourceRequest command, ContactFormListModel model)
        {

            DateTime? startDateValue = (model.SearchStartDate == null) ? null
                         : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.SearchEndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var contactform = _contactUsService.GetAllContactUs(
                fromUtc: startDateValue,
                toUtc: endDateValue,
                email: model.SearchEmail,
                storeId: model.StoreId,
                vendorId: vendorId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var contactFormModel = new List<ContactFormModel>();
            foreach (var item in contactform)
            {
                var store = _storeService.GetStoreById(item.StoreId);
                var m = item.ToModel();
                m.CreatedOn = _dateTimeHelper.ConvertToUserTime(item.CreatedOnUtc, DateTimeKind.Utc);
                m.Enquiry = "";
                m.Email = m.FullName + " - " + m.Email;
                m.Store = store != null ? store.Name : "-empty-";
                contactFormModel.Add(m);
            }

            var gridModel = new DataSourceResult
            {
                Data = contactFormModel,
                Total = contactFormModel.Count
            };
            return Json(gridModel);
        }

        public virtual ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var contactform = _contactUsService.GetContactUsById(id);
            if (contactform == null)
                return RedirectToAction("List");

            var model = contactform.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(contactform.CreatedOnUtc, DateTimeKind.Utc);
            var store = _storeService.GetStoreById(contactform.StoreId);
            model.Store = store != null ? store.Name : "-empty-";
            var email = _emailAccountService.GetEmailAccountById(contactform.EmailAccountId);
            model.EmailAccountName = email != null ? email.DisplayName : "-empty-";
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContactForm))
                return AccessDeniedView();

            var contactform = _contactUsService.GetContactUsById(id);
            if (contactform == null)
                //No email found with the specified id
                return RedirectToAction("List");

            _contactUsService.DeleteContactUs(contactform);

            SuccessNotification(_localizationService.GetResource("Admin.System.ContactForm.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public virtual ActionResult DeleteAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContactForm))
                return AccessDeniedView();

            _contactUsService.ClearTable();
            SuccessNotification(_localizationService.GetResource("Admin.System.ContactForm.DeletedAll"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContactForm))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _contactUsService.DeleteContactUss(_contactUsService.GetContactUsByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }
        public virtual ActionResult Reply(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContactForm))
                return AccessDeniedView();

            var contactform = _contactUsService.GetContactUsById(id);
            if (contactform == null)
                //No email found with the specified id
                return RedirectToAction("List");

            var model = contactform.ToModel();
            model.AvailableEmailAccounts = _emailAccountService.GetAllEmailAccounts().Select(emailAccount => new SelectListItem
            {
                Value = emailAccount.Id.ToString(),
                Text = string.Format("{0} ({1})", emailAccount.DisplayName, emailAccount.Email)
            }).ToList();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(contactform.CreatedOnUtc, DateTimeKind.Utc);
            model.ToName = contactform.FullName;
            model.To = contactform.Email;
            model.Body = contactform.Enquiry + "<p>addtional Info</p>" + contactform.ContactAttributeDescription;
            model.SendImmediately = true;
            return View(model);
        }

        [HttpPost, ActionName("Reply"), FormValueRequired("requeue")]
        public virtual ActionResult Requeue(ContactFormModel contactFormModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();
            var emailAccount = GetEmailAccount(contactFormModel.EmailAccountId);


            var requeuedEmail = new QueuedEmail
            {
                PriorityId = (int)QueuedEmailPriority.High,
                From = emailAccount.Email,
                FromName = contactFormModel.FromName,
                To = contactFormModel.To,
                ToName = contactFormModel.ToName,
                ReplyTo = contactFormModel.ReplyTo,
                ReplyToName = contactFormModel.ReplyToName,
                CC = contactFormModel.CC,
                Bcc = contactFormModel.Bcc,
                Subject = contactFormModel.Subject,
                Body = contactFormModel.Body,
                AttachmentFilePath = contactFormModel.AttachmentFilePath,
                AttachedDownloadId = contactFormModel.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = contactFormModel.EmailAccountId,
                DontSendBeforeDateUtc = (contactFormModel.SendImmediately || !contactFormModel.DontSendBeforeDate.HasValue) ?
                    null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(contactFormModel.DontSendBeforeDate.Value)
            };
            _queuedEmailService.InsertQueuedEmail(requeuedEmail);

            var contactUs = _contactUsService.GetContactUsById(contactFormModel.Id);
            contactUs.QueuedEmailId = requeuedEmail.Id;
            _contactUsService.UpdateContactUs(contactUs);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.Requeued"));
            return RedirectToAction("View", new { id = contactFormModel.Id });
        }

        [NonAction]
        protected virtual EmailAccount GetEmailAccount(int emailAccountId)
        {
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountId)
                ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);

            if (emailAccount == null)
                throw new NopException("Email account could not be loaded");

            return emailAccount;
        }
    }

}
