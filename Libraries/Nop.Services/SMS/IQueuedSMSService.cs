using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.SMS;

namespace Nop.Services.SMS
{
    public partial interface IQueuedSMSService
    {
        /// <summary>
        /// Inserts a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>
        void InsertQueuedSMS(QueuedSMS queuedSMS);

        /// <summary>
        /// Updates a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>
        void UpdateQueuedSMS(QueuedSMS queuedSMS);

        /// <summary>
        /// Deleted a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>
        void DeleteQueuedSMS(QueuedSMS queuedSMS);

        /// <summary>
        /// Deleted a queued smss
        /// </summary>
        /// <param name="queuedSMSs">Queued smss</param>
        void DeleteQueuedSMSs(IList<QueuedSMS> queuedSMSs);

        /// <summary>
        /// Gets a queued sms by identifier
        /// </summary>
        /// <param name="queuedSMSId">Queued sms identifier</param>
        /// <returns>Queued sms</returns>
        QueuedSMS GetQueuedSMSById(int queuedSMSId);

        /// <summary>
        /// Get queued smss by identifiers
        /// </summary>
        /// <param name="queuedSMSIds">queued sms identifiers</param>
        /// <returns>Queued smss</returns>
        IList<QueuedSMS> GetQueuedSMSsByIds(int[] queuedSMSIds);

        /// <summary>
        /// Search queued smss
        /// </summary>
        /// <param name="fromSMS">From SMS</param>
        /// <param name="toSMS">To SMS</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent smss</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only smss for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued sms descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Queued smss</returns>
        IPagedList<QueuedSMS> SearchSMSs(string fromSMS,
            string toSMS, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Delete all queued smss
        /// </summary>
        void DeleteAllSMSs();
    }
}
