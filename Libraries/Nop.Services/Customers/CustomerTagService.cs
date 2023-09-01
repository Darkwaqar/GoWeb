using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer tag service
    /// </summary>
    public partial class CustomerTagService : ICustomerTagService
    {
        #region Fields

        private readonly IRepository<CustomerTag> _customerTagRepository;
        private readonly IRepository<CustomerTagProduct> _customerTagProductRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer tag Id?
        /// </remarks>
        private const string CUSTOMERTAGPRODUCTS_ROLE_KEY = "Nop.customertagproducts.tag-{0}";

        private const string PRODUCTS_CUSTOMER_TAG = "Nop.product.ct";

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public CustomerTagService(IRepository<CustomerTag> customerTagRepository,
            IRepository<CustomerTagProduct> customerTagProductRepository,
            IRepository<Customer> customerRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager
            )
        {
            this._customerTagRepository = customerTagRepository;
            this._customerTagProductRepository = customerTagProductRepository;
            this._eventPublisher = eventPublisher;
            this._customerRepository = customerRepository;
            this._cacheManager = cacheManager;
        }

        #endregion

        /// <summary>
        /// Gets all customer for tag id
        /// </summary>
        /// <returns>Customers</returns>
        public virtual IPagedList<Customer> GetCustomersByTag(int customerTagId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var customerTag = GetCustomerTagById(customerTagId);

            if (customerTag == null)
                throw new ArgumentNullException("productTag");

            var query = from c in _customerRepository.Table
                        where c.CustomerTags.Contains(customerTag)
                        select c;

            return new PagedList<Customer>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Delete a customer tag
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        public virtual void DeleteCustomerTag(CustomerTag customerTag)
        {
            if (customerTag == null)
                throw new ArgumentNullException("productTag");

            var query = from c in _customerRepository.Table
                        where c.CustomerTags.Contains(customerTag)
                        select c;

            foreach (var item in query)
            {
                item.RemoveCustomerTag(customerTag);
               
            }

            _customerTagRepository.Delete(customerTag);

            //event notification
            _eventPublisher.EntityDeleted(customerTag);
        }

        /// <summary>
        /// Gets all customer tags
        /// </summary>
        /// <returns>Customer tags</returns>
        public virtual IList<CustomerTag> GetAllCustomerTags()
        {
            var query = _customerTagRepository.Table;
            return query.ToList();
        }

        /// <summary>
        /// Gets customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag identifier</param>
        /// <returns>Customer tag</returns>
        public virtual CustomerTag GetCustomerTagById(int customerTagId)
        {
            return _customerTagRepository.GetById(customerTagId);
        }

        /// <summary>
        /// Gets customer tag by name
        /// </summary>
        /// <param name="name">Customer tag name</param>
        /// <returns>Customer tag</returns>
        public virtual CustomerTag GetCustomerTagByName(string name)
        {
            var query = from pt in _customerTagRepository.Table
                        where pt.Name == name
                        select pt;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets customer tags search by name
        /// </summary>
        /// <param name="name">Customer tags name</param>
        /// <returns>Customer tags</returns>
        public virtual IList<CustomerTag> GetCustomerTagsByName(string name)
        {
            var query = from pt in _customerTagRepository.Table
                        where pt.Name.ToLower().Contains(name.ToLower())
                        select pt;
            return query.ToList();
        }

        /// <summary>
        /// Inserts a customer tag
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        public virtual void InsertCustomerTag(CustomerTag customerTag)
        {
            if (customerTag == null)
                throw new ArgumentNullException("customerTag");

            _customerTagRepository.Insert(customerTag);

            //event notification
            _eventPublisher.EntityInserted(customerTag);
        }

        /// <summary>
        /// Updates the customer tag
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        public virtual void UpdateCustomerTag(CustomerTag customerTag)
        {
            if (customerTag == null)
                throw new ArgumentNullException("customerTag");

            _customerTagRepository.Update(customerTag);

            //event notification
            _eventPublisher.EntityUpdated(customerTag);
        }

        /// <summary>
        /// Get number of customers
        /// </summary>
        /// <param name="customerTagId">Customer tag identifier</param>
        /// <returns>Number of customers</returns>
        public virtual int GetCustomerCount(int customerTagId)
        {
            var query = _customerRepository.Table.
                Where(x => x.CustomerTags.Select(y => y.Id).Contains(customerTagId)).
                GroupBy(p => p, (k, s) => new { Counter = s.Count() }).ToList();
            if (query.Count > 0)
                return query.FirstOrDefault().Counter;
            return 0;
        }

        #region Customer tag product


        /// <summary>
        /// Gets customer tag products for customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag id</param>
        /// <returns>Customer tag products</returns>
        public virtual IList<CustomerTagProduct> GetCustomerTagProducts(int customerTagId)
        {
            string key = string.Format(CUSTOMERTAGPRODUCTS_ROLE_KEY, customerTagId);
            return _cacheManager.Get(key, () =>
           {
               var query = from cr in _customerTagProductRepository.Table
                           where (cr.CustomerTagId == customerTagId)
                           orderby cr.DisplayOrder
                           select cr;
               return query.ToList();
           });
        }

        /// <summary>
        /// Gets customer tag products for customer tag
        /// </summary>
        /// <param name="customerTagId">Customer tag id</param>
        /// <param name="productId">Product id</param>
        /// <returns>Customer tag product</returns>
        public virtual CustomerTagProduct GetCustomerTagProduct(int customerTagId, int productId)
        {
            var query = from cr in _customerTagProductRepository.Table
                        where cr.CustomerTagId == customerTagId && cr.ProductId == productId
                        orderby cr.DisplayOrder
                        select cr;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets customer tag product
        /// </summary>
        /// <param name="Id">id</param>
        /// <returns>Customer tag product</returns>
        public virtual CustomerTagProduct GetCustomerTagProductById(int id)
        {
            return _customerTagProductRepository.GetById(id);
        }

        /// <summary>
        /// Inserts a customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        public virtual void InsertCustomerTagProduct(CustomerTagProduct customerTagProduct)
        {
            if (customerTagProduct == null)
                throw new ArgumentNullException("customerTagProduct");

            _customerTagProductRepository.Insert(customerTagProduct);

            //clear cache
            _cacheManager.Remove(string.Format(CUSTOMERTAGPRODUCTS_ROLE_KEY, customerTagProduct.CustomerTagId));
            _cacheManager.RemoveByPattern(PRODUCTS_CUSTOMER_TAG);

            //event notification
            _eventPublisher.EntityInserted(customerTagProduct);
        }

        /// <summary>
        /// Updates the customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        public virtual void UpdateCustomerTagProduct(CustomerTagProduct customerTagProduct)
        {
            if (customerTagProduct == null)
                throw new ArgumentNullException("customerTagProduct");

            _customerTagProductRepository.Update(customerTagProduct);

            //clear cache
            _cacheManager.Remove(string.Format(CUSTOMERTAGPRODUCTS_ROLE_KEY, customerTagProduct.CustomerTagId));
            _cacheManager.RemoveByPattern(PRODUCTS_CUSTOMER_TAG);

            //event notification
            _eventPublisher.EntityUpdated(customerTagProduct);
        }

        /// <summary>
        /// Delete a customer tag product
        /// </summary>
        /// <param name="customerTagProduct">Customer tag product</param>
        public virtual void DeleteCustomerTagProduct(CustomerTagProduct customerTagProduct)
        {
            if (customerTagProduct == null)
                throw new ArgumentNullException("customerTagProduct");

            _customerTagProductRepository.Delete(customerTagProduct);

            //clear cache
            _cacheManager.Remove(string.Format(CUSTOMERTAGPRODUCTS_ROLE_KEY, customerTagProduct.CustomerTagId));
            _cacheManager.RemoveByPattern(PRODUCTS_CUSTOMER_TAG);
            //event notification
            _eventPublisher.EntityDeleted(customerTagProduct);
        }

        #endregion

    }
}
