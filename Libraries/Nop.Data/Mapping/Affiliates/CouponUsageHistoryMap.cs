
using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    public partial class CouponUsageHistoryMap : NopEntityTypeConfiguration<CouponUsageHistory>
    {
        public CouponUsageHistoryMap()
        {
            this.ToTable("CouponUsageHistory");
            this.HasKey(gcuh => gcuh.Id);
            this.Property(gcuh => gcuh.UsedValue).HasPrecision(18, 4);
            //this.Property(gcuh => gcuh.UsedValueInCustomerCurrency).HasPrecision(18, 4);

            this.HasRequired(gcuh => gcuh.Coupon)
                .WithMany(gc => gc.CouponUsageHistory)
                .HasForeignKey(gcuh => gcuh.CouponId);

        }
    }
}