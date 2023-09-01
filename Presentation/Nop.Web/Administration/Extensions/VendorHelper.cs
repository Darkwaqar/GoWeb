using Nop.Admin.Models.Vendors;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Orders;

namespace Nop.Admin.Extensions
{
    public static class VendorHelper
    {
        static decimal GetPayoutAmount(decimal SellingPrice, decimal CommissionPercentage)
        {

            var total = SellingPrice - (SellingPrice * CommissionPercentage) / 100;
            //total = Math.Round(total);
            return total;

        }
        public static VendorPayoutModel PreparePayoutModel(this VendorPayout Payout, IOrderService _orderService)
        {
            var model = new VendorPayoutModel()
            {
                CommissionPercentage = Payout.CommissionPercentage,
                Id = Payout.Id,
                OrderId = Payout.OrderId,
                PayoutDate = Payout.PayoutDate,
                PayoutStatus = Payout.PayoutStatus,
                Remarks = Payout.Remarks,
                VendorId = Payout.VendorId,
                VendorOrderTotal = Payout.VendorOrderTotal,
                ShippingCharge = Payout.ShippingCharge,
                PayoutStatusName = Payout.PayoutStatus.ToString(),
            };

            if (Payout.PayoutStatus == PayoutStatus.Cancelled)
            {
                model.CommissionAmount = 0;
                model.PayoutAmount = 0;
                model.VendorOrderTotal = 0;
            }
            else if (_orderService != null)
            {
                var order = _orderService.GetOrderById(Payout.OrderId);
                model.OrderDate = order.CreatedOnUtc;
                /*
                               var vendorItemTotalExShipping = Payout.VendorOrderTotal - model.ShippingCharge;
                               var vendorItemTotalOriginal = (vendorItemTotalExShipping * 100) / (100 + Payout.CommissionPercentage);
                               decimal commission = vendorItemTotalExShipping - vendorItemTotalOriginal;
                               model.CommissionAmount = commission;
                               model.PayoutAmount = vendorItemTotalOriginal;*/

                model.PayoutAmount = GetPayoutAmount(Payout.VendorOrderTotal, Payout.CommissionPercentage);
                model.CommissionAmount = model.VendorOrderTotal - model.PayoutAmount;
            }

            return model;
        }

        public static VendorReviewModel ToVendorReviewModel(this VendorReview Review, Order Order, Product Product)
        {
            var model = new VendorReviewModel()
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
                ProductName = Product.Name,
                CertifiedBuyerReview = Review.CertifiedBuyerReview,
                DisplayCertifiedBadge = Review.DisplayCertifiedBadge
            };
            return model;
        }
    }
}