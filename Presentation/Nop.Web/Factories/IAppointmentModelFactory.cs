using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Appointment;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the appointment model factory
    /// </summary>
    public partial interface IAppointmentModelFactory
    {

        /// <summary>
        /// Prepare the product Appointments model
        /// </summary>
        /// <param name="model">Product Appointments model</param>
        /// <param name="product">Product</param>
        /// <returns>Product Appointments model</returns>
        ProductAppointmentsModel PrepareProductAppointmentsModel(ProductAppointmentsModel model, Product product);

        /// <summary>
        /// Prepare the customer product Appointments model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer product Appointments model</returns>
        CustomerProductAppointmentsModel PrepareCustomerProductAppointmentsModel(int? page);
    }
}
