using Nop.Core.Domain.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
using Nop.Core.Domain.Vendors;
using Nop.Admin.Models.Vendors;
using Nop.Services.Vendors;

namespace Nop.Admin.Controllers
{
    public partial class VendorAttributeController : BaseAdminController
    {
        #region Fields
        private readonly IVendorAttributeService _VendorAttributeService;
        private readonly IVendorAttributeParser _VendorAttributeParser;
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

        public VendorAttributeController(IVendorAttributeService VendorAttributeService,
            IVendorAttributeParser VendorAttributeParser,
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
            this._VendorAttributeService = VendorAttributeService;
            this._VendorAttributeParser = VendorAttributeParser;
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
        protected virtual void UpdateAttributeLocales(VendorAttribute VendorAttribute, VendorAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(VendorAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(VendorAttribute,
                                                               x => x.TextPrompt,
                                                               localized.TextPrompt,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateValueLocales(VendorAttributeValue VendorAttributeValue, VendorAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(VendorAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(VendorAttributeModel model, VendorAttribute VendorAttribute, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && VendorAttribute != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(VendorAttribute).ToList();

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
        protected virtual void SaveStoreMappings(VendorAttribute VendorAttribute, VendorAttributeModel model)
        {
            VendorAttribute.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(VendorAttribute);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(VendorAttribute, store.Id);
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
        protected virtual void PrepareAclModel(VendorAttributeModel model, VendorAttribute category, bool excludeProperties)
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

            var VendorAttributes = _VendorAttributeService.GetAllVendorAttributes();
            var gridModel = new DataSourceResult
            {
                Data = VendorAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    attributeModel.AttributeControlTypeName = x.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
                    return attributeModel;
                }),
                Total = VendorAttributes.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult Create()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new VendorAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //ACL
            PrepareAclModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var VendorAttribute = model.ToEntity();
                _VendorAttributeService.InsertVendorAttribute(VendorAttribute);
                //locales
                UpdateAttributeLocales(VendorAttribute, model);
                //Stores
                SaveStoreMappings(VendorAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendorAttribute", _localizationService.GetResource("ActivityLog.AddNewVendorAttribute"), VendorAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Added"));



                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = VendorAttribute.Id });
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

            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(id);
            if (VendorAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            var model = VendorAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = VendorAttribute.GetLocalized(x => x.Name, languageId, false, false);
                locale.TextPrompt = VendorAttribute.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });
            //ACL
            PrepareAclModel(model, VendorAttribute, false);
            //Stores
            PrepareStoresMappingModel(model, VendorAttribute, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(VendorAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(model.Id);
            if (VendorAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                VendorAttribute = model.ToEntity(VendorAttribute);
                _VendorAttributeService.UpdateVendorAttribute(VendorAttribute);
                //locales
                UpdateAttributeLocales(VendorAttribute, model);
                //Stores
                SaveStoreMappings(VendorAttribute, model);

                //activity log
                _customerActivityService.InsertActivity("EditVendorAttribute", _localizationService.GetResource("ActivityLog.EditVendorAttribute"), VendorAttribute.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = VendorAttribute.Id });
                }
                return RedirectToAction("List");

            }
            //If we got this far, something failed, redisplay form
            //Stores
            PrepareStoresMappingModel(model, VendorAttribute, true);
            //ACL
            PrepareAclModel(model, VendorAttribute, true);

            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(id);
            _VendorAttributeService.DeleteVendorAttribute(VendorAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendorAttribute", _localizationService.GetResource("ActivityLog.DeleteVendorAttribute"), VendorAttribute.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.VendorAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Contact attribute values

        //list
        [HttpPost]
        public virtual ActionResult ValueList(int VendorAttributeId, DataSourceRequest command)
        {
            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(VendorAttributeId);
            var values = VendorAttribute.VendorAttributeValues;
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new VendorAttributeValueModel
                {
                    Id = x.Id,
                    VendorAttributeId = x.VendorAttributeId,
                    Name = VendorAttribute.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                    ColorSquaresRgb = x.ColorSquaresRgb,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult ValueCreatePopup(int VendorAttributeId)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(VendorAttributeId);
            var model = new VendorAttributeValueModel();
            model.VendorAttributeId = VendorAttributeId;

            //color squares
            model.DisplayColorSquaresRgb = VendorAttribute.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";

            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueCreatePopup(string btnId, string formId, VendorAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(model.VendorAttributeId);
            if (VendorAttribute == null)
                //No contact attribute found with the specified id
                return RedirectToAction("List");

            if (VendorAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                var cav = new VendorAttributeValue
                {
                    VendorAttributeId = model.VendorAttributeId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _VendorAttributeService.InsertVendorAttributeValue(cav);
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

            var cav = _VendorAttributeService.GetVendorAttributeValueById(id);
            if (cav == null)
                //No checkout attribute value found with the specified id
                return RedirectToAction("List");

            var model = new VendorAttributeValueModel
            {
                VendorAttributeId = cav.VendorAttributeId,
                Name = cav.Name,
                ColorSquaresRgb = cav.ColorSquaresRgb,
                DisplayColorSquaresRgb = cav.VendorAttribute.AttributeControlType == AttributeControlType.ColorSquares,
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
        public virtual ActionResult ValueEditPopup(string btnId, string formId, VendorAttributeValueModel model)
        {
            var VendorAttribute = _VendorAttributeService.GetVendorAttributeById(model.VendorAttributeId);

            var cav = VendorAttribute.VendorAttributeValues.Where(x => x.Id == model.Id).FirstOrDefault();
            if (cav == null)
                //No contact attribute value found with the specified id
                return RedirectToAction("List");

            if (VendorAttribute.AttributeControlType == AttributeControlType.ColorSquares)
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
                _VendorAttributeService.UpdateVendorAttributeValue(cav);

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

            var cav = _VendorAttributeService.GetVendorAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No contact attribute value found with the specified id");
            _VendorAttributeService.DeleteVendorAttributeValue(cav);

            return new NullJsonResult();
        }
        #endregion
    }
}