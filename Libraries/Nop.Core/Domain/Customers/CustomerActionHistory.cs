using System;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer action
    /// </summary>
    public partial class CustomerActionHistory : BaseEntity
    {
        public int CustomerActionId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreateDateUtc { get; set; }

    }
}
