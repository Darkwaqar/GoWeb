using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Web.Factories;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Globalization;
using Nop.Web.Areas.Mservices.Models.ShoppingCart;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class ShoppingCartController : BaseController
    {
        #region Fields
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly HttpContextBase _httpContext;

        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;

        #endregion

        #region Constructors

        public ShoppingCartController(IProductService productService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPaymentService paymentService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICacheManager cacheManager,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            HttpContextBase httpContext,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._customerService = customerService;
            this._giftCardService = giftCardService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._workflowMessageService = workflowMessageService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._httpContext = httpContext;

            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
        }

        #endregion 

        #region ShopCart
        public ActionResult ShopCart()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
            //    return RedirectToRoute("HomePage");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var result = new ShoppingCartModel();
            _shoppingCartModelFactory.PrepareShoppingCartModel(result, cart);
            var model = result;
            model.ShoppingCartItemsCount = ReturnCartQuantity();
            model.WishListItemsCount = ReturnWishlistQuantity();
            model.OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, false);

            return View(model);
        }
        #endregion

        #region ShopCart | ReturnCartQuantity
        [NonAction]
        private int ReturnCartQuantity()
        {
            var wholeShoppingCart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            return wholeShoppingCart.GetTotalProducts();
        }

        #endregion

        #region ShopCart | ReturnWishlistQuantity

        [NonAction]
        private int ReturnWishlistQuantity()
        {
            var wholeWishList = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id).ToList();

            return wholeWishList.GetTotalProducts();
        }

        #endregion

        #region RemoveCartProduct

        public ActionResult RemoveCartProduct(string removefromcart)
        {
            try
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                    return InvokeHttp400("Permission denied");

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                var allIdsToRemove = removefromcart != null ? removefromcart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

                //current warnings <cart item identifier, warnings>
                var innerWarnings = new Dictionary<int, IList<string>>();
                foreach (var sci in cart)
                {
                    bool remove = allIdsToRemove.Contains(sci.Id);
                    if (remove)
                        _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                }

                //updated cart
                cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var model = new ShoppingCartModel();
                _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
                //update current warnings
                foreach (var kvp in innerWarnings)
                {
                    //kvp = <cart item identifier, warnings>
                    var sciId = kvp.Key;
                    var warnings = kvp.Value;
                    //find model
                    var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                    if (sciModel != null)
                        foreach (var w in warnings)
                            if (!sciModel.Warnings.Contains(w))
                                sciModel.Warnings.Add(w);
                }

                model.ShoppingCartItemsCount = ReturnCartQuantity();
                model.WishListItemsCount = ReturnWishlistQuantity();
                model.OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, false);

                return View(model);
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }
        #endregion

        #region UpdateCartProduct
        [HttpPost]
        public ActionResult UpdateCartProduct(int cartitemid, int itemquantity)
        {
            try
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                    return InvokeHttp400("Permission denied");

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                //current warnings <cart item identifier, warnings>
                var innerWarnings = new Dictionary<int, IList<string>>();
                foreach (var sci in cart)
                {
                    if (itemquantity > 0 && cartitemid == sci.Id)
                    {
                        var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                            sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                            sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                            itemquantity, true);
                        innerWarnings.Add(sci.Id, currSciWarnings);
                    }
                    //  break;
                }

                //updated cart
                cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var model = new ShoppingCartModel();
                _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart);
                //update current warnings
                foreach (var kvp in innerWarnings)
                {
                    //kvp = <cart item identifier, warnings>
                    var sciId = kvp.Key;
                    var warnings = kvp.Value;
                    //find model
                    var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                    if (sciModel != null)
                        foreach (var w in warnings)
                            if (!sciModel.Warnings.Contains(w))
                                sciModel.Warnings.Add(w);
                }

                model.ShoppingCartItemsCount = ReturnCartQuantity();
                model.WishListItemsCount = ReturnWishlistQuantity();
                model.OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, false);

                return View(model);
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }

        #endregion

        #region OrderTotals
        public ActionResult OrderTotals(bool isEditable = false)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable);
            return View(model);
        }
        #endregion

        #region Wishlist

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return InvokeHttp400("Shopping cart not Authorize");

            Customer customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;

            if (customer == null)
                return InvokeHttp400("Please Login To Add Item To Wishlish");

            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistModel();
            _shoppingCartModelFactory.PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }
        #endregion

        #region ApplyDiscountAndGiftCardCoupon

        public ActionResult ApplyDiscountAndGiftCardCoupon(string couponCode, FormCollection form)
        {
            //trim
            if (couponCode != null)
                couponCode = couponCode.Trim();

            //cart
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new DiscountCouponModel();
            if (!String.IsNullOrWhiteSpace(couponCode))
            {
                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discounts = _discountService.GetAllDiscountsForCaching(couponCode: couponCode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = discounts.Any(discount =>
                    {
                        var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, new[] { couponCode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });

                    if (anyValidDiscount)
                    {
                        //valid
                        _workContext.CurrentCustomer.ApplyDiscountCouponCode(couponCode);
                        model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.Applied");
                        model.DiscountBox.IsApplied = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                            //some user errors
                            model.DiscountBox.Message = userErrors.FirstOrDefault();
                        else
                            //general error text
                            model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
                    }
                }
                else
                {
                    //discount cannot be found
                    if (!cart.IsRecurring())
                    {
                        var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: couponCode).FirstOrDefault();
                        bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();
                        if (isGiftCardValid)
                        {
                            _workContext.CurrentCustomer.ApplyGiftCardCouponCode(couponCode);
                            model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                            model.GiftCardBox.IsApplied = true;
                        }
                        else
                        {
                            model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");
                            model.GiftCardBox.IsApplied = false;
                        }
                    }
                    else
                    {
                        model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");
                        model.GiftCardBox.IsApplied = false;
                    }

                }

            }
            else
                //empty coupon code
                model.DiscountBox.Message = _localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount");
            return View(model);
        }
        #endregion

        #region RemoveDiscountAndGiftCardCoupon

        public ActionResult RemoveDiscountAndGiftCardCoupon(string couponCode)
        {
            //trim
            if (couponCode != null)
                couponCode = couponCode.Trim();

            var model = new ShoppingCartModel();
            if (!String.IsNullOrWhiteSpace(couponCode))
            {

                var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: couponCode).FirstOrDefault();
                bool isGiftCardValid = giftCard != null && giftCard.IsGiftCardValid();


                //we find even hidden records here. this way we can display a user-friendly message if it's expired
                var discount = _discountService.GetDiscountByCouponCode(couponCode, true);
                if (discount != null && discount.RequiresCouponCode)
                {
                    var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, couponCode);
                    if (validationResult.IsValid)
                    {
                        _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.DiscountCouponCode, null);
                        return View(null, true, 200, "Discount Removed");
                    }
                    else
                    {
                        return InvokeHttp400("Invalid Discount code");
                    }
                }
                else if (isGiftCardValid)
                {
                    _workContext.CurrentCustomer.RemoveGiftCardCouponCode(couponCode);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    return View(null, true, 200, "Giftcard Removed");
                }
                else
                {
                    return InvokeHttp400("Invalid CouponCode");
                }
            }
            else
            {
                return InvokeHttp400("Invalid CouponCode");
            }


        }
        #endregion

        #region RemoveWishlistItem

        public ActionResult RemoveWishlistItem(string cartitemid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return InvokeHttp400("Permission denied");

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = cartitemid != null
                ? cartitemid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci);

            }

            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistModel();
            _shoppingCartModelFactory.PrepareWishlistModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return View(model, true, 200, "Cart item has been removed from wishlist");
        }
        #endregion

        #region AddProductToCart_Details

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, string AttributesJson)
        {
            try
            {
                var form = JsonStringToNameValueCollection(AttributesJson);
                var product = _productService.GetProductById(productId);
                if (product == null)
                {
                    return InvokeHttp400("Product Not Found");
                }

                #region Update existing shopping cart item?

                int updatecartitemid = 0;
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out updatecartitemid);
                        break;
                    }
                ShoppingCartItem updatecartitem = null;
                if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
                {
                    //search with the same cart type as specified
                    var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(x => x.ShoppingCartTypeId == shoppingCartTypeId)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();
                    updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

                    if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This product does not match a passed shopping cart item identifier"
                        });
                    }
                }

                #endregion

                #region Customer entered price
                decimal customerEnteredPriceConverted = decimal.Zero;
                if (product.CustomerEntersPrice)
                {
                    foreach (string formKey in form.AllKeys)
                    {
                        if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
                        {
                            decimal customerEnteredPrice;
                            if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                                customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                            break;
                        }
                    }
                }
                #endregion

                #region Quantity

                int quantity = 1;
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out quantity);
                        break;
                    }

                #endregion



                //product and gift card attributes
                string attributes = ParseProductAttributes(product, form);

                //rental attributes
                DateTime? rentalStartDate = null;
                DateTime? rentalEndDate = null;
                if (product.IsRental)
                {
                    ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
                }

                var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                    //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                    updatecartitem.ShoppingCartType;

                //save item
                var addToCartWarnings = new List<string>();
                if (updatecartitem == null)
                {
                    //add to the cart
                    addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        product, cartType, _storeContext.CurrentStore.Id,
                        attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true));
                }
                else
                {
                    var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(x => x.ShoppingCartType == updatecartitem.ShoppingCartType)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();
                    var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                        cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate);
                    if (otherCartItemWithSameParameters != null &&
                        otherCartItemWithSameParameters.Id == updatecartitem.Id)
                    {
                        //ensure it's some other shopping cart item
                        otherCartItemWithSameParameters = null;
                    }
                    //update existing item
                    addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        updatecartitem.Id, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true));
                    if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                    {
                        //delete the same shopping cart item (the other one)
                        _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                    }
                }

                #region Return result

                if (addToCartWarnings.Any())
                {
                    return InvokeHttp400(addToCartWarnings[0]);
                }

                //added to the cart/wishlist
                switch (cartType)
                {
                    case ShoppingCartType.Wishlist:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToWishlist", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                            //display notification message and update appropriate blocks
                            var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
                            _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());

                            object ob = new
                            {
                                ShoppingCartItemsCount = ReturnCartQuantity(),
                                WishListItemsCount = ReturnWishlistQuantity()
                            };
                            return View(ob, true, 200, "The product has been added to your wishlist");
                        }
                    case ShoppingCartType.ShoppingCart:
                    default:
                        {
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);



                            //display notification message and update appropriate blocks
                            var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                            _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .GetTotalProducts());


                            object ob = new
                            {
                                ShoppingCartItemsCount = ReturnCartQuantity(),
                                WishListItemsCount = ReturnWishlistQuantity()
                            };

                            //remove current product from wishlist if exist
                            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist && sci.ProductId == product.Id)
                                    .LimitPerStore(_storeContext.CurrentStore.Id)
                                    .ToList();

                            if (cart != null && cart.Count > 0)
                            {
                                foreach (var sci in cart)
                                {
                                    _shoppingCartService.DeleteShoppingCartItem(sci);
                                }
                            }

                            return View(ob, true, 200, string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart")));
                        }
                }


                #endregion
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }
        #endregion

        #region AddProductToCart_Details | JsonStringToNameValueCollection

        [NonAction]
        private NameValueCollection JsonStringToNameValueCollection(string jsonText)
        {
            if (jsonText == null) return new NameValueCollection();

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, string>>(jsonText);

            NameValueCollection nvc = null;
            if (dict != null)
            {
                nvc = new NameValueCollection(dict.Count);
                foreach (var k in dict)
                {
                    nvc.Add(k.Key, k.Value);
                }
            }

            var json = jss.Serialize(dict);
            return nvc;
        }
        #endregion

        #region AddProductToCart_Details |  ParseProductAttributes
        [NonAction]
        protected virtual string ParseProductAttributes(Product product, NameValueCollection form)
        {
            string attributesXml = "";

            #region Product attributes

            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }
        #endregion

        #region AddProductToCart_Details | ParseRentalDates
        [NonAction]
        protected virtual void ParseRentalDates(Product product, NameValueCollection form,
          out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        #endregion

        #region AddItemsToCartFromWishlist

        public virtual ActionResult AddItemsToCartFromWishlist(string WishlistItems)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return InvokeHttp400("Shopping Cart is Disable");

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return InvokeHttp400("Wishlist Is Disable");

            var pageCustomer = _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return InvokeHttp400("Please Login To Add Item To Wishlish");

            var pageCart = pageCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allWarnings = new List<string>();
            var numberOfAddedItems = 0;
            var allIdsToAdd = WishlistItems != null
                ? WishlistItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Product, ShoppingCartType.ShoppingCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true);
                    if (!warnings.Any())
                        numberOfAddedItems++;
                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !warnings.Any()) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        _shoppingCartService.DeleteShoppingCartItem(sci);
                    }
                    allWarnings.AddRange(warnings);
                }
            }

            if (numberOfAddedItems > 0)
            {
                //redirect to the shopping cart page

                if (allWarnings.Any())
                {
                    ErrorNotification(_localizationService.GetResource("Wishlist.AddToCart.Error"), true);
                }

                var cart = pageCustomer.ShoppingCartItems
                   .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
                var model = new WishlistModel();
                model = _shoppingCartModelFactory.PrepareWishlistModel(model, cart, true);
                return View(model);
            }
            else
            {
                //no items added. redisplay the wishlist page
                return InvokeHttp400(_localizationService.GetResource("Wishlist.AddToCart.Error"));
            }
        }

        #endregion

    }
}