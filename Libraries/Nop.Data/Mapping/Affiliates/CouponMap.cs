using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    public partial class CouponMap : NopEntityTypeConfiguration<Coupon>
    {
        public CouponMap()
        {
            this.ToTable("Coupon");
            this.HasKey(gc => gc.Id);

            this.Property(gc => gc.Amount).HasPrecision(18, 4);

            this.Ignore(gc => gc.CouponType);
        }
    }
}