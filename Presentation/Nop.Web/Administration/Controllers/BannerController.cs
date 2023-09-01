using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Admin.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Controllers;
using Nop.Services.Security;
using Nop.Services.Media;
using Nop.Core;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Helpers;
using Nop.Services.Vendors;
using Nop.Core.Caching;
using Nop.Services.Stores;
using Nop.Admin.Models.Banner;
using Nop.Core.Domain.Banners;

namespace Nop.Admin.Controllers
{
    public partial class BannerController : BaseAdminController
    {
        #region Fields

        private readonly IBannerService _bannerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly IVendorService _vendorService;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Constructors
        public BannerController(IBannerService bannerService,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionProvider,
            IPictureService pictureService,
            IWorkContext workContext,
            IVendorService vendorService,
            ICacheManager cacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService)
        {
            this._bannerService = bannerService;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._permissionService = permissionProvider;
            this._pictureService = pictureService;
            this._workContext = workContext;
            this._vendorService = vendorService;
            this._cacheManager = cacheManager;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
        }
        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareStoresMappingModel(BannerModel model, Banner banner, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && banner != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(banner).ToList();

            var allStores = SelectListHelper.GetVendorStoreList(_storeMappingService, _storeService, _cacheManager, _vendorService, _workContext.CurrentVendor?.Id ?? 0);

            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Text,
                    Value = store.Value,
                    Selected = _workContext.CurrentVendor == null ? model.SelectedStoreIds.Contains(int.Parse(store.Value)) : true
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Banner banner, BannerModel model)
        {
            banner.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(banner);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(banner, store.Id);
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
        protected virtual void PrepareBannerModel(BannerModel model)
        {
            //a vendor should have access only to his banners
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);
        }
        #endregion

        #region List
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var model = new BannerListModel();
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
        public virtual ActionResult List(DataSourceRequest command, BannerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedKendoGridJson();

            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var banners = _bannerService.GetAllBanners(model.SearchBannerName,
            model.SearchStoreId, model.SearchVendorId, command.Page - 1, command.PageSize, model.ShowHidden);
            var gridModel = new DataSourceResult
            {
                Data = banners.Select(x =>
                {
                    var bannermodel = x.ToModel();
                    bannermodel.Body = "";
                    return bannermodel;
                }),
                Total = banners.Count
            };
            return Json(gridModel);
        }
        #endregion

        #region Create / Edit / Delete
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var model = new BannerModel();
            //locales
            AddLocales(_languageService, model.Locales);

            PrepareBannerModel(model);
            PrepareStoresMappingModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var banner = model.ToEntity();
                banner.CreatedOnUtc = DateTime.UtcNow;
                _bannerService.InsertBanner(banner);

                //stores
                SaveStoreMappings(banner, model);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Banners.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = banner.Id }) : RedirectToAction("List");
            }

            PrepareBannerModel(model);
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var banner = _bannerService.GetBannerById(id);
            if (banner == null)
                return RedirectToAction("List");

            var model = banner.ToModel();

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
           {
               locale.Name = banner.GetLocalized(x => x.Name, languageId, false, false);
               locale.Body = banner.GetLocalized(x => x.Body, languageId, false, false);
           });
            PrepareBannerModel(model);
            PrepareStoresMappingModel(model, banner, false);
            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(BannerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var banner = _bannerService.GetBannerById(model.Id);
            if (banner == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                banner = model.ToEntity(banner);
                _bannerService.UpdateBanner(banner);
                //stores
                SaveStoreMappings(banner, model);
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Banners.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = banner.Id }) : RedirectToAction("List");
            }

            PrepareBannerModel(model);
            PrepareStoresMappingModel(model, banner, true);

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var banner = _bannerService.GetBannerById(id);
            if (banner == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                _bannerService.DeleteBanner(banner);
                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Banners.Deleted"));
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = id });
        }

        #endregion

        #region Banner pictures

        [ValidateInput(false)]
        public virtual ActionResult BannerPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, string url, int width, int height,
             int vendorId, int categoryId, int productId, int bannerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var banner = _bannerService.GetBannerById(bannerId);
            if (banner == null)
                throw new ArgumentException("No banner found with the specified id");

            //a vendor should have access only to his banners
            if (_workContext.CurrentVendor != null && banner.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(banner.Name));

            _bannerService.InsertBannerPicture(new BannerPicture
            {
                PictureId = pictureId,
                BannerId = bannerId,
                DisplayOrder = displayOrder,
                URL = url,
                Width = width,
                Height = height,
                VendorId = vendorId,
                CategoryId = categoryId,
                ProductId = productId
            });

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual ActionResult BannerPictureList(DataSourceRequest command, int bannerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his banners
            if (_workContext.CurrentVendor != null)
            {
                var banner = _bannerService.GetBannerById(bannerId);
                if (banner != null && banner.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your banner");
                }
            }

            var bannerPictures = _bannerService.GetBannerPicturesByBannerId(bannerId);
            var bannerPicturesModel = bannerPictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new BannerModel.BannerPictureModel
                    {
                        Id = x.Id,
                        BannerId = x.BannerId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        URL = x.URL,
                        Width = x.Width,
                        Height = x.Height,
                        CategoryId = x.CategoryId,
                        VendorId = x.VendorId,
                        ProductId = x.ProductId,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = bannerPicturesModel,
                Total = bannerPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult BannerPictureUpdate(BannerModel.BannerPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var bannerPicture = _bannerService.GetBannerPictureById(model.Id);
            if (bannerPicture == null)
                throw new ArgumentException("No banner picture found with the specified id");

            //a vendor should have access only to his banners
            if (_workContext.CurrentVendor != null)
            {
                var banner = _bannerService.GetBannerById(bannerPicture.BannerId);
                if (banner != null && banner.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your banner");
                }
            }

            var picture = _pictureService.GetPictureById(bannerPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            bannerPicture.DisplayOrder = model.DisplayOrder;
            bannerPicture.URL = model.URL;
            bannerPicture.Height = model.Height;
            bannerPicture.Width = model.Width;
            bannerPicture.VendorId = model.VendorId;
            bannerPicture.CategoryId = model.CategoryId;
            bannerPicture.ProductId = model.ProductId;
            _bannerService.UpdateBannerPicture(bannerPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult BannerPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanners))
                return AccessDeniedView();

            var bannerPicture = _bannerService.GetBannerPictureById(id);
            if (bannerPicture == null)
                throw new ArgumentException("No banner picture found with the specified id");

            var bannerId = bannerPicture.BannerId;

            //a vendor should have access only to his banners
            if (_workContext.CurrentVendor != null)
            {
                var banner = _bannerService.GetBannerById(bannerId);
                if (banner != null && banner.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your banner");
                }
            }
            var pictureId = bannerPicture.PictureId;
            _bannerService.DeleteBannerPicture(bannerPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion
    }
}