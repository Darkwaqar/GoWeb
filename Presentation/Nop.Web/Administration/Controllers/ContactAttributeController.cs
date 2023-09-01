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
using Nop.Core.Domain.Contact;
using Nop.Admin.Models.Contact;

namespace Nop.Admin.Controllers
{
    public partial class ContactAttributeController : BaseAdminController
    {
        #region Fields
        private readonly IContactAttributeService _contactAttributeService;
        private readonly IContactAttributeParser _contactAttributeParser;
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

        public ContactAttributeController(IContactAttributeService contactAttributeService,
            IContactAttributeParser contactAttributeParser,
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
            this._contactAttributeService = contactAttributeService;
            this._contactAttributeParser = contactAttributeParser;
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
        protected virtual void UpdateAttributeLocales(ContactAttribute contactAttribute, ContactAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(contactAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(contactAttribute,
                                                               x => x.TextPrompt,
                                                               localized.TextPrompt,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateValueLocales(ContactAttributeValue contactAttributeValue, ContactAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(contactAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(ContactAttributeModel model, ContactAttribute contactAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && contactAttribute != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(contactAttribute).ToList();

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
        protected virtual void SaveStoreMappings(ContactAttribute contactAttribute, ContactAttributeModel model)
        {
            contactAttribute.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(contactAttribute);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(contactAttribute, store.Id);
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
        protected virtual void PrepareConditionAttributes(ContactAttributeModel model, ContactAttribute contactAttribute)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //currently any contact attribute can have condition.
            model.ConditionAllowed = true;

            if (contactAttribute == null)
                return;

            var selectedAttribute = _contactAttributeParser.ParseContactAttributes(contactAttribute.ConditionAttributeXml).FirstOrDefault();
            var selectedValues = _contactAttributeParser.ParseContactAttributeValues(contactAttribute.ConditionAttributeXml);

            model.ConditionModel = new ConditionModel()
            {
                EnableCondition = !string.IsNullOrEmpty(contactAttribute.ConditionAttributeXml),
                SelectedAttributeId = selectedAttribute != null ? selectedAttribute.Id : 0,
                ConditionAttributes = _contactAttributeService.GetAllContactAttributes()
                    //ignore this attribute and non-combinable attributes
                    .Where(x => x.Id != contactAttribute.Id && x.CanBeUsedAsCondition())
                    .Select(x =>
                        new AttributeConditionModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            AttributeControlType = x.AttributeControlType,
                            Values = _contactAttributeService.GetContactAttributeValues(x.Id)
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
        protected virtual void SaveConditionAttributes(ContactAttribute contactAttribute, ContactAttributeModel model)
        {
            string attributesXml = null;
            if (model.ConditionModel.EnableCondition)
            {
                var attribute = _contactAttributeService.GetContactAttributeById(model.ConditionModel.SelectedAttributeId);
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
                                    attributesXml = _contactAttributeParser.AddContactAttribute(attributesXml, attribute, selectedValue);
                                else
                                    //for conditions we should empty values save even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _contactAttributeParser.AddContactAttribute(attributesXml, attribute, string.Empty);
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var selectedAttribute = model.ConditionModel.ConditionAttributes
                                    .FirstOrDefault(x => x.Id == model.ConditionModel.SelectedAttributeId);
                                var selectedValues = selectedAttribute != null ? selectedAttribute.Values.Where(x => x.Selected).Select(x => x.Value) : null;
                                if (selectedValues.Any())
                                    foreach (var value in selectedValues)
                                        attributesXml = _contactAttributeParser.AddContactAttribute(attributesXml, attribute, value);
                                else
                                    attributesXml = _contactAttributeParser.AddContactAttribute(attributesXml, attribute, string.Empty);
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
            contactAttribute.ConditionAttributeXml = attributesXml;
        }

        [NonAction]
        protected virtual void PrepareAclModel(ContactAttributeModel model, ContactAttribute category, bool excludeProperties)
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

        #region Contact attributes

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

            var contactAttributes = _contactAttributeService.GetAllContactAttributes();
            var gridModel = new DataSourceResult
            {
                Data = contactAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = contactAttributes.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult Create()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new ContactAttributeModel();
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
        public virtual ActionResult Create(ContactAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var contactAttribute = model.ToEntity();
                _contactAttributeService.InsertContactAttribute(contactAttribute);
                //locales
                UpdateAttributeLocales(contactAttribute, model);
                //Stores
                SaveStoreMappings(contactAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewContactAttribute", _localizationService.GetResource("ActivityLog.AddNewContactAttribute"), contactAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ContactAttributes.Added"));



                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = contactAttribute.Id });
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

            var contactAttribute = _contactAttributeService.GetContactAttributeById(id);
            if (contactAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            var model = contactAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
           {
               locale.Name = contactAttribute.GetLocalized(x => x.Name, languageId, false, false);
               locale.TextPrompt = contactAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
           });
            //ACL
            PrepareAclModel(model, contactAttribute, false);
            //Stores
            PrepareStoresMappingModel(model, contactAttribute, false);
            //condition
            PrepareConditionAttributes(model, contactAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(ContactAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var contactAttribute = _contactAttributeService.GetContactAttributeById(model.Id);
            if (contactAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                contactAttribute = model.ToEntity(contactAttribute);
                SaveConditionAttributes(contactAttribute, model);
                _contactAttributeService.UpdateContactAttribute(contactAttribute);
                //locales
                UpdateAttributeLocales(contactAttribute, model);
                //Stores
                SaveStoreMappings(contactAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditContactAttribute", _localizationService.GetResource("ActivityLog.EditContactAttribute"), contactAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ContactAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = contactAttribute.Id });
                }
                return RedirectToAction("List");

            }
            //If we got this far, something failed, redisplay form
            //Stores
            PrepareStoresMappingModel(model, contactAttribute, true);
            //ACL
            PrepareAclModel(model, contactAttribute, true);

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var contactAttribute = _contactAttributeService.GetContactAttributeById(id);
            _contactAttributeService.DeleteContactAttribute(contactAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteContactAttribute", _localizationService.GetResource("ActivityLog.DeleteContactAttribute"), contactAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.ContactAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Contact attribute values

        //list
        [HttpPost]
        public virtual ActionResult ValueList(int contactAttributeId, DataSourceRequest command)
        {
            var contactAttribute = _contactAttributeService.GetContactAttributeById(contactAttributeId);
            var values = contactAttribute.ContactAttributeValues;
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new ContactAttributeValueModel
                {
                    Id = x.Id,
                    ContactAttributeId = x.ContactAttributeId,
                    Name = contactAttribute.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                    ColorSquaresRgb = x.ColorSquaresRgb,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult ValueCreatePopup(int contactAttributeId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var contactAttribute = _contactAttributeService.GetContactAttributeById(contactAttributeId);
            var model = new ContactAttributeValueModel();
            model.ContactAttributeId = contactAttributeId;

            //color squares
            model.DisplayColorSquaresRgb = contactAttribute.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueCreatePopup(string btnId, string formId, ContactAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var contactAttribute = _contactAttributeService.GetContactAttributeById(model.ContactAttributeId);
            if (contactAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            if (contactAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                var cav = new ContactAttributeValue
                {
                    ContactAttributeId = model.ContactAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _contactAttributeService.InsertContactAttributeValue(cav);
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

            var cav = _contactAttributeService.GetContactAttributeValueById(id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List");

            var model = new ContactAttributeValueModel
            {
                ContactAttributeId = cav.ContactAttributeId,
                Name = cav.Name,
                ColorSquaresRgb = cav.ColorSquaresRgb,
                DisplayColorSquaresRgb = cav.ContactAttribute.AttributeControlType == AttributeControlType.ColorSquares,
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
        public virtual ActionResult ValueEditPopup(string btnId, string formId, ContactAttributeValueModel model)
        {
            var contactAttribute = _contactAttributeService.GetContactAttributeById(model.ContactAttributeId);

            var cav = contactAttribute.ContactAttributeValues.Where(x => x.Id == model.Id).FirstOrDefault();
            if (cav == null)
                //No contact attribute value found with the specified id
                return RedirectToAction("List");

            if (contactAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                _contactAttributeService.UpdateContactAttributeValue(cav);

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

            var cav = _contactAttributeService.GetContactAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No contact attribute value found with the specified id");
            _contactAttributeService.DeleteContactAttributeValue(cav);

            return new NullJsonResult();
        }
        #endregion
    }
}