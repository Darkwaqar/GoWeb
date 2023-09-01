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
    public partial class CustomerActionService : ICustomerActionService
    {
        #region Fields

        private const string CUSTOMER_ACTION_TYPE = "Nop.customer.action.type";

        private readonly IRepository<CustomerAction> _customerActionRepository;
        private readonly IRepository<CustomerActionType> _customerActionTypeRepository;
        private readonly IRepository<CustomerActionHistory> _customerActionHistoryRepository;
        private readonly IRepository<ActionConditionEntity> _actionConditionEntity;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public CustomerActionService(IRepository<CustomerAction> customerActionRepository,
            IRepository<CustomerActionType> customerActionTypeRepository,
            IRepository<CustomerActionHistory> customerActionHistoryRepository,
            IRepository<ActionConditionEntity> actionConditionEntity,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._customerActionRepository = customerActionRepository;
            this._customerActionTypeRepository = customerActionTypeRepository;
            this._customerActionHistoryRepository = customerActionHistoryRepository;
            this._actionConditionEntity = actionConditionEntity;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets customer action
        /// </summary>
        /// <param name="id">Customer action identifier</param>
        /// <returns>Customer Action</returns>
        public virtual CustomerAction GetCustomerActionById(int id)
        {
            return _customerActionRepository.GetById(id);
        }


        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer actions</returns>
        public virtual IList<CustomerAction> GetCustomerActions()
        {
            var query = _customerActionRepository.Table;
            return  query.ToList();
        }

        /// <summary>
        /// Inserts a customer action
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        public virtual void InsertCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction");

             _customerActionRepository.Insert(customerAction);

            //event notification
            _eventPublisher.EntityInserted(customerAction);

        }

        /// <summary>
        /// Delete a customer action
        /// </summary>
        /// <param name="customerAction">Customer action</param>
        public virtual void DeleteCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction");

             _customerActionRepository.Delete(customerAction);

            //event notification
            _eventPublisher.EntityDeleted(customerAction);

        }

        /// <summary>
        /// Updates the customer action
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        public virtual void UpdateCustomerAction(CustomerAction customerAction)
        {
            if (customerAction == null)
                throw new ArgumentNullException("customerAction");

             _customerActionRepository.Update(customerAction);

            //event notification
            _eventPublisher.EntityUpdated(customerAction);
        }

        #endregion

        #region Condition Type

        /// <summary>
        /// Gets List of customer action type
        /// </summary>
        /// <returns>List of Customer Action type</returns>
        public virtual IList<CustomerActionType> GetCustomerActionType()
        {
            var query = _customerActionTypeRepository.Table;
            return  query.ToList();
        }

        /// <summary>
        /// Updates the customer action
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>List of Customer Action type</returns>
        public virtual IPagedList<CustomerActionHistory> GetAllCustomerActionHistory(int customerActionId, int pageIndex = 0, int pageSize = 2147483647)
        {
            var query = from h in _customerActionHistoryRepository.Table
                        where h.CustomerActionId == customerActionId
                        select h;

            query = query.OrderByDescending(c => c.CreateDateUtc);
            return new PagedList<CustomerActionHistory>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets List of customer action type
        /// </summary>
        /// <param name="customerActionId">customerActionId</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>PageList of Customer Action History</returns>
        public virtual CustomerActionType GetCustomerActionTypeById(int id)
        {
            return _customerActionTypeRepository.GetById(id);
        }

        /// <summary>
        /// Update a customer action type
        /// </summary>
        /// <param name="customerActionType">Customer action type</param>
        public virtual void UpdateCustomerActionType(CustomerActionType customerActionType)
        {
            if (customerActionType == null)
                throw new ArgumentNullException("customerActionType");

             _customerActionTypeRepository.Update(customerActionType);

            //clear cache
             _cacheManager.Remove(CUSTOMER_ACTION_TYPE);
            //event notification
            _eventPublisher.EntityUpdated(customerActionType);
        }

        #endregion


        /// <summary>
        /// Delete a action condition entitity
        /// </summary>
        /// <param name="actionConditionEntity">action condition Entity</param>
        public virtual void DeleteConditionEntitiy(ActionConditionEntity actionConditionEntity)
        {
            if (actionConditionEntity == null)
                throw new ArgumentNullException("actionConditionEntity");

            _actionConditionEntity.Delete(actionConditionEntity);

            //event notification
            _eventPublisher.EntityDeleted(actionConditionEntity);
        }
    }
}
