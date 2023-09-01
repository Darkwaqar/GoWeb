using Nop.Core.Domain.Appointments;

namespace Nop.Data.Mapping.Appointments
{
    public partial class AppointmentMap : NopEntityTypeConfiguration<ProductAppointment>
    {
        public AppointmentMap()
        {
            this.ToTable("ProductAppointment");
            this.HasKey(pr => pr.Id);

            this.HasRequired(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductId);

            this.HasRequired(pr => pr.Customer)
                .WithMany()
                .HasForeignKey(pr => pr.CustomerId);

            this.HasRequired(pr => pr.Store)
                .WithMany()
                .HasForeignKey(pr => pr.StoreId);
        }
    }
}
