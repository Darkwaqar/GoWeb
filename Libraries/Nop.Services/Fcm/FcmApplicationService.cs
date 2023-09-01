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
    public partial class FcmApplicationService : IFcmApplicationService
    {
        private readonly IRepository<FcmApplication> _fcmApplicationRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="fcmApplicationRepository">Fcm Application repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public FcmApplicationService(IRepository<FcmApplication> fcmApplicationRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            _fcmApplicationRepository = fcmApplicationRepository;
            _eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a fcm application 
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>        
        public virtual void InsertFcmApplication(FcmApplication fcmApplication)
        {
            if (fcmApplication == null)
                throw new ArgumentNullException("fcmApplication");

            _fcmApplicationRepository.Insert(fcmApplication);

            //event notification
            _eventPublisher.EntityInserted(fcmApplication);
        }

        /// <summary>
        /// Updates a fcm application 
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>
        public virtual void UpdateFcmApplication(FcmApplication fcmApplication)
        {
            if (fcmApplication == null)
                throw new ArgumentNullException("fcmApplication");

            _fcmApplicationRepository.Update(fcmApplication);

            //event notification
            _eventPublisher.EntityUpdated(fcmApplication);
        }

        /// <summary>
        /// Deleted a fcm application 
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>
        public virtual void DeleteFcmApplication(FcmApplication fcmApplication)
        {
            if (fcmApplication == null)
                throw new ArgumentNullException("fcmApplication");

            _fcmApplicationRepository.Delete(fcmApplication);

            //event notification
            _eventPublisher.EntityDeleted(fcmApplication);
        }

        /// <summary>
        /// Deleted a fcm application s
        /// </summary>
        /// <param name="fcmApplications">Fcm Applications</param>
        public virtual void DeleteFcmApplications(IList<FcmApplication> fcmApplications)
        {
            if (fcmApplications == null)
                throw new ArgumentNullException("fcmApplications");

            _fcmApplicationRepository.Delete(fcmApplications);

            //event notification
            foreach (var fcmApplication in fcmApplications)
            {
                _eventPublisher.EntityDeleted(fcmApplication);
            }
        }

        /// <summary>
        /// Gets a fcm application  by identifier
        /// </summary>
        /// <param name="fcmApplicationId">Fcm Application identifier</param>
        /// <returns>Fcm Application</returns>
        public virtual FcmApplication GetFcmApplicationById(int fcmApplicationId)
        {
            if (fcmApplicationId == 0)
                return null;

            return _fcmApplicationRepository.GetById(fcmApplicationId);

        }

        /// <summary>
        /// Get fcm application s by identifiers
        /// </summary>
        /// <param name="fcmApplicationIds">fcm application  identifiers</param>
        /// <returns>Fcm Applications</returns>
        public virtual IList<FcmApplication> GetFcmApplicationsByIds(int[] fcmApplicationIds)
        {
            if (fcmApplicationIds == null || fcmApplicationIds.Length == 0)
                return new List<FcmApplication>();

            var query = from qe in _fcmApplicationRepository.Table
                        where fcmApplicationIds.Contains(qe.Id)
                        select qe;
            var fcmApplications = query.ToList();
            //sort by passed identifiers
            var sortedFcmApplications = new List<FcmApplication>();
            foreach (int id in fcmApplicationIds)
            {
                var fcmApplication = fcmApplications.Find(x => x.Id == id);
                if (fcmApplication != null)
                    sortedFcmApplications.Add(fcmApplication);
            }
            return sortedFcmApplications;
        }

        /// <summary>
        /// Gets all fcm application s
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
        /// <param name="loadNewest">A value indicating whether we should sort fcm application  descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<FcmApplication> SearchApplications(int SearchVendorId = 0, int SearchId = 0, string SearchPackage = null, string SearchAppKey = null,
            string SearchName = null, FcmApplicationType SearchFcmApplicationType = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            SearchPackage = (SearchPackage ?? String.Empty).Trim();
            SearchAppKey = (SearchAppKey ?? String.Empty).Trim();
            SearchName = (SearchName ?? String.Empty).Trim();
            var query = _fcmApplicationRepository.Table;
            if (SearchVendorId>0) query = query.Where(qe => qe.VendorId == SearchVendorId);
            if (SearchId > 0) query = query.Where(qe => qe.Id == SearchId);
            if (!String.IsNullOrEmpty(SearchPackage))
                query = query.Where(qe => qe.Package.Contains(SearchPackage));
            if (!String.IsNullOrEmpty(SearchAppKey))
                query = query.Where(qe => qe.AppKey.Contains(SearchAppKey));
            if (!String.IsNullOrEmpty(SearchName))
                query = query.Where(qe => qe.Name.Contains(SearchName));
            if (SearchFcmApplicationType != 0)
            {
                query = query.Where(qe => qe.FcmApplicationType == SearchFcmApplicationType);
            }

            query = query.OrderByDescending(qe => qe.CreatedOnUtc);


            var fcmApplications = new PagedList<FcmApplication>(query, pageIndex, pageSize);
            return fcmApplications;
        }


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>FcmApplication</returns>
        public IList<FcmApplication> GetAllFcmApplication()
        {
            var query = _fcmApplicationRepository.Table;
            var applications = query.ToList();
            return applications;
        }
    }
}
