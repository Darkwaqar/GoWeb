using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductReservationMap : NopEntityTypeConfiguration<ProductReservation>
    {
        public ProductReservationMap()
        {
            this.ToTable("ProductReservation");
            this.HasKey(x => x.Id);
        }
    }
}
