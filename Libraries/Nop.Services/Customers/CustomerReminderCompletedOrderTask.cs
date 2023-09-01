using Nop.Services.Tasks;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Represents a task for customer Completed Order
    /// </summary>
    public partial class CustomerReminderCompletedOrderTask : ITask
    {
        private readonly ICustomerReminderService _customerReminderService;

        public CustomerReminderCompletedOrderTask(ICustomerReminderService customerReminderService)
        {
            this._customerReminderService = customerReminderService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            _customerReminderService.Task_CompletedOrder();
        }
    }
}
