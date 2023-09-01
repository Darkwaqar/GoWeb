using Nop.Core.Domain.Appointments;

namespace Nop.Data.Mapping.Appointments
{
    public partial class AppointmentAttributeMap : NopEntityTypeConfiguration<AppointmentAttribute>
    {
        public AppointmentAttributeMap()
        {
            this.ToTable("AppointmentAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}
