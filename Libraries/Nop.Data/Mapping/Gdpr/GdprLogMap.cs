using Nop.Core.Domain.Gdpr;

namespace Nop.Data.Mapping.Customers
{
    public partial class GdprLogMap : NopEntityTypeConfiguration<GdprLog>
    {
        public GdprLogMap()
        {
            this.ToTable("GdprLog");
            this.HasKey(ca => ca.Id);
        }
    }
}
