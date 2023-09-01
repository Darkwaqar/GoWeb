using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Core.Domain.Polls;

namespace Nop.Services.Polls
{
    /// <summary>
    /// PollCategory service
    /// </summary>
    public partial class PollCategoryService : IPollCategoryService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : pollCategory ID
        /// </remarks>
        private const string POLL_CATEGORIES_BY_ID_KEY = "Nop.pollCategory.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent pollCategory ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// {4} : include all levels (child)
        /// </remarks>
        private const string POLL_CATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "Nop.pollCategory.byparent-{0}-{1}-{2}-{3}-{4}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : pollCategory ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string POLLPOLL_CATEGORIES_ALLBYCATEGORYID_KEY = "Nop.pollpollCategory.allbypollCategoryid-{0}-{1}-{2}-{3}-{4}-{5}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : page index
        /// {2} : page size
        /// </remarks>
        private const string SEARCHPOLL_CATEGORIES_ALL = "Nop.searchpollCategory.all-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : page index
        /// {2} : page size
        /// </remarks>
        private const string SEARCHPOLL_CATEGORIES_ALLCOLLECTION = "Nop.searchpollCategory.allcollection-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : poll ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string POLLPOLL_CATEGORIES_ALLBYPOLLID_KEY = "Nop.pollpollCategory.allbypollid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : pollCategory ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string VENDORMAPPEDPOLL_CATEGORIES_ALLBYCATEGORYID_KEY = "Nop.vendormappedpollCategory.allbypollCategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : poll ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string VENDORMAPPEDPOLL_CATEGORIES_ALLBYVENDORID_KEY = "Nop.vendormappedpollCategory.allbyvendorid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string POLL_CATEGORIES_PATTERN_KEY = "Nop.pollCategory.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string POLLPOLL_CATEGORIES_PATTERN_KEY = "Nop.pollpollCategory.";


        #endregion

        #region Fields

