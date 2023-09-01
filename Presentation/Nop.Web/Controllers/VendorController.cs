using System;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Vendors;
using Nop.Services.Stores;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Web.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using Nop.Services.Helpers;
using Nop.Services.Common;
using Nop.Core.Domain.Tax;
using Nop.Services.Tax;
using Nop.Services.Authentication;
using Nop.Services.Events;
using System.IO;

namespace Nop.Web.Controllers
{
    public partial class VendorController : BasePublicController
    {

        #region Fields
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly TaxSettings _taxSettings;
        private readonly ITaxService _taxService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAddressService _addressService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeFormatter _vendorAttributeFormatter;

        #endregion

        #region Constructors

        public VendorController(IVendorModelFactory vendorModelFactory,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings,
            CaptchaSettings captchaSettings,
            IProductService productService,
            CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            IOrderService orderService,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            DateTimeSettings dateTimeSettings,
            IGenericAttributeService genericAttributeService,
            TaxSettings taxSettings,
            ITaxService taxService,
            IAuthenticationService authenticationService,
            IAddressService addressService,
            IEventPublisher eventPublisher,
            IVendorAttributeService vendorAttributeService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeFormatter vendorAttributeFormatter)
        {
            this._storeService = storeService;
            this._vendorModelFactory = vendorModelFactory;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._storeMappingService = storeMappingService;
            this._localizationSettings = localizationSettings;
            this._vendorSettings = vendorSettings;
            this._captchaSettings = captchaSettings;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
            this._customerActivityService = customerActivityService;
            this._orderService = orderService;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._dateTimeSettings = dateTimeSettings;
            this._genericAttributeService = genericAttributeService;
            this._taxSettings = taxSettings;
            this._taxService = taxService;
            this._authenticationService = authenticationService;
            this._addressService = addressService;
            this._eventPublisher = eventPublisher;
            this._vendorAttributeService = vendorAttributeService;
            this._vendorAttributeParser = vendorAttributeParser;
            this._vendorAttributeFormatter = vendorAttributeFormatter;

        }

        #endregion

        #region Utilites

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        #endregion

        #region Methods

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ApplyVendor()
        {
            if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
                return RedirectToRoute("HomePage");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new ApplyVendorModel();
            model = _vendorModelFactory.PrepareApplyVendorModel(model, true, false, null);
            return View(model);
        }

        [HttpPost, ActionName("ApplyVendor")]
        [PublicAntiForgery]
        [CaptchaValidator]
        public virtual ActionResult ApplyVendorSubmit(ApplyVendorModel model, bool captchaValid, HttpPostedFileBase uploadedFile, FormCollection form)
        {
            if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
                ModelState.AddModelError("", "You cannot Apply For Vendor Account");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            int pictureId = 0;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = uploadedFile.GetPictureBits();
                    var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);

