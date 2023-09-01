using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Appointments;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Appointments
{
    /// <summary>
    /// Product service
    /// </summary>
    public partial class AppointmentService : IAppointmentService
    {
        private readonly IRepository<ProductAppointment> _productAppointmentRepository;
        private readonly IEventPublisher _eventPublisher;

        public AppointmentService(IRepository<ProductAppointment> productAppointmentRepository,
            IEventPublisher eventPublisher)
        {
            this._productAppointmentRepository = productAppointmentRepository;
            this._eventPublisher = eventPublisher;
        }

        #region Product Appointments


        /// <summary>
        /// Inserts a Appointment 
        /// </summary>
        /// <param name="productAppointment">Magazine</param>        
        public virtual void InsertProductAppointment(ProductAppointment productAppointment)
        {
            if (productAppointment == null)
                throw new ArgumentNullException("productAppointment");

            _productAppointmentRepository.Insert(productAppointment);

            //event notification
            _eventPublisher.EntityInserted(productAppointment);
        }

        /// <summary>
        /// Updates product Appointments
        /// </summary>
        /// <param name="productAppointments">Product Appointments</param>
        public virtual void UpdateProductAppointment(ProductAppointment productAppointment)
        {
            if (productAppointment == null)
                throw new ArgumentNullException("productAppointment");

            _productAppointmentRepository.Update(productAppointment);

            //event notification
            _eventPublisher.EntityUpdated(productAppointment);
        }

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
        public virtual IPagedList<ProductAppointment> GetAllProductAppointments(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int productId = 0, int vendorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _productAppointmentRepository.Table;
            if (approved.HasValue)
                query = query.Where(pr => pr.IsApproved == approved);
            if (customerId > 0)
                query = query.Where(pr => pr.CustomerId == customerId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (storeId > 0)
                query = query.Where(pr => pr.StoreId == storeId);
            if (productId > 0)
                query = query.Where(pr => pr.ProductId == productId);
            if (vendorId > 0)
                query = query.Where(pr => pr.Product.VendorId == vendorId);

            query = query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var productAppointments = new PagedList<ProductAppointment>(query, pageIndex, pageSize);

            return productAppointments;
        }

        /// <summary>
        /// Gets product Appointment
        /// </summary>
        /// <param name="productAppointmentId">Product Appointment identifier</param>
        /// <returns>Product Appointment</returns>
        public virtual ProductAppointment GetProductAppointmentById(int productAppointmentId)
        {
            if (productAppointmentId == 0)
                return null;

            return _productAppointmentRepository.GetById(productAppointmentId);
        }

        /// <summary>
        /// Get product Appointments by identifiers
        /// </summary>
        /// <param name="productAppointmentIds">Product Appointment identifiers</param>
        /// <returns>Product Appointments</returns>
        public virtual IList<ProductAppointment> GetProducAppointmentsByIds(int[] productAppointmentIds)
        {
            if (productAppointmentIds == null || productAppointmentIds.Length == 0)
                return new List<ProductAppointment>();

            var query = from pr in _productAppointmentRepository.Table
                        where productAppointmentIds.Contains(pr.Id)
                        select pr;
            var productAppointments = query.ToList();
            //sort by passed identifiers
            var sortedProductAppointments = new List<ProductAppointment>();
            foreach (int id in productAppointmentIds)
            {
                var productAppointment = productAppointments.Find(x => x.Id == id);
                if (productAppointment != null)
                    sortedProductAppointments.Add(productAppointment);
            }
            return sortedProductAppointments;
        }

        /// <summary>
        /// Deletes a product Appointment
        /// </summary>
        /// <param name="productAppointment">Product Appointment</param>
        public virtual void DeleteProductAppointment(ProductAppointment productAppointment)
        {
            if (productAppointment == null)
                throw new ArgumentNullException("productAppointment");

            _productAppointmentRepository.Delete(productAppointment);

            //event notification
            _eventPublisher.EntityDeleted(productAppointment);
        }

        /// <summary>
        /// Deletes product Appointments
        /// </summary>
        /// <param name="productAppointments">Product Appointments</param>
        public virtual void DeleteProductAppointments(IList<ProductAppointment> productAppointments)
        {
            if (productAppointments == null)
                throw new ArgumentNullException("productAppointments");

            _productAppointmentRepository.Delete(productAppointments);

            //event notification
            foreach (var productAppointment in productAppointments)
            {
                _eventPublisher.EntityDeleted(productAppointment);
            }
        }

       

        #endregion
    }
}
