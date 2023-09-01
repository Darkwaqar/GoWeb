namespace Nop.Core.Domain.Appointments
{
    public class ProductAppointmentApprovedEvent
    {
        public ProductAppointmentApprovedEvent(ProductAppointment productAppointment)
        {
            this.ProductAppointment = productAppointment;
        }

        /// <summary>
        /// Product review
        /// </summary>
        public ProductAppointment ProductAppointment { get; private set; }
    }
}
