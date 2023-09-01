using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a Customer ActionType
    /// </summary>
    public partial class CustomerActionType : BaseEntity
    {
        public string SystemKeyword { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string ConditionType { get; set; }

    }
}
