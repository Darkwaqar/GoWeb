using Nop.Core.Domain.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Admin.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Services.Appointments;
using Nop.Core.Domain.Appointments;
using Nop.Admin.Models.Appointments;

namespace Nop.Admin.Controllers
{

    public partial class AppointmentAttributeController : BaseAdminController
    {
        #region Fields
        private readonly IAppointmentAttributeService _AppointmentAttributeService;
        private readonly IAppointmentAttributeParser _AppointmentAttributeParser;
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

        public AppointmentAttributeController(IAppointmentAttributeService AppointmentAttributeService,
            IAppointmentAttributeParser AppointmentAttributeParser,
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
            this._AppointmentAttributeService = AppointmentAttributeService;
            this._AppointmentAttributeParser = AppointmentAttributeParser;
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
        protected virtual void UpdateAttributeLocales(AppointmentAttribute AppointmentAttribute, AppointmentAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(AppointmentAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(AppointmentAttribute,
                                                               x => x.TextPrompt,
                                                               localized.TextPrompt,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateValueLocales(AppointmentAttributeValue AppointmentAttributeValue, AppointmentAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(AppointmentAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(AppointmentAttributeModel model, AppointmentAttribute AppointmentAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && AppointmentAttribute != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(AppointmentAttribute).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(AppointmentAttribute AppointmentAttribute, AppointmentAttributeModel model)
        {
            AppointmentAttribute.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(AppointmentAttribute);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(AppointmentAttribute, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareConditionAttributes(AppointmentAttributeModel model, AppointmentAttribute AppointmentAttribute)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //currently any Appointment attribute can have condition.
            model.ConditionAllowed = true;

            if (AppointmentAttribute == null)
                return;

            var selectedAttribute = _AppointmentAttributeParser.ParseAppointmentAttributes(AppointmentAttribute.ConditionAttributeXml).FirstOrDefault();
            var selectedValues = _AppointmentAttributeParser.ParseAppointmentAttributeValues(AppointmentAttribute.ConditionAttributeXml);

            model.ConditionModel = new ConditionModel()
            {
                EnableCondition = !string.IsNullOrEmpty(AppointmentAttribute.ConditionAttributeXml),
                SelectedAttributeId = selectedAttribute != null ? selectedAttribute.Id : 0,
                ConditionAttributes = _AppointmentAttributeService.GetAllAppointmentAttributes()
                    //ignore this attribute and non-combinable attributes
                    .Where(x => x.Id != AppointmentAttribute.Id && x.CanBeUsedAsCondition())
                    .Select(x =>
                        new AttributeConditionModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            AttributeControlType = x.AttributeControlType,
                            Values = _AppointmentAttributeService.GetAppointmentAttributeValues(x.Id)
                                .Select(v => new SelectListItem()
                                {
                                    Text = v.Name,
                                    Value = v.Id.ToString(),
                                    Selected = selectedAttribute != null && selectedAttribute.Id == x.Id && selectedValues.Any(sv => sv.Id == v.Id)
                                }).ToList()
                        }).ToList()
            };
        }

        [NonAction]
        protected virtual void SaveConditionAttributes(AppointmentAttribute AppointmentAttribute, AppointmentAttributeModel model)
        {
            string attributesXml = null;
            if (model.ConditionModel.EnableCondition)
            {
                var attribute = _AppointmentAttributeService.GetAppointmentAttributeById(model.ConditionModel.SelectedAttributeId);
                if (attribute != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValue = selectedAttribute != null ? selectedAttribute.SelectedValueId : null;
                                if (!String.IsNullOrEmpty(selectedValue))
                                    attributesXml = _AppointmentAttributeParser.AddAppointmentAttribute(attributesXml, attribute, selectedValue);
                                else
                                    //for conditions we should empty values save even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _AppointmentAttributeParser.AddAppointmentAttribute(attributesXml, attribute, string.Empty);
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValues = selectedAttribute != null ? selectedAttribute.Values.Where(x => x.Selected).Select(x => x.Value) : null;
                                if (selectedValues.Any())
                                    foreach (var value in selectedValues)
                                        attributesXml = _AppointmentAttributeParser.AddAppointmentAttribute(attributesXml, attribute, value);
                                else
                                    attributesXml = _AppointmentAttributeParser.AddAppointmentAttribute(attributesXml, attribute, string.Empty);
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //these attribute types are not supported as conditions
                            break;
                    }
                }
            }
            AppointmentAttribute.ConditionAttributeXml = attributesXml;
        }

        [NonAction]
        protected virtual void PrepareAclModel(AppointmentAttributeModel model, AppointmentAttribute category, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && category != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(category).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        #endregion

        #region Appointment attributes

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var AppointmentAttributes = _AppointmentAttributeService.GetAllAppointmentAttributes();
            var gridModel = new DataSourceResult
            {
                Data = AppointmentAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = AppointmentAttributes.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult Create()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new AppointmentAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //ACL
            PrepareAclModel(model, null, false);
            //condition
            PrepareConditionAttributes(model, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(AppointmentAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var AppointmentAttribute = model.ToEntity();
                _AppointmentAttributeService.InsertAppointmentAttribute(AppointmentAttribute);
                //locales
                UpdateAttributeLocales(AppointmentAttribute, model);
                //Stores
                SaveStoreMappings(AppointmentAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewAppointmentAttribute", _localizationService.GetResource("ActivityLog.AddNewAppointmentAttribute"), AppointmentAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.AppointmentAttributes.Added"));



                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = AppointmentAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, null, true);

            //ACL
            PrepareAclModel(model, null, true);

            return View(model);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(id);
            if (AppointmentAttribute == null)
                //No Appointment attribute found with the specified id
                return RedirectToAction("List");

            var model = AppointmentAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
           {
               locale.Name = AppointmentAttribute.GetLocalized(x => x.Name, languageId, false, false);
               locale.TextPrompt = AppointmentAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
           });
            //ACL
            PrepareAclModel(model, AppointmentAttribute, false);
            //Stores
            PrepareStoresMappingModel(model, AppointmentAttribute, false);
            //condition
            PrepareConditionAttributes(model, AppointmentAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(AppointmentAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(model.Id);
            if (AppointmentAttribute == null)
                //No Appointment attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                AppointmentAttribute = model.ToEntity(AppointmentAttribute);
                SaveConditionAttributes(AppointmentAttribute, model);
                _AppointmentAttributeService.UpdateAppointmentAttribute(AppointmentAttribute);
                //locales
                UpdateAttributeLocales(AppointmentAttribute, model);
                //Stores
                SaveStoreMappings(AppointmentAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditAppointmentAttribute", _localizationService.GetResource("ActivityLog.EditAppointmentAttribute"), AppointmentAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.AppointmentAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = AppointmentAttribute.Id });
                }
                return RedirectToAction("List");

            }
            //If we got this far, something failed, redisplay form
            //Stores
            PrepareStoresMappingModel(model, AppointmentAttribute, true);
            //ACL
            PrepareAclModel(model, AppointmentAttribute, true);

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(id);
            _AppointmentAttributeService.DeleteAppointmentAttribute(AppointmentAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteAppointmentAttribute", _localizationService.GetResource("ActivityLog.DeleteAppointmentAttribute"), AppointmentAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.AppointmentAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Appointment attribute values

        //list
        [HttpPost]
        public virtual ActionResult ValueList(int AppointmentAttributeId, DataSourceRequest command)
        {
            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(AppointmentAttributeId);
            var values = AppointmentAttribute.AppointmentAttributeValues;
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new AppointmentAttributeValueModel
                {
                    Id = x.Id,
                    AppointmentAttributeId = x.AppointmentAttributeId,
                    Name = AppointmentAttribute.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                    ColorSquaresRgb = x.ColorSquaresRgb,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult ValueCreatePopup(int AppointmentAttributeId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(AppointmentAttributeId);
            var model = new AppointmentAttributeValueModel();
            model.AppointmentAttributeId = AppointmentAttributeId;

            //color squares
            model.DisplayColorSquaresRgb = AppointmentAttribute.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueCreatePopup(string btnId, string formId, AppointmentAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(model.AppointmentAttributeId);
            if (AppointmentAttribute == null)
                //No Appointment attribute found with the specified id
                return RedirectToAction("List");

            if (AppointmentAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                var cav = new AppointmentAttributeValue
                {
                    AppointmentAttributeId = model.AppointmentAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _AppointmentAttributeService.InsertAppointmentAttributeValue(cav);
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

            var cav = _AppointmentAttributeService.GetAppointmentAttributeValueById(id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List");

            var model = new AppointmentAttributeValueModel
            {
                AppointmentAttributeId = cav.AppointmentAttributeId,
                Name = cav.Name,
                ColorSquaresRgb = cav.ColorSquaresRgb,
                DisplayColorSquaresRgb = cav.AppointmentAttribute.AttributeControlType == AttributeControlType.ColorSquares,
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
        public virtual ActionResult ValueEditPopup(string btnId, string formId, AppointmentAttributeValueModel model)
        {
            var AppointmentAttribute = _AppointmentAttributeService.GetAppointmentAttributeById(model.AppointmentAttributeId);

            var cav = AppointmentAttribute.AppointmentAttributeValues.Where(x => x.Id == model.Id).FirstOrDefault();
            if (cav == null)
                //No Appointment attribute value found with the specified id
                return RedirectToAction("List");

            if (AppointmentAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                _AppointmentAttributeService.UpdateAppointmentAttributeValue(cav);

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

            var cav = _AppointmentAttributeService.GetAppointmentAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No Appointment attribute value found with the specified id");
            _AppointmentAttributeService.DeleteAppointmentAttributeValue(cav);

            return new NullJsonResult();
        }
        #endregion
    }
}