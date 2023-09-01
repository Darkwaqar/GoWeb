using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for Customer reminder abandonded cart 
    /// </summary>
    public partial class CustomerReminderAbandonedCartTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderAbandonedCartTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_AbandonedCart();
        }
    }
}