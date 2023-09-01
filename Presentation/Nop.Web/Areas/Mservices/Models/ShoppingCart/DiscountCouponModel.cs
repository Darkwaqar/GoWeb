using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Areas.Mservices.Models.ShoppingCart
{
    public class DiscountCouponModel
    {
        public DiscountCouponModel()
        {
            DiscountBox = new DiscountBoxModel();
            GiftCardBox = new GiftCardBoxModel();
        }

        public DiscountBoxModel DiscountBox { get; set; }
        public GiftCardBoxModel GiftCardBox { get; set; }


        public partial class DiscountBoxModel 
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public bool IsApplied { get; set; }

            public class DiscountInfoModel
            {
                public string CouponCode { get; set; }
            }
        }

        public partial class GiftCardBoxModel 
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public bool IsApplied { get; set; }
        }

    }
}