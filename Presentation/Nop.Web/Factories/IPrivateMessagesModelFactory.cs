using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Web.Models.PrivateMessages;
using System.Collections.Generic;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the private message model factory
    /// </summary>
    public partial interface IPrivateMessagesModelFactory
    {
        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message index model</returns>
        PrivateMessageIndexModel PreparePrivateMessageIndexModel();

        /// <summary>
        /// Prepare the inbox model
        /// </summary>
        /// <returns>Private message list model</returns>
        IList<PrivateMessageModel> PrepareInboxModel();

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        IList<PrivateMessageModel> PrepareConverstationsModel(int toCustomerId = 0);

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        List<PrivateMessageModel> PrepareVendorModel();


        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Private message model</returns>
        PrivateMessageModel PreparePrivateMessageModel(PrivateMessage pm);


        /// <summary>
        /// Prepare the private message
        /// </summary>
        /// <param name="vendorId">vendor indendifier</param>
        /// <returns>Private message</returns>
        PrivateMessage PreparePrivateMessage(int vendorI = 0);
    }
}
