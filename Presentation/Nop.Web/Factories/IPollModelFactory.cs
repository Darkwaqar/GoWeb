using System.Collections.Generic;
using Nop.Core.Domain.Polls;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the poll model factory
    /// </summary>
    public partial interface IPollModelFactory
    {
        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
        /// <returns>Poll model</returns>
        PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty);

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        PollModel PreparePollModelBySystemName(string systemKeyword);

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        List<PollModel> PrepareHomePagePollModels();


        #region Categories

        /// <summary>
        /// Prepare pollCategory model
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>PollCategory model</returns>
        PollCategoryModel PreparePollCategoryModel(PollCategory pollCategory, CatalogPagingFilteringModel command);


        /// <summary>
        /// Prepare pollCategory navigation model
        /// </summary>
        /// <param name="currentPollCategoryId">Current pollCategory identifier</param>
        /// <param name="currentProductId">Current product identifier</param>
        /// <returns>PollCategory navigation model</returns>
        PollCategoryNavigationModel PreparePollCategoryNavigationModel(int currentPollCategoryId,
            int currentPollId);

        /// <summary>
        /// Prepare homepage pollCategory models
        /// </summary>
        /// <returns>List of homepage pollCategory models</returns>
        List<PollCategoryModel> PrepareHomepagePollCategoryModels();

        /// <summary>
        /// Prepare pollCategory (simple) models
        /// </summary>
        /// <returns>List of pollCategory (simple) models</returns>
        List<PollCategorySimpleModel> PreparePollCategorySimpleModels();

        /// <summary>
        /// Prepare pollCategory (simple) models
        /// </summary>
        /// <param name="rootPollCategoryId">Root pollCategory identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>List of pollCategory (simple) models</returns>
        List<PollCategorySimpleModel> PreparePollCategorySimpleModels(int rootPollCategoryId,
             IList<PollCategory> allCategories = null);

        #endregion
    }
}
