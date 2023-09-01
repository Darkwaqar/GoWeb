using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Events;
using Nop.Core.Domain.SMS;

namespace Nop.Services.SMS
{
    public partial class QueuedSMSService : IQueuedSMSService
    {
        private readonly IRepository<QueuedSMS> _queuedSMSRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedSMSRepository">Queued sms repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public QueuedSMSService(IRepository<QueuedSMS> queuedSMSRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext, 
            IDataProvider dataProvider, 
            CommonSettings commonSettings)
        {
            _queuedSMSRepository = queuedSMSRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>        
        public virtual void InsertQueuedSMS(QueuedSMS queuedSMS)
        {
            if (queuedSMS == null)
                throw new ArgumentNullException("queuedSMS");

            _queuedSMSRepository.Insert(queuedSMS);

            //event notification
            _eventPublisher.EntityInserted(queuedSMS);
        }

        /// <summary>
        /// Updates a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>
        public virtual void UpdateQueuedSMS(QueuedSMS queuedSMS)
        {
            if (queuedSMS == null)
                throw new ArgumentNullException("queuedSMS");

            _queuedSMSRepository.Update(queuedSMS);

            //event notification
            _eventPublisher.EntityUpdated(queuedSMS);
        }

        /// <summary>
        /// Deleted a queued sms
        /// </summary>
        /// <param name="queuedSMS">Queued sms</param>
        public virtual void DeleteQueuedSMS(QueuedSMS queuedSMS)
        {
            if (queuedSMS == null)
                throw new ArgumentNullException("queuedSMS");

            _queuedSMSRepository.Delete(queuedSMS);

            //event notification
            _eventPublisher.EntityDeleted(queuedSMS);
        }

        /// <summary>
        /// Deleted a queued smss
        /// </summary>
        /// <param name="queuedSMSs">Queued smss</param>
        public virtual void DeleteQueuedSMSs(IList<QueuedSMS> queuedSMSs)
        {
            if (queuedSMSs == null)
                throw new ArgumentNullException("queuedSMSs");

            _queuedSMSRepository.Delete(queuedSMSs);

            //event notification
            foreach (var queuedSMS in queuedSMSs)
            {
                _eventPublisher.EntityDeleted(queuedSMS);
            }
        }

        /// <summary>
        /// Gets a queued sms by identifier
        /// </summary>
        /// <param name="queuedSMSId">Queued sms identifier</param>
        /// <returns>Queued sms</returns>
        public virtual QueuedSMS GetQueuedSMSById(int queuedSMSId)
        {
            if (queuedSMSId == 0)
                return null;

            return _queuedSMSRepository.GetById(queuedSMSId);

        }

        /// <summary>
        /// Get queued smss by identifiers
        /// </summary>
        /// <param name="queuedSMSIds">queued sms identifiers</param>
        /// <returns>Queued smss</returns>
        public virtual IList<QueuedSMS> GetQueuedSMSsByIds(int[] queuedSMSIds)
        {
            if (queuedSMSIds == null || queuedSMSIds.Length == 0)
                return new List<QueuedSMS>();

            var query = from qe in _queuedSMSRepository.Table
                        where queuedSMSIds.Contains(qe.Id)
                        select qe;
            var queuedSMSs = query.ToList();
            //sort by passed identifiers
            var sortedQueuedSMSs = new List<QueuedSMS>();
            foreach (int id in queuedSMSIds)
            {
                var queuedSMS = queuedSMSs.Find(x => x.Id == id);
                if (queuedSMS != null)
                    sortedQueuedSMSs.Add(queuedSMS);
            }
            return sortedQueuedSMSs;
        }

        /// <summary>
        /// Gets all queued smss
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
        /// <returns>SMS item list</returns>
        public virtual IPagedList<QueuedSMS> SearchSMSs(string fromSMS,
            string toSMS, DateTime? createdFromUtc, DateTime? createdToUtc, 
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            fromSMS = (fromSMS ?? String.Empty).Trim();
            toSMS = (toSMS ?? String.Empty).Trim();
            
            var query = _queuedSMSRepository.Table;
            if (!String.IsNullOrEmpty(fromSMS))
                query = query.Where(qe => qe.From.Contains(fromSMS));
            if (!String.IsNullOrEmpty(toSMS))
                query = query.Where(qe => qe.To.Contains(toSMS));
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

            var queuedSMSs = new PagedList<QueuedSMS>(query, pageIndex, pageSize);
            return queuedSMSs;
        }

        /// <summary>
        /// Delete all queued smss
        /// </summary>
        public virtual void DeleteAllSMSs()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                string queuedSMSTableName = _dbContext.GetTableName<QueuedSMS>();
                _dbContext.ExecuteSqlCommand(String.Format("TRUNCATE TABLE [{0}]", queuedSMSTableName));
            }
            else
            {
                var queuedSMSs = _queuedSMSRepository.Table.ToList();
                foreach (var qe in queuedSMSs)
                    _queuedSMSRepository.Delete(qe);
            }
        }
    }
}
