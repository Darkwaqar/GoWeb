using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class BidMap : NopEntityTypeConfiguration<Bid>
    {
        public BidMap()
        {
            this.ToTable("Bid");
            this.HasKey(x => x.Id);

        }
    }
}
