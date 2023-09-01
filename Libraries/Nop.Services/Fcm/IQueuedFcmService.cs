using Nop.Core;
using Nop.Core.Domain.Messages;
using System;
using System.Collections.Generic;

namespace Nop.Services.Fcm
{
    public partial interface IQueuedFcmService
    {

        /// <summary>
        /// Inserts a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>
        void InsertQueuedFcm(QueuedFcm queuedFcm);

        /// <summary>
        /// Updates a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>
        void UpdateQueuedFcm(QueuedFcm queuedFcm);

        /// <summary>
        /// Deleted a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>
        void DeleteQueuedFcm(QueuedFcm queuedFcm);

        /// <summary>
        /// Deleted a queued fcms
        /// </summary>
        /// <param name="queuedFcms">Queued fcms</param>
        void DeleteQueuedFcms(IList<QueuedFcm> queuedFcms);

        /// <summary>
        /// Gets a queued fcm by identifier
        /// </summary>
        /// <param name="queuedFcmId">Queued fcm identifier</param>
        /// <returns>Queued fcm</returns>
        QueuedFcm GetQueuedFcmById(int queuedFcmId);

        /// <summary>
        /// Get queued fcms by identifiers
        /// </summary>
        /// <param name="queuedFcmIds">queued fcm identifiers</param>
        /// <returns>Queued fcms</returns>
        IList<QueuedFcm> GetQueuedFcmsByIds(int[] queuedFcmIds);

        /// <summary>
        /// Search queued fcms
        /// </summary>
        /// <param name="fromFcm">From Fcm</param>
        /// <param name="toFcm">To Fcm</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent fcms</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only fcms for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued fcm descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Queued fcms</returns>
        IPagedList<QueuedFcm> SearchFcms(string searchFromFcm, string searchToFcm, string searchPackage, string searchAppKey, int searchVendorId, int searchStoreId,
            DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Delete all queued fcms
        /// </summary>
        void DeleteAllFcms();
    }
}
