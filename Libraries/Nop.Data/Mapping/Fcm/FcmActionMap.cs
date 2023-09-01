using Nop.Core.Domain.Fcm;

namespace Nop.Data.Mapping.Fcm
{
    public partial class FcmActionMap : NopEntityTypeConfiguration<FcmAction>
    {
        public FcmActionMap()
        {
            this.ToTable("FcmAction");
            this.HasKey(d => d.Id);
            this.Property(d => d.Name).IsRequired().HasMaxLength(500);
        }
    }
}
