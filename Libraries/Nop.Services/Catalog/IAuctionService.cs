using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using System.Collections.Generic;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Auction service interface
    /// </summary>
    public partial interface IAuctionService
    {
        /// <summary>
        /// Deletes bid
        /// </summary>
        /// <param name="bid"></param>
        void DeleteBid(Bid bid);

        /// <summary>
        /// Inserts bid
        /// </summary>
        /// <param name="bid"></param>
        void InsertBid(Bid bid);

        /// <summary>
        /// Updates bid
        /// </summary>
        /// <param name="bid"></param>
        void UpdateBid(Bid bid);

        /// <summary>
        /// Gets bids for product Id
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Bids</returns>
        IPagedList<Bid> GetBidsByProductId(int productId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets bids for Customer Id
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>Bids</returns>
        IPagedList<Bid> GetBidsByCustomerId(int customerId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets bid for specified Id
        /// </summary>
        /// <param name="Id">Id</param>
        /// <returns>Bid</returns>
        Bid GetBid(int Id);

        /// <summary>
        /// Get latest bid for product Id
        /// </summary>
        /// <param name="productId">productId</param>
        /// <returns>Bid</returns>
        Bid GetLatestBid(int productId);

        /// <summary>
        /// Updates highest bid
        /// </summary>
        /// <param name="product"></param>
        /// <param name="bid"></param>
        /// <param name="bidder"></param>
        void UpdateHighestBid(Product product, decimal bid, int bidder);

        /// <summary>
        /// Updates auction ended
        /// </summary>
        /// <param name="product"></param>
        /// <param name="ended"></param>
        /// <param name="enddate"></param>
        void UpdateAuctionEnded(Product product, bool ended, bool enddate = false);

        /// <summary>
        /// Gets auctions that have to be ended
        /// </summary>
        IList<Product> GetAuctionsToEnd();

        /// <summary>
        /// New bid
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="product"></param>
        /// <param name="store"></param>
        /// <param name="warehouseId"></param>
        /// <param name="language"></param>
        /// <param name="amount"></param>
        void NewBid(Customer customer, Product product, Store store, Language language, int warehouseId, decimal amount);

        /// <summary>
        /// Cancel bid for product
        /// </summary>
        /// <param name="orderId">order id</param>
        void CancelBidByOrder(int orderId);
    }
}
