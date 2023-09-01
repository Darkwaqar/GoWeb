using System;

namespace Nop.Core.Domain.Vendors
{
    public partial class VendorPayout : BaseEntity
    {
        public  int VendorId { get; set; }
        public  int OrderId { get; set; }
        public  decimal VendorOrderTotal { get; set; }
        public decimal CommissionPercentage { get; set; }
        public  PayoutStatus PayoutStatus { get; set; }
        public  DateTime? PayoutDate { get; set; }
        public  string Remarks { get; set; }
        public  decimal ShippingCharge { get; set; }
    }
}
