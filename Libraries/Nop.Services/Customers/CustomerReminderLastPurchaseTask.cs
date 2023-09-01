using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for Customer Reminder Last Purchase
    /// </summary>
    public partial class CustomerReminderLastPurchaseTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderLastPurchaseTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_LastPurchase();
        }
    }
}