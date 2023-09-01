namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer tag product
    /// </summary>
    public partial class CustomerTagProduct : BaseEntity
    {

        /// <summary>
        /// Gets or sets the customer tag id
        /// </summary>
        public int CustomerTagId { get; set; }

        /// <summary>
        /// Gets or sets the product Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
