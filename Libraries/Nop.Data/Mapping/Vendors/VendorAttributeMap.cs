using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorAttributeMap : NopEntityTypeConfiguration<VendorAttribute>
    {
        public VendorAttributeMap()
        {
            this.ToTable("VendorAttribute");
            this.HasKey(v => v.Id);
            this.Property(v => v.Name).HasMaxLength(400).IsRequired();

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}
