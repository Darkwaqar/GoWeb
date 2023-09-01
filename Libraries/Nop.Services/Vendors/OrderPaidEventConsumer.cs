using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IVendorService _vendorService;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        public OrderPaidEventConsumer(IVendorService vendorService, TaxSettings taxSettings, VendorSettings vendorSettings)
        {
            this._vendorService = vendorService;
            this._taxSettings = taxSettings;
            this._vendorSettings = vendorSettings;
        }
        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            var order = eventMessage.Order;
            var vendorIds = order.OrderItems.Select(m => m.Product.VendorId).Distinct();

            //to store the vendor based order items 
            var vendorItems = new Dictionary<int, IList<OrderItem>>();
            //to store the vendors in a dictionary because there may be more than one vendors in one order
            var vendorsExtended = new Dictionary<int, Vendor>();

            foreach (var vendorId in vendorIds)
            {
                vendorItems.Add(vendorId, order.OrderItems.Where(m => m.Product.VendorId == vendorId).ToList());
                var vendor = _vendorService.GetVendorById(vendorId);
                vendorsExtended.Add(vendorId, vendor);
            }

            foreach (var vendorItem in vendorItems)
            {
                var vendorId = vendorItem.Key;
                var orderItems = vendorItem.Value;

                var orderItemTotal = decimal.Zero;
                var discountTotal = decimal.Zero;
                if (_taxSettings.PricesIncludeTax)
                {
                    orderItemTotal = orderItems.Sum(m => m.PriceInclTax);
                    discountTotal = orderItems.Sum(m => m.DiscountAmountInclTax);
                }
                else
                {
                    orderItemTotal = orderItems.Sum(m => m.PriceExclTax);
                    discountTotal = orderItems.Sum(m => m.DiscountAmountExclTax);
                }
                orderItemTotal = orderItemTotal - discountTotal;

                //create a new payout for each vendor
                var vendorPayout = new VendorPayout
                {
                    VendorId = vendorId,
                    CommissionPercentage =
                        vendorsExtended[vendorId] == null
                            ? _vendorSettings.DefaultCommissionPercentage
                            : vendorsExtended[vendorId].CommissionPercentage,
                    OrderId = order.Id,
                    PayoutDate = null,
                    Remarks = "",
                    VendorOrderTotal = orderItemTotal,
                    PayoutStatus = PayoutStatus.Pending,
                    ShippingCharge =
                        (vendorsExtended[vendorId] == null
                            ? _vendorSettings.DefaultShippingCharge
                            : vendorsExtended[vendorId].ShippingCharge) * orderItems.Count
                };

                _vendorService.SaveVendorPayout(vendorPayout);
            }

        }
    }
}
