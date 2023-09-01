using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.MServices.Models.Product;
using Nop.Web.Areas.MServices.Models.Product;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Web.Areas.MServices;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using Nop.Web.Areas.Mservices.Models.Product;
using Nop.Services.Customers;
using System.Text.RegularExpressions;
using Nop.Services.ShopTheLook;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class ProductController : BaseController
    {
        #region Fields
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IShippingService _shippingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IShopTheLookService _shoptheLookService;

        #endregion

        #region Constructors

        public ProductController(ICategoryService categoryService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IVendorService vendorService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            ISpecificationAttributeService specificationAttributeService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IWorkflowMessageService workflowMessageService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            IShippingService shippingService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            ICacheManager cacheManager,
            IOrderService orderService,
            ICustomerService customerService,
            IShopTheLookService shopTheLookService)
        {
            this._productModelFactory = productModelFactory;
            this._categoryService = categoryService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceFormatter = priceFormatter;
            this._specificationAttributeService = specificationAttributeService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._workflowMessageService = workflowMessageService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._shippingService = shippingService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._localizationSettings = localizationSettings;
            this._cacheManager = cacheManager;
            this._orderService = orderService;
            this._customerService = customerService;
            this._shoptheLookService = shopTheLookService;
        }

        #endregion

        #region GetSpecificationAttribute
        public ActionResult GetSpecificationAttribute()
        {
            try
            {
                var spcificationAttributes = _specificationAttributeService.GetSpecificationAttributes(0, 10);

                if (spcificationAttributes != null && spcificationAttributes.Count > 0)
                {
                    List<SpecificationAttributeModelAPI> listAttrs = new List<SpecificationAttributeModelAPI>();

                    foreach (SpecificationAttribute item in spcificationAttributes)
                    {
                        SpecificationAttributeModelAPI spa = new SpecificationAttributeModelAPI();
                        spa.id = item.Id;
                        spa.name = item.Name;
                        spa.displayOrder = item.DisplayOrder;

                        if (item.SpecificationAttributeOptions != null && item.SpecificationAttributeOptions.Count > 0)
                        {
                            foreach (SpecificationAttributeOption child in item.SpecificationAttributeOptions)
                            {
                                SpecificationAttributeOptionModelAPI option = new SpecificationAttributeOptionModelAPI();
                                option.id = child.Id;
                                option.name = child.Name;
                                option.displayOrder = child.DisplayOrder;

                                spa.options.Add(option);
                            }
                        }

                        listAttrs.Add(spa);
                    }

                    return View(listAttrs);
                }
                else
                {
                    return InvokeHttp400("No attribute found");
                }
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }

        #endregion

        #region ProductDetailsWithRelatedProducts

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ProductDetailsWithRelatedProducts(int productId = 0, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp400("product not exists or  deleted");

            //Is published?
            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a product before publishing
            if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return InvokeHttp400("product not published");

            //ACL (access control list)
            if (!_aclService.Authorize(product))
                return InvokeHttp400("product not authorized");

            //Store mapping
            if (!_storeMappingService.Authorize(product))
                return InvokeHttp400("product not authorized");

            //availability dates
            if (!product.IsAvailable())
                return InvokeHttp400("product not exits or deleted");

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return InvokeHttp400("product is not Group product");
            }

            //update existing shopping cart item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
            }

            /****** Prepare the Product Detail Model ******/
            var pageDetailModel = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);
            if (pageDetailModel.FullDescription != null)
                pageDetailModel.FullDescription = Regex.Replace(pageDetailModel.FullDescription, "<.*?>", String.Empty);
            pageDetailModel.PageShareCode = ""; // model.PageShareCode.Replace("<p>", "").Replace("</p>", "").Replace(Environment.NewLine, "");
            /****** End of Product Detail Model ******/


            /****** Prepare the Related Product Model ******/
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var RelatedProductModel = convertToProductModelAPI(products).ToList();

            //Product Pointers for Shop the Look
            var pointers = _shoptheLookService.SearchPointers(ProductId: productId);

            #region Quantities of carts

            var wholeShoppingCart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            int itemCountShopCart = wholeShoppingCart.GetTotalProducts();

            var wholeWishList = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            int itemCountWishList = wholeWishList.GetTotalProducts();

            #endregion

            return Json(new
            {
                PageDetailModel = pageDetailModel,
                RelatedProducts = RelatedProductModel,
                ProductPointers = pointers,
                ShoppingCartItemsCount = itemCountShopCart,
                WishListItemsCount = itemCountWishList
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ProductDetailsWithRelatedProducts | convertToProductModelAPI
        private List<ProductModelAPI> convertToProductModelAPI(IList<Product> list)
        {
            List<ProductModelAPI> listmodel = new List<ProductModelAPI>();
            foreach (var item in list)
            {
                string picurl = "";
                var productpic = _pictureService.GetPicturesByProductId(item.Id);
                if (productpic != null && productpic.Count > 0)
                {
                    Picture Pic = _pictureService.GetPictureById(productpic[0].Id);
                    picurl = _pictureService.GetPictureUrl(Pic, _mediaSettings.ProductDetailsPictureSize);
                }

                decimal Price = _currencyService.ConvertFromPrimaryStoreCurrency(item.Price, _workContext.WorkingCurrency);

                var model = new ProductModelAPI
                {
                    id = item.Id,
                    title = item.Name,
                    formattedPrice = _priceFormatter.FormatPrice(Price),
                    price = Price,
                    thumbUrl = picurl
                };
                listmodel.Add(model);
            }
            return listmodel;
        }

        #endregion

        #region ProductRating
        public virtual ActionResult ProductRating(int productId = 0)
        {

            if (productId == 0)
                return InvokeHttp400("No Product Found");

            var product = _productService.GetProductById(productId);
            var vendor = _vendorService.GetVendorById(product.VendorId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return InvokeHttp400("Product Not Found");

            var productReviews = _catalogSettings.ShowProductReviewsPerStore
               ? product.ProductReviews.Where(pr => pr.IsApproved && pr.StoreId == _storeContext.CurrentStore.Id).OrderBy(pr => pr.CreatedOnUtc)
               : product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);

            var model = new ReviewSummaryModel()
            {
                Name = product.Name,
                Image = _pictureService.GetPictureUrl(_pictureService.GetPicturesByProductId(product.Id).FirstOrDefault().Id, 300),
                TotalReviews = productReviews.Count(),
            };

            foreach (var productReview in productReviews)
            {
                var review = new ReviewListModel
                {
                    Id = productReview.ProductId,
                    ProductImage = _pictureService.GetPictureUrl(_pictureService.GetPicturesByProductId(product.Id).FirstOrDefault().Id, 300),
                    Rating = productReview.Rating,
                    ReviewText = productReview.ReviewText,
                    Title = productReview.Title,
                    CreatedOnUtc = productReview.CreatedOnUtc
                };
                model.Reviews.Add(review);
            }
            for (var index = 0; index < 5; index++)
            {
                model.RatingCounts[index] = productReviews.Count(pr => pr.Rating == (5 - index));
            }

            return View(model);
        }


        public virtual ActionResult ProductReviewsAdd(AddProductReviewModel model, int productId = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return InvokeHttp400("Product Not Found");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.Fields.Title.Required"));
            }
            if (String.IsNullOrWhiteSpace(model.ReviewText))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.Fields.ReviewText.Required"));
            }

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing &&
                !_orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id, productId: productId, osIds: new List<int> { (int)OrderStatus.Complete }).Any())
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));

            if (ModelState.IsValid)
            {
                //save review
                int rating = model.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.Title,
                    ReviewText = model.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };
                product.ProductReviews.Add(productReview);
                _productService.UpdateProduct(product);

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview", _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name);

                //raise event
                if (productReview.IsApproved)
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));


                model.SuccessfullyAdded = true;
                if (!isApproved)
                    return View("", message: _localizationService.GetResource("Reviews.SeeAfterApproving"));
                else
                    return View("", message: _localizationService.GetResource("Reviews.SuccessfullyAdded"));

            }
            return InvokeHttp400(ModelState.Where(x => x.Value.Errors.Count > 0).First().Value.Errors.First().ErrorMessage);
        }

        #endregion

        #region ProductDetails

        public ActionResult GetProductDetailsByProductId(int productId = 0, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);

            if (product == null)
            {
                return InvokeHttp400("product does not exists");
            }
            //update existing shopping cart item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
            }

            /****** Prepare the Product Detail Model ******/
            var pageDetailModel = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",product.Id, _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);
            if (pageDetailModel.FullDescription != null)
                pageDetailModel.FullDescription = Regex.Replace(pageDetailModel.FullDescription, "<.*?>", String.Empty);
            pageDetailModel.PageShareCode = ""; // model.PageShareCode.Replace("<p>", "").Replace("</p>", "").Replace(Environment.NewLine, "");
            /****** End of Product Detail Model ******/


            /****** Prepare the Related Product Model ******/
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var RelatedProductModel = _productModelFactory.PrepareProductOverviewModels(products).ToList();

            //Product Pointers for Shop the Look
            var pointers = _shoptheLookService.SearchPointers(ProductId: productId);

            #region Quantities of carts

            var wholeShoppingCart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            int itemCountShopCart = wholeShoppingCart.GetTotalProducts();

            var wholeWishList = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            int itemCountWishList = wholeWishList.GetTotalProducts();

            #endregion

            return Json(new
            {
                PageDetailModel = pageDetailModel,
                RelatedProducts = RelatedProductModel,
                ProductPointers = pointers,
                ShoppingCartItemsCount = itemCountShopCart,
                WishListItemsCount = itemCountWishList
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}