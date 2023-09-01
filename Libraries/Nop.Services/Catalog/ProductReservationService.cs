using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product reservation service
    /// </summary>
    public partial class ProductReservationService : IProductReservationService
    {
        private readonly IRepository<ProductReservation> _productReservationRepository;
        private readonly IRepository<CustomerReservations> _customerReservationsRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;

        public ProductReservationService(IRepository<ProductReservation> productReservationRepository,
            IRepository<CustomerReservations> customerReservationsRepository,
            IEventPublisher eventPublisher,
            IWorkContext workContext)
        {
            this._productReservationRepository = productReservationRepository;
            this._customerReservationsRepository = customerReservationsRepository;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
        }

        /// <summary>
        /// Deletes a product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        public virtual void DeleteProductReservation(ProductReservation productReservation)
        {
            if (productReservation == null)
                throw new ArgumentNullException("productReservation");

            _productReservationRepository.Delete(productReservation);
            _eventPublisher.EntityDeleted(productReservation);
        }

        /// <summary>
        /// Gets product reservations for product Id
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Product reservations</returns>
        public virtual IPagedList<ProductReservation> GetProductReservationsByProductId(int productId, bool? showVacant, DateTime? date,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _productReservationRepository.Table;
            query = query.Where(x => x.ProductId == productId);
            if (showVacant.HasValue)
            {
                if (showVacant.Value)
                {
                    query = query.Where(x => x.OrderId == 0);
                }
                else
                {
                    query = query.Where(x => x.OrderId != 0);
                }
            }

            if (date.HasValue)
            {
                var min = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0, 0);
                var max = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 23, 59, 59, 999);

                query = query.Where(x => x.Date >= min && x.Date <= max);
            }

            query = query.OrderBy(x => x.Date);

            var productReservation = new PagedList<ProductReservation>(query, pageIndex, pageSize);
            return productReservation;
        }

        /// <summary>
        /// Adds product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        public virtual void InsertProductReservation(ProductReservation productReservation)
        {
            if (productReservation == null)
                throw new ArgumentNullException("productAttribute");

            _productReservationRepository.Insert(productReservation);
            _eventPublisher.EntityInserted(productReservation);
        }

        /// <summary>
        /// Updates product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        public virtual void UpdateProductReservation(ProductReservation productReservation)
        {
            if (productReservation == null)
                throw new ArgumentNullException("productAttribute");

            _productReservationRepository.Update(productReservation);
            _eventPublisher.EntityInserted(productReservation);
        }


        /// <summary>
        /// Adds customer reservations
        /// </summary>
        /// <param name="cr"></param>
        public virtual void InsertCustomerReservationsHelper(CustomerReservations cr)
        {
            if (cr == null)
                throw new ArgumentNullException("CustomerReservations");

            _customerReservationsRepository.Insert(cr);
            _eventPublisher.EntityInserted(cr);
        }

        /// <summary>
        /// Deletes customer reservations 
        /// </summary>
        /// <param name="cr"></param>
        public virtual void DeleteCustomerReservationsHelper(CustomerReservations cr)
        {
            if (cr == null)
                throw new ArgumentNullException("CustomerReservations");

            _customerReservationsRepository.Delete(cr);
            _eventPublisher.EntityDeleted(cr);
        }

        /// <summary>
        /// Gets product reservation for specified Id
        /// </summary>
        /// <param name="Id">Product Id</param>
        /// <returns>Product reservation</returns>
        public virtual ProductReservation GetProductReservation(int Id)
        {
            return _productReservationRepository.GetById(Id);
        }

        /// <summary>
        /// Cancel reservations by orderId 
        /// </summary>
        /// <param name="orderId"></param>
        public virtual void CancelReservationsByOrderId(int orderId)
        {
            if (orderId > 0)
            {
                var query = _productReservationRepository.Table;
                query = query.Where(x => x.OrderId == orderId);
                foreach (var item in query = query.Where(x => x.OrderId == orderId))
                {
                    item.OrderId = 0;
                    _productReservationRepository.Update(item);
                }
            }
        }


        /// <summary>
        /// Gets customer reservations  by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>CustomerReservationsHelper</returns>
        public virtual CustomerReservations GetCustomerReservationsHelperById(string Id)
        {
            return _customerReservationsRepository.GetById(Id);
        }

        /// <summary>
        /// Gets customer reservations 
        /// </summary>
        /// <returns>List<CustomerReservationsHelper></returns>
        public virtual IList<CustomerReservations> GetCustomerReservationsHelpers()
        {
            return _customerReservationsRepository.Table.Where(x => x.CustomerId == _workContext.CurrentCustomer.Id).ToList();
        }

        /// <summary>
        /// Gets customer reservations by Shopping Cart Item id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>List<CustomerReservationsHelper></returns>
        public virtual IList<CustomerReservations> GetCustomerReservationsHelperBySciId(int sciId)
        {
            return _customerReservationsRepository.Table.Where(x => x.ShoppingCartItemId == sciId).ToList();
        }
    }
}
