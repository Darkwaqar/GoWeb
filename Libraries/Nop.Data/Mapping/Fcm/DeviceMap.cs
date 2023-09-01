using Nop.Core.Domain.Fcm;

namespace Nop.Data.Mapping.Fcm
{
    public partial class DeviceMap : NopEntityTypeConfiguration<Device>
    {
        public DeviceMap()
        {
            this.ToTable("Devices");
            this.HasKey(a => a.Id);
            this.Property(a => a.Latitude).HasPrecision(18, 9);
            this.Property(a => a.Longitude).HasPrecision(18, 9);
        }
    }
}
