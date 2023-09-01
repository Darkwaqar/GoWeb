using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using System.Net;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Customers;
using Nop.Admin.Models.Vendors;
using System.Collections.Generic;
using System.Linq;
using Nop.Web.Areas.Mservices.Models.Vendors;
using Nop.Web.Models.Vendors;
using Nop.Services.Catalog;
using Nop.Web.Areas.Mservices.Models.Product;
using Nop.Core.Domain.Catalog;
using Nop.Services.Orders;
using Nop.Services.Logging;

namespace Nop.Web.Areas.MServices.Controllers
{
    public partial class VendorController : BaseController
    {
        #region Fields

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
        private readonly MediaSettings _mediaSettings;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IOrderService _orderService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Constructors

        public VendorController(IWorkContext workContext,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings,
            CaptchaSettings captchaSettings,
            MediaSettings mediaSettings,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IStoreContext storeContext,
            IProductService productService,
            CatalogSettings catalogSettings,
            IOrderService orderService,
            ICustomerActivityService customerActivityService)


        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._customerService = customerService;
            this._workflowMessageService = workflowMessageService;
            this._vendorService = vendorService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._localizationSettings = localizationSettings;
            this._vendorSettings = vendorSettings;
            this._captchaSettings = captchaSettings;
            this._mediaSettings = mediaSettings;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._storeContext = storeContext;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
            this._orderService = orderService;
            this._customerActivityService = customerActivityService;

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

        #region GetAboutUsByVendorId

        public ActionResult GetAboutUsByVendorId(int vendorId = 0)
        {
            try
            {
                var vendor = _vendorService.GetVendorById(vendorId);
                if (vendor == null || vendor.Deleted || !vendor.Active)
                    return InvokeHttp400("Not a valid vendor");

                return View(WebUtility.HtmlEncode(vendor.AboutUs));
            }
            catch (Exception ex)
            {
                return InvokeHttp400(ex.ToString());
            }
        }
        #endregion

        #region SubscribeVendorNewsletter
        public virtual ActionResult SubscribeVendorNewsletter(string email = "", int vendorId = 0)
        {
            string result;
            bool success = false;
            string active = "UnSubscribe";
            var currentCustomer = _workContext.CurrentCustomer;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
                active = "Subscribe";
            }

            if (currentCustomer != null && !currentCustomer.IsGuest())
            {
                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndVendorId(currentCustomer.Email, vendorId);
                if (subscription != null)
                {
                    if (subscription.Active)
                    {
                        subscription.Active = false;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                        active = "Subscribe";
                    }
                    else
                    {
                        subscription.Active = true;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                        active = "UnSubscribe";
                    }
                }
                else
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = currentCustomer.Email,
                        Active = true,
                        StoreId = _storeContext.CurrentStore.Id,
                        VendorId = vendorId,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    active = "UnSubscribe";
                }
            }
            else
            {
                var subscription = new NewsLetterSubscription
                {
                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                    Email = email,
                    Active = true,
                    StoreId = _storeContext.CurrentStore.Id,
                    VendorId = vendorId,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                active = "UnSubscribe";
            }

            success = true;
            return Json(new
            {
                Success = success,
                Active = active,
                Result = result
            });
        }
        #endregion

        #region GetVendorBanner
        public virtual ActionResult GetVendorBanner(int vendorId = 0)
        {

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp400("Not a valid vendor");

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

            return View(vendorBannersModel);
        }
        #endregion

        #region GetVendorByCategoryId

        public virtual ActionResult GetVendorByCategoryId(int CategoryId = 0)
        {
            List<VendorModelSearchAPI> model = new List<VendorModelSearchAPI>();
            var Vendors = _vendorService.GetVendorByCategoryID(CategoryId);

            foreach (var Vendor in Vendors)
            {
                var vendorModel = new VendorModelSearchAPI
                {
                    Id = Vendor.Id,
                    Title = Vendor.Name,
                    MpLogoUrl = _pictureService.GetPictureUrl(Vendor.mpLogo)
                };
                model.Add(vendorModel);
            }

            if (model.Count > 0)
                return View(model);
            else
                return InvokeHttp400("No Vendor Found");
        }
        #endregion

        #region VendorRating
        public ActionResult VendorRating(int VendorId = 0)
        {
            if (VendorId == 0)
                return InvokeHttp400("No Vendor Found");

            var vendor = _vendorService.GetVendorById(VendorId);

            var vendorReviews = _vendorService.GetVendorReviews(VendorId, null, true, 1, int.MaxValue).ToList();

            var model = new ReviewSummaryModel()
            {
                Name = vendor.Name,
                Image = _pictureService.GetPictureUrl(vendor.PictureId, 300),
                TotalReviews = vendorReviews.Count(),
            };

            foreach (var VendorReview in vendorReviews)
            {
                var review = new ReviewListModel
                {
                    Id = VendorReview.ProductId,
                    ProductImage = _pictureService.GetPictureUrl(_pictureService.GetPicturesByProductId(VendorReview.ProductId).Count > 0 ? _pictureService.GetPicturesByProductId(VendorReview.ProductId).FirstOrDefault().Id : 0, 300),
                    Rating = VendorReview.Rating,
                    ReviewText = VendorReview.ReviewText,
                    Title = VendorReview.Title,
                    CreatedOnUtc = VendorReview.CreatedOnUTC
                };
                model.Reviews.Add(review);
            }
            for (var index = 0; index < 5; index++)
            {
                model.RatingCounts[index] = vendorReviews.Count(pr => pr.Rating == (5 - index));
            }

            return View(model);

        }



        public virtual ActionResult VendorReviewsAdd(PublicReviewEditModel model, int OrderId = 0, int ProductId = 0)
        {
            if (OrderId == 0)
                return InvokeHttp400("Order Not Found");

            if (ProductId == 0)
                return InvokeHttp400("Product Not Found");

            var ProductReview = model.AddProductReviewModel;
            var VendorReview = model.VendorReviewListModel;
            var customer = _workContext.CurrentCustomer;
            var order = _orderService.GetOrderById(OrderId);
            var product = _productService.GetProductById(ProductId);
            var vendor = _vendorService.GetVendorById(product.VendorId);
            var customerProductReviews = product.ProductReviews.Where(m => m.CustomerId == customer.Id).OrderByDescending(m => m.CreatedOnUtc);
            var customerVendorReview = _vendorService.GetVendorReview(product.VendorId, customer.Id, OrderId, ProductId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return InvokeHttp400("Product Not Found");


            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }
            if (ModelState.IsValid)
            {
                var canCustomerReviewVendor = _vendorService.CanCustomerReviewVendor(product.VendorId, customer.Id, order);
                if (!canCustomerReviewVendor)
                {
                    return InvokeHttp400("You Cannot Review the vendor");
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

                if (!isApproved)
                    return View("", message: _localizationService.GetResource("Reviews.SeeAfterApproving"));
                else
                    return View("", message: _localizationService.GetResource("Reviews.SuccessfullyAdded"));
            }
            return InvokeHttp400(ModelState.Where(x => x.Value.Errors.Count > 0).First().Value.Errors.First().ErrorMessage);
        }
        #endregion
    }
}
