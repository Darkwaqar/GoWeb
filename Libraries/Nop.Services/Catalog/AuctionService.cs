using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Auction service
    /// </summary>
    public partial class AuctionService : IAuctionService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";

        #endregion

        #region Fields
        private readonly IRepository<Bid> _bidRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IRepository<Product> _productRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkflowMessageService _workflowMessageService;
        #endregion

        #region Ctor
        public AuctionService(IRepository<Bid> bidRepository,
            IEventPublisher eventPublisher,
            IProductService productService,
            IRepository<Product> productRepository,
            ICacheManager cacheManager,
            IWorkflowMessageService workflowMessageService)
        {
            this._bidRepository = bidRepository;
            this._eventPublisher = eventPublisher;
            this._productService = productService;
            this._productRepository = productRepository;
            this._cacheManager = cacheManager;
            this._workflowMessageService = workflowMessageService;
        }
        #endregion
        public virtual void DeleteBid(Bid bid)
        {
            if (bid == null)
                throw new ArgumentNullException("bid");

            _bidRepository.Delete(bid);
            _eventPublisher.EntityDeleted(bid);

            var productToUpdate = _productService.GetProductById(bid.ProductId);
            var _bid = GetBidsByProductId(bid.ProductId);
            var highestBid = _bid.OrderByDescending(x => x.Amount).FirstOrDefault();
            if (productToUpdate != null)
            {
                UpdateHighestBid(productToUpdate, highestBid != null ? highestBid.Amount : 0, highestBid != null ? highestBid.CustomerId : 0);
            }
        }

        public virtual Bid GetBid(int Id)
        {
            return _bidRepository.GetById(Id);
        }

        public virtual Bid GetLatestBid(int productId)
        {
            var query = _bidRepository.Table;
            query = query.Where(x => x.ProductId == productId).OrderByDescending(x => x.Date);

            var bid = query.FirstOrDefault();
            return bid;
        }

        public virtual IPagedList<Bid> GetBidsByProductId(int productId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _bidRepository.Table;
            query = query.Where(x => x.ProductId == productId).OrderByDescending(x => x.Date);

            var productbids = new PagedList<Bid>(query, pageIndex, pageSize);
            return productbids;
        }

        public virtual IPagedList<Bid> GetBidsByCustomerId(int customerId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _bidRepository.Table;
            query = query.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.Date);
            var productbids = new PagedList<Bid>(query, pageIndex, pageSize);
            return productbids;

        }

        public virtual void InsertBid(Bid bid)
        {
            if (bid == null)
                throw new ArgumentNullException("bid");

            _bidRepository.Insert(bid);
            _eventPublisher.EntityInserted(bid);
        }

        public virtual void UpdateBid(Bid bid)
        {
            if (bid == null)
                throw new ArgumentNullException("bid");

            _bidRepository.Update(bid);
            _eventPublisher.EntityUpdated(bid);
        }

        public virtual void UpdateHighestBid(Product product, decimal bid, int highestBidder)
        {
            product.HighestBid = bid;
            product.HighestBidder = highestBidder;

            _productRepository.Update(product);

            _eventPublisher.EntityUpdated(product);
            _cacheManager.Remove(string.Format(PRODUCTS_BY_ID_KEY, product.Id));
        }

        public virtual IList<Product> GetAuctionsToEnd()
        {
            var query = _productRepository.Table;
            query = query.Where(x => x.ProductType == ProductType.AuctionProduct &&
            !x.AuctionEnded && x.AvailableEndDateTimeUtc < DateTime.UtcNow);

            var products = query.ToList();
            return products;
        }

        public virtual void UpdateAuctionEnded(Product product, bool ended, bool enddate = false)
        {
            product.AuctionEnded = ended;
            product.UpdatedOnUtc = DateTime.UtcNow;
            if (enddate)
                product.AvailableEndDateTimeUtc = DateTime.UtcNow;

            _productRepository.Update(product);

            _eventPublisher.EntityUpdated(product);
            _cacheManager.Remove(string.Format(PRODUCTS_BY_ID_KEY, product.Id));
        }


        /// <summary>
        /// New bid
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="product"></param>
        /// <param name="store"></param>
        /// <param name="warehouseId"></param>
        /// <param name="language"></param>
        /// <param name="amount"></param>
        public virtual void NewBid(Customer customer, Product product, Store store, Language language, int warehouseId, decimal amount)
        {
            var latestbid = GetLatestBid(product.Id);
            InsertBid(new Bid
            {
                Date = DateTime.UtcNow,
                Amount = amount,
                CustomerId = customer.Id,
                ProductId = product.Id,
                StoreId = store.Id,
                WarehouseId = warehouseId,
            });

            if (latestbid != null)
            {
                if (latestbid.CustomerId != customer.Id)
                {
                    _workflowMessageService.SendOutBidCustomerNotification(product, language.Id, latestbid);
                }
            }
            product.HighestBid = amount;
            UpdateHighestBid(product, amount, customer.Id);
        }

        /// <summary>
        /// Cancel bid
        /// </summary>
        /// <param name="OrderId">OrderId</param>
        public virtual void CancelBidByOrder(int orderId)
        {
            var query = _bidRepository.Table;
            query = query.Where(x => x.OrderId == orderId);

            var bid = query.FirstOrDefault();
            if (bid != null)
            {
                query = query.Where(x => x.ProductId == bid.ProductId);
                foreach (var item in query)
                {
                    _bidRepository.Delete(item);
                }
                var product = _productService.GetProductById(bid.ProductId);
                if (product != null)
                {
                    UpdateHighestBid(product, 0, 0);
                    UpdateAuctionEnded(product, false);
                }
            }
        }
    }
}
