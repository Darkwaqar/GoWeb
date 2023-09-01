using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Vendors;
using Nop.Core.Domain.Catalog;
using Nop.Services.Stores;
using Nop.Core.Domain.Stores;
using Nop.Services.Polls;

namespace Nop.Admin.Helpers
{
    /// <summary>
    /// Select list helper
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetCategoryList(ICategoryService categoryService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (categoryService == null)
                throw new ArgumentNullException("categoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORIES_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var categories = categoryService.GetAllCategories(showHidden: showHidden);

                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get pollCategory list
        /// </summary>
        /// <param name="pollCategoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetPollCategoryList(IPollCategoryService pollCategoryService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (pollCategoryService == null)
                throw new ArgumentNullException("pollCategoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.POLLCATEGORIES_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var categories = pollCategoryService.GetAllCategories(showHidden: showHidden);

                return categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get GetMarketPlaceCategory list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetMarketPlaceCategoryList(ICategoryService categoryService, ICacheManager cacheManager, bool showHidden = false, int vendorId = 0)
        {
            if (categoryService == null)
                throw new ArgumentNullException("categoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.AVALIBLE_VENDOR_MARKET_CATEGORIES_LIST_KEY, vendorId, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                IList<Category> categories = categoryService.GetAllCategories(showHidden: showHidden);
                if (vendorId == 0)
                {
                    categories= categories.Where(x => x.VendorId == vendorId).ToList();
                }
                else
                {
                    var vendorMappedCategories = categoryService.GetVendorMappedCategoriesByVendorId(vendorId, showHidden).Select(x => x.CategoryId);
                    var categoryIds = new List<int>();
                    categoryIds.AddRange(vendorMappedCategories);
                    foreach (var vendorMappedCategory in vendorMappedCategories)
                    {
                        //include subcategories
                        categoryIds.AddRange(GetChildCategoryIds(vendorMappedCategory, categoryService));
                    }
                    categories = categories.Where(x => categoryIds.Contains(x.Id)).ToList();
                }

                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        public static List<int> GetChildCategoryIds(int parentCategoryId, ICategoryService categoryService)
        {
            var categoriesIds = new List<int>();
            var categories = categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, false, true);
            foreach (var category in categories)
            {
                if (category.VendorId==0)
                {
                    categoriesIds.Add(category.Id);
                }
            }
            return categoriesIds;
        }

        /// <summary>
        /// Get GetMarketPlaceCategory list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetVendorCategoriesList(ICategoryService categoryService, ICacheManager cacheManager, bool showHidden = false, int vendorId = 0)
        {
            if (categoryService == null)
                throw new ArgumentNullException("categoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.VENDOR_CATEGORIES_LIST_KEY, vendorId, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {

                IList<Category> categories;
                if (vendorId == 0)
                {
                    categories = categoryService.GetAllCategories(showHidden: showHidden).ToList();
                }
                else
                {
                    var vendorMappedCategories = categoryService.GetVendorMappedCategoriesByVendorId(vendorId, true).Select(c => c.CategoryId).ToList();
                    categories = categoryService.GetAllCategories(showHidden: showHidden).Where(cat => cat.VendorId == vendorId || vendorMappedCategories.Contains(cat.Id)).ToList();
                }

                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get manufacturer list
        /// </summary>
        /// <param name="manufacturerService">Manufacturer service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer list</returns>
        public static List<SelectListItem> GetManufacturerList(IManufacturerService manufacturerService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (manufacturerService == null)
                throw new ArgumentNullException("manufacturerService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURERS_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var manufacturers = manufacturerService.GetAllManufacturers(showHidden: showHidden);
                return manufacturers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get vendor list
        /// </summary>
        /// <param name="vendorService">Vendor service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendor list</returns>
        public static List<SelectListItem> GetVendorList(IVendorService vendorService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (vendorService == null)
                throw new ArgumentNullException("vendorService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.VENDORS_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var vendors = vendorService.GetAllVendors(showHidden: showHidden);
                return vendors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        public static List<SelectListItem> GetVendorStoreList(IStoreMappingService storeMappingService, IStoreService storeService, ICacheManager cacheManager,
            IVendorService vendorService, int vendorId = 0)
        {
            if (storeMappingService == null)
                throw new ArgumentNullException("storeMappingService");

            if (storeService == null)
                throw new ArgumentNullException("storeService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            if (vendorService == null)
                throw new ArgumentNullException("vendorService");

            string cacheKey = string.Format(ModelCacheEventConsumer.VENDOR_STORE_LIST_KEY, vendorId);
            var listItems = cacheManager.Get(cacheKey, () =>
            {

                IList<Store> stores = storeService.GetAllStores();
                if (vendorId > 0)
                {
                    var vendor = vendorService.GetVendorById(vendorId);
                    var vendorMappedStore = storeMappingService.GetStoresIdsWithAccess(vendor).ToList();
                    stores = stores.ToList().Where(x => vendorMappedStore.Contains(x.Id)).ToList();
                }

                return stores.Select(store => new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }
    }
}