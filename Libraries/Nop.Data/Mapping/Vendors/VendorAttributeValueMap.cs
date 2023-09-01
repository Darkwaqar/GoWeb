
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorAttributeValueMap : NopEntityTypeConfiguration<VendorAttributeValue>
    {

        public VendorAttributeValueMap()
        {
            this.ToTable("VendorAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);
            this.Property(cav => cav.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(cav => cav.VendorAttribute)
                .WithMany(ca => ca.VendorAttributeValues)
                .HasForeignKey(cav => cav.VendorAttributeId);
        }
    }
}
