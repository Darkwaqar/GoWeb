using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorPayoutMap : NopEntityTypeConfiguration<VendorPayout>
    {
        public VendorPayoutMap()
        {
            this.ToTable("VendorPayouts");
            this.HasKey(m => m.Id);
            this.Property(m => m.VendorId);
            this.Property(m => m.OrderId);
            this.Property(m => m.VendorOrderTotal);
            this.Property(m => m.CommissionPercentage);
            this.Property(m => m.PayoutStatus);
            this.Property(m => m.PayoutDate);
            this.Property(m => m.Remarks);
            this.Property(m => m.ShippingCharge);
        }
    }
}
   
