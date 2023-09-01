using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CategoryMap : NopEntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            this.ToTable("Category");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.MetaKeywords).HasMaxLength(400);
            this.Property(c => c.MetaTitle).HasMaxLength(400);
            this.Property(c => c.PriceRanges).HasMaxLength(400);
            this.Property(c => c.PageSizeOptions).HasMaxLength(200);
            this.Property(c => c.ShowOnHomePage).IsOptional();
            this.Property(c => c.VendorId).IsOptional();
            this.Property(c => c.Collection).IsOptional();
            this.Property(c => c.CollectionName).HasMaxLength(200);
            this.Property(c => c.CollectionDiscription).HasMaxLength(400);
            this.Property(c => c.CollectionPrictureId).IsOptional();
            this.Property(c => c.CollectionLogoId).IsOptional();
        }
    }
}