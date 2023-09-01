using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Vendors;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Admin.Helpers;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using System.Globalization;
using Nop.Services.Orders;
using Nop.Core.Domain.Catalog;

namespace Nop.Admin.Controllers
{
    public partial class VendorController : BaseAdminController
    {
        #region Fields
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly VendorSettings _vendorSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ICacheManager _cacheManager;
        private readonly IAclService _aclService;
        private readonly IOrderService _orderService;

        #endregion

        #region Constructors

        public VendorController(ICustomerService customerService,
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IVendorService vendorService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            IWorkContext workContext,
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IDateTimeHelper dateTimeHelper,
            VendorSettings vendorSettings,
            ICustomerActivityService customerActivityService,
            IAddressService addressService,
            ICountryService countryService,
            IStoreService storeService,
            ICacheManager cacheManager,
            IAclService aclService,
            IStateProvinceService stateProvinceService,
            IOrderService orderService)
        {
            this._categoryService = categoryService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._vendorService = vendorService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._dateTimeHelper = dateTimeHelper;
            this._vendorSettings = vendorSettings;
            this._customerActivityService = customerActivityService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._storeMappingService = storeMappingService;
            this._cacheManager = cacheManager;
            this._aclService = aclService;
            this._orderService = orderService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));

            //cover picture
            var coverpicture = _pictureService.GetPictureById(vendor.CoverPictureId);
            if (coverpicture != null)
                _pictureService.SetSeoFilename(coverpicture.Id, _pictureService.GetPictureSeName(vendor.Name));

            var mpLogo = _pictureService.GetPictureById(vendor.mpLogo);
            if (mpLogo != null)
                _pictureService.SetSeoFilename(mpLogo.Id, _pictureService.GetPictureSeName(vendor.Name));

