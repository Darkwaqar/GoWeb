using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Polls;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Polls;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class PollCategoryController : BaseAdminController
    {
        #region Fields

        private readonly IPollCategoryService _pollCategoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPollService _pollService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IDiscountService _discountService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public PollCategoryController(IPollCategoryService pollCategoryService,
            IManufacturerService manufacturerService, IPollService pollService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IDiscountService discountService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager,
            ICacheManager cacheManager)
        {
            this._pollCategoryService = pollCategoryService;
            this._manufacturerService = manufacturerService;
            this._pollService = pollService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._discountService = discountService;
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(PollCategory pollCategory, PollCategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pollCategory,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(pollCategory,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(pollCategory,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(pollCategory,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(pollCategory,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = pollCategory.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(pollCategory, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(PollCategory pollCategory)
        {
            var picture = _pictureService.GetPictureById(pollCategory.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(pollCategory.Name));

        }

        private void PrepareAllVendorModel(PollCategoryModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var currentVendor = _workContext.CurrentVendor;
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = currentVendor.Name,
                    Value = currentVendor.Id.ToString()
                });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"),
                    Value = "0"
                });

                var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
                foreach (var c in vendors)
                    model.AvailableVendors.Add(c);
            }
        }


        [NonAction]
        protected virtual void PrepareAclModel(PollCategoryModel model, PollCategory pollCategory, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && pollCategory != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(pollCategory).ToList();

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

        [NonAction]
        protected virtual void SavePollCategoryAcl(PollCategory pollCategory, PollCategoryModel model)
        {
            pollCategory.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(pollCategory);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(pollCategory, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(PollCategoryModel model, PollCategory pollCategory, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && pollCategory != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(pollCategory).ToList();

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
        protected virtual void SaveStoreMappings(PollCategory pollCategory, PollCategoryModel model)
        {
            pollCategory.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(pollCategory);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(pollCategory, store.Id);
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

            var model = new PollCategoryListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            if (_workContext.CurrentVendor != null)
            {
                model.AvailableVendors.Add(new SelectListItem { Text = _workContext.CurrentVendor.Name, Value = "0" });
            }
            else
            {
                model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
                foreach (var s in _vendorService.GetAllVendors(showHidden: true))
                    model.AvailableVendors.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            }
            model.ShowHidden = true;

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, PollCategoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            if (_workContext.CurrentVendor != null)
            {
                var currentvendor = _workContext.CurrentVendor;
                model.SearchVendorId = currentvendor.Id;
            }
            var categories = _pollCategoryService.SearchVendorCategories(model.SearchPollCategoryName,
            model.SearchStoreId, model.SearchVendorId, command.Page - 1, command.PageSize, model.ShowHidden, model.ShowMarketplace, model.ShowFeatured);
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var pollCategoryModel = x.ToModel();
                    //picture
                    pollCategoryModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(pollCategoryModel.PictureId, 75, true);
                    return pollCategoryModel;
                }),
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new PollCategoryModel();
            //locales
            AddLocales(_languageService, model.Locales);
            
            //Vendor
            PrepareAllVendorModel(model);
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.Published = true;
            model.IncludeInTopMenu = true;
            model.AllowCustomersToSelectPageSize = true;

            return View(model);
        }



        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(PollCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var pollCategory = model.ToEntity();
                pollCategory.CreatedOnUtc = DateTime.UtcNow;
                pollCategory.UpdatedOnUtc = DateTime.UtcNow;
                _pollCategoryService.InsertPollCategory(pollCategory);
                //search engine name
                model.SeName = pollCategory.ValidateSeName(model.SeName, pollCategory.Name, true);
                _urlRecordService.SaveSlug(pollCategory, model.SeName, 0);
                //locales
                UpdateLocales(pollCategory, model);
                _pollCategoryService.UpdatePollCategory(pollCategory);
                //update picture seo file name
                UpdatePictureSeoNames(pollCategory);
                //ACL (customer roles)
                SavePollCategoryAcl(pollCategory, model);
                //Stores
                SaveStoreMappings(pollCategory, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewPollCategory", _localizationService.GetResource("ActivityLog.AddNewPollCategory"), pollCategory.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = pollCategory.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
           
          
            //Vendor
            PrepareAllVendorModel(model);
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollCategory = _pollCategoryService.GetPollCategoryById(id);
            if (pollCategory == null || pollCategory.Deleted)
                //No pollCategory found with the specified id
                return RedirectToAction("List");

            var model = pollCategory.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = pollCategory.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = pollCategory.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = pollCategory.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = pollCategory.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = pollCategory.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = pollCategory.GetSeName(languageId, false, false);
            });
            
           
            //Vendor
            PrepareAllVendorModel(model);
            //ACL
            PrepareAclModel(model, pollCategory, false);
            //Stores
            PrepareStoresMappingModel(model, pollCategory, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(PollCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollCategory = _pollCategoryService.GetPollCategoryById(model.Id);
            if (pollCategory == null || pollCategory.Deleted)
                //No pollCategory found with the specified id
                return RedirectToAction("List");


            if (ModelState.IsValid)
            {
                int prevPictureId = pollCategory.PictureId;
                pollCategory = model.ToEntity(pollCategory);
                pollCategory.UpdatedOnUtc = DateTime.UtcNow;
                _pollCategoryService.UpdatePollCategory(pollCategory);
                //search engine name
                model.SeName = pollCategory.ValidateSeName(model.SeName, pollCategory.Name, true);
                _urlRecordService.SaveSlug(pollCategory, model.SeName, 0);
                //locales
                UpdateLocales(pollCategory, model);

                _pollCategoryService.UpdatePollCategory(pollCategory);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != pollCategory.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
               
                //update picture seo file name
                UpdatePictureSeoNames(pollCategory);
                //ACL
                SavePollCategoryAcl(pollCategory, model);
                //Stores
                SaveStoreMappings(pollCategory, model);

                //activity log
                _customerActivityService.InsertActivity("EditPollCategory", _localizationService.GetResource("ActivityLog.EditPollCategory"), pollCategory.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = pollCategory.Id });
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
          
            
            //Vendor
            PrepareAllVendorModel(model);
         
            //ACL
            PrepareAclModel(model, pollCategory, true);
            //Stores
            PrepareStoresMappingModel(model, pollCategory, true);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollCategory = _pollCategoryService.GetPollCategoryById(id);
            if (pollCategory == null)
                //No pollCategory found with the specified id
                return RedirectToAction("List");

            _pollCategoryService.DeletePollCategory(pollCategory);

            //activity log
            _customerActivityService.InsertActivity("DeletePollCategory", _localizationService.GetResource("ActivityLog.DeletePollCategory"), pollCategory.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));
            return RedirectToAction("List");
        }


        #endregion

        #region Export / Import

        public virtual ActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var xml = _exportManager.ExportCategoriesToXml();
                return new XmlDownloadResult(xml, "categories.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual ActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportPollCategoriesToXlsx(_pollCategoryService.GetAllCategories(showHidden: true).Where(p => !p.Deleted));

                return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ImportFromXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportCategoriesFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #endregion

        #region Polls

        [HttpPost]
        public virtual ActionResult PollList(DataSourceRequest command, int pollCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var pollCategories = _pollCategoryService.GetPollCategoriesByPollCategoryId(pollCategoryId,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = pollCategories.Select(x => new PollCategoryModel.PollCategoryPollModel
                {
                    Id = x.Id,
                    PollCategoryId = x.PollCategoryId,
                    PollId = x.PollId,
                    PollName = _pollService.GetPollById(x.PollId).Name,
                    IsFeaturedPoll = x.IsFeaturedPoll,
                    DisplayOrder = x.DisplayOrder
                }),
                Total = pollCategories.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult PollUpdate(PollCategoryModel.PollCategoryPollModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollPollCategory = _pollCategoryService.GetPollPollCategoryById(model.Id);
            if (pollPollCategory == null)
                throw new ArgumentException("No poll pollCategory mapping found with the specified id");

            pollPollCategory.IsFeaturedPoll = model.IsFeaturedPoll;
            pollPollCategory.DisplayOrder = model.DisplayOrder;
            _pollCategoryService.UpdatePollPollCategory(pollPollCategory);

            return new NullJsonResult();
        }

        public virtual ActionResult PollDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollPollCategory = _pollCategoryService.GetPollPollCategoryById(id);
            if (pollPollCategory == null)
                throw new ArgumentException("No poll pollCategory mapping found with the specified id");

            //var pollCategoryId = pollPollCategory.PollCategoryId;
            _pollCategoryService.DeletePollPollCategory(pollPollCategory);

            return new NullJsonResult();
        }

        public virtual ActionResult PollAddPopup(int pollCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var model = new PollCategoryModel.AddPollCategoryPollModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetPollCategoryList(_pollCategoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);


            return View(model);
        }

        [HttpPost]
        public virtual ActionResult PollAddPopupList(DataSourceRequest command, PollCategoryModel.AddPollCategoryPollModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();

            var gridModel = new DataSourceResult();
            var polls = _pollService.SearchPolls(
                pollCategoryIds: new List<int> { model.SearchPollCategoryId },
                storeId: model.SearchStoreId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = polls.Select(x => x.ToModel());
            gridModel.Total = polls.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult PollAddPopup(string btnId, string formId, PollCategoryModel.AddPollCategoryPollModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (model.SelectedPollIds != null)
            {
                foreach (int id in model.SelectedPollIds)
                {
                    var poll = _pollService.GetPollById(id);
                    if (poll != null)
                    {
                        var existingPollCategories = _pollCategoryService.GetPollCategoriesByPollCategoryId(model.PollCategoryId, showHidden: true);
                        if (existingPollCategories.FindPollPollCategory(id, model.PollCategoryId) == null)
                        {
                            _pollCategoryService.InsertPollPollCategory(
                                new PollPollCategory
                                {
                                    PollCategoryId = model.PollCategoryId,
                                    PollId = id,
                                    IsFeaturedPoll = false,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Disable / Enable Poll

        [HttpPost]
        public virtual ActionResult EnablePolls(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollCategory = _pollCategoryService.GetPollCategoryById(id);
            if (pollCategory == null)
                //No pollCategory found with the specified id
                return RedirectToAction("List");

            _pollCategoryService.EnablePollCategoryPollByPollCategoryId(pollCategory.Id);

            //activity log
            _customerActivityService.InsertActivity("EditPollCategory", _localizationService.GetResource("ActivityLog.PollCategoryPollEnabled"), pollCategory.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.PollCategoryPollEnabled"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult DisablePolls(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var pollCategory = _pollCategoryService.GetPollCategoryById(id);
            if (pollCategory == null)
                //No pollCategory found with the specified id
                return RedirectToAction("List");

            _pollCategoryService.DisablePollCategoryPollByPollCategoryId(pollCategory.Id);

            //activity log
            _customerActivityService.InsertActivity("EditPollCategory", _localizationService.GetResource("ActivityLog.PollCategoryPollDisable"), pollCategory.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.PollCategoryPollDisable"));
            return RedirectToAction("List");
        }
        #endregion
    }
}