                    if (picture != null)
                        pictureId = picture.Id;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Vendors.ApplyAccount.Picture.ErrorMessage"));
                }
            }

            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            //vendor attributes
            //parse vendor attributes
            var vendorAttributesXml = _vendorModelFactory.ParseVendorAttributes(form);
            var VendorAttributeWarnings = _vendorModelFactory.GetVendorAttributesWarnings(vendorAttributesXml);
            if (VendorAttributeWarnings.Any())
            {
                foreach (var item in VendorAttributeWarnings)
                {
                    ModelState.AddModelError("", item);
                }
            }

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);
                model.VendorAttributeXml = vendorAttributesXml;
                model.VendorAttributeInfo = _vendorAttributeFormatter.FormatAttributes(vendorAttributesXml);

                //disabled by default
                var vendor = new Vendor
                {
                    Name = model.Name,
                    Email = model.Email,
                    //some default settings
                    PageSize = 6,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions,
                    PictureId = pictureId,
                    Description = description,
                };
                _vendorService.InsertVendor(vendor);
                //search engine name (the same as vendor name)
                var seName = vendor.ValidateSeName(vendor.Name, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, seName, 0);

                //associate to the current customer
                //but a store owner will have to manually add this customer role to "Vendors" role
                //if he wants to grant access to admin area
                _workContext.CurrentCustomer.VendorId = vendor.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                //notify store owner here (email)
                _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotification(_workContext.CurrentCustomer,
                    vendor, _localizationSettings.DefaultAdminLanguageId,model.Email.Trim(), model.Name, model.VendorAttributeInfo, model.VendorAttributeXml);

                model.DisableFormInput = true;
                model.Result = _localizationService.GetResource("Vendors.ApplyAccount.Submitted");
                return View(model);
            }

            foreach (var error in ModelState.Values.Where(x => x.Errors.Count > 0).ToList())
            {
                ModelState.AddModelError("", error.Errors.FirstOrDefault().ErrorMessage);
            }
            //If we got this far, something failed, redisplay form
            model = _vendorModelFactory.PrepareApplyVendorModel(model, false, true, vendorAttributesXml);
            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var model = new VendorInfoModel();
            model = _vendorModelFactory.PrepareVendorInfoModel(model, false);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [PublicAntiForgery]
        [ValidateInput(false)]
        [FormValueRequired("save-info-button")]
        public virtual ActionResult Info(VendorInfoModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            Picture picture = null;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType;
                    var vendorPictureBinary = uploadedFile.GetPictureBits();
                    picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.VendorInfo.Picture.ErrorMessage"));
                }
            }

            var vendor = _workContext.CurrentVendor;
            var prevPicture = _pictureService.GetPictureById(vendor.PictureId);

            if (ModelState.IsValid)
            {
                var description = Core.Html.HtmlHelper.FormatText(model.Description, false, false, true, false, false, false);

                vendor.Name = model.Name;
                vendor.Email = model.Email;
                vendor.Description = description;

                if (picture != null)
                {
                    vendor.PictureId = picture.Id;

                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                _vendorService.UpdateVendor(vendor);

                //notifications
                if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                    _workflowMessageService.SendVendorInformationChangeNotification(vendor, _localizationSettings.DefaultAdminLanguageId);

                return RedirectToAction("Info");
            }

            //If we got this far, something failed, redisplay form
            model = _vendorModelFactory.PrepareVendorInfoModel(model, true);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [PublicAntiForgery]
        [ValidateInput(false)]
        [FormValueRequired("remove-picture")]
        public virtual ActionResult RemovePicture()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentVendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("CustomerInfo");

            var vendor = _workContext.CurrentVendor;
            var picture = _pictureService.GetPictureById(vendor.PictureId);

            if (picture != null)
                _pictureService.DeletePicture(picture);

            vendor.PictureId = 0;
            _vendorService.UpdateVendor(vendor);

            //notifications
            if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                _workflowMessageService.SendVendorInformationChangeNotification(vendor, _localizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }
        #endregion

        #region Extended

        [ChildActionOnly]
        public ActionResult VendorRating(int vendorId = 0)
        {
            if (vendorId == 0)
                return null;

            var vendor = _vendorService.GetVendorById(vendorId);
            var allReviews = _vendorService.GetVendorReviews(vendorId, null, true);
            var averageRating = allReviews.Any() ? allReviews.Average(x => x.Rating) : 0;
            var model = new VendorRatingModel()
            {
                AverageRating = (decimal)averageRating,
                TotalRatings = allReviews.Count,
                VendorId = vendorId,
                VendorSeName = vendor.GetSeName(_workContext.WorkingLanguage.Id)
            };
            return PartialView(model);
        }


        [Authorize]
        public ActionResult UserReviews(bool Redirection = false)
        {
            var customer = _workContext.CurrentCustomer;
            var productReviews = _productService.GetAllProductReviews(customer.Id, null);
            var vendorReviews = _vendorService.GetVendorReviews(null, customer.Id, null);

            var model = new CustomerReviewSummaryModel()
            {
                TotalProductReviews = productReviews.Count,
                TotalVendorReviews = vendorReviews.Count,
                IsRedirection = Redirection
            };
            if (productReviews.Count > 0)
            {
                for (var index = 0; index < 5; index++)
                {
                    model.ProductRatingCounts[index] = productReviews.Count(pr => pr.Rating == (5 - index));
                }
            }

            if (vendorReviews.Count > 0)
            {
                for (var index = 0; index < 5; index++)
                {
                    model.VendorRatingCounts[index] = vendorReviews.Count(pr => pr.Rating == (5 - index));
                }
            }
            if (productReviews.Count > 0)
            {
                var recentProductReview = productReviews.OrderByDescending(pr => pr.CreatedOnUtc).First();
                model.LastRatedProductReview = recentProductReview;
            }
            if (vendorReviews.Count > 0)
            {
                var recentVendorReview = vendorReviews.OrderByDescending(pr => pr.CreatedOnUTC).First();
                model.LastRatedVendorReview = recentVendorReview;
            }
            model.CustomerName = customer.FormatUserName();
            return View(model);
        }

        [Authorize]
        public ActionResult UserReviewsData(string Type)
        {
            Type = Type.ToLower();
            var customer = _workContext.CurrentCustomer;
            if (customer != null && customer.IsRegistered())
            {
                if (Type == "product")
                {
                    var productReviews = _productService.GetAllProductReviews(customer.Id, null, storeId: _storeContext.CurrentStore.Id).ToList();
                    productReviews = productReviews.OrderByDescending(pr => pr.CreatedOnUtc).ToList();
                    var model = new PublicProductReviewDisplayModel()
                    {
                        ProductReviews = productReviews.ToModel(_pictureService)
                    };

                    foreach (var pr in productReviews)
                    {
                        if (model.ProductImageUrl.ContainsKey(pr.ProductId))
                            continue;
                        var imageUrl = pr.Product.ProductPictures.Any() ? _pictureService.GetPictureUrl(pr.Product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl();
                        model.ProductImageUrl.Add(pr.ProductId, imageUrl);
                    }
                    return PartialView("UserProductReviews", model);
                }
                if (Type == "vendor")
                {
                    var vendorReviews = _vendorService.GetVendorReviews(null, customer.Id, null, 1, int.MaxValue).ToList();


                    var model = new PublicVendorReviewDisplayModel()
                    {
                        VendorReviews = vendorReviews.ToListModel(_pictureService, _productService, _vendorService, _customerService)
                    };
                    return PartialView("UserVendorReviews", model);
                }
                if (Type == "pending")
                {
                    var customerOrders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id, customerId: customer.Id).Where(x => x.OrderStatus == OrderStatus.Complete || x.PaymentStatus == PaymentStatus.Paid || x.ShippingStatus == ShippingStatus.Delivered).ToList();

                    var pendingReviewProducts = _vendorService.GetProductsWithPendingReviews(customerOrders, customer.Id);

                    var model = new PublicPendingReviewDisplayModel();
                    var vendorList = new Dictionary<int, Vendor>(); //storing vendors for performance

                    foreach (var prp in pendingReviewProducts)
                    {

                        var order = prp.Key;
                        var orderModel = new PendingOrderModel()
                        {
                            OrderId = order.Id,
                        };
                        var reviewModelList = new List<PendingReviewModel>();

                        foreach (var product in prp.Value)
                        {
                            Vendor v = null;
                            if (vendorList.ContainsKey(product.VendorId))
                                v = vendorList[product.VendorId];
                            else
                            {
                                v = _vendorService.GetVendorById(product.VendorId);
                                vendorList.Add(v.Id, v);//add it to a dictionary for avoiding next time database query for same vendor
                            }

                            var prModel = new PendingReviewModel()
                            {
                                OrderId = order.Id,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                ProductImageUrl = product.ProductPictures.Any() ? _pictureService.GetPictureUrl(product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl(),
                                ProductSeName = product.GetSeName(),
                                VendorName = v.Name,
                                VendorSeName = v.GetSeName()
                            };
                            reviewModelList.Add(prModel);
                        }
                        model.PendingReviews.Add(orderModel, reviewModelList);
                    }
                    return PartialView("UserPendingReviews", model);
                }
                return RedirectToRoute("PageNotFound");
            }
            return RedirectToRoute("Homepage");
        }


        public ActionResult VendorDetails(string SeName)
        {
            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
            var record = urlRecordService.GetBySlug(SeName);
            int vendorId = 0;
            if (record != null)
            {
                if (record.IsActive && record.EntityName.ToLowerInvariant() == "vendor")
                {
                    vendorId = record.EntityId;
                }
            }
            if (vendorId == 0)
                return RedirectToRoute("PageNotFound");

            var vendor = _vendorService.GetVendorById(vendorId);

            var vendorReviews = _vendorService.GetVendorReviews(vendorId, null, null, 1, int.MaxValue).ToList();

            var model = new VendorReviewSummaryModel()
            {
                VendorReviewDisplayModel = new PublicVendorReviewDisplayModel()
                {
                    VendorReviews = vendorReviews.ToListModel(_pictureService, _productService, _vendorService, _customerService)
                },
                VendorName = vendor.Name,
                TotalVendorReviews = vendorReviews.Count,
            };
            for (var index = 0; index < 5; index++)
            {
                model.VendorRatingCounts[index] = vendorReviews.Count(pr => pr.Rating == (5 - index));
            }

            return View(model);

        }


        [Authorize]
        public ActionResult EditExtendedReview(int OrderId, int ProductId)
        {
            var product = _productService.GetProductById(ProductId);
            var order = _orderService.GetOrderById(OrderId);
            var vendor = _vendorService.GetVendorById(product.VendorId);
            var customer = _workContext.CurrentCustomer;
            var customerProductReviews = product.ProductReviews.Where(m => m.CustomerId == customer.Id).OrderByDescending(m => m.CreatedOnUtc);
            var customerVendorReview = _vendorService.GetVendorReview(product.VendorId, customer.Id, OrderId, product.Id);
            var model = new PublicReviewEditModel();
            model = _vendorModelFactory.PreparePublicReviewEditModel(model, product, order, vendor, customer, customerProductReviews, customerVendorReview);

            return PartialView(model);
        }

        [FormValueRequired("add-review")]
        [HttpPost, ActionName("EditExtendedReview")]
        public ActionResult EditExtendedReviewPost(int OrderId, int ProductId, PublicReviewEditModel model)
        {
            var ProductReview = model.AddProductReviewModel;
            var VendorReview = model.VendorReviewListModel;
            var customer = _workContext.CurrentCustomer;
            var order = _orderService.GetOrderById(OrderId);
            var product = _productService.GetProductById(ProductId);
            var vendor = _vendorService.GetVendorById(product.VendorId);
            var customerProductReviews = product.ProductReviews.Where(m => m.CustomerId == customer.Id).OrderByDescending(m => m.CreatedOnUtc);
            var customerVendorReview = _vendorService.GetVendorReview(product.VendorId, customer.Id, OrderId, ProductId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("HomePage");


            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }
            if (ModelState.IsValid)
            {
                var canCustomerReviewVendor = _vendorService.CanCustomerReviewVendor(product.VendorId, customer.Id, order);
                if (!canCustomerReviewVendor)
                {
                    return RedirectToRoute("PageNotFound");
                }
                var rating = ProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;
                ProductReview productReview;
                if (customerProductReviews.Any())
                {
                    productReview = customerProductReviews.First();
                    productReview.Title = ProductReview.Title;
                    productReview.ReviewText = ProductReview.ReviewText;
                    productReview.Rating = rating;
                    productReview.IsApproved = isApproved;
                    //updating
                }
                else
                {
                    productReview = new ProductReview()
                    {
                        ProductId = product.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        Title = ProductReview.Title,
                        ReviewText = ProductReview.ReviewText,
                        Rating = rating,
                        HelpfulYesTotal = 0,
                        HelpfulNoTotal = 0,
                        IsApproved = isApproved,
                        CreatedOnUtc = DateTime.UtcNow,
                        StoreId = _storeContext.CurrentStore.Id,
                    };
                    product.ProductReviews.Add(productReview);
                }
                _productService.UpdateProduct(product);

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview", product.Id, _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name);


                rating = VendorReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;



                if (customerVendorReview == null)
                {
                    customerVendorReview = new VendorReview()
                    {
                        IsApproved = isApproved,
                        CertifiedBuyerReview = true,
                        DisplayCertifiedBadge = true,
                        CreatedOnUTC = DateTime.Now,
                        CustomerId = customer.Id,
                        HelpfulnessNoTotal = 0,
                        HelpfulnessYesTotal = 0,
                        OrderId = OrderId,
                        ProductId = product.Id,
                        Rating = rating,
                        ReviewText = VendorReview.ReviewText,
                        Title = VendorReview.Title,
                        VendorId = product.VendorId
                    };
                }
                else
                {
                    customerVendorReview.IsApproved = isApproved;
                    customerVendorReview.ReviewText = VendorReview.ReviewText;
                    customerVendorReview.Title = VendorReview.Title;
                    customerVendorReview.Rating = rating;
                    customerVendorReview.HelpfulnessYesTotal = 0;
                    customerVendorReview.HelpfulnessNoTotal = 0;
                }
                _vendorService.SaveVendorReview(customerVendorReview);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewVendorReviews)
                    _workflowMessageService.SendVendorReviewNotificationMessage(_workContext.CurrentCustomer, vendor, product, customerVendorReview, _localizationSettings.DefaultAdminLanguageId);

                return RedirectToRoute("VendorReviewCenterLoader", new { Redirection = true });
            }

            //If we got this far, something failed, redisplay form
            model = _vendorModelFactory.PreparePublicReviewEditModel(model, product, order, vendor, customer, customerProductReviews, customerVendorReview);
            return PartialView(model);

        }



        #endregion

        #region Vendor Attribute Form

        [HttpPost]
        public virtual ActionResult VendorAttributeChange(FormCollection form)
        {
            var attributeXml = _vendorModelFactory.ParseVendorAttributes(form);

            var enabledAttributeIds = new List<int>();
            var disabledAttributeIds = new List<int>();
            var attributes = _vendorAttributeService.GetAllVendorAttributes(_storeContext.CurrentStore.Id);
            foreach (var attribute in attributes)
            {
                var conditionMet = _vendorAttributeParser.IsConditionMet(attribute, attributeXml);
                if (conditionMet.HasValue)
                {
                    if (conditionMet.Value)
                        enabledAttributeIds.Add(attribute.Id);
                    else
                        disabledAttributeIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributeids = enabledAttributeIds.ToArray(),
                disabledattributeids = disabledAttributeIds.ToArray()
            });
        }

        [HttpPost]
        public virtual ActionResult UploadFileVendorAttribute(int attributeId)
        {
            var attribute = _vendorAttributeService.GetVendorAttributeById(attributeId);
            if (attribute == null || attribute.AttributeControlType != AttributeControlType.FileUpload)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                }, MimeTypes.TextPlain);
            }

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            if (attribute.ValidationFileMaximumSize.HasValue)
            {
                //compare in bytes
                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    //when returning JSON the mime-type must be set to text/plain
                    //otherwise some browsers will pop-up a "Save As" dialog.
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("Vendor.MaximumUploadedFileSize"), attribute.ValidationFileMaximumSize.Value),
                        downloadGuid = Guid.Empty,
                    }, MimeTypes.TextPlain);
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            EngineContext.Current.Resolve<IDownloadService>().InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("Vendor.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            }, MimeTypes.TextPlain);
        }
        #endregion
    }
}
