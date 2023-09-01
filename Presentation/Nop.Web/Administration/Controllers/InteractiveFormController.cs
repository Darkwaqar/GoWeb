using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Admin.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Controllers;
using Nop.Admin.Models.Messages;
using Nop.Services.Security;
using Nop.Core.Domain.Interactive;

namespace Nop.Admin.Controllers
{
    public partial class InteractiveFormController : BaseAdminController
    {
        #region Fields

        private readonly IInteractiveFormService _interactiveFormService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion

        #region Constructors
        public InteractiveFormController(IInteractiveFormService interactiveFormService,
            ILocalizationService localizationService,
            ILanguageService languageService,
            ICustomerActivityService customerActivityService,
            IEmailAccountService emailAccountService,
            IPermissionService permissionProvider,
            ILocalizedEntityService localizedEntityService)
        {
            this._interactiveFormService = interactiveFormService;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._customerActivityService = customerActivityService;
            this._emailAccountService = emailAccountService;
            this._permissionService = permissionProvider;
            this._localizedEntityService = localizedEntityService;
        }
        #endregion

        #region Utilities

        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(", %sendbutton%");
            sb.Append(", %errormessage%");
            return sb.ToString();
        }

        [NonAction]
        protected virtual void PrepareEmailAccountsModel(InteractiveFormModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableEmailAccounts = _emailAccountService.GetAllEmailAccounts().Select(emailAccount => new SelectListItem
            {
                Value = emailAccount.Id.ToString(),
                Text = string.Format("{0} ({1})", emailAccount.DisplayName, emailAccount.Email)
            }).ToList();
        }

        [NonAction]
        protected virtual void UpdateValueLocales(InteractiveFormAttributeValue formAttributeValue, InteractiveFormAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(formAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region List

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var forms = _interactiveFormService.GetAllForms();
            var gridModel = new DataSourceResult
            {
                Data = forms.Select(x =>
                {
                    var model = x.ToModel();
                    model.Body = "";
                    return model;
                }),
                Total = forms.Count
            };
            return Json(gridModel);
        }
        #endregion

        #region Create / Edit / Delete

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var model = new InteractiveFormModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //email accounts
            PrepareEmailAccountsModel(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(InteractiveFormModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var form = model.ToEntity();
                form.CreatedOnUtc = DateTime.UtcNow;

                _interactiveFormService.InsertForm(form);
                _customerActivityService.InsertActivity("InteractiveFormAdd", _localizationService.GetResource("ActivityLog.InteractiveFormAdd"), form.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = form.Id }) : RedirectToAction("List");
            }
            //locales
            AddLocales(_languageService, model.Locales);
            //email accounts
            PrepareEmailAccountsModel(model);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            var form = _interactiveFormService.GetFormById(id);
            if (form == null)
                return RedirectToAction("List");

            var model = form.ToModel();

            AddLocales(_languageService, model.Locales, (locale, languageId) =>
           {
               locale.Name = form.GetLocalized(x => x.Name, languageId, false, false);
               locale.Body = form.GetLocalized(x => x.Body, languageId, false, false);
           });
            //email accounts
            PrepareEmailAccountsModel(model);

            //available tokens
            model.AvailableTokens = FormatTokens(form.InteractiveFormAttributes.Select(x => "%" + x.SystemName + "%").ToArray());
            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(InteractiveFormModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var form = _interactiveFormService.GetFormById(model.Id);
            if (form == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                form = model.ToEntity(form);
                _interactiveFormService.UpdateForm(form);

                _customerActivityService.InsertActivity("InteractiveFormEdit", _localizationService.GetResource("ActivityLog.InteractiveFormUpdate"), form.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = form.Id }) : RedirectToAction("List");
            }

            AddLocales(_languageService, model.Locales, (locale, languageId) =>
           {
               locale.Name = form.GetLocalized(x => x.Name, languageId, false, false);
               locale.Body = form.GetLocalized(x => x.Body, languageId, false, false);
           });
            //email accounts
            PrepareEmailAccountsModel(model);

            //available tokens
            model.AvailableTokens = FormatTokens(form.InteractiveFormAttributes.Select(x => "%" + x.SystemName + "%").ToArray());

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var form = _interactiveFormService.GetFormById(id);
            if (form == null)
                return RedirectToAction("List");

            _interactiveFormService.DeleteForm(form);
            _customerActivityService.InsertActivity("InteractiveFormDelete", _localizationService.GetResource("ActivityLog.InteractiveFormDeleted"), form.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.InteractiveForms.Deleted"));
            return RedirectToAction("List");
        }
        #endregion
    }
}