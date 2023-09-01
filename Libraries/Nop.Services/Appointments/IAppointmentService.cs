using Nop.Core;
using Nop.Core.Domain.Appointments;
using System;
using System.Collections.Generic;

namespace Nop.Services.Appointments
{
    public partial interface IAppointmentService
    {
        #region Product Appointments

        /// <summary>
        /// Inserts a product Appointments
        /// </summary>
        /// <param name="productAppointments">Product Appointments</param>
        void InsertProductAppointment(ProductAppointment productAppointment);
        
        /// <summary>
        /// Updates product Appointments
        /// </summary>
        /// <param name="productAppointments">Product Appointments</param>
        void UpdateProductAppointment(ProductAppointment productAppointment);

        /// <summary>
        /// Gets all product Appointments
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a Appointment); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or Appointment text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="productId">The product identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to products of this vendor); pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Appointments</returns>
        IPagedList<ProductAppointment> GetAllProductAppointments(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0, int vendorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets product Appointment
        /// </summary>
        /// <param name="productAppointmentId">Product Appointment identifier</param>
        /// <returns>Product Appointment</returns>
        ProductAppointment GetProductAppointmentById(int productAppointmentId);

        /// <summary>
        /// Get product Appointments by identifiers
        /// </summary>
        /// <param name="productAppointmentIds">Product Appointment identifiers</param>
        /// <returns>Product Appointments</returns>
        IList<ProductAppointment> GetProducAppointmentsByIds(int[] productAppointmentIds);

        /// <summary>
        /// Deletes a product Appointment
        /// </summary>
        /// <param name="productAppointment">Product Appointment</param>
        void DeleteProductAppointment(ProductAppointment productAppointment);

        /// <summary>
        /// Deletes product Appointments
        /// </summary>
        /// <param name="productAppointments">Product Appointments</param>
        void DeleteProductAppointments(IList<ProductAppointment> productAppointments);
        #endregion
    }
}