        private readonly IRepository<PollCategory> _pollCategoryRepository;
        private readonly IRepository<PollPollCategory> _pollPollCategoryRepository;
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAclService _aclService;
        private readonly CommonSettings _commonSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="pollCategoryRepository">PollCategory repository</param>
        /// <param name="pollPollCategoryRepository">PollPollCategory repository</param>
        /// <param name="pollRepository">Poll repository</param>
        /// <param name="vendorRepository">Vendor repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="dbContext">DB context</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="aclService">ACL service</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public PollCategoryService(ICacheManager cacheManager,
            IRepository<PollCategory> pollCategoryRepository,
            IRepository<PollPollCategory> pollPollCategoryRepository,
            IRepository<Poll> pollRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<AclRecord> aclRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IDbContext dbContext,
            IDataProvider dataProvider,
            IWorkContext workContext,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            IStoreMappingService storeMappingService,
            IAclService aclService,
            CommonSettings commonSettings,
            CatalogSettings catalogSettings,
            ILocalizationService localizationService,
            ICustomerService customerService)
        {
            this._cacheManager = cacheManager;
            this._pollCategoryRepository = pollCategoryRepository;
            this._pollPollCategoryRepository = pollPollCategoryRepository;
            this._pollRepository = pollRepository;
            this._vendorRepository = vendorRepository;
            this._aclRepository = aclRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._storeMappingService = storeMappingService;
            this._aclService = aclService;
            this._commonSettings = commonSettings;
            this._catalogSettings = catalogSettings;
            this._localizationService = localizationService;
            this._customerService = customerService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        public virtual void DeletePollCategory(PollCategory pollCategory)
        {
            if (pollCategory == null)
                throw new ArgumentNullException("pollCategory");

            pollCategory.Deleted = true;
            UpdatePollCategory(pollCategory);

            //event notification
            _eventPublisher.EntityDeleted(pollCategory);

        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="pollCategoryName">PollCategory name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<PollCategory> GetAllCategories(string pollCategoryName = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (_commonSettings.UseStoredProcedureForLoadingCategories &&
                _commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled for loading categories and supported by the database. 
                //It's much faster with a large number of categories than the LINQ implementation below 

                //prepare parameters
                var showHiddenParameter = _dataProvider.GetParameter();
                showHiddenParameter.ParameterName = "ShowHidden";
                showHiddenParameter.Value = showHidden;
                showHiddenParameter.DbType = DbType.Boolean;

                var nameParameter = _dataProvider.GetParameter();
                nameParameter.ParameterName = "Name";
                nameParameter.Value = pollCategoryName ?? string.Empty;
                nameParameter.DbType = DbType.String;

                var storeIdParameter = _dataProvider.GetParameter();
                storeIdParameter.ParameterName = "StoreId";
                storeIdParameter.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
                storeIdParameter.DbType = DbType.Int32;

                //pass allowed customer role identifiers as comma-delimited string
                var customerRoleIdsParameter = _dataProvider.GetParameter();
                customerRoleIdsParameter.ParameterName = "CustomerRoleIds";
                customerRoleIdsParameter.Value = !_catalogSettings.IgnoreAcl
                    ? string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()) : string.Empty;
                customerRoleIdsParameter.DbType = DbType.String;

                var pageIndexParameter = _dataProvider.GetParameter();
                pageIndexParameter.ParameterName = "PageIndex";
                pageIndexParameter.Value = pageIndex;
                pageIndexParameter.DbType = DbType.Int32;

                var pageSizeParameter = _dataProvider.GetParameter();
                pageSizeParameter.ParameterName = "PageSize";
                pageSizeParameter.Value = pageSize;
                pageSizeParameter.DbType = DbType.Int32;

                var totalRecordsParameter = _dataProvider.GetParameter();
                totalRecordsParameter.ParameterName = "TotalRecords";
                totalRecordsParameter.Direction = ParameterDirection.Output;
                totalRecordsParameter.DbType = DbType.Int32;

                //invoke stored procedure
                var categories = _dbContext.ExecuteStoredProcedureList<PollCategory>("PollCategoryLoadAllPaged",
                    showHiddenParameter, nameParameter, storeIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter);
                var totalRecords = (totalRecordsParameter.Value != DBNull.Value) ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<PollCategory>(categories, pageIndex, pageSize, totalRecords);
            }
            else
            {
                //stored procedures aren't supported. Use LINQ
                var query = _pollCategoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                if (!String.IsNullOrWhiteSpace(pollCategoryName))
                    query = query.Where(c => c.Name.Contains(pollCategoryName));
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
                {
                    if (!showHidden && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var unsortedCategories = query.ToList();

                //paging
                return new PagedList<PollCategory>(unsortedCategories, pageIndex, pageSize);
            }
        }

      

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<PollCategory> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false)
        {
            var query = from c in _pollCategoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomePage
                        select c;

            var categories = query.ToList();
            if (!showHidden)
            {
                categories = categories
                    .Where(c => _aclService.Authorize(c) && _storeMappingService.Authorize(c))
                    .ToList();
            }

            return categories;
        }


       

        /// <summary>
        /// Gets all collection displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<PollCategory> GetAllCollectionDisplayedOnHomePage(bool showHidden = false)
        {
            var query = from c in _pollCategoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted
                        select c;

            var categories = query.ToList();
            if (!showHidden)
            {
                categories = categories
                    .Where(c => _aclService.Authorize(c) && _storeMappingService.Authorize(c))
                    .ToList();
            }

            return categories;
        }

        /// <summary>
        /// Gets a pollCategory
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        /// <returns>PollCategory</returns>
        public virtual PollCategory GetPollCategoryById(int pollCategoryId)
        {
            if (pollCategoryId == 0)
                return null;

            string key = string.Format(POLL_CATEGORIES_BY_ID_KEY, pollCategoryId);
            return _cacheManager.Get(key, () => _pollCategoryRepository.GetById(pollCategoryId));
        }

        /// <summary>
        /// Inserts pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        public virtual void InsertPollCategory(PollCategory pollCategory)
        {
            if (pollCategory == null)
                throw new ArgumentNullException("pollCategory");

            _pollCategoryRepository.Insert(pollCategory);

            //cache
            _cacheManager.RemoveByPattern(POLL_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(POLLPOLL_CATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(pollCategory);
        }

        /// <summary>
        /// Updates the pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        public virtual void UpdatePollCategory(PollCategory pollCategory)
        {
            if (pollCategory == null)
                throw new ArgumentNullException("pollCategory");

            _pollCategoryRepository.Update(pollCategory);

            //cache
            _cacheManager.RemoveByPattern(POLL_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(POLLPOLL_CATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(pollCategory);
        }


        /// <summary>
        /// Deletes a poll pollCategory mapping
        /// </summary>
        /// <param name="pollPollCategory">Poll pollCategory</param>
        public virtual void DeletePollPollCategory(PollPollCategory pollPollCategory)
        {
            if (pollPollCategory == null)
                throw new ArgumentNullException("pollPollCategory");

            _pollPollCategoryRepository.Delete(pollPollCategory);

            //cache
            _cacheManager.RemoveByPattern(POLL_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(POLLPOLL_CATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(pollPollCategory);
        }

       

        /// <summary>
        /// Gets poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll a pollCategory mapping collection</returns>
        public virtual IPagedList<PollPollCategory> GetPollCategoriesByPollCategoryId(int pollCategoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (pollCategoryId == 0)
                return new PagedList<PollPollCategory>(new List<PollPollCategory>(), pageIndex, pageSize);

            string key = string.Format(POLLPOLL_CATEGORIES_ALLBYCATEGORYID_KEY, showHidden, pollCategoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _pollPollCategoryRepository.Table
                            join p in _pollRepository.Table on pc.PollId equals p.Id
                            where pc.PollCategoryId == pollCategoryId  &&
                                  (showHidden || p.Published)
                            orderby pc.DisplayOrder, pc.Id
                            select pc;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pc in query
                                join c in _pollCategoryRepository.Table on pc.PollCategoryId equals c.Id
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pc;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pc in query
                                join c in _pollCategoryRepository.Table on pc.PollCategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
                                select pc;
                    }
                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(pc => pc.DisplayOrder).ThenBy(pc => pc.Id);
                }

                var pollCategories = new PagedList<PollPollCategory>(query, pageIndex, pageSize);
                return pollCategories;
            });
        }

        /// <summary>
        /// Gets a poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Poll pollCategory mapping collection</returns>
        public virtual IList<PollPollCategory> GetPollCategoriesByPollId(int pollId, bool showHidden = false)
        {
            return GetPollCategoriesByPollId(pollId, _storeContext.CurrentStore.Id, showHidden);
        }

        /// <summary>
        /// Gets a poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Poll pollCategory mapping collection</returns>
        public virtual IList<PollPollCategory> GetPollCategoriesByPollId(int pollId, int storeId, bool showHidden = false)
        {
            if (pollId == 0)
                return new List<PollPollCategory>();

            string key = string.Format(POLLPOLL_CATEGORIES_ALLBYPOLLID_KEY, showHidden, pollId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _pollPollCategoryRepository.Table
                            join c in _pollCategoryRepository.Table on pc.PollCategoryId equals c.Id
                            where pc.PollId == pollId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder, pc.Id
                            select pc;

                var allPollCategories = query.ToList();
                var result = new List<PollPollCategory>();
                if (!showHidden)
                {
                    foreach (var pc in allPollCategories)
                    {
                        //ACL (access control list) and store mapping
                        var pollCategory = pc.PollCategory;
                        if (_aclService.Authorize(pollCategory) && _storeMappingService.Authorize(pollCategory, storeId))
                            result.Add(pc);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allPollCategories);
                }
                return result;
            });
        }


        /// <summary>
        /// Gets a poll pollCategory mapping 
        /// </summary>
        /// <param name="pollPollCategoryId">Poll pollCategory mapping identifier</param>
        /// <returns>Poll pollCategory mapping</returns>
        public virtual PollPollCategory GetPollPollCategoryById(int pollPollCategoryId)
        {
            if (pollPollCategoryId == 0)
                return null;

            return _pollPollCategoryRepository.GetById(pollPollCategoryId);
        }




        /// <summary>
        /// Inserts a poll pollCategory mapping
        /// </summary>
        /// <param name="pollPollCategory">>Poll pollCategory mapping</param>
        public virtual void InsertPollPollCategory(PollPollCategory pollPollCategory)
        {
            if (pollPollCategory == null)
                throw new ArgumentNullException("pollPollCategory");

            _pollPollCategoryRepository.Insert(pollPollCategory);

            //cache
            _cacheManager.RemoveByPattern(POLL_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(POLLPOLL_CATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(pollPollCategory);
        }

        /// <summary>
        /// Updates the poll pollCategory mapping 
        /// </summary>
        /// <param name="pollPollCategory">>Poll pollCategory mapping</param>
        public virtual void UpdatePollPollCategory(PollPollCategory pollPollCategory)
        {
            if (pollPollCategory == null)
                throw new ArgumentNullException("pollPollCategory");

            _pollPollCategoryRepository.Update(pollPollCategory);

            //cache
            _cacheManager.RemoveByPattern(POLL_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(POLLPOLL_CATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(pollPollCategory);
        }




        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="pollCategoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        public virtual string[] GetNotExistingCategories(string[] pollCategoryNames)
        {
            if (pollCategoryNames == null)
                throw new ArgumentNullException("pollCategoryNames");

            var query = _pollCategoryRepository.Table;
            var queryFilter = pollCategoryNames.Distinct().ToArray();
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Get pollCategory IDs for polls
        /// </summary>
        /// <param name="pollIds">Polls IDs</param>
        /// <returns>PollCategory IDs for polls</returns>
        public virtual IDictionary<int, int[]> GetPollPollCategoryIds(int[] pollIds)
        {
            var query = _pollPollCategoryRepository.Table;

            return query.Where(p => pollIds.Contains(p.PollId))
                .Select(p => new { p.PollId, p.PollCategoryId }).ToList()
                .GroupBy(a => a.PollId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.PollCategoryId).ToArray());
        }

        #endregion

        #region Extend Services

       
       


        


        /// <summary>
        /// Enable PollCategory Poll by PollCategory identifier
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        public virtual void EnablePollCategoryPollByPollCategoryId(int pollCategoryId = 0)
        {
            var pollpollCategory = _pollPollCategoryRepository.Table;
            var polls = _pollRepository.Table;

            pollpollCategory = pollpollCategory.Where(x => x.PollCategoryId == pollCategoryId);

            var pro = new List<Poll>();
            foreach (var ppollCategory in pollpollCategory.ToList())
            {
                ppollCategory.Poll.Published = true;
                pro.Add(ppollCategory.Poll);
            }

            //update
            _pollRepository.Update(pro);

            //event notification
            foreach (var poll in pro)
            {
                _eventPublisher.EntityUpdated(poll);
            }
        }

        /// <summary>
        /// Disable PollCategory Poll by PollCategory identifier
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        public virtual void DisablePollCategoryPollByPollCategoryId(int pollCategoryId = 0)
        {
            var pollpollCategory = _pollPollCategoryRepository.Table;
            var polls = _pollRepository.Table;

            pollpollCategory = pollpollCategory.Where(x => x.PollCategoryId == pollCategoryId);

            var pro = new List<Poll>();
            foreach (var ppollCategory in pollpollCategory.ToList())
            {
                ppollCategory.Poll.Published = false;
                pro.Add(ppollCategory.Poll);
            }

            //update
            _pollRepository.Update(pro);

            //event notification
            foreach (var poll in pro)
            {
                _eventPublisher.EntityUpdated(poll);
            }
        }


        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="pollCategoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="VendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="pollCategoryID">Category identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<PollCategory> SearchVendorCategories(string pollCategoryName = "", int storeId = 0, int vendorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool showMarketplace = false, bool ShowFeatured = false)
        {
            if (_commonSettings.UseStoredProcedureForLoadingCategories &&
                _commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled for loading categories and supported by the database. 
                //It's much faster with a large number of categories than the LINQ implementation below 

                //prepare parameters
                var showHiddenParameter = _dataProvider.GetParameter();
                showHiddenParameter.ParameterName = "ShowHidden";
                showHiddenParameter.Value = showHidden;
                showHiddenParameter.DbType = DbType.Boolean;

                var showMarketPlaceParameter = _dataProvider.GetParameter();
                showMarketPlaceParameter.ParameterName = "ShowMarketPlace";
                showMarketPlaceParameter.Value = showHidden;
                showMarketPlaceParameter.DbType = DbType.Boolean;

                var showFeaturedParameter = _dataProvider.GetParameter();
                showFeaturedParameter.ParameterName = "ShowFeatured";
                showFeaturedParameter.Value = showHidden;
                showFeaturedParameter.DbType = DbType.Boolean;

                var nameParameter = _dataProvider.GetParameter();
                nameParameter.ParameterName = "Name";
                nameParameter.Value = pollCategoryName ?? string.Empty;
                nameParameter.DbType = DbType.String;

                var storeIdParameter = _dataProvider.GetParameter();
                storeIdParameter.ParameterName = "StoreId";
                storeIdParameter.Value = !_catalogSettings.IgnoreStoreLimitations ? storeId : 0;
                storeIdParameter.DbType = DbType.Int32;


                var vendorIdParameter = _dataProvider.GetParameter();
                vendorIdParameter.ParameterName = "VendorId";
                vendorIdParameter.Value = !_catalogSettings.IgnoreStoreLimitations ? vendorId : 0;
                vendorIdParameter.DbType = DbType.Int32;

                //pass allowed customer role identifiers as comma-delimited string
                var customerRoleIdsParameter = _dataProvider.GetParameter();
                customerRoleIdsParameter.ParameterName = "CustomerRoleIds";
                customerRoleIdsParameter.Value = !_catalogSettings.IgnoreAcl
                    ? string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()) : string.Empty;
                customerRoleIdsParameter.DbType = DbType.String;

                var pageIndexParameter = _dataProvider.GetParameter();
                pageIndexParameter.ParameterName = "PageIndex";
                pageIndexParameter.Value = pageIndex;
                pageIndexParameter.DbType = DbType.Int32;

                var pageSizeParameter = _dataProvider.GetParameter();
                pageSizeParameter.ParameterName = "PageSize";
                pageSizeParameter.Value = pageSize;
                pageSizeParameter.DbType = DbType.Int32;

                var totalRecordsParameter = _dataProvider.GetParameter();
                totalRecordsParameter.ParameterName = "TotalRecords";
                totalRecordsParameter.Direction = ParameterDirection.Output;
                totalRecordsParameter.DbType = DbType.Int32;

                //invoke stored procedure
                var pollcategories = _dbContext.ExecuteStoredProcedureList<PollCategory>("PollCategoryLoadAllPagedAdvance",
                    showHiddenParameter, showMarketPlaceParameter, showFeaturedParameter, nameParameter, storeIdParameter, vendorIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter);
                var totalRecords = (totalRecordsParameter.Value != DBNull.Value) ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<PollCategory>(pollcategories, pageIndex, pageSize, totalRecords);
            }
            else
            {
                //stored procedures aren't supported. Use LINQ
                var query = _pollCategoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                if (ShowFeatured)
                    query = query.Where(c => c.ShowOnHomePage);
                if (!String.IsNullOrWhiteSpace(pollCategoryName))
                    query = query.Where(c => c.Name.Contains(pollCategoryName));
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                if (showMarketplace)
                {
                    query = query.Where(c => c.VendorId == 0);
                }
                else
                {
                    if (vendorId != 0)
                    {
                        query = query.Where(c => c.VendorId == vendorId);
                    }
                }


                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
                {
                    if (!showHidden && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    if (vendorId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "PollCategory" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || vendorId == sm.StoreId
                                select c;
                    }

                    //only distinct categories (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var unsortedCategories = query.ToList();


                //paging
                return new PagedList<PollCategory>(unsortedCategories, pageIndex, pageSize);
            }
        }

        #endregion
    }
}