            var shopbanner = _pictureService.GetPictureById(vendor.ShopBanner);
            if (shopbanner != null)
                _pictureService.SetSeoFilename(shopbanner.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        [NonAction]
        protected virtual void UpdateLocales(Vendor vendor, VendorModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(vendor,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = vendor.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(vendor, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareVendorModel(VendorModel model, Vendor vendor, bool excludeProperties, bool prepareEntireAddressModel)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var address = _addressService.GetAddressById(vendor != null ? vendor.AddressId : 0);

            if (vendor != null)
            {
                if (!excludeProperties)
                {
                    if (address != null)
                    {
                        model.Address = address.ToModel();
                    }
                }

                //associated customer emails
                model.AssociatedCustomers = _customerService
                    .GetAllCustomers(vendorId: vendor.Id)
                    .Select(c => new VendorModel.AssociatedCustomerInfo()
                    {
                        Id = c.Id,
                        Email = c.Email
                    })
                    .ToList();
            }
            //Store
            if (vendor != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(vendor).ToList();

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

            if (prepareEntireAddressModel)
            {
                model.Address.CountryEnabled = true;
                model.Address.StateProvinceEnabled = true;
                model.Address.CityEnabled = true;
                model.Address.StreetAddressEnabled = true;
                model.Address.StreetAddress2Enabled = true;
                model.Address.ZipPostalCodeEnabled = true;
                model.Address.PhoneEnabled = true;
                model.Address.FaxEnabled = true;

                //address
                model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(showHidden: true))
                    model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (address != null && c.Id == address.CountryId) });

                var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                        model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (address != null && s.Id == address.StateProvinceId) });
                }
                else
                    model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });


                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#000000", Text = "Black", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#000000")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#000080", Text = "Navy", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#000080")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#008000", Text = "Green", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#008000")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#D3D3D3", Text = "LightGray", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#D3D3D3")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#FFD700", Text = "Red", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#FFD700")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#FF0000", Text = "Gold", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#FF0000")) });
                model.AvaliableThemeColor.Add(new SelectListItem { Value = "#FFFF00", Text = "Yellow", Selected = (model.ThemeColor != null && model.ThemeColor.Equals("#FFFF00")) });


                var mobileAppSetting = _vendorService.GetMobileAppSetting(vendor != null ? vendor.Id : 0);
                if (mobileAppSetting == null)
                {
                    mobileAppSetting = new MobileAppSetting
                    {
                        CallToOrderEnabled = true,
                        CatalogEnabled = true,
                        ImageAspectRatio = 1,
                        LoyalityEnabled = true,
                        NewTabEnabled = true,
                        ShopTheLookEnabled = true,
                        ShortcutEnabled = true,
                        AboutUsEnabled = true,
                        TabAnimation = 1,
                        ShopTheLookType = 1,
                        HomeTabType = 1
                    };
                }
                model.MobileAppSettingModel = mobileAppSetting.ToModel();

                model.MobileAppSettingModel.AvailableTabAnimation.Add(new SelectListItem { Value = "1", Text = "List Animation", Selected = mobileAppSetting.TabAnimation.Equals(1) ? true : false });
                model.MobileAppSettingModel.AvailableTabAnimation.Add(new SelectListItem { Value = "2", Text = "Box Animation", Selected = mobileAppSetting.TabAnimation.Equals(2) ? true : false });
                model.MobileAppSettingModel.AvailableTabAnimation.Add(new SelectListItem { Value = "3", Text = "Banner Animation", Selected = mobileAppSetting.TabAnimation.Equals(3) ? true : false });
                model.MobileAppSettingModel.AvailableTabAnimation.Add(new SelectListItem { Value = "4", Text = "Reverse Scroll Animation", Selected = mobileAppSetting.TabAnimation.Equals(4) ? true : false });

                model.MobileAppSettingModel.AvailableImageAspectRatio.Add(new SelectListItem { Value = "1.00", Text = "1", Selected = mobileAppSetting.ImageAspectRatio.Equals(1.00M) ? true : false });
                model.MobileAppSettingModel.AvailableImageAspectRatio.Add(new SelectListItem { Value = "1.50", Text = "1.5", Selected = mobileAppSetting.ImageAspectRatio.Equals(1.50M) ? true : false });

                model.MobileAppSettingModel.AvailableShopTheLookType.Add(new SelectListItem { Value = "1", Text = "Type 1", Selected = mobileAppSetting.ShopTheLookType.Equals(1) ? true : false });
                model.MobileAppSettingModel.AvailableShopTheLookType.Add(new SelectListItem { Value = "2", Text = "Type 2", Selected = mobileAppSetting.ShopTheLookType.Equals(2) ? true : false });

                model.MobileAppSettingModel.AvailableHomeTabType.Add(new SelectListItem { Value = "1", Text = "Type 1", Selected = mobileAppSetting.HomeTabType.Equals(1) });
                model.MobileAppSettingModel.AvailableHomeTabType.Add(new SelectListItem { Value = "2", Text = "Type 2", Selected = mobileAppSetting.HomeTabType.Equals(2) });

                //Social
                var socialLinks = _vendorService.GetSocialLinks(vendor != null ? vendor.Id : 0);
                if (socialLinks == null)
                {
                    socialLinks = new SocialLinks();
                }
                model.SocialLinks = socialLinks.ToModel();

                //paymentInfo
                var paymentInfo = _vendorService.GetVendorPaymentInfo(vendor != null ? vendor.Id : 0);
                if (paymentInfo == null)
                {
                    paymentInfo = new VendorPaymentInfo();
                }
                model.PaymentInfo = paymentInfo.ToModel();


                //Plateform

                model.AvailablePlatform.Add(new SelectListItem { Value = "1", Text = "Android MarketPlace", Selected = mobileAppSetting.HomeTabType.Equals(1) });
                model.AvailablePlatform.Add(new SelectListItem { Value = "2", Text = "Android Store", Selected = mobileAppSetting.HomeTabType.Equals(2) });
                model.AvailablePlatform.Add(new SelectListItem { Value = "3", Text = "IOS MarketPlace", Selected = mobileAppSetting.HomeTabType.Equals(3) });
                model.AvailablePlatform.Add(new SelectListItem { Value = "4", Text = "IOS Store", Selected = mobileAppSetting.HomeTabType.Equals(4) });
                model.AvailablePlatform.Add(new SelectListItem { Value = "5", Text = "Web MarketPlace", Selected = mobileAppSetting.HomeTabType.Equals(5) });
                model.AvailablePlatform.Add(new SelectListItem { Value = "6", Text = "Web Store", Selected = mobileAppSetting.HomeTabType.Equals(6) });

            }

        }

        [NonAction]
        protected virtual void SaveStoreMappings(Vendor vendor, VendorModel model)
        {
            vendor.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(vendor);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(vendor, store.Id);
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
        protected virtual void PrepareCategoryMappingModel(VendorModel model, Vendor vendor, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && vendor != null)
                model.SelectedMappedCategoryIds = _categoryService.GetVendorMappedCategoriesByVendorId(vendor.Id, true).Select(c => c.CategoryId).ToList();

            var allMarketPlaceCategories = SelectListHelper.GetMarketPlaceCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in allMarketPlaceCategories)
            {
                c.Selected = model.SelectedMappedCategoryIds.Contains(int.Parse(c.Value));
                model.AvailableCategories.Add(c);
            }
        }

        [NonAction]
        protected virtual void SaveCategoryMappings(Vendor vendor, VendorModel model)
        {
            var existingVendorMappedCategories = _categoryService.GetVendorMappedCategoriesByVendorId(vendor.Id, true);

            //delete categories
            foreach (var existingVendorCategory in existingVendorMappedCategories)
                if (!model.SelectedMappedCategoryIds.Contains(existingVendorCategory.CategoryId))
                    _categoryService.DeleteVendorMappedCategory(existingVendorCategory);

            //add categories
            foreach (var categoryId in model.SelectedMappedCategoryIds)
                if (existingVendorMappedCategories.FindVendorMappedCategory(vendor.Id, categoryId) != null)
                {

                    //var existingCategoryMapping = _categoryService.GetVendorMappedCategoriesByCategoryId(categoryId, showHidden: true);
                    var existingCategoryMapping = existingVendorMappedCategories.Where(x=>x.CategoryId== categoryId);
                    if (existingCategoryMapping.Any())
                    {
                        var category = existingCategoryMapping.FirstOrDefault();
                        if (category != null)
                        {
                            //find next display order
                            category.DisplayOrder = category.DisplayOrder + 1;
                            _categoryService.UpdateVendorMappedCategory(category);
                        }
                    }
                }
                else
                {
                    _categoryService.InsertVendorMappedCategory(new VendorMappedCategory
                    {
                        VendorId = vendor.Id,
                        CategoryId = categoryId,
                        DisplayOrder = 1
                    });
                }
        }

        #endregion

        #region Vendors

        #region List

        //list
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var model = new VendorListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            model.ShowHidden = true;
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, VendorListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            var vendors = _vendorService.GetAllVendorsLimitedToStore(model.SearchName, model.SearchStoreId, command.Page - 1, command.PageSize, model.ShowHidden);
            var gridModel = new DataSourceResult
            {
                Data = vendors.Select(x =>
                {
                    var vendorModel = x.ToModel();
                    PrepareVendorModel(vendorModel, x, false, false);
                    return vendorModel;
                }),
                Total = vendors.TotalCount,
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        //create
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();


            var model = new VendorModel();
            PrepareVendorModel(model, null, false, true);
            PrepareCategoryMappingModel(model, null, false);
            //locales
            AddLocales(_languageService, model.Locales);

            //default values
            model.PageSize = 6;
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;
            model.ShippingCharge = _vendorSettings.DefaultShippingCharge;
            model.CommissionPercentage = _vendorSettings.DefaultCommissionPercentage;


            //default value
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Create(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendor = model.ToEntity();
                _vendorService.InsertVendor(vendor);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendor", _localizationService.GetResource("ActivityLog.AddNewVendor"), vendor.Id);

                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);

                //address
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                _addressService.InsertAddress(address);
                vendor.AddressId = address.Id;
                _vendorService.UpdateVendor(vendor);
                //Stores
                SaveStoreMappings(vendor, model);

                //MobileAppSetting
                var mobileAppSetting = model.MobileAppSettingModel.ToEntity();
                mobileAppSetting.VendorId = vendor.Id;
                _vendorService.InsertMobileAppSetting(mobileAppSetting);

                //Socials
                var socialLinks = model.SocialLinks.ToEntity();
                socialLinks.VendorId = vendor.Id;
                _vendorService.InsertSocialLinks(socialLinks);

                //Payments
                var payments = model.PaymentInfo.ToEntity();
                payments.VendorId = vendor.Id;
                _vendorService.InsertVendorPaymentInfo(payments);


                //locales
                UpdateLocales(vendor, model);
                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                //categories
                SaveCategoryMappings(vendor, model);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareVendorModel(model, null, true, true);
            PrepareCategoryMappingModel(model, null, true);
            return View(model);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var model = vendor.ToModel();
            PrepareVendorModel(model, vendor, false, true);
            PrepareCategoryMappingModel(model, vendor, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = vendor.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = vendor.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = vendor.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = vendor.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = vendor.GetSeName(languageId, false, false);
            });

            //associated customer emails
            model.AssociatedCustomers = _customerService
                .GetAllCustomers(vendorId: vendor.Id)
                .Select(c => new VendorModel.AssociatedCustomerInfo()
                {
                    Id = c.Id,
                    Email = c.Email
                })
                .ToList();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = vendor.PictureId;
                int prevCoverId = vendor.CoverPictureId;
                int prevmpLogo = vendor.mpLogo;
                int shopBanner = vendor.ShopBanner;
                vendor = model.ToEntity(vendor);
                _vendorService.UpdateVendor(vendor);

                //activity log
                _customerActivityService.InsertActivity("EditVendor", _localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id);

                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);


                //Stores
                SaveStoreMappings(vendor, model);

                //address
                var address = _addressService.GetAddressById(vendor.AddressId);
                if (address == null)
                {
                    address = model.Address.ToEntity();
                    address.CreatedOnUtc = DateTime.UtcNow;
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.InsertAddress(address);
                    vendor.AddressId = address.Id;
                    _vendorService.UpdateVendor(vendor);
                }
                else
                {
                    address = model.Address.ToEntity(address);
                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.UpdateAddress(address);
                }


                //locales
                UpdateLocales(vendor, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                if (prevCoverId > 0 && prevCoverId != vendor.CoverPictureId)
                {
                    var prevCover = _pictureService.GetPictureById(prevCoverId);
                    if (prevCover != null)
                        _pictureService.DeletePicture(prevCover);
                }

                if (prevmpLogo > 0 && prevmpLogo != vendor.mpLogo)
                {
                    var prevLogo = _pictureService.GetPictureById(prevmpLogo);
                    if (prevLogo != null)
                        _pictureService.DeletePicture(prevLogo);
                }

                if (shopBanner > 0 && shopBanner != vendor.ShopBanner)
                {
                    var prevLogo = _pictureService.GetPictureById(shopBanner);
                    if (prevLogo != null)
                        _pictureService.DeletePicture(prevLogo);
                }




                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                //categories
                SaveCategoryMappings(vendor, model);

                //update MobieApp Setting
                var MobileAppSetting = _vendorService.GetMobileAppSetting(vendor.Id);
                if (MobileAppSetting == null)
                {
                    MobileAppSetting = model.MobileAppSettingModel.ToEntity();
                    MobileAppSetting.VendorId = vendor.Id;
                    _vendorService.InsertMobileAppSetting(MobileAppSetting);

                }
                else
                {
                    MobileAppSetting = model.MobileAppSettingModel.ToEntity(MobileAppSetting);
                    _vendorService.UpdateMobileAppSetting(MobileAppSetting);
                }

                //update Social

                var socialLinks = _vendorService.GetSocialLinks(vendor.Id);
                if (socialLinks == null)
                {
                    socialLinks = model.SocialLinks.ToEntity();
                    socialLinks.VendorId = vendor.Id;
                    _vendorService.InsertSocialLinks(socialLinks);

                }
                else
                {
                    socialLinks = model.SocialLinks.ToEntity(socialLinks);
                    _vendorService.UpdateSocialLinks(socialLinks);
                }

                //update Payment Info

                var paymentInfo = _vendorService.GetVendorPaymentInfo(vendor.Id);
                if (paymentInfo == null)
                {
                    paymentInfo = model.PaymentInfo.ToEntity();
                    paymentInfo.VendorId = vendor.Id;
                    _vendorService.InsertVendorPaymentInfo(paymentInfo);

                }
                else
                {
                    paymentInfo = model.PaymentInfo.ToEntity(paymentInfo);
                    _vendorService.UpdateVendorPaymentInfo(paymentInfo);
                }


                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareVendorModel(model, vendor, true, true);
            PrepareCategoryMappingModel(model, vendor, true);
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = _customerService.GetAllCustomers(vendorId: vendor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.VendorId = 0;
                _customerService.UpdateCustomer(customer);
            }

            //delete a vendor
            _vendorService.DeleteVendor(vendor);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendor", _localizationService.GetResource("ActivityLog.DeleteVendor"), vendor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Disable/ Enable Categories

        [HttpPost]
        public virtual ActionResult DisableCategory(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");
            //enable Categories
            _categoryService.DisableVendorCategoriesByVendorId(id);

            //activity log
            _customerActivityService.InsertActivity("EditVendor", _localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Category.Disabled"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult EnableCategory(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");
            //enable Categories
            _categoryService.EnableVendorCategoriesByVendorId(id);

            //activity log
            _customerActivityService.InsertActivity("EditVendor", _localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Category.Enabled"));
            return RedirectToAction("List");
        }
        #endregion

        #region Disable/ Enable Products

        [HttpPost]
        public virtual ActionResult DisableProducts(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(Id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");
            //enable Products
            _categoryService.DisableVendorProductsByVendorId(Id);

            //activity log
            _customerActivityService.InsertActivity("EditVendor", _localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Products.Disabled"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult EnableProducts(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(Id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");
            //enable Categories
            _categoryService.EnableVendorProductsByVendorId(Id);

            //activity log
            _customerActivityService.InsertActivity("EditVendor", _localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Products.Enabled"));
            return RedirectToAction("List");
        }
        #endregion

        #endregion

        #region VendorNotes

        [HttpPost]
        public virtual ActionResult VendorNotesSelect(int vendorId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            var vendorNoteModels = new List<VendorModel.VendorNote>();
            foreach (var vendorNote in vendor.VendorNotes
                .OrderByDescending(vn => vn.CreatedOnUtc))
            {
                vendorNoteModels.Add(new VendorModel.VendorNote
                {
                    Id = vendorNote.Id,
                    VendorId = vendorNote.VendorId,
                    Note = vendorNote.FormatVendorNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(vendorNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = vendorNoteModels,
                Total = vendorNoteModels.Count
            };

            return Json(gridModel);
        }

        [ValidateInput(false)]
        public virtual ActionResult VendorNoteAdd(int vendorId, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var vendorNote = new VendorNote
            {
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
            };
            vendor.VendorNotes.Add(vendorNote);
            _vendorService.UpdateVendor(vendor);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult VendorNoteDelete(int id, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            var vendorNote = vendor.VendorNotes.FirstOrDefault(vn => vn.Id == id);
            if (vendorNote == null)
                throw new ArgumentException("No vendor note found with the specified id");
            _vendorService.DeleteVendorNote(vendorNote);

            return new NullJsonResult();
        }

        #endregion

        #region VendorBanners

        [ValidateInput(false)]
        public virtual ActionResult VendorBannerAdd(int ImageId, int displayOrder,
          string Description, string Title,
          int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (ImageId == 0)
                throw new ArgumentException();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            var picture = _pictureService.GetPictureById(ImageId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                Description,
                Title);

            _pictureService.SetSeoFilename(ImageId, _pictureService.GetPictureSeName(vendor.Name));

            _vendorService.InsertVendorBanner(new VendorBanner
            {
                VendorId = vendor.Id,
                ImageId = picture.Id,
                Url = _pictureService.GetPictureUrl(picture.Id),
                Title = Title,
                Description = Description,
                DisplayOrder = displayOrder,

            });

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult VendorBannerList(DataSourceRequest command, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            var vendorBanners = _vendorService.GetVendorBannersByVendorId(vendorId);
            var vendorBannersModel = vendorBanners
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.ImageId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new VendorModel.VendorBanner
                    {
                        Id = x.Id,
                        VendorId = x.VendorId,
                        ImageId = x.ImageId,
                        Url = _pictureService.GetPictureUrl(picture),
                        Title = x.Title,
                        Description = x.Description,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = vendorBannersModel,
                Total = vendorBannersModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult VendorBannerUpdate(VendorModel.VendorBanner model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendorBanner = _vendorService.GetVendorBannerById(model.Id);
            if (vendorBanner == null)
                throw new ArgumentException("No vendor banner found with the specified id");

            var picture = _pictureService.GetPictureById(vendorBanner.ImageId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.Description,
                model.Title);

            vendorBanner.Title = model.Title;
            vendorBanner.Description = model.Description;
            vendorBanner.DisplayOrder = model.DisplayOrder;
            _vendorService.UpdateVendorBanner(vendorBanner);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult VendorBannerDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var vendorBanner = _vendorService.GetVendorBannerById(id);
            if (vendorBanner == null)
                throw new ArgumentException("No vendor banner found with the specified id");

            var vendorId = vendorBanner.VendorId;

            var pictureId = vendorBanner.ImageId;
            _vendorService.DeleteVendorBanner(vendorBanner);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Payouts

        [HttpPost]
        public ActionResult ListPayouts(int VendorId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var records = _vendorService.GetVendorPayouts(VendorId, null, command.Page, command.PageSize);

            CultureInfo cInf = new CultureInfo(_workContext.WorkingCurrency.DisplayLocale);
            RegionInfo rInf = new RegionInfo(cInf.LCID);
            var dataModel = records
                .Select(x =>
                {
                    var vendorPayoutModel = VendorHelper.PreparePayoutModel(x, _orderService);
                    vendorPayoutModel.CurrencySymbol = rInf.CurrencySymbol;
                    return vendorPayoutModel;
                })
                .ToList();
            var model = new DataSourceResult
            {
                Data = dataModel,
                Total = records.Count,
                ExtraData = new
                {
                    VendorTotal = rInf.CurrencySymbol + " " + dataModel.Sum(m => m.VendorOrderTotal).ToString("0.00"),
                    CommissionTotal =
                        rInf.CurrencySymbol + " " + dataModel.Sum(m => m.CommissionAmount).ToString("0.00"),
                    PayoutTotal = rInf.CurrencySymbol + " " + dataModel.Sum(m => m.PayoutAmount).ToString("0.00")
                }
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        public ActionResult DeletePayout(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();


            var payout = _vendorService.GetVendorPayout(Id);
            if (payout != null)
                _vendorService.DeleteVendorPayout(payout);

            return new NullJsonResult();

        }

        [HttpPost]
        public ActionResult UpdatePayout(VendorPayoutModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();



            var payout = _vendorService.GetVendorPayout(model.Id);
            if (payout != null)
            {
                payout.CommissionPercentage = model.CommissionPercentage;
                payout.PayoutDate = model.PayoutDate;
                payout.PayoutStatus = model.PayoutStatus;
                payout.Remarks = model.Remarks;
                payout.VendorOrderTotal = model.VendorOrderTotal;
                payout.ShippingCharge = model.ShippingCharge;
                _vendorService.SaveVendorPayout(payout);
            }
            return new NullJsonResult();
        }
        #endregion

        #region Reviews

        [HttpPost]
        public ActionResult ListReviews(int VendorId, DataSourceRequest command)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var records = _vendorService.GetVendorReviews(VendorId, null, null, command.Page, command.PageSize);
            var dataModel = records.Select(x =>
            {
                var order = _orderService.GetOrderById(x.OrderId);
                var orderItems = order.OrderItems.First(oi => oi.Product.Id == x.ProductId);
                return x.ToVendorReviewModel(order, orderItems.Product);

            });

            var model = new DataSourceResult
            {
                Data = dataModel,
                Total = records.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveReview(VendorReviewModel model)
        {

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return Json(new
                {
                    success = false,
                    error = "Order Not Avalible"
                });
            //only requested review
            var custId = _workContext.CurrentCustomer.Id;
            if (!_vendorService.IsVendorReviewDone(model.VendorId, custId, model.OrderId, model.ProductId))
            {

                //check if customer can review this vendor for this order
                if (_vendorService.CanCustomerReviewVendor(model.VendorId, custId, order))
                {
                    //yes he can
                    var review = new VendorReview
                    {
                        CreatedOnUTC = DateTime.Now,
                        CustomerId = custId,
                        HelpfulnessNoTotal = 0,
                        HelpfulnessYesTotal = 0,
                        IsApproved = false,
                        OrderId = model.OrderId,
                        ProductId = model.ProductId,
                        Rating = model.Rating,
                        ReviewText = model.ReviewText,
                        Title = model.Title,
                        VendorId = model.VendorId
                    };
                    _vendorService.SaveVendorReview(review);
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    error = "You have already reviewed this order"
                });
            }
            return Json(new
            {
                success = false,
                error = "You need to login to review a vendor"
            });
        }

        [HttpPost]
        public ActionResult DeleteReview(int Id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();


            var review = _vendorService.GetVendorReview(Id);
            if (review != null)
                _vendorService.DeleteVendorReview(review);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult UpdateReview(VendorReviewModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var review = _vendorService.GetVendorReview(model.Id);
            if (review == null)
                return new NullJsonResult();

            review.IsApproved = model.IsApproved;
            review.Rating = model.Rating;
            review.ReviewText = model.ReviewText;
            review.Title = model.Title;
            review.HelpfulnessNoTotal = model.HelpfulnessNoTotal;
            review.HelpfulnessYesTotal = model.HelpfulnessYesTotal;
            _vendorService.SaveVendorReview(review);
            return new NullJsonResult();
        }

        #endregion

    }

}
