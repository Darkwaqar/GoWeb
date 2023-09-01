using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for customer last activity
    /// </summary>
    public partial class CustomerReminderLastActivityTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderLastActivityTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_LastActivity();
        }
    }
}