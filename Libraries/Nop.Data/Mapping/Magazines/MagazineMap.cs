using Nop.Core.Domain.Magazines;

namespace Nop.Data.Mapping.Magazines
{
    public partial class MagazineMap : NopEntityTypeConfiguration<Magazine>
    {
        public MagazineMap()
        {
            this.ToTable("Magazine");
            this.HasKey(a => a.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
        }
    }
}
