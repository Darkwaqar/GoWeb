using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ICategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteCategory(Category category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Category> GetAllCategories(string categoryName = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="includeAllLevels">A value indicating whether we should load all child levels</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesByParentCategoryId(int parentCategoryId,
            bool showHidden = false, bool includeAllLevels = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false);


        /// <summary>
        /// Gets all categories displayed on the home page by vendorId
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="vendorId">Vendor Indicator</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllFeaturedCategoriesByVendorId(int vendorId = 0, bool showHidden = false);


        /// <summary>
        /// Gets all collection displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<Category> GetAllCollectionDisplayedOnHomePage(bool showHidden = false);

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        void InsertCategory(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        void UpdateCategory(Category category);

        /// <summary>
        /// Deletes a product category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        void DeleteProductCategory(ProductCategory productCategory);


        /// <summary>
        /// Deletes a vendor mapped category mapping
        /// </summary>
        /// <param name="productCategory">Product category</param>
        void DeleteVendorMappedCategory(VendorMappedCategory vendorMappedCategory);

        /// <summary>
        /// Gets product category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<ProductCategory> GetProductCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        IList<ProductCategory> GetProductCategoriesByProductId(int productId, bool showHidden = false);
        /// <summary>
        /// Gets a product category mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        IList<ProductCategory> GetProductCategoriesByProductId(int productId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a product category mapping 
        /// </summary>
        /// <param name="productCategoryId">Product category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        ProductCategory GetProductCategoryById(int productCategoryId);


        /// <summary>
        /// Gets vendor mapped category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<VendorMappedCategory> GetVendorMappedCategoriesByCategoryId(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a vendor category mapping collection
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        IList<VendorMappedCategory> GetVendorMappedCategoriesByVendorId(int vendorId, bool showHidden = false);
        /// <summary>
        /// Gets a vendor category mapping collection
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Product category mapping collection</returns>
        IList<VendorMappedCategory> GetVendorMappedCategoriesByVendorId(int vendorId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a vendor category mapping 
        /// </summary>
        /// <param name="vendorMappedCategoryId">vendor mapped category mapping identifier</param>
        /// <returns>Product category mapping</returns>
        VendorMappedCategory GetVendorMappedCategoryById(int vendorMappedCategoryId);

        /// <summary>
        /// Inserts a product category mapping
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void InsertProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Updates the product category mapping 
        /// </summary>
        /// <param name="productCategory">>Product category mapping</param>
        void UpdateProductCategory(ProductCategory productCategory);

        /// <summary>
        /// Inserts a Vendor category mapping
        /// </summary>
        /// <param name="vendorMappedCategory">>Vendor mapped category mapping</param>
        void InsertVendorMappedCategory(VendorMappedCategory vendorMappedCategory);

        /// <summary>
        /// Updates the Vendor mapped category mapping 
        /// </summary>
        /// <param name="productCategory">>Vendor Mapped category mapping</param>
        void UpdateVendorMappedCategory(VendorMappedCategory productCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        string[] GetNotExistingCategories(string[] categoryNames);

         /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        string[] GetNotExistingVendorCategories(List<string> categoryNames, int vendorId);

        /// <summary>
        /// Get category IDs for products
        /// </summary>
        /// <param name="productIds">Products IDs</param>
        /// <returns>Category IDs for products</returns>
        IDictionary<int, int[]> GetProductCategoryIds(int[] productIds);

        /// <summary>
        /// Gets list of category
        /// </summary>
        /// <param name="VendorId">Vendor identifier</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<Category> GetCategoriesByVendorId(int vendorId, bool showHidden = false);


        /// <summary>
        /// Gets all Vendor categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="vendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="isFeatured">A value indicating whether to show featured records</param>
        /// <returns>Categories</returns>
        IPagedList<Category> SearchVendorCategories(string categoryName = "", int storeId = 0,
            int vendorId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool showMarketplace = false, bool ShowFeatured = false);

        /// <summary>
        /// Disable Category by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        void DisableVendorCategoriesByVendorId(int vendorId = 0);

        /// <summary>
        /// Enable Category by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        void EnableVendorCategoriesByVendorId(int vendorId = 0);

        /// <summary>
        /// Disable Produts by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        void DisableVendorProductsByVendorId(int vendorId = 0);

        /// <summary>
        /// Enable Products by Vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        void EnableVendorProductsByVendorId(int vendorId = 0);


        /// <summary>
        /// Enable Category Product by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        void EnableCategoryProductByCategoryId(int categoryId = 0);

        /// <summary>
        /// Disable Category Product by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        void DisableCategoryProductByCategoryId(int categoryId = 0);

        /// <summary>
        /// Gets Searchable category mapping collection
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<Category> GetSearchableCategories(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets Collection category mapping collection
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product a category mapping collection</returns>
        IPagedList<Category> GetCollectionCategories(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);


        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>Formatted breadcrumb</returns>
        string GetFormattedBreadCrumb(Category category, IList<Category> allCategories = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Category breadcrumb </returns>
        IList<Category> GetCategoryBreadCrumb(Category category, IList<Category> allCategories = null, bool showHidden = false);
        
    }
}
