using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PoolCategoryMap : NopEntityTypeConfiguration<PollCategory>
    {
        public PoolCategoryMap()
        {
            this.ToTable("PoolCategory");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.MetaKeywords).HasMaxLength(400);
            this.Property(c => c.MetaTitle).HasMaxLength(400);
            this.Property(c => c.PageSizeOptions).HasMaxLength(200);
            this.Property(c => c.ShowOnHomePage).IsOptional();
            this.Property(c => c.VendorId).IsOptional();
        }
    }
}