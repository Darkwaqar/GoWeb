using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Helpers;
using Nop.Admin.Models.PrivateMessages;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Admin.Infrastructure.Cache;

namespace Nop.Admin.Controllers
{
    public partial class PrivateMessagesController : BaseAdminController
    {

        #region Fields

        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ForumSettings _forumSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public PrivateMessagesController(
            IForumService forumService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ForumSettings forumSettings,
            IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings,
            ICacheManager cacheManager)
        {
            this._forumService = forumService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumSettings = forumSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._customerSettings = customerSettings;
            this._cacheManager = cacheManager;
        }

        #endregion

        public ActionResult Index()
        {
            var model = new PrivateMessageIndexModel();
            model.InboxModel.AddRange(PrepareInboxModel());

            var ToCustomerId = model.InboxModel.Count > 0 ? model.InboxModel.FirstOrDefault().ToCustomerId != _workContext.CurrentCustomer.Id ? model.InboxModel.FirstOrDefault().ToCustomerId : model.InboxModel.FirstOrDefault().FromCustomerId : 0;
            if (ToCustomerId != 0)
            {
                model.ConversationsModel.AddRange(PrepareConverstationsModel(ToCustomerId));

            }
            model.NumberOfUnreadMessage = model.InboxModel.Sum(x => x.TotalUnread);
            model.FromCustomerId = _workContext.CurrentCustomer.Id;
            model.ToCustomerId = ToCustomerId;
            model.IsGuest = _workContext.CurrentCustomer.IsGuest();

            model.Inbox = this.RenderPartialViewToString("Inbox", model.InboxModel);
            model.Conversations = this.RenderPartialViewToString("Conversations", model.ConversationsModel);

            return PartialView(model);
        }

        private IEnumerable<PrivateMessageModel> PrepareConverstationsModel(int toCustomerId)
        {
            var model = new List<PrivateMessageModel>();
            var allmessage = _forumService.GetConverstation(_storeContext.CurrentStore.Id,
                 toCustomerId, _workContext.CurrentCustomer.Id, null, null, null, string.Empty);
            foreach (var pm in allmessage)
                model.Add(PreparePrivateMessageModel(pm));

            return model;
        }

        private IEnumerable<PrivateMessageModel> PrepareInboxModel()
        {
            var list = _forumService.GetInbox(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id, null, null, null, string.Empty);

            var customersOnline = _cacheManager.Get(string.Format(ModelCacheEventConsumer.ONLINE_INBOX_VENDOR_KEY, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id),
                   //as we cache per Store
                   //let's cache just for 3 minute
                   3, () =>
                   {
                       return _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes), null);
                   });
            return PrepareInboxModel(list, customersOnline);
        }


        public virtual PrivateMessageModel PreparePrivateMessageModel(PrivateMessage pm)
        {
            if (pm == null)
                throw new ArgumentNullException("pm");

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromCustomerId = pm.FromCustomer.Id,
                CustomerFromName = pm.FromCustomer.Email,
                ToCustomerId = pm.ToCustomer.Id,
                CustomerToName = pm.ToCustomer.Email,
                Message = pm.FormatPrivateMessageText(),
                CreatedOn = _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc),
                IsRead = pm.IsRead,
                AlignLeft = pm.FromCustomer.Id != _workContext.CurrentCustomer.Id,
            };
            if (!pm.IsRead && pm.ToCustomerId == _workContext.CurrentCustomer.Id)
            {
                pm.IsRead = true;
                _forumService.UpdatePrivateMessage(pm);
            }

            return model;
        }


        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Private message model</returns>
        public virtual List<PrivateMessageModel> PrepareInboxModel(IPagedList<PrivateMessage> pms, IPagedList<Customer> allCustomerOnline)
        {
            if (pms == null)
                throw new ArgumentNullException("pm");

            var model = new List<PrivateMessageModel>();

            foreach (var pm in pms)
            {
                var privateMessageModel = new PrivateMessageModel();
                if (!model.Any(x => x.ToCustomerId == pm.ToCustomerId && x.FromCustomerId == pm.FromCustomerId || x.ToCustomerId == pm.FromCustomerId && x.FromCustomerId == pm.ToCustomerId))
                {
                    var customerId = _workContext.CurrentCustomer.Id == pm.FromCustomer.Id ? pm.ToCustomer.Id : pm.FromCustomer.Id;
                    privateMessageModel.Id = pm.Id;
                    privateMessageModel.FromCustomerId = pm.FromCustomer.Id;
                    privateMessageModel.CustomerFromName = pm.FromCustomer.FormatUserName() == null ? pm.FromCustomer.Email : pm.FromCustomer.FormatUserName();
                    privateMessageModel.AllowViewingFromProfile = _customerSettings.AllowViewingProfiles && pm.FromCustomer != null && !pm.FromCustomer.IsGuest();
                    privateMessageModel.ToCustomerId = pm.ToCustomer.Id;
                    privateMessageModel.CustomerToName = pm.ToCustomer.FormatUserName() == null ? pm.ToCustomer.Email : pm.ToCustomer.FormatUserName();
                    privateMessageModel.AllowViewingToProfile = _customerSettings.AllowViewingProfiles && pm.ToCustomer != null && !pm.ToCustomer.IsGuest();
                    privateMessageModel.Message = pm.FormatPrivateMessageText();
                    privateMessageModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc);
                    privateMessageModel.IsRead = pm.IsRead;
                    privateMessageModel.AlignLeft = pm.FromCustomer.Id != _workContext.CurrentCustomer.Id;
                    privateMessageModel.Deleted = pm.IsDeletedByAuthor || pm.IsDeletedByRecipient;
                    privateMessageModel.Online = allCustomerOnline.Any(x => x.Id == customerId);
                    privateMessageModel.Count = 1;
                    privateMessageModel.TotalUnread = pm.IsRead ? 0 : 1;

                    model.Add(privateMessageModel);
                }
                else
                {
                    privateMessageModel = model.Where(x => x.ToCustomerId == pm.ToCustomerId && x.FromCustomerId == pm.FromCustomerId || x.ToCustomerId == pm.FromCustomerId && x.FromCustomerId == pm.ToCustomerId).FirstOrDefault();
                    privateMessageModel.Count = privateMessageModel.Count + 1;
                    privateMessageModel.TotalUnread = pm.IsRead ? privateMessageModel.TotalUnread : privateMessageModel.TotalUnread + 1;
                }
            }

            return model;
        }


    }
}