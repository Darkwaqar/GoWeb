using Nop.Core.Domain.Localization;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tasks;
using System.Linq;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Represents a task end auctions
    /// </summary>
    public partial class EndAuctionsTask : ITask
    {
        private readonly IAuctionService _auctionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public EndAuctionsTask(IAuctionService auctionService,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationService,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            IProductService productService,
            ILogger logger)
        {
            this._auctionService = auctionService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationService;
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
            this._productService = productService;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var auctionsToEnd = _auctionService.GetAuctionsToEnd();
            foreach (var auctionToEnd in auctionsToEnd)
            {
                var bid = (_auctionService.GetBidsByProductId(auctionToEnd.Id)).OrderByDescending(x => x.Amount).FirstOrDefault();
                if (bid == null)
                {
                    _auctionService.UpdateAuctionEnded(auctionToEnd, true);
                    _workflowMessageService.SendAuctionEndedStoreOwnerNotification(auctionToEnd, _localizationSettings.DefaultAdminLanguageId, null);
                    continue;
                }
                var customer = _customerService.GetCustomerById(bid.CustomerId);
                var product = _productService.GetProductById(bid.ProductId);
                var warnings = _shoppingCartService.AddToCart(customer, product, Core.Domain.Orders.ShoppingCartType.Auctions,
                    bid.StoreId, customerEnteredPrice: bid.Amount);

                if (!warnings.Any())
                {
                    bid.Win = true;
                    _auctionService.UpdateBid(bid);
                    _workflowMessageService.SendAuctionEndedStoreOwnerNotification(auctionToEnd, _localizationSettings.DefaultAdminLanguageId, bid);
                    _workflowMessageService.SendAuctionEndedCustomerNotificationWin(auctionToEnd, 0, bid);
                    _workflowMessageService.SendAuctionEndedCustomerNotificationLost(auctionToEnd, 0, bid);
                    _auctionService.UpdateAuctionEnded(auctionToEnd, true);
                }
                else
                {
                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, $"EndAuctionTask - Product {auctionToEnd.Name}", string.Join(",", warnings.ToArray()));
                }
            }
        }
    }
}
