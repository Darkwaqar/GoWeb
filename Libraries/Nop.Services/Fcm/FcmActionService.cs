using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Events;
using Nop.Core.Domain.Fcm;

namespace Nop.Services.Fcm
{
    public partial class FcmActionService : IFcmActionService
    {

        private readonly IRepository<FcmAction> _fcmActionRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fcmActionRepository">Fcm Action repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public FcmActionService(IRepository<FcmAction> fcmActionRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _fcmActionRepository = fcmActionRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a fcm action 
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>        
        public virtual void InsertFcmAction(FcmAction fcmAction)
        {
            if (fcmAction == null)
                throw new ArgumentNullException("fcmAction");

            _fcmActionRepository.Insert(fcmAction);

            //event notification
            _eventPublisher.EntityInserted(fcmAction);
        }

        /// <summary>
        /// Updates a fcm action 
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>
        public virtual void UpdateFcmAction(FcmAction fcmAction)
        {
            if (fcmAction == null)
                throw new ArgumentNullException("fcmAction");

            _fcmActionRepository.Update(fcmAction);

            //event notification
            _eventPublisher.EntityUpdated(fcmAction);
        }

        /// <summary>
        /// Deleted a fcm action 
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>
        public virtual void DeleteFcmAction(FcmAction fcmAction)
        {
            if (fcmAction == null)
                throw new ArgumentNullException("fcmAction");

            _fcmActionRepository.Delete(fcmAction);

            //event notification
            _eventPublisher.EntityDeleted(fcmAction);
        }

        /// <summary>
        /// Deleted a fcm action s
        /// </summary>
        /// <param name="fcmActions">Fcm Actions</param>
        public virtual void DeleteFcmActions(IList<FcmAction> fcmActions)
        {
            if (fcmActions == null)
                throw new ArgumentNullException("fcmActions");

            _fcmActionRepository.Delete(fcmActions);

            //event notification
            foreach (var fcmAction in fcmActions)
            {
                _eventPublisher.EntityDeleted(fcmAction);
            }
        }

        /// <summary>
        /// Gets a fcm action  by identifier
        /// </summary>
        /// <param name="fcmActionId">Fcm Action identifier</param>
        /// <returns>Fcm Action</returns>
        public virtual FcmAction GetFcmActionById(int fcmActionId)
        {
            if (fcmActionId == 0)
                return null;

            return _fcmActionRepository.GetById(fcmActionId);

        }

        /// <summary>
        /// Get fcm action s by identifiers
        /// </summary>
        /// <param name="fcmActionIds">fcm action  identifiers</param>
        /// <returns>Fcm Actions</returns>
        public virtual IList<FcmAction> GetFcmActionsByIds(int[] fcmActionIds)
        {
            if (fcmActionIds == null || fcmActionIds.Length == 0)
                return new List<FcmAction>();

            var query = from qe in _fcmActionRepository.Table
                        where fcmActionIds.Contains(qe.Id)
                        select qe;
            var fcmActions = query.ToList();
            //sort by passed identifiers
            var sortedFcmActions = new List<FcmAction>();
            foreach (int id in fcmActionIds)
            {
                var fcmAction = fcmActions.Find(x => x.Id == id);
                if (fcmAction != null)
                    sortedFcmActions.Add(fcmAction);
            }
            return sortedFcmActions;
        }

        /// <summary>
        /// Search fcmactions
        /// </summary>
        /// <param name="SearchVendorId">Search by vendor identifier</param>
        /// <param name="SearchName">Search by name</param>
        /// <param name="ShowHidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Fcm Actions</returns>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<FcmAction> SearchFcmActions(int SearchVendorId = 0, string SearchName = null, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _fcmActionRepository.Table;

            if (SearchVendorId > 0)
                query = query.Where(v => v.VendorId == SearchVendorId);
            if (!String.IsNullOrEmpty(SearchName))
                query = query.Where(qe => qe.Name.Contains(SearchName));
            if (!showHidden)
                query = query.Where(v => v.Active);

            query = query.OrderByDescending(qe => qe.CreatedOnUtc);

            var fcmActions = new PagedList<FcmAction>(query, pageIndex, pageSize);
            return fcmActions;
        }


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FcmAction</returns>
        public IList<FcmAction> GetAllFcmAction()
        {
            var query = _fcmActionRepository.Table;
            var applications = query.ToList();
            return applications;
        }
    }
}
