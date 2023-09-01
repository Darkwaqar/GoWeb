using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for customer Reminder birthday
    /// </summary>
    public partial class CustomerReminderBirthdayTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderBirthdayTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_Birthday();
        }
    }
}
