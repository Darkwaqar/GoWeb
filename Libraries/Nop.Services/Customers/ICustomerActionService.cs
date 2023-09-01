using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Services.Customers
{
    public partial interface ICustomerActionService
    {

        /// <summary>
        /// Gets customer action
        /// </summary>
        /// <param name="id">Customer action identifier</param>
        /// <returns>Customer Action</returns>
        CustomerAction GetCustomerActionById(int id);

        /// <summary>
        /// Gets all customer actions
        /// </summary>
        /// <returns>Customer actions</returns>
        IList<CustomerAction> GetCustomerActions();

        /// <summary>
        /// Inserts a customer action
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        void InsertCustomerAction(CustomerAction customerAction);

        /// <summary>
        /// Delete a customer action
        /// </summary>
        /// <param name="customerAction">Customer action</param>
        void DeleteCustomerAction(CustomerAction customerAction);

        /// <summary>
        /// Updates the customer action
        /// </summary>
        /// <param name="customerTag">Customer tag</param>
        void UpdateCustomerAction(CustomerAction customerAction);

        /// <summary>
        /// Gets List of customer action type
        /// </summary>
        /// <returns>List of Customer Action type</returns>
        IList<CustomerActionType> GetCustomerActionType();

        /// <summary>
        /// Updates the customer action
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>List of Customer Action type</returns>
        CustomerActionType GetCustomerActionTypeById(int id);

        /// <summary>
        /// Gets List of customer action type
        /// </summary>
        /// <param name="customerActionId">customerActionId</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>PageList of Customer Action History</returns>
        IPagedList<CustomerActionHistory> GetAllCustomerActionHistory(int customerActionId, int pageIndex = 0, int pageSize = 2147483647);

        /// <summary>
        /// Update a customer action type
        /// </summary>
        /// <param name="customerActionType">Customer action type</param>
        void UpdateCustomerActionType(CustomerActionType customerActionType);


        /// <summary>
        /// Delete a action condition entitity
        /// </summary>
        /// <param name="actionConditionEntity">action condition Entity</param>
        void DeleteConditionEntitiy(ActionConditionEntity actionConditionEntity);
    }
}
