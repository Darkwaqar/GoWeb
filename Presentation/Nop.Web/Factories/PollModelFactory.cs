using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Polls;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Polls;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the poll model factory
    /// </summary>
    public partial class PollModelFactory : IPollModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IPollCategoryService _pollCategoryService;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Constructors

        public PollModelFactory(IWorkContext workContext,
            IPollService pollService,
            ICacheManager cacheManager,
            MediaSettings mediaSettings,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IPollCategoryService pollCategoryService,
            IWebHelper webHelper)
        {
            this._workContext = workContext;
            this._pollService = pollService;
            this._cacheManager = cacheManager;
            this._mediaSettings = mediaSettings;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._pollCategoryService = pollCategoryService;
            this._webHelper = webHelper;
        }

        #endregion

        #region pollCategories
        /// <summary>
        /// Prepare pollCategory model
        /// </summary>
        /// <param name="pollCategory">PollCategory</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>PollCategory model</returns>
        public virtual PollCategoryModel PreparePollCategoryModel(PollCategory pollCategory, CatalogPagingFilteringModel command)
        {
            if (pollCategory == null)
                throw new ArgumentNullException("pollCategory");

            var model = new PollCategoryModel
            {
                Id = pollCategory.Id,
                Name = pollCategory.GetLocalized(x => x.Name),
                Description = pollCategory.GetLocalized(x => x.Description),
                MetaKeywords = pollCategory.GetLocalized(x => x.MetaKeywords),
                MetaDescription = pollCategory.GetLocalized(x => x.MetaDescription),
                MetaTitle = pollCategory.GetLocalized(x => x.MetaTitle),
                SeName = pollCategory.GetSeName(),
            };

            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //prepare picture model
            var pollCategoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, pollCategory.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            model.PictureModel = _cacheManager.Get(pollCategoryPictureCacheKey, () =>
            {
                var picture = _pictureService.GetPictureById(pollCategory.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.PollCategory.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.PollCategory.ImageAlternateTextFormat"), model.Name)
                };
                return pictureModel;
            });
         

            var pollCategoryIds = new List<int>();
            pollCategoryIds.Add(pollCategory.Id);
            //polls
            IList<int> alreadyFilteredSpecOptionIds = new List<int>();
            
            var polls = _pollService.SearchPolls(
                pollCategoryIds: pollCategoryIds,
                storeId: _storeContext.CurrentStore.Id
               );
            model.Polls = PreparePollModels(polls).ToList();

            return model;
        }

       
        /// <summary>
        /// Prepare pollCategory navigation model
        /// </summary>
        /// <param name="currentPollCategoryId">Current pollCategory identifier</param>
        /// <param name="currentPollId">Current poll identifier</param>
        /// <returns>PollCategory navigation model</returns>
        public virtual PollCategoryNavigationModel PreparePollCategoryNavigationModel(int currentPollCategoryId, int currentPollId)
        {
            //get active pollCategory
            int activePollCategoryId = 0;
            if (currentPollCategoryId > 0)
            {
                //pollCategory details page
                activePollCategoryId = currentPollCategoryId;
            }
            else if (currentPollId > 0)
            {
                //poll details page
                var pollCategories = _pollCategoryService.GetPollCategoriesByPollId(currentPollId);
                if (pollCategories.Any())
                    activePollCategoryId = pollCategories[0].PollCategoryId;
            }

            var cachedCategoriesModel = PreparePollCategorySimpleModels();
            var model = new PollCategoryNavigationModel
            {
                CurrentPollCategoryId = activePollCategoryId,
                PollCategories = cachedCategoriesModel
            };

            return model;
        }
        /// <summary>
        /// Prepare homepage pollCategory models
        /// </summary>
        /// <returns>List of homepage pollCategory models</returns>
        public virtual List<PollCategoryModel> PrepareHomepagePollCategoryModels()
        {
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                pictureSize,
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id,
                _webHelper.IsCurrentConnectionSecured());

            var model = _cacheManager.Get(categoriesCacheKey, () =>
                _pollCategoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(pollCategory =>
                {
                    var catModel = new PollCategoryModel
                    {
                        Id = pollCategory.Id,
                        Name = pollCategory.GetLocalized(x => x.Name),
                        Description = pollCategory.GetLocalized(x => x.Description),
                        MetaKeywords = pollCategory.GetLocalized(x => x.MetaKeywords),
                        MetaDescription = pollCategory.GetLocalized(x => x.MetaDescription),
                        MetaTitle = pollCategory.GetLocalized(x => x.MetaTitle),
                        SeName = pollCategory.GetSeName(),
                    };

                    //prepare picture model
                    var pollCategoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, pollCategory.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    catModel.PictureModel = _cacheManager.Get(pollCategoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(pollCategory.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                            ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.PollCategory.ImageLinkTitleFormat"), catModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.PollCategory.ImageAlternateTextFormat"), catModel.Name)
                        };
                        return pictureModel;
                    });

                    return catModel;
                })
                .ToList()
            );

            return model;
        }


       
    

        /// <summary>
        /// Prepare pollCategory (simple) models
        /// </summary>
        /// <returns>List of pollCategory (simple) models</returns>
        public virtual List<PollCategorySimpleModel> PreparePollCategorySimpleModels()
        {
            //load and cache them
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () => PreparePollCategorySimpleModels(0));
        }

        /// <summary>
        /// Prepare pollCategory (simple) models
        /// </summary>
        /// <param name="rootPollCategoryId">Root pollCategory identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available pollCategories; pass null to load them internally</param>
        /// <returns>List of pollCategory (simple) models</returns>
        public virtual List<PollCategorySimpleModel> PreparePollCategorySimpleModels(int rootPollCategoryId,
            IList<PollCategory> allCategories = null)
        {
            var result = new List<PollCategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for pollCategories.
            //it'll load all pollCategories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentPollCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all pollCategories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var pollCategories = _pollCategoryService.GetAllCategoriesByParentPollCategoryId(rootPollCategoryId);
            if (allCategories == null)
            {
                //load pollCategories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all pollCategories are loaded only once
                allCategories = _pollCategoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
            }
            var pollCategories = allCategories;
            foreach (var pollCategory in pollCategories)
            {
                var pollCategoryModel = new PollCategorySimpleModel
                {
                    Id = pollCategory.Id,
                    Name = pollCategory.GetLocalized(x => x.Name),
                    SeName = pollCategory.GetSeName(),
                    IncludeInTopMenu = pollCategory.IncludeInTopMenu
                };

                result.Add(pollCategoryModel);
            }

            return result;
        }





        #endregion

        #region Methods


        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
        /// <returns>Poll model</returns>
        public virtual List<PollModel> PreparePollModels(IEnumerable<Poll> polls, bool setAlreadyVotedProperty = false)
        {
            if (polls == null)
                throw new ArgumentNullException("polls");

            var models = new List<PollModel>();

            foreach (var poll in polls)
            {
                models.Add(PreparePollModel(poll, setAlreadyVotedProperty)); ;
            }

            return models;
        }

        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
        /// <returns>Poll model</returns>
        public virtual PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id),
                Name = poll.Name
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;

            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            foreach (var pa in answers)
            {

                var PollAnswerModel = new PollAnswerModel
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0
                };
                //prepare picture model
                var pollPictureCacheKey = string.Format(ModelCacheEventConsumer.POLL_ANSWER_PICTURE_MODEL_KEY, pa.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                PollAnswerModel.PictureModel = _cacheManager.Get(pollPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(pa.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.poll.ImageLinkTitleFormat"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.poll.ImageAlternateTextFormat"), model.Name)
                    };
                    return pictureModel;
                });
                model.Answers.Add(PollAnswerModel);
            }
           

            return model;
        }

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        public virtual PollModel PreparePollModelBySystemName(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var cacheKey = string.Format(ModelCacheEventConsumer.POLL_BY_SYSTEMNAME_MODEL_KEY, systemKeyword, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                Poll poll = _pollService.GetPolls(languageId: _workContext.WorkingLanguage.Id, systemKeyword: systemKeyword).FirstOrDefault();
                if (poll == null)
                    //we do not cache nulls. that's why let's return an empty record (ID = 0)
                    return new PollModel { Id = 0};

                return PreparePollModel(poll, false);
            });
            if (cachedModel == null || cachedModel.Id == 0)
                return null;

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel)cachedModel.Clone();
            model.AlreadyVoted = _pollService.AlreadyVoted(model.Id, _workContext.CurrentCustomer.Id);

            return model;
        }

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        public virtual List<PollModel> PrepareHomePagePollModels()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_POLLS_MODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
                _pollService.GetPolls(_workContext.WorkingLanguage.Id, true)
                .Select(x => PreparePollModel(x, false))
                .ToList());
            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var p in cachedModel)
            {
                var pollModel = (PollModel) p.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }
            
            return model;
        }

        #endregion
    }
}
