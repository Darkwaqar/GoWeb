namespace Nop.Core.Domain.Customers
{
    public partial class ActionConditionEntity : BaseEntity
    {
        public string EntityName { get; set; }
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }
    }
}
