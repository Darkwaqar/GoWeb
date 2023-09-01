using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Polls;


namespace Nop.Services.Polls
{
    /// <summary>
    /// PollCategory service interface
    /// </summary>
    public partial interface IPollCategoryService
    {
        /// <summary>
        /// Delete pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        void DeletePollCategory(PollCategory pollCategory);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="pollCategoryName">PollCategory name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<PollCategory> GetAllCategories(string pollCategoryName = "", int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<PollCategory> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false);

        /// <summary>
        /// Gets all collection displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IList<PollCategory> GetAllCollectionDisplayedOnHomePage(bool showHidden = false);

        /// <summary>
        /// Gets a pollCategory
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        /// <returns>PollCategory</returns>
        PollCategory GetPollCategoryById(int pollCategoryId);

        /// <summary>
        /// Inserts pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        void InsertPollCategory(PollCategory pollCategory);

        /// <summary>
        /// Updates the pollCategory
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        void UpdatePollCategory(PollCategory pollCategory);

        /// <summary>
        /// Deletes a poll pollCategory mapping
        /// </summary>
        /// <param name="pollPollCategory">Poll pollCategory</param>
        void DeletePollPollCategory(PollPollCategory pollPollCategory);


        /// <summary>
        /// Gets poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll a pollCategory mapping collection</returns>
        IPagedList<PollPollCategory> GetPollCategoriesByPollCategoryId(int pollCategoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Poll pollCategory mapping collection</returns>
        IList<PollPollCategory> GetPollCategoriesByPollId(int pollId, bool showHidden = false);
        /// <summary>
        /// Gets a poll pollCategory mapping collection
        /// </summary>
        /// <param name="pollId">Poll identifier</param>
        /// <param name="storeId">Store identifier (used in multi-store environment). "showHidden" parameter should also be "true"</param>
        /// <param name="showHidden"> A value indicating whether to show hidden records</param>
        /// <returns> Poll pollCategory mapping collection</returns>
        IList<PollPollCategory> GetPollCategoriesByPollId(int pollId, int storeId, bool showHidden = false);

        /// <summary>
        /// Gets a poll pollCategory mapping 
        /// </summary>
        /// <param name="pollPollCategoryId">Poll pollCategory mapping identifier</param>
        /// <returns>Poll pollCategory mapping</returns>
        PollPollCategory GetPollPollCategoryById(int pollPollCategoryId);

        /// <summary>
        /// Inserts a poll pollCategory mapping
        /// </summary>
        /// <param name="pollPollCategory">>Poll pollCategory mapping</param>
        void InsertPollPollCategory(PollPollCategory pollPollCategory);

        /// <summary>
        /// Updates the poll pollCategory mapping 
        /// </summary>
        /// <param name="pollPollCategory">>Poll pollCategory mapping</param>
        void UpdatePollPollCategory(PollPollCategory pollPollCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="pollCategoryNames">The nemes of the categories to check</param>
        /// <returns>List of names not existing categories</returns>
        string[] GetNotExistingCategories(string[] pollCategoryNames);

        /// <summary>
        /// Get pollCategory IDs for polls
        /// </summary>
        /// <param name="pollIds">Polls IDs</param>
        /// <returns>PollCategory IDs for polls</returns>
        IDictionary<int, int[]> GetPollPollCategoryIds(int[] pollIds);


        /// <summary>
        /// Enable PollCategory Poll by PollCategory identifier
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        void EnablePollCategoryPollByPollCategoryId(int pollCategoryId = 0);

        /// <summary>
        /// Disable PollCategory Poll by PollCategory identifier
        /// </summary>
        /// <param name="pollCategoryId">PollCategory identifier</param>
        void DisablePollCategoryPollByPollCategoryId(int pollCategoryId = 0);
        /// <summary>
        /// Gets all Vendor categories
        /// </summary>
        /// <param name="pollCategoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="vendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="isFeatured">A value indicating whether to show featured records</param>
        /// <returns>Categories</returns>
        IPagedList<PollCategory> SearchVendorCategories(string pollCategoryName = "", int storeId = 0,
            int vendorId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool showMarketplace = false, bool ShowFeatured = false);
    }
}
