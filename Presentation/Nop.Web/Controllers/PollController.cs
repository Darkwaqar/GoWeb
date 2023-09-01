using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    public partial class PollController : BasePublicController
    {
        #region Fields

        private readonly IPollModelFactory _pollModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly IPollCategoryService _PollCategoryService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        #endregion

        #region Constructors

        public PollController(IPollModelFactory pollModelFactory,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPollService pollService,
            IPollCategoryService pollCategoryService,
             IAclService aclService,
              IStoreMappingService storeMappingService,
               IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._pollModelFactory = pollModelFactory;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._pollService = pollService;
            this._PollCategoryService = pollCategoryService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
        }

        #endregion


        #region Categories

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult PollCategory(int PollCategoryId, CatalogPagingFilteringModel command)
        {
            var PollCategory = _PollCategoryService.GetPollCategoryById(PollCategoryId);
            if (PollCategory == null || PollCategory.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !PollCategory.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(PollCategory) ||
                //Store mapping
                !_storeMappingService.Authorize(PollCategory);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();


            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "PollCategory", new { id = PollCategory.Id, area = "Admin" }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewPollCategory", PollCategory.Id, _localizationService.GetResource("ActivityLog.PublicStore.ViewPollCategory"), PollCategory.Name);
            EngineContext.Current.Resolve<ICustomerActionEventService>().Viewed(_workContext.CurrentCustomer, HttpContext.Request.Path.ToString(), "");

            //model
            var model = _pollModelFactory.PreparePollCategoryModel(PollCategory, command);

            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult PollCategoryNavigation(int currentPollCategoryId, int currentPollId)
        {
            var model = _pollModelFactory.PreparePollCategoryNavigationModel(currentPollCategoryId, currentPollId);
            return PartialView(model);
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public virtual ActionResult PollBlock(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return Content("");

            var model = _pollModelFactory.PreparePollModelBySystemName(systemKeyword);
            if (model == null)
                return Content("");

            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                return Json(new
                {
                    error = "No poll answer found with the specified id",
                });

            var poll = pollAnswer.Poll;
            if (!poll.Published)
                return Json(new
                {
                    error = "Poll is not available",
                });

            if (_workContext.CurrentCustomer.IsGuest() && !poll.AllowGuestsToVote)
                return Json(new
                {
                    error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote"),
                });

            bool alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
            if (!alreadyVoted)
            {
                //vote
                pollAnswer.PollVotingRecords.Add(new PollVotingRecord
                {
                    PollAnswerId = pollAnswer.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
                //update totals
                pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                _pollService.UpdatePoll(poll);
            }

            return Json(new
            {
                html = this.RenderPartialViewToString("_Poll", _pollModelFactory.PreparePollModel(poll, true)),
            });
        }
        
        [ChildActionOnly]
        public virtual ActionResult HomePagePolls()
        {
            var model = _pollModelFactory.PrepareHomePagePollModels();
            if (!model.Any())
                Content("");

            return PartialView(model);
        }

        #endregion

    }
}
