using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Events;
using Nop.Core.Domain.Magazines;

namespace Nop.Services.Magazines
{
    public partial class MagazineService : IMagazineService
    {
        private readonly IRepository<Magazine> _magazineRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IEventPublisher _eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="magazineRepository">Magazine repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        public MagazineService(IRepository<Magazine> magazineRepository,
            IEventPublisher eventPublisher,
            IDbContext dbContext,
            IDataProvider dataProvider,
            CommonSettings commonSettings)
        {
            this._magazineRepository = magazineRepository;
            this._eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
        }

        /// <summary>
        /// Inserts a magazine 
        /// </summary>
        /// <param name="magazine">Magazine</param>        
        public virtual void InsertMagazine(Magazine magazine)
        {
            if (magazine == null)
                throw new ArgumentNullException("magazine");

            _magazineRepository.Insert(magazine);

            //event notification
            _eventPublisher.EntityInserted(magazine);
        }

        /// <summary>
        /// Updates a magazine 
        /// </summary>
        /// <param name="magazine">Magazine</param>
        public virtual void UpdateMagazine(Magazine magazine)
        {
            if (magazine == null)
                throw new ArgumentNullException("magazine");

            _magazineRepository.Update(magazine);

            //event notification
            _eventPublisher.EntityUpdated(magazine);
        }

        /// <summary>
        /// Deleted a magazine 
        /// </summary>
        /// <param name="magazine">Magazine</param>
        public virtual void DeleteMagazine(Magazine magazine)
        {
            if (magazine == null)
                throw new ArgumentNullException("magazine");

            _magazineRepository.Delete(magazine);

            //event notification
            _eventPublisher.EntityDeleted(magazine);
        }

        /// <summary>
        /// Deleted a magazine s
        /// </summary>
        /// <param name="magazines">Magazines</param>
        public virtual void DeleteMagazines(IList<Magazine> magazines)
        {
            if (magazines == null)
                throw new ArgumentNullException("magazines");

            _magazineRepository.Delete(magazines);

            //event notification
            foreach (var magazine in magazines)
            {
                _eventPublisher.EntityDeleted(magazine);
            }
        }

        /// <summary>
        /// Gets a magazine  by identifier
        /// </summary>
        /// <param name="magazineId">Magazine identifier</param>
        /// <returns>Magazine</returns>
        public virtual Magazine GetMagazineById(int magazineId)
        {
            if (magazineId == 0)
                return null;

            return _magazineRepository.GetById(magazineId);

        }

        /// <summary>
        /// Get magazines by identifiers
        /// </summary>
        /// <param name="magazineIds">magazine  identifiers</param>
        /// <returns>Magazines</returns>
        public virtual IList<Magazine> GetMagazinesByIds(int[] magazineIds)
        {
            if (magazineIds == null || magazineIds.Length == 0)
                return new List<Magazine>();

            var query = from qe in _magazineRepository.Table
                        where magazineIds.Contains(qe.Id)
                        select qe;
            var magazines = query.ToList();
            //sort by passed identifiers
            var sortedMagazines = new List<Magazine>();
            foreach (int id in magazineIds)
            {
                var magazine = magazines.Find(x => x.Id == id);
                if (magazine != null)
                    sortedMagazines.Add(magazine);
            }
            return sortedMagazines;
        }

        /// <summary>
        /// Search magazine
        /// </summary>
        /// <param name="SearchName">Search by name</param>
        /// <param name="ShowHidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Magazines</returns>
        /// <returns>Fcm item list</returns>
        public virtual IPagedList<Magazine> SearchMagazines(string SearchName = null, string SearchDescription = null, bool SearchActive = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _magazineRepository.Table;
            if (!String.IsNullOrEmpty(SearchName))
                query = query.Where(qe => qe.Name.Contains(SearchName));
            if (!String.IsNullOrEmpty(SearchDescription))
                query = query.Where(qe => qe.Name.Contains(SearchDescription));
            if (SearchActive)
                query = query.Where(v => v.Published);

            query = query.OrderByDescending(qe => qe.CreatedOnUtc);

            var magazines = new PagedList<Magazine>(query, pageIndex, pageSize);
            return magazines;
        }


        /// <summary>
        /// Gets all magazine
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Magazine</returns>
        public IList<Magazine> GetAllMagazine()
        {
            var query = _magazineRepository.Table;
            var applications = query.ToList();
            return applications;
        }
    }
}