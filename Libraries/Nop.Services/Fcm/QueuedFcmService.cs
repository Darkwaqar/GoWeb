using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Events;

namespace Nop.Services.Fcm
{
    public partial class QueuedFcmService : IQueuedFcmService
    {

        private readonly IRepository<QueuedFcm> _queuedFcmRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedFcmRepository">Queued fcm repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public QueuedFcmService(IRepository<QueuedFcm> queuedFcmRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _queuedFcmRepository = queuedFcmRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>        
        public virtual void InsertQueuedFcm(QueuedFcm queuedFcm)
        {
            if (queuedFcm == null)
                throw new ArgumentNullException("queuedFcm");

            _queuedFcmRepository.Insert(queuedFcm);

            //event notification
            _eventPublisher.EntityInserted(queuedFcm);
        }

        /// <summary>
        /// Updates a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>
        public virtual void UpdateQueuedFcm(QueuedFcm queuedFcm)
        {
            if (queuedFcm == null)
                throw new ArgumentNullException("queuedFcm");

            _queuedFcmRepository.Update(queuedFcm);

            //event notification
            _eventPublisher.EntityUpdated(queuedFcm);
        }

        /// <summary>
        /// Deleted a queued fcm
        /// </summary>
        /// <param name="queuedFcm">Queued fcm</param>
        public virtual void DeleteQueuedFcm(QueuedFcm queuedFcm)
        {
            if (queuedFcm == null)
                throw new ArgumentNullException("queuedFcm");

            _queuedFcmRepository.Delete(queuedFcm);

            //event notification
            _eventPublisher.EntityDeleted(queuedFcm);
        }

        /// <summary>
        /// Deleted a queued fcms
        /// </summary>
        /// <param name="queuedFcms">Queued fcms</param>
        public virtual void DeleteQueuedFcms(IList<QueuedFcm> queuedFcms)
        {
            if (queuedFcms == null)
                throw new ArgumentNullException("queuedFcms");

            _queuedFcmRepository.Delete(queuedFcms);

            //event notification
            foreach (var queuedFcm in queuedFcms)
            {
                _eventPublisher.EntityDeleted(queuedFcm);
            }
        }

        /// <summary>
        /// Gets a queued fcm by identifier
        /// </summary>
        /// <param name="queuedFcmId">Queued fcm identifier</param>
        /// <returns>Queued fcm</returns>
        public virtual QueuedFcm GetQueuedFcmById(int queuedFcmId)
        {
            if (queuedFcmId == 0)
                return null;

            return _queuedFcmRepository.GetById(queuedFcmId);

        }

        /// <summary>
        /// Get queued fcms by identifiers
        /// </summary>
        /// <param name="queuedFcmIds">queued fcm identifiers</param>
        /// <returns>Queued fcms</returns>
        public virtual IList<QueuedFcm> GetQueuedFcmsByIds(int[] queuedFcmIds)
        {
            if (queuedFcmIds == null || queuedFcmIds.Length == 0)
                return new List<QueuedFcm>();

            var query = from qe in _queuedFcmRepository.Table
                        where queuedFcmIds.Contains(qe.Id)
                        select qe;
            var queuedFcms = query.ToList();
            //sort by passed identifiers
            var sortedQueuedFcms = new List<QueuedFcm>();
            foreach (int id in queuedFcmIds)
            {
                var queuedFcm = queuedFcms.Find(x => x.Id == id);
                if (queuedFcm != null)
                    sortedQueuedFcms.Add(queuedFcm);
            }
            return sortedQueuedFcms;
        }

        /// <summary>
        /// Gets all queued fcms
        /// </summary>
        /// <param name="fromFcm">From Fcm</param>
        /// <param name="toFcm">To Fcm</param>
        /// <param name="searchPackage">From Fcm</param>
        /// <param name="searchAppKey">To Fcm</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent fcms</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only fcms for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued fcm descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<QueuedFcm> SearchFcms(string fromFcm, string toFcm, string searchPackage, string searchAppKey, int searchVendorId, int searchStoreId,
            DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            fromFcm = (fromFcm ?? String.Empty).Trim();
            toFcm = (toFcm ?? String.Empty).Trim();
            searchPackage = (searchPackage ?? String.Empty).Trim();
            searchAppKey = (searchAppKey ?? String.Empty).Trim();
            var query = _queuedFcmRepository.Table;
            if (!String.IsNullOrEmpty(fromFcm))
                query = query.Where(qe => qe.From.Contains(fromFcm));
            if (!String.IsNullOrEmpty(toFcm))
                query = query.Where(qe => qe.DeviceId.Contains(toFcm));
            if (!String.IsNullOrEmpty(searchPackage))
                query = query.Where(qe => qe.Package.Contains(searchPackage));
            if (!String.IsNullOrEmpty(searchAppKey))
                query = query.Where(qe => qe.AppKey.Contains(searchAppKey));
            if (searchVendorId != 0)
                query = query.Where(qe => qe.VendorId == searchVendorId);
            if (searchStoreId != 0)
                query = query.Where(qe => qe.StoreId == searchStoreId);
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            if (loadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }
            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = loadNewest ?
                //load the newest records
                query.OrderByDescending(qe => qe.CreatedOnUtc) :
                //load by priority
                query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);

            var queuedFcms = new PagedList<QueuedFcm>(query, pageIndex, pageSize);
            return queuedFcms;
        }

        /// <summary>
        /// Delete all queued fcms
        /// </summary>
        public virtual void DeleteAllFcms()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                string queuedFcmTableName = _dbContext.GetTableName<QueuedFcm>();
                _dbContext.ExecuteSqlCommand(String.Format("TRUNCATE TABLE [{0}]", queuedFcmTableName));
            }
            else
            {
                var queuedFcms = _queuedFcmRepository.Table.ToList();
                foreach (var qe in queuedFcms)
                    _queuedFcmRepository.Delete(qe);
            }
        }
    }
}
