using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for Customer Register Customer
    /// </summary>
    public partial class CustomerReminderRegisteredCustomerTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderRegisteredCustomerTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_RegisteredCustomer();
        }
    }
}