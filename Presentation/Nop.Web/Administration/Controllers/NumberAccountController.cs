using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.SMS;
using Nop.Core.Domain.SMS;
using Nop.Admin.Models.SMS;

namespace Nop.Admin.Controllers
{
	public partial class NumberAccountController : BaseAdminController
	{
        private readonly INumberAccountService _numberAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly ISMSSender _smsSender;
        private readonly IStoreContext _storeContext;
        private readonly NumberAccountSettings _numberAccountSettings;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;

        public NumberAccountController(INumberAccountService numberAccountService,
            ILocalizationService localizationService, ISettingService settingService,
            ISMSSender smsSender, IStoreContext storeContext,
            NumberAccountSettings numberAccountSettings, IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._numberAccountService = numberAccountService;
            this._localizationService = localizationService;
            this._numberAccountSettings = numberAccountSettings;
            this._smsSender = smsSender;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
        }

		public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

			return View();
		}

		[HttpPost]
		public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedKendoGridJson();

            var numberAccountModels = _numberAccountService.GetAllNumberAccounts()
                                    .Select(x => x.ToModel())
                                    .ToList();
            foreach (var eam in numberAccountModels)
                eam.IsDefaultNumberAccount = eam.Id == _numberAccountSettings.DefaultNumberAccountId;

            var gridModel = new DataSourceResult
            {
                Data = numberAccountModels,
                Total = numberAccountModels.Count()
            };

            return Json(gridModel);
        }

        public virtual ActionResult MarkAsDefaultNumber(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

            var defaultNumberAccount = _numberAccountService.GetNumberAccountById(id);
            if (defaultNumberAccount != null)
            {
                _numberAccountSettings.DefaultNumberAccountId = defaultNumberAccount.Id;
                _settingService.SaveSetting(_numberAccountSettings);
            }
            return RedirectToAction("List");
        }

		public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

            var model = new NumberAccountModel();
			return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public virtual ActionResult Create(NumberAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var numberAccount = model.ToEntity();
                _numberAccountService.InsertNumberAccount(numberAccount);

                //activity log
                _customerActivityService.InsertActivity("AddNewNumberAccount", _localizationService.GetResource("ActivityLog.AddNewNumberAccount"), numberAccount.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.NumberAccounts.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = numberAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

		public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

			var numberAccount = _numberAccountService.GetNumberAccountById(id);
            if (numberAccount == null)
                //No number account found with the specified id
                return RedirectToAction("List");

			return View(numberAccount.ToModel());
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(NumberAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

            var numberAccount = _numberAccountService.GetNumberAccountById(model.Id);
            if (numberAccount == null)
                //No number account found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                numberAccount = model.ToEntity(numberAccount);
                _numberAccountService.UpdateNumberAccount(numberAccount);

                //activity log
                _customerActivityService.InsertActivity("EditNumberAccount", _localizationService.GetResource("ActivityLog.EditNumberAccount"), numberAccount.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.NumberAccounts.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = numberAccount.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestnumber")]
        public virtual ActionResult SendTestNumber(NumberAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
                return AccessDeniedView();

            var numberAccount = _numberAccountService.GetNumberAccountById(model.Id);
            if (numberAccount == null)
                //No number account found with the specified id
                return RedirectToAction("List");

            if (!CommonHelper.IsValidNumber(model.SendTestSMSTo))
            {
                ErrorNotification(_localizationService.GetResource("Admin.Common.WrongNumber"), false);
                return View(model);
            }

            try
            {
                if (String.IsNullOrWhiteSpace(model.SendTestSMSTo))
                    throw new NopException("Enter test number address");

                string subject = _storeContext.CurrentStore.Name + ". Testing number functionality.";
                string body = "SMS works fine.";
                _smsSender.SendSMS(numberAccount, subject, body, numberAccount.Number, numberAccount.DisplayName, model.SendTestSMSTo, null);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.NumberAccounts.SendTestNumber.Success"), false);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

	    [HttpPost]
	    public virtual ActionResult Delete(int id)
	    {
	        if (!_permissionService.Authorize(StandardPermissionProvider.ManageNumberAccounts))
	            return AccessDeniedView();

	        var numberAccount = _numberAccountService.GetNumberAccountById(id);
	        if (numberAccount == null)
	            //No number account found with the specified id
	            return RedirectToAction("List");

	        try
	        {
	            _numberAccountService.DeleteNumberAccount(numberAccount);

                //activity log
                _customerActivityService.InsertActivity("DeleteNumberAccount", _localizationService.GetResource("ActivityLog.DeleteNumberAccount"), numberAccount.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.NumberAccounts.Deleted"));

                return RedirectToAction("List");
	        }
	        catch (Exception exc)
	        {
	            ErrorNotification(exc);
	            return RedirectToAction("Edit", new {id = numberAccount.Id});
	        }
	    }
	}
}
