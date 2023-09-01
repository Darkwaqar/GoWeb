using Nop.Core.Domain.Gdpr;

namespace Nop.Data.Mapping.Gdpr
{
    public partial class GdprConsentMap : NopEntityTypeConfiguration<GdprConsent>
    {
        public GdprConsentMap()
        {
            this.ToTable("GdprConsent");
            this.HasKey(ca => ca.Id);
        }
    }
}
