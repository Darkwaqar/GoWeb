using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Models.Checkout;
using Nop.Web.MServices.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using Nop.Web.Factories;
using Nop.Web.Areas.MServices.Controllers;
using Nop.Services.Discounts;
using Nop.Web.Models.Common;
using static Nop.Web.Areas.Mservices.Models.Common.CountryandStatesModel;
using Nop.Services.Vendors;
using Nop.Services.Seo;
using Nop.Web.Areas.Mservices.Models.Checkout;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class CheckoutController : BaseController
    {
        #region Fields
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressService _addressService;
        private readonly ICacheManager _cacheManager;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IVendorService _vendorServices;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public CheckoutController(ICheckoutModelFactory checkoutModelFactory,
            ICustomerModelFactory customerModelFactory,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IWorkContext workContext,
              IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IShoppingCartService shoppingCartService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IPaymentService paymentService,
            IPluginFinder pluginFinder,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            ILogger logger,
            IOrderService orderService,
            IWebHelper webHelper,
            HttpContextBase httpContext,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressService addressService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeFormatter productAttributeFormatter,
            ICacheManager cacheManager,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            TaxSettings taxSettings,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IVendorService vendorService,
            ICustomerActivityService customerActivityService)
        {
            this._customerModelFactory = customerModelFactory;
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._checkoutModelFactory = checkoutModelFactory;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;
            this._logger = logger;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._customerSettings = customerSettings;
            this._addressService = addressService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._priceCalculationService = priceCalculationService;
            this._cacheManager = cacheManager;
            this._productAttributeParser = productAttributeParser;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._taxSettings = taxSettings;
            this._productAttributeService = productAttributeService;
            this._vendorServices = vendorService;
            this._customerActivityService = customerActivityService;
        }

        #endregion

        #region ConvertToAddressMobileModel
        private AddressMobileModel ConvertToAddressMobileModel(AddressModel address)
        {
            AddressMobileModel model = new AddressMobileModel
            {
                Id = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                CountryId = address.CountryId,
                CountryName = address.CountryName,
                StateProvinceId = address.StateProvinceId,
                StateProvinceName = address.StateProvinceName,
                City = address.City,
                Address1 = address.Address1,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
            };
            return model;
        }
        #endregion

        #region Model| GetShippingAndBillingAddress | ConvertToMyAddressModel
        public AddressMobileModel ConvertToAddressMobileModel(Address Address)
        {
            AddressMobileModel model = new AddressMobileModel
            {
                Id = Address.Id,
                FirstName = Address.FirstName,
                LastName = Address.LastName,
                Email = Address.Email,
                Company = Address.Company,
                Address1 = Address.Address1,
                Address2 = Address.Address2,
                City = Address.City,
                ZipPostalCode = Address.ZipPostalCode,
                PhoneNumber = Address.PhoneNumber,
                CountryId = Convert.ToInt32(Address.CountryId),
                StateProvinceId = Convert.ToInt32(Address.StateProvinceId)
            };
            return model;
        }
        #endregion

        #region GetAllDetailsForCheckOut

        public ActionResult GetAllDetailsForCheckOut()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!cart.Any())
                return InvokeHttp400("No Items found in the cart");

            List<ShoppingCartModel.VendorShoppingCartItemModel> VendorShoppingCartItems;

            var ShopCartItems = GetAllShopCartItems(cart, out VendorShoppingCartItems);

            var OrderTotal = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, true);

            var CustomersAddresses = _customerModelFactory.PrepareCustomerAddressListModel().Addresses;
            List<AddressMobileModel> Addresses = new List<AddressMobileModel>();
            foreach (var address in CustomersAddresses)
            {
                Addresses.Add(ConvertToAddressMobileModel(address));
            }

            AddressMobileModel shippingAddress = null;
            AddressMobileModel billingAddress = null;

            if (_workContext.CurrentCustomer.ShippingAddress != null)
            {
                shippingAddress = ConvertToAddressMobileModel(_workContext.CurrentCustomer.ShippingAddress);
            }
            if (_workContext.CurrentCustomer.BillingAddress != null)
            {
                billingAddress = ConvertToAddressMobileModel(_workContext.CurrentCustomer.BillingAddress);
            }

            var shippingMethodModel = _checkoutModelFactory.PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);

            //filter by country
            int filterByCountryId = 0;
            if (_addressSettings.CountryEnabled &&
                _workContext.CurrentCustomer.BillingAddress != null &&
                _workContext.CurrentCustomer.BillingAddress.Country != null)
            {
                filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
            }

            var paymentMethodModel = _checkoutModelFactory.PreparePaymentMethodModel(cart, filterByCountryId);

            IList<Country> AvalibleCountries = new List<Country>();
            foreach (var country in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                var c = new Country
                {
                    Name = country.Name,
                    CountryId = country.Id
                };
                foreach (var Provinces in country.StateProvinces)
                {
                    var p = new StateProvince
                    {
                        Name = Provinces.Name,
                        ProvinceId = Provinces.Id
                    };
                    c.States.Add(p);
                };
                AvalibleCountries.Add(c);
            }

            return View(new
            {
                ShippingLocation = shippingAddress,
                BillingAddress = billingAddress,
                Addresses = Addresses,
                ShopingCartItems = ShopCartItems,
                VendorShoppingCartItems = VendorShoppingCartItems,
                Total = OrderTotal,
                ShippingMethod = shippingMethodModel,
                PaymentMethods = paymentMethodModel,
                AvalibleCountries = AvalibleCountries,
                ShippingRequired = cart.RequiresShipping(),
                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep

            });

        }
        #endregion

        #region GetAllDetailsForCheckOut | GetAllShopCartItems
        [NonAction]
        public virtual List<ShoppingCartModel.ShoppingCartItemModel> GetAllShopCartItems(List<ShoppingCartItem> cart, out List<ShoppingCartModel.VendorShoppingCartItemModel> VendorShoppingCartItems)
        {
            List<ShoppingCartModel.ShoppingCartItemModel> Items = new List<ShoppingCartModel.ShoppingCartItemModel>();
            VendorShoppingCartItems = new List<ShoppingCartModel.VendorShoppingCartItemModel>();

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                    AttributeInfoXml = sci.AttributesXml,
                    ShortDescription = sci.Product.ShortDescription,
                };

                //vendor properties
                if (sci.Product.VendorId > 0)
                {
                    var vendor = _vendorServices.GetVendorById(sci.Product.VendorId);
                    cartItemModel.VendorId = vendor.Id;
                    cartItemModel.VendorName = vendor.Name;
                    cartItemModel.VendorSeName = vendor.GetSeName();

                    decimal sciSubTotal = _priceCalculationService.GetSubTotal(sci);
                    if (!VendorShoppingCartItems.Any(x => x.VendorId == vendor.Id))
                    {
                        var VendorShoppingCartItem = new ShoppingCartModel.VendorShoppingCartItemModel();

                        VendorShoppingCartItem.VendorId = vendor.Id;
                        VendorShoppingCartItem.Total = sciSubTotal;
                        if (vendor.FreeShippingOverXEnabled)
                        {
                            if (vendor.FreeShippingOverXValue > VendorShoppingCartItem.Total)
                            {
                                VendorShoppingCartItem.ShippingCharges = vendor.ShippingCharge;
                            }
                        }
                        else
                        {
                            VendorShoppingCartItem.ShippingCharges = vendor.ShippingCharge;
                        }

                        VendorShoppingCartItems.Add(VendorShoppingCartItem);
                    }
                    else
                    {
                        var VendorShoppingCartItem = VendorShoppingCartItems.Where(x => x.VendorId == vendor.Id).FirstOrDefault();
                        VendorShoppingCartItem.Total += sciSubTotal;
                        if (vendor.FreeShippingOverXEnabled)
                        {
                            if (vendor.FreeShippingOverXValue > VendorShoppingCartItem.Total)
                            {
                                VendorShoppingCartItem.ShippingCharges = vendor.ShippingCharge;
                            }
                        }
                        else
                        {
                            VendorShoppingCartItem.ShippingCharges = vendor.ShippingCharge;
                        }
                    }
                }




                #region Product attributes

                //performance optimization
                //We cache a value indicating whether a product has attributes
                IList<ProductAttributeMapping> productAttributeMapping = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, sci.Product.Id);
                var hasProductAttributesCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasProductAttributesCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load attributes and cache the result (true/false)
                    productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(sci.Product.Id);
                    hasProductAttributesCache = productAttributeMapping.Any();
                    _cacheManager.Set(cacheKey, hasProductAttributesCache, 60);
                }
                if (hasProductAttributesCache.Value && productAttributeMapping == null)
                {
                    //cache indicates that the product has attributes
                    //let's load them
                    productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(sci.Product.Id);
                }
                if (productAttributeMapping == null)
                {
                    productAttributeMapping = new List<ProductAttributeMapping>();
                }
                foreach (var attribute in productAttributeMapping)
                {
                    var attributeModel = new ProductDetailsModel.ProductAttributeModel
                    {
                        Id = attribute.Id,
                        ProductId = sci.Product.Id,
                        ProductAttributeId = attribute.ProductAttributeId,
                        Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
                        Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
                        TextPrompt = attribute.TextPrompt,
                        IsRequired = attribute.IsRequired,
                        AttributeControlType = attribute.AttributeControlType,
                        //DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                        HasCondition = !String.IsNullOrEmpty(attribute.ConditionAttributeXml)
                    };
                    if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                    {
                        attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                    }

                    if (attribute.ShouldHaveValues())
                    {
                        //values
                        var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                        foreach (var attributeValue in attributeValues)
                        {
                            var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                            {
                                Id = attributeValue.Id,
                                Name = attributeValue.GetLocalized(x => x.Name),
                                ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                                IsPreSelected = attributeValue.IsPreSelected
                            };
                            attributeModel.Values.Add(valueModel);
                        }
                    }

                    cartItemModel.ProductAttributes.Add(attributeModel);
                }

                #endregion

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    List<DiscountForCaching> scDiscounts;
                    int? maximumDiscountQty;
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts, out maximumDiscountQty), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                    cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                    //display an applied discount amount
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                cartItemModel.Picture = _shoppingCartModelFactory.PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                Items.Add(cartItemModel);
                VendorShoppingCartItems.Where(x => x.VendorId == cartItemModel.VendorId).FirstOrDefault().Items.Add(cartItemModel);
            }

            return Items;
        }


        #endregion

        #region UseRewardPoints
        public ActionResult UseRewardPoints(Boolean useRewardPoints = false)
        {

            if (_rewardPointsSettings.Enabled)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                   SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, useRewardPoints,
                   _storeContext.CurrentStore.Id);
            }
            return View("", true, 200, "Reward Point Checked");

        }
        #endregion

        #region ConfirmOrder
        [ValidateInput(false)]
        public ActionResult ConfirmOrder(FormCollection form, string shippingmethod = "", string paymentmethod = "", Boolean useRewardPoints = false)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                if (cart.Count == 0)
                    return InvokeHttp400("Your cart is empty");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    return InvokeHttp400("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    return InvokeHttp400(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));


                #region RewardPoint

                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                       SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, useRewardPoints,
                       _storeContext.CurrentStore.Id);
                }

                #endregion

                #region shipping location

                if (cart.RequiresShipping())
                {

                    //parse selected method 
                    string shippingoption = shippingmethod;
                    if (String.IsNullOrEmpty(shippingoption))
                        return InvokeHttp400("Selected shipping method can't be parsed");

                    var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);

                    if (splittedOption.Length != 2)
                        return InvokeHttp400("Selected shipping method can't be parsed");

                    string selectedName = splittedOption[0];

                    string shippingRateComputationMethodSystemName = splittedOption[1];

                    //find it
                    //performance optimization. try cache first
                    var shippingOptions = _workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(SystemCustomerAttributeNames.OfferedShippingOptions, _storeContext.CurrentStore.Id);
                    if (shippingOptions == null || shippingOptions.Count == 0)
                    {
                        //not found? let's load them using shipping service
                        shippingOptions = _shippingService
                            .GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress, _workContext.CurrentCustomer, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id)
                            .ShippingOptions
                            .ToList();
                    }
                    else
                    {
                        //loaded cached results. let's filter result by a chosen shipping rate computation method
                        shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
                    }

                    var shippingOption = shippingOptions
                        .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOption == null)
                        return InvokeHttp400("Selected shipping method can't be loaded");

                    //save
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);
                }
                #endregion

                #region payment method

                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    return InvokeHttp400("Selected payment method can't be parsed");

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
                }

                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethod == null ||
                    !paymentMethod.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethod.PluginDescriptor, _storeContext.CurrentStore.Id))
                    return InvokeHttp400("Selected payment method can't be parsed");

                var paymentControllerType = paymentMethod.GetControllerType();
                var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as Framework.Controllers.BasePaymentController;
                if (paymentController == null)
                    return InvokeHttp400("Payment controller cannot be loaded");

                var warnings = paymentController.ValidatePaymentForm(form);
                if (warnings.Any())
                    return InvokeHttp400(warnings.FirstOrDefault());

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);

                #endregion

                #region PlaceOrder
                //get payment info

                var processPaymentRequest = paymentController.GetPaymentInfo(form);
                if (processPaymentRequest == null)
                {
                    processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                //place order
                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    var model = new CheckoutServieModel();
                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        Session["ServicesRedirection"] = true;
                        _paymentService.PostProcessPayment(postProcessPaymentRequest);

                        if (Session["RediredURL"] != null)
                        {
                            string redirectUrl = Session["RediredURL"].ToString();

                            model.PaymentMethodType = PaymentMethodType.Redirection;
                            model.RedirectUrl = redirectUrl;
                            model.OrderId = placeOrderResult.PlacedOrder.Id;
                            return View(model, true, 200, "Order placed successfully");
                        }
                    }
                    model.PaymentMethodType = PaymentMethodType.Standard;
                    model.RedirectUrl = "";
                    model.OrderId = placeOrderResult.PlacedOrder.Id;

                    //success
                    return View(model, true, 200, "Order placed successfully");
                }
                //error
                var confirmOrderModel = new CheckoutConfirmModel();
                foreach (var error in placeOrderResult.Errors)
                    confirmOrderModel.Warnings.Add(error);

                if (confirmOrderModel.Warnings.Count > 0)
                    return InvokeHttp400(confirmOrderModel.Warnings[0]);
                else
                    return InvokeHttp400("Order can not be processed");

                #endregion

            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return InvokeHttp400(exc.Message);
            }
        }
        #endregion

        #region ConfirmOrder| IsPaymentWorkflowRequired

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, ignoreRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }
        #endregion

        #region ConfirmOrder| IsMinimumOrderPlacementIntervalValid

        [NonAction]
        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }
        #endregion

        #region GfhAddAddress
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GfhAddAddress(AddressMobileModel model, FormCollection form)
        {

            var customer = _workContext.CurrentCustomer;


            if (!customer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            if (model.FirstName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.FirstName.Required"));
            if (model.LastName == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.LastName.Required"));
            if (model.Email == null) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Email.Required"));
            if (_addressSettings.CountryEnabled)
            {
                if (model.CountryId == null || model.CountryId == 0) ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (_addressSettings.CountryEnabled && _addressSettings.StateProvinceEnabled)
            {
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = _stateProvinceService.GetStateProvincesByCountryId(countryId).Any();

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        ModelState.AddModelError("", _localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (_addressSettings.StreetAddressRequired && _addressSettings.StreetAddressEnabled)
            {
                if (model.Address1 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (_addressSettings.StreetAddress2Required && _addressSettings.StreetAddress2Required)
            {
                if (model.Address2 == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (_addressSettings.ZipPostalCodeRequired && _addressSettings.ZipPostalCodeEnabled)
            {
                if (model.ZipPostalCode == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (_addressSettings.CityRequired && _addressSettings.CityEnabled)
            {
                if (model.City == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (_addressSettings.PhoneRequired && _addressSettings.PhoneEnabled)
            {
                if (model.PhoneNumber == null) ModelState.AddModelError("", _localizationService.GetResource("Account.Fields.Phone.Required"));
            }

            if (model.Id != 0)
            {
                //custom address attributes

                //find address (ensure that it belongs to the current customer)
                var address = customer.Addresses.FirstOrDefault(a => a.Id == model.Id);
                if (address == null)
                    //address is not found
                    return InvokeHttp400("Address Not Found");

                if (ModelState.IsValid)
                {
                    address.Id = model.Id;
                    address.FirstName = model.FirstName;
                    address.LastName = model.LastName;
                    address.Email = model.Email;
                    address.Company = model.Company;
                    address.Address1 = model.Address1;
                    address.Address2 = model.Address2;
                    address.City = model.City;
                    address.ZipPostalCode = model.ZipPostalCode;
                    address.PhoneNumber = model.PhoneNumber;
                    if (model.CountryId != null)
                        address.CountryId = Convert.ToInt32(model.CountryId);
                    if (model.StateProvinceId != null)
                        address.StateProvinceId = Convert.ToInt32(model.StateProvinceId);

                    _addressService.UpdateAddress(address);
                    if (customer.Addresses.Any())
                    {
                        customer.ShippingAddress = customer.Addresses.OrderByDescending(x => x.CreatedOnUtc).FirstOrDefault();
                        return View(model, true, 200, "Address Updated successfully.");
                    }
                    return InvokeHttp400("Address Not Found");

                }

                return InvokeHttp400(ModelState.LastOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);
            }
            else
            {


                Address address = ReturnAddressModel(model);
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                if (ModelState.IsValid)
                {
                    customer.Addresses.Add(address);
                    if (customer.Addresses.Any())
                    {
                        customer.ShippingAddress = customer.Addresses.OrderByDescending(x => x.CreatedOnUtc).FirstOrDefault();
                        customer.BillingAddress = customer.Addresses.OrderByDescending(x => x.CreatedOnUtc).FirstOrDefault();
                        _customerService.UpdateCustomer(customer);
                        return View(model, true, 200, "Address Saved successfully.");
                    }
                    return InvokeHttp400("Address Not found");

                }


                return InvokeHttp400(ModelState.LastOrDefault().Value.Errors.FirstOrDefault().ErrorMessage);

            }

        }

        #endregion

        #region GfhAddAddress | ReturnAddressModel

        private Address ReturnAddressModel(AddressMobileModel model)
        {
            Address address = new Address();
            address.FirstName = model.FirstName;
            address.LastName = model.LastName;
            address.Email = model.Email;
            address.Company = model.Company;
            address.Address1 = model.Address1;
            address.Address2 = model.Address2;
            address.City = model.City;
            address.ZipPostalCode = model.ZipPostalCode;
            address.PhoneNumber = model.PhoneNumber;
            if (model.CountryId != null)
                address.CountryId = Convert.ToInt32(model.CountryId);
            if (model.StateProvinceId != null)
                address.StateProvinceId = Convert.ToInt32(model.StateProvinceId);

            return address;
        }

        #endregion

        #region GfhAddBillingAddress
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GfhAddBillingAddress(AddressMobileModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return InvokeHttp400("Customer is not registered");

            if (_workContext.CurrentCustomer.BillingAddress != null)
            {
                Address address = _addressService.GetAddressById(_workContext.CurrentCustomer.BillingAddress.Id);

                address.FirstName = model.FirstName;
                address.LastName = model.LastName;
                address.Email = model.Email;
                address.Company = model.Company;
                address.Address1 = model.Address1;
                address.Address2 = model.Address2;
                address.City = model.City;
                address.ZipPostalCode = model.ZipPostalCode;
                address.PhoneNumber = model.PhoneNumber;
                if (model.CountryId != null)
                    address.CountryId = Convert.ToInt32(model.CountryId);
                if (model.StateProvinceId != null)
                    address.StateProvinceId = Convert.ToInt32(model.StateProvinceId);

                _addressService.UpdateAddress(address);
                _workContext.CurrentCustomer.BillingAddress = address;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return View(null, true, 0, "Billing address Updated successfully.");
            }
            else
            {
                Address address = ReturnAddressModel(model);
                _addressService.InsertAddress(address);
                _workContext.CurrentCustomer.BillingAddress = address;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                return View(null, true, 0, "Billing address saved successfully.");
            }

        }


        #endregion

        #region OpcSaveShippingAndBilling

        // todo method has to be umcommented after resolving pickupinstorfee issue
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult OpcSaveShippingAndBilling(CustomerAddressEditModel model, FormCollection form)
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return InvokeHttp400("User not register");

                var customer = _workContext.CurrentCustomer;

                if (model.Address.Id > 0)
                {
                    //find address (ensure that it belongs to the current customer)
                    var address = customer.Addresses.FirstOrDefault(a => a.Id == model.Address.Id);
                    if (address == null)
                        return InvokeHttp400("Address Not Found");

                    address.Id = model.Address.Id;
                    address.FirstName = model.Address.FirstName;
                    address.LastName = model.Address.LastName;
                    address.Email = model.Address.Email;
                    address.Company = model.Address.Company;
                    address.Address1 = model.Address.Address1;
                    address.Address2 = model.Address.Address2;
                    address.ZipPostalCode = model.Address.ZipPostalCode;
                    address.PhoneNumber = model.Address.PhoneNumber;
                    address.FaxNumber = model.Address.FaxNumber;
                    if (model.Address.CountryId != null)
                        address.CountryId = Convert.ToInt32(model.Address.CountryId);
                    else address.CountryId = null;
                    if (model.Address.StateProvinceId != null)
                        address.StateProvinceId = Convert.ToInt32(model.Address.StateProvinceId);
                    else address.StateProvinceId = null;

                    _addressService.UpdateAddress(address);
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _workContext.CurrentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);


                    return View("", true, 200, "Address Updated");
                }
                else
                {
                    Address address = new Address
                    {
                        FirstName = model.Address.FirstName,
                        LastName = model.Address.LastName,
                        Email = model.Address.Email,
                        Company = model.Address.Company,
                        Address1 = model.Address.Address1,
                        Address2 = model.Address.Address2,
                        ZipPostalCode = model.Address.ZipPostalCode,
                        PhoneNumber = model.Address.PhoneNumber,
                        FaxNumber = model.Address.FaxNumber
                    };
                    if (model.Address.CountryId != null)
                        address.CountryId = Convert.ToInt32(model.Address.CountryId);
                    else address.CountryId = null;
                    if (model.Address.StateProvinceId != null)
                        address.StateProvinceId = Convert.ToInt32(model.Address.StateProvinceId);
                    else address.StateProvinceId = null;
                    address.CreatedOnUtc = DateTime.UtcNow;

                    _workContext.CurrentCustomer.Addresses.Add(address);
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _workContext.CurrentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                    return View("", true, 200, "Address Added");
                }

            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { success = false, message = exc.Message });
            }
        }
        #endregion

        #region SelectBillingAddress / SelectShippingAddress

        public virtual ActionResult SelectBillingAddress(int addressId = 0, bool shipToSameAddress = false)
        {
            var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return InvokeHttp400("Address Not Found");

            _workContext.CurrentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //ship to the same address?
            if (_shippingSettings.ShipToSameAddress && shipToSameAddress && cart.RequiresShipping())
            {
                _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                //reset selected shipping method (in case if "pick up in store" was selected)
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return View("", true, 200, "Billing / Shipping Address Selected");
            }
            return View("", true, 200, "Shipping Address Selected");
        }


        public virtual ActionResult SelectShippingAddress(int addressId = 0)
        {
            var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                return InvokeHttp400("Address Not Found");

            _workContext.CurrentCustomer.ShippingAddress = address;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            if (_shippingSettings.AllowPickUpInStore)
            {
                //set value indicating that "pick up in store" option has not been chosen
                _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
            }
            return View("", true, 200, "Shipping Address Selected");
        }

        #endregion

        #region CancelRedirection

        public ActionResult CancelRedirection()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                   customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                   .FirstOrDefault();
            if (order == null)
            {
                return InvokeHttp400("Order not found");
            }
            var currentCustomer = _workContext.CurrentCustomer;

            if (order.OrderStatus != OrderStatus.Cancelled || order.PaymentStatus != PaymentStatus.Paid || order.OrderStatus != OrderStatus.Processing)
            {
                _shoppingCartService.MigrateOrderShoppingCart(order, currentCustomer);
                _orderProcessingService.CancelOrder(order, true);
                _customerActivityService.InsertActivity("EditOrder", order.Id, _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
            }

            return View("Order Canceled and Restored Products");
        }

        #endregion

    }
}