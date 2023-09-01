using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;

namespace Nop.Web.Areas.Mservices.Models.Order
{
    public partial class OrderListingModel
    {

        public int Id { get; set; }
        public string CustomOrderNumber { get; set; }
        public string OrderTotal { get; set; }
        public bool IsReturnRequestAllowed { get; set; }
        public OrderStatus OrderStatusEnum { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Image { get; set; }
    }

    public partial class OrderListing
    {
        public OrderListing()
        {
            Orders = new List<OrderListingModel>();
        }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public List<OrderListingModel> Orders { get; set; }
    }
}