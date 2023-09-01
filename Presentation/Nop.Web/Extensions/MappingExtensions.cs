using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Models.Common;
using Nop.Web.Models.Vendors;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Extensions
{
    public static class MappingExtensions
    {
        //address
        public static Address ToEntity(this AddressModel model, bool trimFields = true)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity, trimFields);
        }
        public static Address ToEntity(this AddressModel model, Address destination, bool trimFields = true)
        {
            if (model == null)
                return destination;

            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                if (model.City != null)
                    model.City = model.City.Trim();
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }
            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }

        public static IList<VendorReviewListModel> ToListModel(this IList<VendorReview> VendorReviews, IPictureService _pictureService,
            IProductService _productService, IVendorService _vendorService, ICustomerService _customerService)
        {
            var vrList = new List<VendorReviewListModel>();
            foreach (var vr in VendorReviews)
                vrList.Add(vr.ToListModel(_pictureService, _productService, _vendorService, _customerService));
            return vrList;
        }


        public static IList<ProductReviewListModel> ToModel(this IList<ProductReview> ProductReviews, IPictureService _pictureService)
        {
            return ProductReviews.Select(pr => pr.ToModel(_pictureService)).ToList();
        }

        public static IList<ProductReviewListModel> ToModel(this IOrderedEnumerable<ProductReview> ProductReviews, IPictureService _pictureService)
        {
            return ProductReviews.Select(pr => pr.ToModel(_pictureService)).ToList();
        }

        public static ProductReviewListModel ToModel(this ProductReview ProductReview, IPictureService _pictureService)
        {
            var model = new ProductReviewListModel()
            {
                CustomerId = ProductReview.CustomerId,
                ProductId = ProductReview.ProductId,
                ProductName = ProductReview.Product.Name,
                Rating = ProductReview.Rating,
                ReviewText = ProductReview.ReviewText,
                ProductSeName = ProductReview.Product.GetSeName(),
                ProductImageUrl = ProductReview.Product.ProductPictures.Any() ? _pictureService.GetPictureUrl(ProductReview.Product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl(),
                Title = ProductReview.Title,
                CreatedOnUtc = ProductReview.CreatedOnUtc
            };
            return model;
        }


        public static VendorReviewListModel ToListModel(this VendorReview Review, IPictureService _pictureService,
            IProductService _productService, IVendorService _vendorService, ICustomerService _customerService)
        {
            var Product = _productService.GetProductById(Review.ProductId);
            var Vendor = _vendorService.GetVendorById(Review.VendorId);
            var Customer = _customerService.GetCustomerById(Review.CustomerId);
            var model = new VendorReviewListModel()
            {
                CreatedOnUTC = Review.CreatedOnUTC,
                CustomerId = Review.CustomerId,
                CustomerName = Customer.GetFullName(),
                HelpfulnessNoTotal = Review.HelpfulnessNoTotal,
                HelpfulnessYesTotal = Review.HelpfulnessYesTotal,
                Id = Review.Id,
                IsApproved = Review.IsApproved,
                ProductId = Review.ProductId,
                Rating = Review.Rating,
                ReviewText = Review.ReviewText,
                Title = Review.Title,
                VendorId = Review.VendorId,
                OrderId = Review.OrderId,
                CertifiedBuyerReview = Review.CertifiedBuyerReview,
                DisplayCertifiedBadge = Review.DisplayCertifiedBadge,

            };
            if (Product != null)
            {
                model.ProductName = Product.Name;
                model.ProductSeName = Product.GetSeName();
                model.ProductImageUrl = Product.ProductPictures.Any() ? _pictureService.GetPictureUrl(Product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl();

            }
            if (Vendor != null)
            {
                model.VendorName = Vendor.Name;
                model.VendorSeName = Vendor.GetSeName();
                model.VendorImageUrl = _pictureService.GetPictureUrl(Vendor.PictureId);
            }

            return model;
        }

        public static VendorReviewListModel ToListModel(this VendorReview Review, IPictureService _pictureService,
            Product Product, Vendor Vendor, Order Order)
        {

            var model = new VendorReviewListModel()
            {
                CreatedOnUTC = Review.CreatedOnUTC,
                CustomerId = Review.CustomerId,
                HelpfulnessNoTotal = Review.HelpfulnessNoTotal,
                HelpfulnessYesTotal = Review.HelpfulnessYesTotal,
                Id = Review.Id,
                IsApproved = Review.IsApproved,
                ProductId = Review.ProductId,
                Rating = Review.Rating,
                ReviewText = Review.ReviewText,
                Title = Review.Title,
                VendorId = Review.VendorId,
                OrderId = Review.OrderId,
                CertifiedBuyerReview = Review.CertifiedBuyerReview,
                DisplayCertifiedBadge = Review.DisplayCertifiedBadge,
            };
            if (Product != null)
            {
                model.ProductName = Product.Name;
                model.ProductSeName = Product.GetSeName();
                model.ProductImageUrl = Product.ProductPictures.Any() ? _pictureService.GetPictureUrl(Product.ProductPictures.FirstOrDefault().Picture) : _pictureService.GetDefaultPictureUrl();

            }
            if (Vendor != null)
            {
                model.VendorName = Vendor.Name;
                model.VendorSeName = Vendor.GetSeName();
            }
            return model;
        }

    }
}