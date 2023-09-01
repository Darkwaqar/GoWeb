using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Events;
using System;

namespace Nop.Services.Vendors
{
    public class OrderCancelledEventConsumer : IConsumer<OrderCancelledEvent>
    {
        private IVendorService _extendedVendorService;
        private TaxSettings _taxSettings;

        public OrderCancelledEventConsumer(IVendorService extendedVendorService, TaxSettings taxSettings)
        {
            this._extendedVendorService = extendedVendorService;
            this._taxSettings = taxSettings;
        }
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            var order = eventMessage.Order;
            var payouts = _extendedVendorService.GetVendorPayoutsByOrder(order.Id);

            //mark each payout as cancelled as order has been cancelled
            foreach (var p in payouts)
            {
                p.PayoutStatus = PayoutStatus.Cancelled;
                p.Remarks += " (Order cancelled on " + DateTime.Now.ToString("dd MMM yyyy") + ")";
                _extendedVendorService.SaveVendorPayout(p);
            }


        }
    }
}
