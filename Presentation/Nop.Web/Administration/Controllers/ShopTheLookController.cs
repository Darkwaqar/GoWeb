using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.ShopTheLook;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.ShopTheLook;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class ShopTheLookController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly VendorSettings _vendorSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILanguageService _languageService;
        private readonly IShopTheLookService _shoptheLookService;

        public ShopTheLookController(IPermissionService permissionService,
            IWorkContext workContext,
            VendorSettings vendorSettings,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            ICacheManager cacheManager,
            IStoreService storeService,
            IShippingService shippingService,
            IVendorService vendorService,
            ICategoryService categoryService,
            IProductService productService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ILanguageService languageService,
            IShopTheLookService shopTheLookService)
        {
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._vendorSettings = vendorSettings;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._cacheManager = cacheManager;
            this._storeService = storeService;
            this._shippingService = shippingService;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._productAttributeService = productAttributeService;
            this._languageService = languageService;
            this._shoptheLookService = shopTheLookService;
        }

        //list products
        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductListModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            model.AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts;

            //categories
            //model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            //foreach (var c in categories)
            //    model.AvailableCategories.Add(c);

            PrepareAllCategoriesModel(model);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        //edit product
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = product.ToModel();
            //PrepareProductModel(model, product, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = product.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = product.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.FullDescription = product.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.MetaKeywords = product.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = product.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = product.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = product.GetSeName(languageId, false, false);
            });

            return View(model);
        }

        public virtual ActionResult AddPointer(int pictureId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            var model = new ProductModel.ProductPictureModel
            {
                Id = picture.Id,
                ProductId = productId,
                PictureId = picture.Id,
                PictureUrl = _pictureService.GetPictureUrl(picture),
                OverrideAltAttribute = picture.AltAttribute,
                OverrideTitleAttribute = picture.TitleAttribute,
                DisplayOrder = 0
            };
            return View(model);
        }

        public virtual ActionResult SavePointers(List<PointerModel> Pointers, int pictureId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _shoptheLookService.DeleteAllPointers(pictureId);

            foreach (var pointer in Pointers)
            {
                _shoptheLookService.InsertPointer(pointer.ToEntity());
            }


            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult GetPointers(int id)
        {
            if (id == 0)
                throw new ArgumentException();

            var pointers = _shoptheLookService.SearchPointers(PictureId: id);
            return Json(pointers);
        }


        #region Utilities

        [NonAction]
        protected virtual void PrepareAllCategoriesModel(ProductListModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var allVendorCategories = SelectListHelper.GetVendorCategoriesList(_categoryService, _cacheManager, true, _workContext.CurrentVendor?.Id ?? 0);
            foreach (var c in allVendorCategories)
            {
                model.AvailableCategories.Add(c);
            }
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        #endregion


    }
}