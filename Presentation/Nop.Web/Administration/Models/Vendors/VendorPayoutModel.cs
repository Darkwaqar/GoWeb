using Nop.Core.Domain.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Admin.Models.Vendors
{
    public partial class VendorPayoutModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Vendors.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.VendorOrderTotal")]
        public decimal VendorOrderTotal { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CommissionPercentage")]
        public decimal CommissionPercentage { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PayoutStatus")]
        public PayoutStatus PayoutStatus { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PayoutStatusName")]
        public string PayoutStatusName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PayoutDate")]
        public DateTime? PayoutDate { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.Remarks")]
        public string Remarks { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.PayoutAmount")]
        public decimal PayoutAmount { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CommissionAmount")]
        public decimal CommissionAmount { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.ShippingCharge")]
        public decimal ShippingCharge { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.CurrencySymbol")]
        public string CurrencySymbol { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Fields.OrderDate")]
        public DateTime OrderDate { get; set; }
    }
}