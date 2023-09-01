using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorMappedCategoryMap : NopEntityTypeConfiguration<VendorMappedCategory>
    {
        public VendorMappedCategoryMap()
        {
            this.ToTable("Vendor_Mapped_Category");
            this.HasKey(vmc => vmc.Id);

            this.HasRequired(vmc => vmc.Category)
               .WithMany()
               .HasForeignKey(vmc => vmc.CategoryId);


            this.HasRequired(vmc => vmc.Vendor)
                .WithMany(p => p.VendorMappedCategories)
                .HasForeignKey(vmc => vmc.VendorId);
        }
    }
}
