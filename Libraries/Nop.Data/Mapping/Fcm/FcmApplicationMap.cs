using Nop.Core.Domain.Fcm;


namespace Nop.Data.Mapping.Fcm
{
    public partial class FcmApplicationMap : NopEntityTypeConfiguration<FcmApplication>
    {
        public FcmApplicationMap()
        {
            this.ToTable("FcmApplication");
            this.HasKey(d => d.Id);
            this.Property(d => d.Name).IsRequired().HasMaxLength(500);
            this.Property(d => d.Package).IsRequired().HasMaxLength(500);
        }
    }
}
