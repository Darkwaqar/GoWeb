using Nop.Core.Domain.Appointments;

namespace Nop.Data.Mapping.Appointments
{
    public partial class AppointmentAttributeValueMap : NopEntityTypeConfiguration<AppointmentAttributeValue>
    {
        public AppointmentAttributeValueMap()
        {
            this.ToTable("AppointmentAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);
            this.Property(cav => cav.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(cav => cav.AppointmentAttribute)
                .WithMany(ca => ca.AppointmentAttributeValues)
                .HasForeignKey(cav => cav.AppointmentAttributeId);
        }
    }
}
