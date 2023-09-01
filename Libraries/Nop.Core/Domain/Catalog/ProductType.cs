namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product type
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// Simple
        /// </summary>
        SimpleProduct = 5,
        /// <summary>
        /// Grouped (product with variants)
        /// </summary>
        GroupedProduct = 10,
        /// <summary>
        /// Grouped (magazine with product)
        /// </summary>
        MagazineProduct = 15,

        /// <summary>
        /// Auction (Auction with product)
        /// </summary>
        AuctionProduct = 20,

        /// <summary>
        /// Reservation (Reservation with product)
        /// </summary>
        ReservationProduct = 25,
    }
}
