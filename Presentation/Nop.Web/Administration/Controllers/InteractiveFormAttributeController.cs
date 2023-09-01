using Nop.Core.Domain.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Messages;
using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Interactive;

namespace Nop.Admin.Controllers
{
    public partial class InteractiveFormAttributeController : BaseAdminController
    {
        #region Fields
        private readonly IInteractiveFormAttributeService _interactiveFormAttributeService;
        private readonly IInteractiveFormService _interactiveFormService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Constructors

        public InteractiveFormAttributeController(IInteractiveFormAttributeService interactiveFormAttributeService,
            IInteractiveFormService interactiveFormService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            ICustomerService customerService)
        {
            this._interactiveFormAttributeService = interactiveFormAttributeService;
            this._interactiveFormService = interactiveFormService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._customerService = customerService;
        }
        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdateAttributeLocales(InteractiveFormAttribute interactiveFormAttribute, InteractiveFormAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(interactiveFormAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

            }
        }

        [NonAction]
        protected virtual void UpdateValueLocales(InteractiveFormAttributeValue interactiveFormAttributeValue, InteractiveFormAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(interactiveFormAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion

        #region InteractiveForm attributes

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List(int interactiveFormId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new InteractiveFormAttributeModel();
            model.InteractiveFormId = interactiveFormId;

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(int interactiveFormId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var interactiveFormAttributes = _interactiveFormAttributeService.GetAllInteractiveFormAttributes(interactiveFormId);
            var gridModel = new DataSourceResult
            {
                Data = interactiveFormAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = interactiveFormAttributes.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult Create(int interactiveFormId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new InteractiveFormAttributeModel();
            model.InteractiveFormId = interactiveFormId;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(InteractiveFormAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {

                var interactiveFormAttribute = model.ToEntity();
                _interactiveFormAttributeService.InsertInteractiveFormAttribute(interactiveFormAttribute);
                //locales
                UpdateAttributeLocales(interactiveFormAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewInteractiveFormAttribute", _localizationService.GetResource("ActivityLog.AddNewInteractiveFormAttribute"), interactiveFormAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.InteractiveFormAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { interactiveFormId = model.InteractiveFormId, interactiveFormAttributeId = interactiveFormAttribute.Id });
                }
                return RedirectToAction("List", new { interactiveFormId = model.InteractiveFormId });
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult Edit(int interactiveFormId, int interactiveFormAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var form = _interactiveFormService.GetFormById(interactiveFormId);
            if (form == null)
                return RedirectToAction("List", "InteractiveForm");

            var attribute = form.InteractiveFormAttributes.FirstOrDefault(x => x.Id == interactiveFormAttributeId);
            if (attribute == null)
                return RedirectToAction("List", "InteractiveForm");

            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(interactiveFormAttributeId);
            if (interactiveFormAttribute == null)
                //No interactiveForm attribute found with the specified id
                return RedirectToAction("List", "InteractiveForm");

            var model = interactiveFormAttribute.ToModel();
            model.InteractiveFormId = interactiveFormId;
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = interactiveFormAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(InteractiveFormAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var form = _interactiveFormService.GetFormById(model.InteractiveFormId);
            if (form == null)
                return RedirectToAction("List", "InteractiveForm");

            var attribute = form.InteractiveFormAttributes.FirstOrDefault(x => x.Id == model.Id);
            if (attribute == null)
                return RedirectToAction("List", "InteractiveForm");

            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(model.Id);
            if (interactiveFormAttribute == null)
                //No interactiveForm attribute found with the specified id
                return RedirectToAction("List", "InteractiveForm");

            if (ModelState.IsValid)
            {
                interactiveFormAttribute = model.ToEntity(interactiveFormAttribute);
                _interactiveFormAttributeService.UpdateInteractiveFormAttribute(interactiveFormAttribute);
                //locales
                UpdateAttributeLocales(interactiveFormAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditInteractiveFormAttribute", _localizationService.GetResource("ActivityLog.EditInteractiveFormAttribute"), interactiveFormAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.InteractiveFormAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { interactiveFormId = model.InteractiveFormId, interactiveFormAttributeId = interactiveFormAttribute.Id });
                }
                return RedirectToAction("List", new { interactiveFormId = model.InteractiveFormId });

            }

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id, int interactiveFormId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var form = _interactiveFormService.GetFormById(interactiveFormId);
            if (form == null)
                return RedirectToAction("List", "InteractiveForm");

            var attribute = form.InteractiveFormAttributes.FirstOrDefault(x => x.Id == id);
            if (attribute == null)
                return RedirectToAction("List", "InteractiveForm");

            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(id);
            _interactiveFormAttributeService.DeleteInteractiveFormAttribute(interactiveFormAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteInteractiveFormAttribute", _localizationService.GetResource("ActivityLog.DeleteInteractiveFormAttribute"), interactiveFormAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.InteractiveFormAttributes.Deleted"));
            return RedirectToAction("List", new { interactiveFormId = interactiveFormId });
        }

        #endregion

        #region InteractiveForm attribute values

        //list
        [HttpPost]
        public virtual ActionResult ValueList(int interactiveFormAttributeId, DataSourceRequest command)
        {
            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(interactiveFormAttributeId);
            var values = interactiveFormAttribute.InteractiveFormAttributeValues;
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new InteractiveFormAttributeValueModel
                {
                    Id = x.Id,
                    InteractiveFormAttributeId = x.InteractiveFormAttributeId,
                    Name = interactiveFormAttribute.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                    ColorSquaresRgb = x.ColorSquaresRgb,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult ValueCreatePopup(int interactiveFormAttributeId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(interactiveFormAttributeId);
            var model = new InteractiveFormAttributeValueModel();
            model.InteractiveFormAttributeId = interactiveFormAttributeId;

            //color squares
            model.DisplayColorSquaresRgb = interactiveFormAttribute.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueCreatePopup(string btnId, string formId, InteractiveFormAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(model.InteractiveFormAttributeId);
            if (interactiveFormAttribute == null)
                //No interactiveForm attribute found with the specified id
                return RedirectToAction("List", "InteractiveForm");

            if (interactiveFormAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                var cav = new InteractiveFormAttributeValue
                {
                    InteractiveFormAttributeId = model.InteractiveFormAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _interactiveFormAttributeService.InsertInteractiveFormAttributeValue(cav);
                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var cav = _interactiveFormAttributeService.GetInteractiveFormAttributeValueById(id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List", "InteractiveForm");

            var model = new InteractiveFormAttributeValueModel
            {
                InteractiveFormAttributeId = cav.InteractiveFormAttributeId,
                Name = cav.Name,
                ColorSquaresRgb = cav.ColorSquaresRgb,
                DisplayColorSquaresRgb = cav.InteractiveFormAttribute.AttributeControlType == AttributeControlType.ColorSquares,
                IsPreSelected = cav.IsPreSelected,
                DisplayOrder = cav.DisplayOrder,
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueEditPopup(string btnId, string formId, InteractiveFormAttributeValueModel model)
        {
            var interactiveFormAttribute = _interactiveFormAttributeService.GetInteractiveFormAttributeById(model.InteractiveFormAttributeId);

            var cav = interactiveFormAttribute.InteractiveFormAttributeValues.Where(x => x.Id == model.Id).FirstOrDefault();
            if (cav == null)
                //No interactiveForm attribute value found with the specified id
                return RedirectToAction("List", "InteractiveForm");

            if (interactiveFormAttribute.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            if (ModelState.IsValid)
            {
                cav.Name = model.Name;
                cav.ColorSquaresRgb = model.ColorSquaresRgb;
                cav.IsPreSelected = model.IsPreSelected;
                cav.DisplayOrder = model.DisplayOrder;
                _interactiveFormAttributeService.UpdateInteractiveFormAttributeValue(cav);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var cav = _interactiveFormAttributeService.GetInteractiveFormAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No interactiveForm attribute value found with the specified id");
            _interactiveFormAttributeService.DeleteInteractiveFormAttributeValue(cav);

            return new NullJsonResult();
        }
        #endregion
    }
}