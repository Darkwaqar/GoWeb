using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Media;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the order model factory
    /// </summary>
    public partial interface IOrderModelFactory
    {
        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        CustomerOrderListModel PrepareCustomerOrderListModel(int? page = 0);

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        OrderDetailsModel PrepareOrderDetailsModel(Order order);

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment);

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page);

        /// <summary>
        /// Prepare the Order item picture model
        /// </summary>
        /// <param name="sci">Order item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>Picture model</returns>
        PictureModel PrepareOrderItemPictureModel(OrderItem sci, int pictureSize, bool showDefaultPicture, string productName);
    }
}
