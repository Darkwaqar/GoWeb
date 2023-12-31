﻿using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Product tag service interface
    /// </summary>
    public partial interface ICustomerTagService
    {
        /// <summary>
        /// Delete a customer tag
        /// </summary>
        /// <param name="productTag">Customer tag</param>
        void DeleteCustomerTag(CustomerTag customerTag);

        /// <summary>
        /// Gets all customer tags
        /// </summary>
        /// <returns>Customer tags</returns>
        IList<CustomerTag> GetAllCustomerTags();

        /// <summary>
        /// Gets all customer for tag id
        /// </summary>
        /// <returns>Customers</returns>
        IPagedList<Customer> GetCustomersByTag(int customerTagId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// Gets customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag identifier</param>
        /// <returns>Product tag</returns>
        CustomerTag GetCustomerTagById(int customerTagId);

        /// <summary>
        /// Gets customer tag by name
        /// </summary>
        /// <param name="name">Customer tag name</param>
        /// <returns>Customer tag</returns>
        CustomerTag GetCustomerTagByName(string name);

        /// <summary>
        /// Gets customer tags search by name
        /// </summary>
        /// <param name="name">Customer tags name</param>
        /// <returns>Customer tags</returns>
        IList<CustomerTag> GetCustomerTagsByName(string name);

        /// <summary>
        /// Inserts a customer tag
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        void InsertCustomerTag(CustomerTag customerTag);

        /// <summary>
        /// Updates the customer tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        void UpdateCustomerTag(CustomerTag customerTag);

        /// <summary>
        /// Get number of customers
        /// </summary>
        /// <param name="customerTagId">Customer tag identifier</param>
        /// <returns>Number of products</returns>
        int GetCustomerCount(int customerTagId);

        /// <summary>
        /// Gets customer tag products for customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag id</param>
        /// <returns>Customer tag products</returns>
        IList<CustomerTagProduct> GetCustomerTagProducts(int customerTagId);

        /// <summary>
        /// Gets customer tag products for customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag id</param>
        /// <param name="productId">Product id</param>
        /// <returns>Customer tag product</returns>
        CustomerTagProduct GetCustomerTagProduct(int customerTagId, int productId);

        /// <summary>
        /// Gets customer tag product
        /// </summary>
        /// <param name="Id">id</param>
        /// <returns>Customer tag product</returns>
        CustomerTagProduct GetCustomerTagProductById(int id);

        /// <summary>
        /// Inserts a customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        void InsertCustomerTagProduct(CustomerTagProduct customerTagProduct);

        /// <summary>
        /// Updates the customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        void UpdateCustomerTagProduct(CustomerTagProduct customerTagProduct);

        /// <summary>
        /// Delete a customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        void DeleteCustomerTagProduct(CustomerTagProduct customerTagProduct);

    }
}
