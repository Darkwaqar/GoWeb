namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a customer reservation helper to the product
    /// </summary>
    public partial class CustomerReservations : BaseEntity
    {
        /// <summary>
        /// Gets or sets customer identifier
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Gets or sets shopping cart identifier
        /// </summary>
        public int ShoppingCartItemId { get; set; }
        /// <summary>
        /// Gets or sets reservation identifier
        /// </summary>
        public int ReservationId { get; set; }
    }
}
