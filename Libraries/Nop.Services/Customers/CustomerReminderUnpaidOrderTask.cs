using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for Customer Unpaid Order
    /// </summary>
    public partial class CustomerReminderUnpaidOrderTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderUnpaidOrderTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_UnpaidOrder();
        }
    }
}