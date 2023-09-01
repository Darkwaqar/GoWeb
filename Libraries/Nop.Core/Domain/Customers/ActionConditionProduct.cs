using Nop.Core.Domain.Catalog;
using static Nop.Core.Domain.Customers.CustomerAction;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a action condition mapping
    /// </summary>
    public partial class ActionConditionProduct : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer action identifier
        /// </summary>
        public int CustomerActionId { get; set; }

        /// <summary>
        /// Gets or sets the customer action condition identifier
        /// </summary>
        public int CustomerActionConditionId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }
    }
}
