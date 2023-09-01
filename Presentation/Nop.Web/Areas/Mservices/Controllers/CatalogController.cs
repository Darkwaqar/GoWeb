using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.MServices.Models.Vendors;
using Nop.Web.MServices.Models.Product;
using Nop.Web.Areas.MServices.Models.Product;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Web.Factories;
using Nop.Web.Areas.Mservices.Models.Vendors;
using Nop.Web.Areas.Mservices.Models.Product;
using Nop.Web.Models.Catalog;
using Nop.Web.Areas.Mservices.Models.Common;
using Nop.Web.Areas.Mservices.Models.Category;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class CatalogController : BaseController
    {
        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly ITaxService _taxService;
        private readonly IPictureService _pictureService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IProductTagService _productTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Constructors

        public CatalogController(ICatalogModelFactory catalogModelFactory,
              ICacheManager cacheManager,
            ISpecificationAttributeService specificationAttributeService,
            IPriceFormatter priceFormatter,
            IPriceCalculationService priceCalculationService,
            IPictureService pictureService,
            ITaxService taxService,
            IProductModelFactory productModelFactory,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings)
        {
            this._cacheManager = cacheManager;
            this._specificationAttributeService = specificationAttributeService;
            this._priceFormatter = priceFormatter;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._catalogModelFactory = catalogModelFactory;
            this._productModelFactory = productModelFactory;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._productTagService = productTagService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Uttiles

        #endregion

        #region ProductList

        public ActionResult GetFeaturedProductsByVendorId(int vendorId = 0, bool alternativePicture = false)
        {

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
               featuredProducts: true,
               visibleIndividuallyOnly: true,
               storeId: _storeContext.CurrentStore.Id,
               vendorId: vendorId);

            var FeaturedProducts = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            return View(FeaturedProducts);
        }

        public ActionResult GetProductsByVendorId(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
           decimal? priceMin = null, decimal? priceMax = null)
        {

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                orderBy: (ProductSortingEnum)OrderBy,
                filteredSpecs: fSpecs);


            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }


        public ActionResult GetNewProductsByVendorId(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vandor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
            loadFilterableSpecificationAttributeOptionIds: true,
            storeId: _storeContext.CurrentStore.Id,
            visibleIndividuallyOnly: true,
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            markedAsNewOnly: true,
            priceMin: priceMin,
            priceMax: priceMax,
            vendorId: vendorId,
            orderBy: (ProductSortingEnum)OrderBy,
            filteredSpecs: fSpecs);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }


        public ActionResult GetSaleProductsByVendorId(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
           decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProductsAdvance(out IList<int> filterableSpecificationAttributeOptionIds,
            loadFilterableSpecificationAttributeOptionIds: true,
            storeId: _storeContext.CurrentStore.Id,
            visibleIndividuallyOnly: true,
            markedAsSalesOnly: true,
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            filteredSpecs: fSpecs,
            priceMin: priceMin,
            priceMax: priceMax,
            orderBy: (ProductSortingEnum)OrderBy,
            vendorId: vendorId);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }


        public ActionResult SearchVendorProductsByVendorId(string searchTerm = null, int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
           decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null)
                return InvokeHttp400("Not a valid Vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");


            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
            loadFilterableSpecificationAttributeOptionIds: true,
            storeId: _storeContext.CurrentStore.Id,
            visibleIndividuallyOnly: true,
            keywords: searchTerm,
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            vendorId: vendorId,
            filteredSpecs: fSpecs,
            priceMin: priceMin,
            priceMax: priceMax,
            orderBy: (ProductSortingEnum)OrderBy,
            searchDescriptions: true,
            searchProductTags: true);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }


        public ActionResult GetProductsByCategoryId(int categoryId = 0, int pageNumber = 1, int pageSize = 10, String specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published || !_aclService.Authorize(category) || !_storeMappingService.Authorize(category))
                return InvokeHttp400("category is either not found or unauthorized");

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }


            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                categoryIds: categoryIds,
                visibleIndividuallyOnly: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                orderBy: (ProductSortingEnum)OrderBy,
                filteredSpecs: fSpecs);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }

        public ActionResult GetShopTheLookByVendorId(int vendorId = 0, int pageNumber = 1, int pageSize = 10, String specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null)
                return InvokeHttp400("Not a valid Vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }
            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                filteredSpecs: fSpecs,
                orderBy: (ProductSortingEnum)OrderBy,
                productType: ProductType.GroupedProduct);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }

        public ActionResult GetProductByCategoryAndVendorId(int categoryId = 0, int vendorId = 0, int pageNumber = 1, int pageSize = 10, String specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp400("Not a valid vendor");

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published || !_aclService.Authorize(category) || !_storeMappingService.Authorize(category))
                return InvokeHttp400("category is either not found or unauthorized");

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }


            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                categoryIds: categoryIds,
                visibleIndividuallyOnly: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                orderBy: (ProductSortingEnum)OrderBy,
                filteredSpecs: fSpecs);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }

        #endregion

        #region Tabs / Featured / All / Latest / Sales /depricateed

        public ActionResult GetFeaturedProducts(int vendorId = 0)
        {

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");


            List<CategoryModelAPI> listcategory = new List<CategoryModelAPI>();

            //TODO Put vendor id to get ALL PRODUCT DISPLAYED ON HOME PAGE 
            List<Product> featuredProducts = _productService.GetAllVendorHomePageProduct(vendorId).ToList();

            foreach (Product prd in featuredProducts)
            {
                List<ProductCategory> listprdCategory = _categoryService.GetProductCategoriesByProductId(prd.Id).ToList();

                ProductCategory prdCategory = null;
                if (listprdCategory == null || listprdCategory.Count == 0)
                    continue;
                else
                    prdCategory = listprdCategory[0];

                int cid = prdCategory.CategoryId;

                var contansCheck = listcategory.Where(x => x.id == cid).ToList();

                if (contansCheck == null || contansCheck.Count == 0)
                {
                    CategoryModelAPI cat = new CategoryModelAPI
                    {
                        id = cid,
                        thumbUrl = _pictureService.GetPictureUrl(prdCategory.Category.PictureId, _mediaSettings.ProductThumbPictureSize),
                        AlternativePictureUrl = _pictureService.GetPictureUrl(prdCategory.Category.AlternativePictureId, _mediaSettings.ProductThumbPictureSize),
                        CategoryName = prdCategory.Category.Name
                    };

                    ProductCompactModelAPI productmodel = new ProductCompactModelAPI();
                    var productPicture = _pictureService.GetPicturesByProductId(prd.Id).FirstOrDefault();
                    if (productPicture != null)
                    {
                        productmodel.thumbUrl = _pictureService.GetPictureUrl(productPicture.Id, _mediaSettings.ProductThumbPictureSize);
                    }
                    else
                        productmodel.thumbUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.ProductThumbPictureSize);
                    productmodel.id = prd.Id;
                    productmodel.title = prd.Name;

                    cat.productModel.Add(productmodel);

                    listcategory.Add(cat);
                }
                else
                {
                    for (int i = 0; i < listcategory.Count; i++)
                    {
                        if (listcategory[i].id == cid)
                        {
                            ProductCompactModelAPI productmodel = new ProductCompactModelAPI();
                            var productPicture = _pictureService.GetPicturesByProductId(prd.Id).First();
                            if (productPicture != null)
                            {
                                productmodel.thumbUrl = _pictureService.GetPictureUrl(productPicture.Id, _mediaSettings.ProductThumbPictureSize);
                            }
                            else
                                productmodel.thumbUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.ProductThumbPictureSize);
                            productmodel.id = prd.Id;
                            productmodel.title = prd.Name;

                            listcategory[i].productModel.Add(productmodel);
                        }
                    }
                }
            }

            if (listcategory.Count > 0)
                return View(listcategory);
            else
                return InvokeHttp400("No Items Found");
        }


        public ActionResult GetAllVendorProducts(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                productType: ProductType.SimpleProduct,
                orderBy: (ProductSortingEnum)OrderBy,
                filteredSpecs: fSpecs);


            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();

            List<ProductModelAPI> mymodel = convertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;

            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);

        }


        public ActionResult GetVendorNewProducts(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vandor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
            loadFilterableSpecificationAttributeOptionIds: true,
            storeId: _storeContext.CurrentStore.Id,
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            markedAsNewOnly: true,
            priceMin: priceMin,
            priceMax: priceMax,
            vendorId: vendorId,
            productType: ProductType.SimpleProduct,
            orderBy: (ProductSortingEnum)OrderBy,
            filteredSpecs: fSpecs);

            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();
            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }


            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };

            return View(productList);

        }


        public ActionResult GetVendorSaleItems(int vendorId = 0, int pageNumber = 1, int pageSize = 10, string specs = null, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }

            var products = _productService.SearchProductsAdvance(out IList<int> filterableSpecificationAttributeOptionIds,
            loadFilterableSpecificationAttributeOptionIds: true,
            storeId: _storeContext.CurrentStore.Id,
            markedAsSalesOnly: true,
            filteredSpecs: fSpecs,
            priceMin: priceMin,
            priceMax: priceMax,
            productType: ProductType.SimpleProduct,
            orderBy: (ProductSortingEnum)OrderBy,
            vendorId: vendorId);

            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();
            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;


            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }


            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);

        }

        #endregion

        #region SearchVendor depricated 

        public ActionResult SearchVendor(string search = "", int platform = 0)
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return InvokeHttp400("we don't allow viewing of vendors if vendors block is hidden");

            var model = new List<VendorModelSearchAPI>();
            var vendors = _vendorService.SearchVendors(search);
            foreach (var vendor in vendors)
            {
                if (!vendor.Deleted && vendor.Active)
                {
                    var vendorModel = new VendorModelSearchAPI
                    {
                        Id = vendor.Id,
                        Title = vendor.GetLocalized(x => x.Name),
                        //description = vendor.Description,
                        VendorUrl = vendor.GetLocalized(x => x.Url),
                        MpLogoUrl = _pictureService.GetPictureUrl(vendor.mpLogo)
                    };
                    vendorModel.LogoUrl = _pictureService.GetPictureUrl(vendor.PictureId);
                    model.Add(vendorModel);
                }
            }

            return View(model);

        }

        #endregion

        #region GetVendorById
        public ActionResult GetVendorById(int vendorId = 0)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vandor is Not Active");

            var model = _catalogModelFactory.PrepareVendorMobileModel(vendor);

            return View(model);
        }
        #endregion

        #region SearchAllVendorProducts depricated
        public ActionResult SearchAllVendorProducts(string searchTerm = null, int vendorId = 0, int pageNumber = 1, int pageSize = 10, int OrderBy = 5,
            decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null)
                return InvokeHttp400("Not a valid Vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vandor is Not Active");

            var products = _productService.SearchProducts(
            storeId: _storeContext.CurrentStore.Id,
            keywords: searchTerm,
            pageIndex: pageNumber - 1,
            pageSize: pageSize,
            vendorId: vendorId,
            searchDescriptions: true,
            orderBy: (ProductSortingEnum)OrderBy,
            searchProductTags: true);

            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;



            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }


            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);
        }

        #endregion

        #region SearchAllVendorProducts | convertToProductModelAPI
        private List<ProductModelAPI> ConvertToProductModelAPI(IPagedList<Product> list)
        {
            List<ProductModelAPI> listmodel = new List<ProductModelAPI>();
            foreach (var item in list)
            {
                string picurl = "";
                var productpic = _pictureService.GetPicturesByProductId(item.Id);
                if (productpic != null && productpic.Count > 0)
                {
                    Picture Pic = _pictureService.GetPictureById(productpic[0].Id);
                    picurl = _pictureService.GetPictureUrl(Pic, 400);
                }

                decimal Price = _currencyService.ConvertFromPrimaryStoreCurrency(item.Price, _workContext.WorkingCurrency);

                var model = new ProductModelAPI
                {
                    id = item.Id,
                    title = item.Name,
                    thumbUrl = picurl
                };




                #region Product Price

                decimal taxRate;
                decimal oldPriceBase = _taxService.GetProductPrice(item, item.OldPrice, out taxRate);
                decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(item, _priceCalculationService.GetFinalPrice(item, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(item, _priceCalculationService.GetFinalPrice(item, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                {
                    model.oldPrice = oldPrice;
                    model.formattedOldPrice = _priceFormatter.FormatPrice(oldPrice);
                }

                model.price = finalPriceWithoutDiscount;
                model.formattedPrice = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                {
                    model.priceWithDiscount = finalPriceWithDiscount;
                    model.formattedPriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                }

                //currency code
                model.currencyCode = _workContext.WorkingCurrency.CurrencyCode;

                if (item.SpecialPrice.HasValue)
                {
                    decimal SpecialPrice = _currencyService.ConvertFromPrimaryStoreCurrency(item.SpecialPrice.Value, _workContext.WorkingCurrency);
                    model.formattedSpecialPrice = _priceFormatter.FormatPrice(SpecialPrice);
                    model.specialPrice = SpecialPrice;
                }


                model.ProductType = item.ProductType;
                model.ProductTypeId = item.ProductTypeId;
                model.VendorId = item.VendorId;
                var vendor = _vendorService.GetVendorById(item.VendorId);
                if (vendor != null)
                {
                    model.VendorImage = _pictureService.GetPictureUrl(vendor.mpLogo);
                    model.VendorName = vendor.Name;
                }

                listmodel.Add(model);
            }

            return listmodel;
        }

        #endregion


        #endregion

        #region GetVendorNewProducts | PrepareSpecAttribute
        [NonAction]
        private List<SpecificationAttributeModelAPI> PrepareSpecAttribute(IList<int> optionIds)
        {

            List<SpecificationAttributeModelAPI> listAttrs = new List<SpecificationAttributeModelAPI>();
            foreach (int item in optionIds)
            {
                var specAttribute = _specificationAttributeService.GetSpecificationAttributeByOptionId(item);
                var specOption = _specificationAttributeService.GetSpecificationAttributeOptionById(item);

                if (IsDuplicateSpecList(listAttrs, specAttribute))
                    AddIntoSpecList(ref listAttrs, specAttribute, specOption, false);
                else
                    AddIntoSpecList(ref listAttrs, specAttribute, specOption, true);
            }

            return listAttrs;
        }
        #endregion

        #region PrepareSpecAttribute | IsDuplicateSpecList
        [NonAction]
        private bool IsDuplicateSpecList(List<SpecificationAttributeModelAPI> mainList, SpecificationAttribute specAttribute)
        {
            foreach (var item in mainList)
            {
                if (item.id == specAttribute.Id)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region PrepareSpecAttribute | AddIntoSpecList

        [NonAction]
        private void AddIntoSpecList(ref List<SpecificationAttributeModelAPI> mainList, SpecificationAttribute specAttribute,
            SpecificationAttributeOption specOption, bool createNewAttribute)
        {
            if (createNewAttribute)
            {
                SpecificationAttributeModelAPI spa = new SpecificationAttributeModelAPI();
                spa.id = specAttribute.Id;
                spa.name = specAttribute.Name;
                spa.displayOrder = specAttribute.DisplayOrder;

                SpecificationAttributeOptionModelAPI option = new SpecificationAttributeOptionModelAPI();
                option.id = specOption.Id;
                option.name = specOption.Name;
                option.displayOrder = specOption.DisplayOrder;
                spa.options.Add(option);

                mainList.Add(spa);
            }
            else
            {
                foreach (var item in mainList)
                {
                    if (item.id == specAttribute.Id)
                    {
                        SpecificationAttributeOptionModelAPI option = new SpecificationAttributeOptionModelAPI();
                        option.id = specOption.Id;
                        option.name = specOption.Name;
                        option.displayOrder = specOption.DisplayOrder;
                        item.options.Add(option);

                        break;
                    }
                }
            }
        }

        #endregion

        #region GetAllCategories
        public ActionResult GetAllCategories(int vendorId = 0)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null)
                return InvokeHttp400("Not a valid vendor");

            if (vendor.Deleted)
                return InvokeHttp400("Vendor is Deleted");

            if (!vendor.Active)
                return InvokeHttp400("Vendor is Not Active");


            var Categories = _categoryService.GetCategoriesByVendorId(vendor.Id);

            var haveShoptheLook = _productService.SearchProducts(vendorId: vendor.Id, productType: ProductType.GroupedProduct).Count() != 0;

            List<CategoryCompactModelAPI> listcategory = new List<CategoryCompactModelAPI>();


            foreach (var categroy in Categories)
            {
                CategoryCompactModelAPI categoryapi = new CategoryCompactModelAPI
                {
                    id = categroy.Id,
                    title = categroy.Name,
                    thumbUrl = _pictureService.GetPictureUrl(categroy.PictureId, _mediaSettings.CategoryThumbPictureSize),
                    AlternativePictureUrl = _pictureService.GetPictureUrl(categroy.AlternativePictureId, _mediaSettings.CategoryThumbPictureSize)
                };
                var subcategories = _categoryService.GetAllCategoriesByParentCategoryId(categroy.Id).Where(x => x.VendorId == vendorId);
                foreach (var subcategory in subcategories)
                {
                    SubCategoriesmodel subcategoryModel = new SubCategoriesmodel
                    {
                        id = subcategory.Id,
                        title = subcategory.Name,
                        thumbUrl = _pictureService.GetPictureUrl(subcategory.PictureId, _mediaSettings.CategoryThumbPictureSize),
                        AlternativePictureUrl = _pictureService.GetPictureUrl(subcategory.AlternativePictureId, _mediaSettings.CategoryThumbPictureSize)
                    };
                    categoryapi.Subcatagories.Add(subcategoryModel);
                }
                listcategory.Add(categoryapi);
            }

            if (listcategory.Count > 0)
            {
                Object Obj = new
                {
                    categories = listcategory,
                    haveShoptheLook = haveShoptheLook
                };
                return View(Obj);
            }
            else
                return InvokeHttp400("Category not Found");

        }
        #endregion

        #region GetProductByCategoryAndVendor

        public ActionResult GetProductByCategoryAndVendor(int categoryId = 0, int vendorId = 0, int pageNumber = 1, int OrderBy = 5,
            int pageSize = 10, String specs = null, decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp400("Not a valid vendor");

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published || !_aclService.Authorize(category) || !_storeMappingService.Authorize(category))
                return InvokeHttp400("category is either not found or unauthorized");

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }


            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                orderBy: (ProductSortingEnum)OrderBy,
                filteredSpecs: fSpecs);

            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();

            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;

            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);
        }
        #endregion

        #region GetProductByCategoryAndVendor | GetChildCategoryIds

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }
        #endregion

        #region GetFeaturedVendors

        public ActionResult GetFeaturedVendors()
        {

            List<VendorModelSearchAPI> selectedVendorsList = new List<VendorModelSearchAPI>();
            var vendor = _vendorService.GetAllFeaturedVendors();

            foreach (var item in vendor)
            {
                var vendorModel = new VendorModelSearchAPI
                {
                    Id = item.Id,
                    Title = item.Name,
                    MpLogoUrl = _pictureService.GetPictureUrl(item.mpLogo)
                };
                selectedVendorsList.Add(vendorModel);
            }

            if (selectedVendorsList.Count > 0)
                return View(selectedVendorsList);
            else
                return InvokeHttp400("No Featured Vendor Found");



        }
        #endregion

        #region GetAllVendorProducts | convertToProductModelAPI 

        private List<ProductModelAPI> convertToProductModelAPI(IPagedList<Product> list)
        {
            List<ProductModelAPI> listmodel = new List<ProductModelAPI>();
            foreach (var item in list)
            {
                string picurl = "";
                var productpic = _pictureService.GetPicturesByProductId(item.Id);
                if (productpic != null && productpic.Count > 0)
                {
                    Picture Pic = _pictureService.GetPictureById(productpic[0].Id);
                    picurl = _pictureService.GetPictureUrl(Pic, 400);
                }

                decimal Price = _currencyService.ConvertFromPrimaryStoreCurrency(item.Price, _workContext.WorkingCurrency);

                var model = new ProductModelAPI
                {
                    id = item.Id,
                    title = item.Name,
                    thumbUrl = picurl
                };

                decimal taxRate;
                decimal oldPriceBase = _taxService.GetProductPrice(item, item.OldPrice, out taxRate);
                decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(item, _priceCalculationService.GetFinalPrice(item, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(item, _priceCalculationService.GetFinalPrice(item, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                {
                    model.oldPrice = oldPrice;
                    model.formattedOldPrice = _priceFormatter.FormatPrice(oldPrice);
                }

                model.price = finalPriceWithoutDiscount;
                model.formattedPrice = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                {
                    model.priceWithDiscount = finalPriceWithDiscount;
                    model.formattedPriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                }

                //currency code
                model.currencyCode = _workContext.WorkingCurrency.CurrencyCode;

                if (item.SpecialPrice.HasValue)
                {
                    decimal SpecialPrice = _currencyService.ConvertFromPrimaryStoreCurrency(item.SpecialPrice.Value, _workContext.WorkingCurrency);
                    model.formattedSpecialPrice = _priceFormatter.FormatPrice(SpecialPrice);
                    model.specialPrice = SpecialPrice;
                }

                listmodel.Add(model);
            }

            return listmodel;
        }
        #endregion

        #region GetShopTheLookByVendor

        public ActionResult GetShopTheLookByVendor(int vendorId = 0, int pageNumber = 1,
            int pageSize = 10, String specs = null, decimal? priceMin = null, decimal? priceMax = null)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp400("Not a valid vendor");

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }
            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                vendorId: vendorId,
                filteredSpecs: fSpecs,
                productType: ProductType.GroupedProduct);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            //var mymodel = _productModelFactory.PrepareProductOverviewModels(products,prepareSpecificationAttributes:true);
            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();

            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);
        }
        #endregion

        #region GetMagazine

        public ActionResult GetMagazine(int pageNumber = 1,
            int pageSize = 10, String specs = null, decimal? priceMin = null, decimal? priceMax = null)
        {
            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }
            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                filteredSpecs: fSpecs,
                productType: ProductType.MagazineProduct);

            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;
            var productList = _productModelFactory.PrepareProductOverviewModels(products, prepareSpecificationAttributes: true).ToList();
            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds).OrderBy(x => x.displayOrder).ToList();

            var productListModel = new NewProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = productList,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productListModel);
        }
        #endregion

        #region VendorAll
        public virtual ActionResult VendorAll(bool Brand = false, bool Designer = false, bool SheEarns = false)
        {

            var vendorCacheKey = string.Format(ModelCacheEventConsumer.ALL_VENDOR_MODEL_KEY, Brand, Designer, SheEarns);
            var model = _cacheManager.Get(vendorCacheKey, () =>
            {
                var Vendors = _vendorService.GetAllVendors().ToList();
                var vendormodel = new List<VendorModelSearchAPI>();
                if (Brand == true)
                    Vendors = Vendors.Where(x => x.Brand).ToList();
                if (Designer == true)
                    Vendors = Vendors.Where(x => x.Designer).ToList();
                if (SheEarns == true)
                    Vendors = Vendors.Where(x => x.SheEarns).ToList();

                foreach (var Vendor in Vendors.OrderBy(x => x.Name))
                {
                    var vendorModel = new VendorModelSearchAPI
                    {
                        Id = Vendor.Id,
                        Title = Vendor.GetLocalized(x => x.Name),
                        VendorUrl = Vendor.GetLocalized(x => x.Url),
                        MpLogoUrl = _pictureService.GetPictureUrl(Vendor.mpLogo),
                        LogoUrl = _pictureService.GetPictureUrl(Vendor.PictureId),
                        Description = Vendor.Description
                    };
                    vendormodel.Add(vendorModel);
                }
                return vendormodel;
            });

            return View(model);
        }
        #endregion

        #region GetSelectedVendorByIds

        public ActionResult GetSelectedVendorByIds(String ids)
        {
            try
            {
                if (ids != null && ids.Trim() != string.Empty && ids.Length > 0)
                {
                    List<VendorMobileModel> selectedVendorsList = new List<VendorMobileModel>();

                    String[] separatedIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String item in separatedIds)
                    {
                        var vendor = _vendorService.GetVendorById(int.Parse(item));
                        if (vendor == null || vendor.Deleted || !vendor.Active)
                            continue;

                        selectedVendorsList.Add(_catalogModelFactory.PrepareVendorMobileModel(vendor));
                    }

                    if (selectedVendorsList.Count > 0)
                        return View(selectedVendorsList);
                    else
                        return InvokeHttp400("No Vendor Found");
                }
                else
                {
                    return InvokeHttp400("Provide vendor ids");
                }
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }

        #endregion

        #region GetAllCategories depricated
        public virtual ActionResult CategoriesAll(int CategoryId = 0)
        {
            var Categories = _categoryService.GetAllCategoriesByParentCategoryId(CategoryId);

            List<CategoryCompactModelAPI> model = new List<CategoryCompactModelAPI>();

            foreach (var categroy in Categories)
            {
                CategoryCompactModelAPI categoryapi = new CategoryCompactModelAPI
                {
                    id = categroy.Id,
                    title = categroy.Name,
                    thumbUrl = _pictureService.GetPictureUrl(categroy.PictureId, _mediaSettings.CategoryThumbPictureSize)
                };
                var subcategories = _categoryService.GetAllCategoriesByParentCategoryId(categroy.Id);
                if (subcategories.Count > 0)
                {

                    foreach (var subcategory in subcategories)
                    {
                        SubCategoriesmodel subcategoryModel = new SubCategoriesmodel
                        {
                            id = subcategory.Id,
                            title = subcategory.Name,
                            thumbUrl = _pictureService.GetPictureUrl(subcategory.PictureId, _mediaSettings.CategoryThumbPictureSize)
                        };

                        string cacheKey1 = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                       string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                       _storeContext.CurrentStore.Id,
                       subcategory.Id);
                        subcategoryModel.NumberOfProducts = _cacheManager.Get(cacheKey1, () =>
                        {
                            var categoryIds = new List<int>();
                            categoryIds.Add(subcategory.Id);
                            //include subcategories
                            if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                                categoryIds.AddRange(GetChildCategoryIds(subcategory.Id));
                            return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                        });
                        if (subcategoryModel.NumberOfProducts > 0)
                        {
                            categoryapi.Subcatagories.Add(subcategoryModel);
                        }
                    }

                }

                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                       string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                       _storeContext.CurrentStore.Id,
                       CategoryId);

                categoryapi.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                {
                    var categoryIds = new List<int>();
                    categoryIds.Add(CategoryId);
                    //include subcategories
                    if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                        categoryIds.AddRange(GetChildCategoryIds(CategoryId));
                    return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                });

                if (categoryapi.NumberOfProducts > 0)
                {
                    model.Add(categoryapi);
                }
            }
            return View(model);
        }

        #endregion

        #region SearchVendorCategoryProduct

        public ActionResult SearchAll(string search = "")
        {
            var model = new AdvanceSearchModel();
            search = search.Trim();
            var searchs = search.Split(null);

            List<Product> totalproducts = new List<Product>();
            List<Vendor> totalVendors = new List<Vendor>();

            foreach (var item in searchs)
            {
                var products = _productService.SearchProducts(pageSize: 20, searchProductTags: true, keywords: item);
                totalproducts.AddRange(products);
                var vendors = _vendorService.SearchVendors(item);
                totalVendors.AddRange(vendors);
            }

            var freqs = GetFrequencies(totalproducts);

            var sorted = from pair in freqs
                         orderby pair.Value descending
                         select pair;


            var freqsVendor = GetFrequencies(totalVendors);

            var sortedvendor = from pair in freqsVendor
                               orderby pair.Value descending
                               select pair;

            foreach (var item in sortedvendor.Take(3))
            {
                if (model.Vendors.Count < 3)
                {
                    var vendor = item.Key;
                    var searchItem = new SearchItem
                    {
                        Name = vendor.Name,
                        Image = _pictureService.GetPictureUrl(vendor.mpLogo, 120),
                        VendorId = vendor.Id,
                        VendorImage = _pictureService.GetPictureUrl(vendor.mpLogo),
                        VendorName = vendor.Name
                    };
                    if (!model.Vendors.Any(x => x.VendorId == vendor.Id))
                    {
                        model.Vendors.Add(searchItem);
                    }

                }

            }

            foreach (var item in sorted.Take(3))
            {
                var product = item.Key;
                var vendor = _vendorService.GetVendorById(product.VendorId);
                var categories = _categoryService.GetCategoriesByVendorId(vendorId: product.VendorId);
                var productImage = _pictureService.GetPicturesByProductId(product.Id);

                if (vendor == null || !vendor.Active)
                {
                    continue;
                }



                foreach (var category in categories.Where(x => x.VendorId != 0))
                {
                    if (model.Categories.Count < 3)
                    {
                        var searchItemCategory = new SearchItem
                        {
                            Name = category.Name,
                            Image = _pictureService.GetPictureUrl(category.PictureId, 120),
                            VendorId = category.VendorId,
                            CategoryId = category.Id,
                            VendorImage = "",
                            VendorName = ""
                        };
                        if (!model.Categories.Any(x => x.CategoryId == category.Id))
                        {
                            model.Categories.Add(searchItemCategory);
                        }
                    }

                    if (model.Products.Count < 3)
                    {
                        var searchItemproducts = new SearchItem
                        {
                            Name = product.Name,
                            Image = _pictureService.GetPictureUrl(productImage.Any() ? productImage.First().Id : 0, 120),
                            VendorId = product.VendorId,
                            CategoryId = product.ProductCategories.Any() ? product.ProductCategories.First().Id : 0,
                            ProductId = product.Id,
                            VendorImage = _pictureService.GetPictureUrl(vendor.mpLogo),
                            VendorName = vendor.Name
                        };

                        if (!model.Products.Any(x => x.ProductId == product.Id))
                        {
                            model.Products.Add(searchItemproducts);
                        }
                    }
                }
            }

            return View(model);
        }

        #endregion

        #region MarketPlaceCategories

        public ActionResult MarketCategories(int CategoryId = 0)
        {
            //prepare models
            var marketCategoryCacheKey = string.Format(ModelCacheEventConsumer.MARKET_CATEGORY_MODEL_KEY, CategoryId);
            var model = _cacheManager.Get(marketCategoryCacheKey, 480, () =>
             {
                 var marketplace = new List<MarketPlaceCategoryItemModel>();
                 var Categories = _catalogModelFactory.PrepareMarketPlaceCategorySimpleModels(CategoryId);
                 foreach (var category in Categories)
                 {
                     var searchItem = new MarketPlaceCategoryItemModel
                     {
                         Name = category.Name,
                         Image = category.PictureModel.ImageUrl,
                         CategoryId = category.Id,
                         HasSubCategory = category.SubCategories.Any()
                     };
                     marketplace.Add(searchItem);
                 }
                 return marketplace;
             });
            return View(model);
        }

        #endregion

        #region GetProductByCategory

        public ActionResult GetProductByCategory(int categoryId = 0, int pageNumber = 1,
           int pageSize = 10, String specs = null, decimal? priceMin = null, decimal? priceMax = null)
        {

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published || !_aclService.Authorize(category) || !_storeMappingService.Authorize(category))
                return InvokeHttp400("category is either not found or unauthorized");

            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            List<int> fSpecs = null;
            if (specs != null && specs.Trim().Length > 0)
            {
                String[] s = specs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in s)
                {
                    if (fSpecs == null)
                        fSpecs = new List<int>();

                    fSpecs.Add(Convert.ToInt32(item));
                }
            }


            //products
            var products = _productService.SearchProducts(out IList<int> filterableSpecificationAttributeOptionIds,
                loadFilterableSpecificationAttributeOptionIds: true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize,
                priceMin: priceMin,
                priceMax: priceMax,
                productType: ProductType.SimpleProduct,
                filteredSpecs: fSpecs);

            var SpecList = PrepareSpecAttribute(filterableSpecificationAttributeOptionIds.ToList());

            List<ProductModelAPI> mymodel = ConvertToProductModelAPI(products);
            int pagenumber = products.PageIndex + 1;
            int availableTotalPages = products.TotalPages;

            priceMax = priceMin = 0;
            foreach (var product in products)
            {
                if (product.Price < priceMin || product.Price == 0)
                {
                    priceMin = product.Price;
                }
                if (product.Price > priceMax)
                {
                    priceMax = product.Price;
                }
            }

            var productList = new ProductList
            {
                PageNumber = pagenumber,
                TotalPages = availableTotalPages,
                Products = mymodel,
                SpecList = SpecList,
                PriceMax = priceMax,
                PriceMin = priceMin
            };
            return View(productList);
        }
        #endregion

        #region SearchTermAutoComplete
        public virtual ActionResult SearchTermAutoComplete()
        {
            var model = _catalogModelFactory.PrepareAllProductTagsModel();
            return View(model);
        }
        #endregion

        public List<SearchItem> CompaireList(List<SearchItem> s1, List<SearchItem> p1)
        {

            if (s1.Count > p1.Count)
            {
                return CompaireList(p1, s1);
            }

            List<SearchItem> result = new List<SearchItem>();
            foreach (var s in s1)
            {

                foreach (var p in p1)
                {
                    if (p == s)
                    {
                        result.Add(p);
                        break;
                    }
                }

            }

            return result;
        }

        Dictionary<Product, int> GetFrequencies(List<Product> values)
        {
            var result = new Dictionary<Product, int>();
            foreach (Product value in values)
            {
                if (result.TryGetValue(value, out int count))
                {
                    // Increase existing value.
                    result[value] = count + 1;
                }
                else
                {
                    // New value, set to 1.
                    result.Add(value, 1);
                }
            }
            // Return the dictionary.
            return result;
        }

        Dictionary<Vendor, int> GetFrequencies(List<Vendor> values)
        {
            var result = new Dictionary<Vendor, int>();
            foreach (Vendor value in values)
            {
                if (result.TryGetValue(value, out int count))
                {
                    // Increase existing value.
                    result[value] = count + 1;
                }
                else
                {
                    // New value, set to 1.
                    result.Add(value, 1);
                }
            }
            // Return the dictionary.
            return result;
        }


        #region AdvanceSearch

        public ActionResult AdvanceSearch(string SearchTerm = "", int CategoryId = 0, int ManufacturerId = 0, int VendorId = 0, string PriceFrom = "", string PriceTo = "",
            int OrderBy = 0, int pageNumber = 1, int pageSize = 10)
        {
            var model = new SearchModel();
            model.q = SearchTerm;
            if (CategoryId != 0)
                model.cid = CategoryId;
            if (ManufacturerId != 0)
                model.mid = ManufacturerId;
            if (VendorId != 0)
                model.vid = VendorId;
            if (!String.IsNullOrEmpty(PriceFrom))
                model.pf = PriceFrom;
            if (!String.IsNullOrEmpty(PriceTo))
                model.pt = PriceTo;
            model.isc = true;
            model.adv = true;
            model.asv = true;
            model.sid = true;
            var command = new CatalogPagingFilteringModel();
            command.OrderBy = OrderBy;
            command.PageNumber = pageNumber;
            command.PageSize = pageSize;
            model = _catalogModelFactory.PrepareMobileSearchModel(model, command);
            return View(model);
        }
        #endregion

        public virtual ActionResult GetCategoryByCategoryId(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp400("Category is deleted"); ;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", category.Id, _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            //model
            var model = _catalogModelFactory.PrepareCategoryModel(category, command);

            return View(model);
        }

        public virtual ActionResult GetAllCategoriesByParentCategoryId(int categoryId = 0)
        {
            var model = _catalogModelFactory.PrepareMarketPlaceCategorySimpleModels(categoryId, false);
            return View(model);
        }

    }

}