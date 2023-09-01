using Nop.Core.Domain.Banners;

namespace Nop.Data.Mapping.Banners
{
    public partial class BannerMap : NopEntityTypeConfiguration<Banner>
    {
        public BannerMap()
        {
            this.ToTable("Banner");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
            this.Property(ea => ea.CreatedOnUtc).IsRequired();
        }
    }
}
