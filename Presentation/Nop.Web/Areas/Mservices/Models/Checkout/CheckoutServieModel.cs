using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Services.Payments;

namespace Nop.Web.Areas.Mservices.Models.Checkout
{
    public class CheckoutServieModel
    {
        public string RedirectUrl { get; internal set; }
        public PaymentMethodType PaymentMethodType { get; internal set; }
        public int OrderId { get; internal set; }
    }
}