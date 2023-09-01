using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product reservation service interface
    /// </summary>
    public partial interface IProductReservationService
    {
        /// <summary>
        /// Deletes a product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        void DeleteProductReservation(ProductReservation productReservation);

        /// <summary>
        /// Adds product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        void InsertProductReservation(ProductReservation productReservation);

        /// <summary>
        /// Updates product reservation
        /// </summary>
        /// <param name="productReservation">Product reservation</param>
        void UpdateProductReservation(ProductReservation productReservation);


        /// <summary>
        /// Adds customer reservations
        /// </summary>
        /// <param name="cr"></param>
        void InsertCustomerReservationsHelper(CustomerReservations cr);

        /// <summary>
        /// Gets product reservations for product Id
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Product reservations</returns>
        IPagedList<ProductReservation> GetProductReservationsByProductId(int productId, bool? showVacant, DateTime? date,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets product reservation for specified Id
        /// </summary>
        /// <param name="Id">Product Id</param>
        /// <returns>Product reservation</returns>
        ProductReservation GetProductReservation(int Id);

        /// <summary>
        /// Cancel reservations by orderId 
        /// </summary>
        /// <param name="orderId"></param>
        void CancelReservationsByOrderId(int orderId);



        /// <summary>
        /// Gets customer reservations  by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>CustomerReservationsHelper</returns>
        CustomerReservations GetCustomerReservationsHelperById(string Id);


        /// <summary>
        /// Gets customer reservations 
        /// </summary>
        /// <returns>List<CustomerReservationsHelper></returns>
        IList<CustomerReservations> GetCustomerReservationsHelpers();


        /// <summary>
        /// Gets customer reservations by Shopping Cart Item id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>List<CustomerReservationsHelper></returns>
        IList<CustomerReservations> GetCustomerReservationsHelperBySciId(int sciId);
        
    }
}
