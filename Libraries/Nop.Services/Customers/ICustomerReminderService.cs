using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Services.Customers
{
    public partial interface ICustomerReminderService
    {
        /// <summary>
        /// Gets customer reminder
        /// </summary>
        /// <param name="id">Customer reminder identifier</param>
        /// <returns>Customer reminder</returns>
        CustomerReminder GetCustomerReminderById(int id);


        /// <summary>
        /// Gets all customer reminders
        /// </summary>
        /// <returns>Customer reminders</returns>
        IList<CustomerReminder> GetCustomerReminders();

        /// <summary>
        /// Inserts a customer reminder
        /// </summary>
        /// <param name="CustomerReminder">Customer reminder</param>
        void InsertCustomerReminder(CustomerReminder customerReminder);

        /// <summary>
        /// Delete a customer reminder
        /// </summary>
        /// <param name="customerReminder">Customer reminder</param>
        void DeleteCustomerReminder(CustomerReminder customerReminder);

        /// <summary>
        /// Updates the customer reminder
        /// </summary>
        /// <param name="CustomerReminder">Customer reminder</param>
        void UpdateCustomerReminder(CustomerReminder customerReminder);

        /// <summary>
        /// Gets customer reminders history for reminder
        /// </summary>
        /// <returns>SerializeCustomerReminderHistory</returns>
        IPagedList<SerializeCustomerReminderHistory> GetAllCustomerReminderHistory(int customerReminderId, int pageIndex = 0, int pageSize = 2147483647);

        /// <summary>
        /// Run task Abandoned Cart
        /// </summary>
        void Task_AbandonedCart(int id = 0);
        void Task_RegisteredCustomer(int id = 0);
        void Task_LastActivity(int id = 0);
        void Task_LastPurchase(int id = 0);
        void Task_Birthday(int id = 0);
        void Task_CompletedOrder(int id = 0);
        void Task_UnpaidOrder(int id = 0);

        /// <summary>
        /// Delete a reminder condition entitity
        /// </summary>
        /// <param name="reminderConditionEntity">reminder condition Entity</param>
        void DeleteConditionEntitiy(ReminderConditionEntity reminderConditionEntity);
    }
}
