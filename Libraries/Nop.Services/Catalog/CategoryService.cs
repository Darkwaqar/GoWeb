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

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service
    /// </summary>
    public partial class CategoryService : ICategoryService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        private const string CATEGORIES_BY_ID_KEY = "Nop.category.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// {4} : include all levels (child)
        /// </remarks>
        private const string CATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "Nop.category.byparent-{0}-{1}-{2}-{3}-{4}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : category ID
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.productcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : page index
        /// {2} : page size
        /// </remarks>
        private const string SEARCHCATEGORIES_ALL = "Nop.searchcategory.all-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : page index
        /// {2} : page size
        /// </remarks>
        private const string SEARCHCATEGORIES_ALLCOLLECTION = "Nop.searchcategory.allcollection-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : product ID
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY = "Nop.productcategory.allbyproductid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        private const string VENDORMAPPEDCATEGORIES_ALLBYCATEGORYID_KEY = "Nop.vendormappedcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        private const string VENDORMAPPEDCATEGORIES_ALLBYVENDORID_KEY = "Nop.vendormappedcategory.allbyvendorid-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CATEGORIES_PATTERN_KEY = "Nop.category.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTCATEGORIES_PATTERN_KEY = "Nop.productcategory.";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string VENDORMAPPEDCATEGORIES_PATTERN_KEY = "Nop.vendormappedcategory.";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SEARCHCATEGORIES_PATTERN_KEY = "Nop.searchcategory.";


        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public const string  CATEGORY_BREADCRUMB_CACHE_KEY= "Nop.category.breadcrumb-{0}-{1}-{2}-{3}";

        #endregion

        #region Fields

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<VendorMappedCategory> _vendorMappedCategoryRepository;
        private readonly IRepository<Product> _productRepository;
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
        /// <param name="categoryRepository">Category repository</param>
        /// <param name="productCategoryRepository">ProductCategory repository</param>
        /// <param name="VendorMappedCategory">VendorMappedCategory repository</param>
        /// <param name="productRepository">Product repository</param>
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
        public CategoryService(ICacheManager cacheManager,
            IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<VendorMappedCategory> vendorMappedCategoryRepository,
            IRepository<Product> productRepository,
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
            this._categoryRepository = categoryRepository;
            this._productCategoryRepository = productCategoryRepository;
            this._vendorMappedCategoryRepository = vendorMappedCategoryRepository;
            this._productRepository = productRepository;
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
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void DeleteCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            category.Deleted = true;
            UpdateCategory(category);

            //event notification
            _eventPublisher.EntityDeleted(category);

            //reset a "Parent category" property of all child subcategories
            var subcategories = GetAllCategoriesByParentCategoryId(category.Id, true);
            foreach (var subcategory in subcategories)
            {
                subcategory.ParentCategoryId = 0;
                UpdateCategory(subcategory);
            }
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<Category> GetAllCategories(string categoryName = "", int storeId = 0,
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
                nameParameter.Value = categoryName ?? string.Empty;
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
                var categories = _dbContext.ExecuteStoredProcedureList<Category>("CategoryLoadAllPaged",
                    showHiddenParameter, nameParameter, storeIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter);
                var totalRecords = (totalRecordsParameter.Value != DBNull.Value) ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<Category>(categories, pageIndex, pageSize, totalRecords);
            }
            else
            {
                //stored procedures aren't supported. Use LINQ
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                if (!String.IsNullOrWhiteSpace(categoryName))
                    query = query.Where(c => c.Name.Contains(categoryName));
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations) || (!showHidden && !_catalogSettings.IgnoreAcl))
                {
                    if (!showHidden && !_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
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
                    query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var unsortedCategories = query.ToList();

                //sort categories
                var sortedCategories = unsortedCategories.SortCategoriesForTree();

                //paging
                return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false, bool includeAllLevels = false)
        {
            string key = string.Format(CATEGORIES_BY_PARENT_CATEGORY_ID_KEY, parentCategoryId, showHidden, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, includeAllLevels);
            return _cacheManager.Get(key, () =>
            {
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentCategoryId);
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from c in query
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || currentStoreId == sm.StoreId
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

                var categories = query.ToList();
                if (includeAllLevels)
                {
                    var childCategories = new List<Category>();
                    //add child levels
                    foreach (var category in categories)
                    {
                        childCategories.AddRange(GetAllCategoriesByParentCategoryId(category.Id, showHidden, includeAllLevels));
                    }
                    categories.AddRange(childCategories);
                }
                return categories;
            });
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false)
        {
            var query = from c in _categoryRepository.Table
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
        /// Gets all categories displayed on the home page by vendorId
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="vendorId">Vendor Indicator</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetAllFeaturedCategoriesByVendorId(int vendorId = 0, bool showHidden = false)
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.ShowOnHomePage &&
                        c.VendorId == vendorId
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
        public virtual IList<Category> GetAllCollectionDisplayedOnHomePage(bool showHidden = false)
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder, c.Id
                        where c.Published &&
                        !c.Deleted &&
                        c.Collection
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
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual Category GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            string key = string.Format(CATEGORIES_BY_ID_KEY, categoryId);
            return _cacheManager.Get(key, () => _categoryRepository.GetById(categoryId));
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            _categoryRepository.Insert(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SEARCHCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORMAPPEDCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(category);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public virtual void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            //validate category hierarchy
            var parentCategory = GetCategoryById(category.ParentCategoryId);
            while (parentCategory != null)
            {
                if (category.Id == parentCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }
                parentCategory = GetCategoryById(parentCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SEARCHCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORMAPPEDCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }


        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        public virtual void DeleteProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            _productCategoryRepository.Delete(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productCategory);
        }

        /// <summary>
        /// Deletes a vendor mapped category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        public virtual void DeleteVendorMappedCategory(VendorMappedCategory vendorMappedCategory)
        {
            if (vendorMappedCategory == null)
                throw new ArgumentNullException("vendorMappedCategory");

            _vendorMappedCategoryRepository.Delete(vendorMappedCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORMAPPEDCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(vendorMappedCategory);
        }

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public virtual IPagedList<ProductCategory> GetProductCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<ProductCategory>(new List<ProductCategory>(), pageIndex, pageSize);

            string key = string.Format(PRODUCTCATEGORIES_ALLBYCATEGORYID_KEY, showHidden, categoryId, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _productCategoryRepository.Table
                            join p in _productRepository.Table on pc.ProductId equals p.Id
                            where pc.CategoryId == categoryId &&
                                  !p.Deleted &&
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
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pc;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pc in query
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
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

                var productCategories = new PagedList<ProductCategory>(query, pageIndex, pageSize);
                return productCategories;
            });
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<ProductCategory> GetProductCategoriesByProductId(int productId, bool showHidden = false)
        {
            return GetProductCategoriesByProductId(productId, _storeContext.CurrentStore.Id, showHidden);
        }

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<ProductCategory> GetProductCategoriesByProductId(int productId, int storeId, bool showHidden = false)
        {
            if (productId == 0)
                return new List<ProductCategory>();

            string key = string.Format(PRODUCTCATEGORIES_ALLBYPRODUCTID_KEY, showHidden, productId, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pc in _productCategoryRepository.Table
                            join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                            where pc.ProductId == productId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby pc.DisplayOrder, pc.Id
                            select pc;

                var allProductCategories = query.ToList();
                var result = new List<ProductCategory>();
                if (!showHidden)
                {
                    foreach (var pc in allProductCategories)
                    {
                        //ACL (access control list) and store mapping
                        var category = pc.Category;
                        if (_aclService.Authorize(category) && _storeMappingService.Authorize(category, storeId))
                            result.Add(pc);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allProductCategories);
                }
                return result;
            });
        }


        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public virtual ProductCategory GetProductCategoryById(int productCategoryId)
        {
            if (productCategoryId == 0)
                return null;

            return _productCategoryRepository.GetById(productCategoryId);
        }



        /// <summary>
        /// Gets vendor category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendor a category mapping collection</returns>
        public virtual IPagedList<VendorMappedCategory> GetVendorMappedCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (categoryId == 0)
                return new PagedList<VendorMappedCategory>(new List<VendorMappedCategory>(), pageIndex, pageSize);

            string key = string.Format(VENDORMAPPEDCATEGORIES_ALLBYCATEGORYID_KEY, categoryId, showHidden, pageIndex, pageSize, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            return _cacheManager.Get(key, () =>
            {
                var query = from vmc in _vendorMappedCategoryRepository.Table
                            join p in _vendorRepository.Table on vmc.VendorId equals p.Id
                            where vmc.CategoryId == categoryId &&
                                  !p.Deleted &&
                                  (showHidden || p.Active)
                            orderby vmc.DisplayOrder, vmc.Id
                            select vmc;

                if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
                {
                    if (!_catalogSettings.IgnoreAcl)
                    {
                        //ACL (access control list)
                        var allowedCustomerRolesIds = _workContext.CurrentCustomer.GetCustomerRoleIds();
                        query = from pc in query
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join acl in _aclRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select pc;
                    }
                    if (!_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.CurrentStore.Id;
                        query = from pc in query
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
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

                var vendorMappedCategories = new PagedList<VendorMappedCategory>(query, pageIndex, pageSize);
                return vendorMappedCategories;
            });
        }


        /// <summary>
        /// Gets a vendor category mapping collection
        /// </summary>
        /// <param name="VendorId">Vendor identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<VendorMappedCategory> GetVendorMappedCategoriesByVendorId(int vendorId, bool showHidden = false)
        {
            return GetVendorMappedCategoriesByVendorId(vendorId, _storeContext.CurrentStore.Id, showHidden);
        }


        /// <summary>
        /// Gets a vendor category mapping collection
        /// </summary>
        /// <param name="vendorId">Product identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        public virtual IList<VendorMappedCategory> GetVendorMappedCategoriesByVendorId(int vendorId, int storeId, bool showHidden = false)
        {
            if (vendorId == 0)
                return new List<VendorMappedCategory>();

            string key = string.Format(VENDORMAPPEDCATEGORIES_ALLBYVENDORID_KEY, vendorId, showHidden, _workContext.CurrentCustomer.Id, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from vmc in _vendorMappedCategoryRepository.Table
                            join c in _categoryRepository.Table on vmc.CategoryId equals c.Id
                            where vmc.VendorId == vendorId &&
                                  !c.Deleted &&
                                  (showHidden || c.Published)
                            orderby vmc.DisplayOrder, vmc.Id
                            select vmc;

                var allVendorMappedCategories = query.ToList();
                var result = new List<VendorMappedCategory>();
                if (!showHidden)
                {
                    foreach (var pc in allVendorMappedCategories)
                    {
                        //ACL (access control list) and store mapping
                        var category = pc.Category;
                        if (_aclService.Authorize(category) && _storeMappingService.Authorize(category, storeId))
                            result.Add(pc);
                    }
                }
                else
                {
                    //no filtering
                    result.AddRange(allVendorMappedCategories);
                }
                return result;
            });
        }


        /// <summary>
        /// Gets a vendor category mapping 
        /// </summary>
        /// <param name="vendorMappedCategoryId">vendor mapped category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        public virtual VendorMappedCategory GetVendorMappedCategoryById(int vendorMappedCategoryId)
        {
            if (vendorMappedCategoryId == 0)
                return null;

            return _vendorMappedCategoryRepository.GetById(vendorMappedCategoryId);
        }

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void InsertProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            _productCategoryRepository.Insert(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productCategory);
        }

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void UpdateProductCategory(ProductCategory productCategory)
        {
            if (productCategory == null)
                throw new ArgumentNullException("productCategory");

            _productCategoryRepository.Update(productCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productCategory);
        }


        /// <summary>
        /// Inserts a vendor mapped category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void InsertVendorMappedCategory(VendorMappedCategory vendorMappedCategory)
        {
            if (vendorMappedCategory == null)
                throw new ArgumentNullException("vendorMappedCategory");

            _vendorMappedCategoryRepository.Insert(vendorMappedCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORMAPPEDCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(vendorMappedCategory);
        }

        /// <summary>
        /// Updates the vendor mapped category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        public virtual void UpdateVendorMappedCategory(VendorMappedCategory vendorMappedCategory)
        {
            if (vendorMappedCategory == null)
                throw new ArgumentNullException("vendorMappedCategory");

            _vendorMappedCategoryRepository.Update(vendorMappedCategory);

            //cache
            _cacheManager.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORMAPPEDCATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(vendorMappedCategory);
        }


        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        public virtual string[] GetNotExistingCategories(string[] categoryNames)
        {
            if (categoryNames == null)
                throw new ArgumentNullException("categoryNames");

            var query = _categoryRepository.Table;
            var queryFilter = categoryNames.Distinct().ToArray();
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();

            return queryFilter.Except(filter).ToArray();
        }

        /// <summary>
        /// Returns a list of names of not existing vendor categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        public virtual string[] GetNotExistingVendorCategories(List<string> categoryNames, int vendorId)
        {
            if (categoryNames == null)
                throw new ArgumentNullException("categoryNames");

            var query = _categoryRepository.Table;
            query = query.Where(x => x.VendorId == vendorId);
            var queryFilter = categoryNames.Distinct().ToArray();
            var filter = query.Select(c => c.Name).Where(c => queryFilter.Contains(c)).ToList();

            return queryFilter.Except(filter).ToArray();
        }


        /// <summary>
        /// Get category IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Category IDs for products</returns>
        public virtual IDictionary<int, int[]> GetProductCategoryIds(int[] productIds)
        {
            var query = _productCategoryRepository.Table;

            return query.Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.CategoryId }).ToList()
                .GroupBy(a => a.ProductId)
                .ToDictionary(items => items.Key, items => items.Select(a => a.CategoryId).ToArray());
        }

        /// <summary>
        /// Gets list of categories
        /// </summary>
        /// <param name="VendorId">Vendor identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<Category> GetCategoriesByVendorId(int vendorId, bool showHidden = false)
        {

            var query = from vmc in _vendorMappedCategoryRepository.Table
                        join c in _categoryRepository.Table on vmc.CategoryId equals c.ParentCategoryId
                        where vmc.VendorId == vendorId && c.VendorId == vendorId &&
                              !c.Deleted &&
                              (showHidden || c.Published)
                        orderby c.DisplayOrder
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
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="VendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="categoryID">Category identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IPagedList<Category> SearchVendorCategories(string categoryName = "", int storeId = 0, int vendorId = 0,
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
                nameParameter.Value = categoryName ?? string.Empty;
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
                var categories = _dbContext.ExecuteStoredProcedureList<Category>("CategoryLoadAllPagedAdvance",
                    showHiddenParameter, showMarketPlaceParameter, showFeaturedParameter, nameParameter, storeIdParameter, vendorIdParameter, customerRoleIdsParameter,
                    pageIndexParameter, pageSizeParameter, totalRecordsParameter);
                var totalRecords = (totalRecordsParameter.Value != DBNull.Value) ? Convert.ToInt32(totalRecordsParameter.Value) : 0;

                //paging
                return new PagedList<Category>(categories, pageIndex, pageSize, totalRecords);
            }
            else
            {
                //stored procedures aren't supported. Use LINQ
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                if (ShowFeatured)
                    query = query.Where(c => c.ShowOnHomePage);
                if (!String.IsNullOrWhiteSpace(categoryName))
                    query = query.Where(c => c.Name.Contains(categoryName));
                query = query.Where(c => !c.Deleted);
                query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
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
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = acl.EntityId, c2 = acl.EntityName } into c_acl
                                from acl in c_acl.DefaultIfEmpty()
                                where !c.SubjectToAcl || allowedCustomerRolesIds.Contains(acl.CustomerRoleId)
                                select c;
                    }
                    if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                                from sm in c_sm.DefaultIfEmpty()
                                where !c.LimitedToStores || storeId == sm.StoreId
                                select c;
                    }

                    if (vendorId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        query = from c in query
                                join sm in _storeMappingRepository.Table
                                on new { c1 = c.Id, c2 = "Category" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
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
                    query = query.OrderBy(c => c.ParentCategoryId).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Id);
                }

                var unsortedCategories = query.ToList();

                //sort categories
                var sortedCategories = unsortedCategories.SortCategoriesForTree();

                //paging
                return new PagedList<Category>(sortedCategories, pageIndex, pageSize);
            }
        }

        #endregion

        #region Extend Services

        /// <summary>
        /// Disable Category by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        public virtual void DisableVendorCategoriesByVendorId(int vendorId = 0)
        {
            var categorys = _categoryRepository.Table;
            categorys = categorys.Where(v => v.VendorId == vendorId);

            foreach (var category in categorys)
            {
                category.Published = false;
            }
            //update
            _categoryRepository.Update(categorys);

            //event notification
            foreach (var category in categorys)
            {
                _eventPublisher.EntityUpdated(category);
            }

        }

        /// <summary>
        /// Enable Category by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        public virtual void EnableVendorCategoriesByVendorId(int vendorId = 0)
        {
            var categorys = _categoryRepository.Table;
            categorys = categorys.Where(v => v.VendorId == vendorId);

            foreach (var category in categorys)
            {
                category.Published = true;
            }
            //update
            _categoryRepository.Update(categorys);

            //event notification
            foreach (var category in categorys)
            {
                _eventPublisher.EntityUpdated(category);
            }

        }


        /// <summary>
        /// Disable Products by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        public virtual void DisableVendorProductsByVendorId(int vendorId = 0)
        {
            var products = _productRepository.Table;
            products = products.Where(v => v.VendorId == vendorId);

            foreach (var product in products)
            {
                product.Published = false;
            }
            //update
            _productRepository.Update(products);

            //event notification
            foreach (var product in products)
            {
                _eventPublisher.EntityUpdated(product);
            }

        }

        /// <summary>
        /// Enable Products by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        public virtual void EnableVendorProductsByVendorId(int vendorId = 0)
        {
            var products = _productRepository.Table;
            products = products.Where(v => v.VendorId == vendorId);

            foreach (var product in products)
            {
                product.Published = true;
            }
            //update
            _productRepository.Update(products);

            //event notification
            foreach (var product in products)
            {
                _eventPublisher.EntityUpdated(product);
            }

        }


        /// <summary>
        /// Enable Category Product by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        public virtual void EnableCategoryProductByCategoryId(int categoryId = 0)
        {
            var productcategory = _productCategoryRepository.Table;
            var products = _productRepository.Table;

            productcategory = productcategory.Where(x => x.CategoryId == categoryId);

            var pro = new List<Product>();
            foreach (var pcategory in productcategory.ToList())
            {
                pcategory.Product.Published = true;
                pro.Add(pcategory.Product);
            }

            //update
            _productRepository.Update(pro);

            //event notification
            foreach (var product in pro)
            {
                _eventPublisher.EntityUpdated(product);
            }
        }

        /// <summary>
        /// Disable Category Product by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        public virtual void DisableCategoryProductByCategoryId(int categoryId = 0)
        {
            var productcategory = _productCategoryRepository.Table;
            var products = _productRepository.Table;

            productcategory = productcategory.Where(x => x.CategoryId == categoryId);

            var pro = new List<Product>();
            foreach (var pcategory in productcategory.ToList())
            {
                pcategory.Product.Published = false;
                pro.Add(pcategory.Product);
            }

            //update
            _productRepository.Update(pro);

            //event notification
            foreach (var product in pro)
            {
                _eventPublisher.EntityUpdated(product);
            }
        }

        /// <summary>
        /// Gets Searchable category mapping collection
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public virtual IPagedList<Category> GetSearchableCategories(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            string key = string.Format(SEARCHCATEGORIES_ALL, showHidden, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {

                var query = from c in _categoryRepository.Table
                            orderby c.DisplayOrder, c.Id
                            where c.Searchable &&
                            !c.Deleted &&
                                  (showHidden || c.Published)
                            select c;

                var searchCategories = new PagedList<Category>(query, pageIndex, pageSize);
                return searchCategories;
            });
        }

        /// <summary>
        /// Gets Searchable category mapping collection
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        public virtual IPagedList<Category> GetCollectionCategories(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            string key = string.Format(SEARCHCATEGORIES_ALLCOLLECTION, showHidden, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {

                var query = from c in _categoryRepository.Table
                            orderby c.DisplayOrder, c.Id
                            where c.Collection &&
                            !c.Deleted &&
                                  (showHidden || c.Published)
                            select c;

                var searchCategories = new PagedList<Category>(query, pageIndex, pageSize);
                return searchCategories;
            });
        }

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        public virtual string GetFormattedBreadCrumb(Category category, IList<Category> allCategories = null,
            string separator = ">>", int languageId = 0)
        {
            var result = string.Empty;

            var breadcrumb = GetCategoryBreadCrumb(category, allCategories, true);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                var categoryName = _localizationService.GetLocalized(breadcrumb[i], x => x.Name, languageId);
                result = string.IsNullOrEmpty(result) ? categoryName : $"{result} {separator} {categoryName}";
            }

            return result;
        }

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        public virtual IList<Category> GetCategoryBreadCrumb(Category category, IList<Category> allCategories = null, bool showHidden = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            string breadcrumbCacheKey = string.Format(CATEGORY_BREADCRUMB_CACHE_KEY, category,
                CustomerExtensions.GetCustomerRoleIds(_workContext.CurrentCustomer),
                _storeContext.CurrentStore,
                _workContext.WorkingLanguage);

            return _cacheManager.Get(breadcrumbCacheKey, () =>
            {
                var result = new List<Category>();

                //used to prevent circular references
                var alreadyProcessedCategoryIds = new List<int>();

                while (category != null && //not null
                       !category.Deleted && //not deleted
                       (showHidden || category.Published) && //published
                       (showHidden || _aclService.Authorize(category)) && //ACL
                       (showHidden || _storeMappingService.Authorize(category)) && //Store mapping
                       !alreadyProcessedCategoryIds.Contains(category.Id)) //prevent circular references
                {
                    result.Add(category);

                    alreadyProcessedCategoryIds.Add(category.Id);

                    category = allCategories != null
                        ? allCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId)
                        : GetCategoryById(category.ParentCategoryId);
                }

                result.Reverse();

                return result;
            });
        }
        #endregion
    }
}
