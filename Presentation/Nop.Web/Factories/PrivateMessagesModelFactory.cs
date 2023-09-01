using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Models.Common;
using Nop.Web.Models.PrivateMessages;
using System.Linq;
using Nop.Core.Caching;
using Nop.Web.Infrastructure.Cache;
using Nop.Services.Catalog;
using Nop.Services.Seo;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the private message model factory
    /// </summary>
    public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
    {
        #region Fields

        private readonly IForumService _forumService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ForumSettings _forumSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public PrivateMessagesModelFactory(IForumService forumService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper,
            ForumSettings forumSettings,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            ICacheManager cacheManager)
        {
            this._forumService = forumService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._dateTimeHelper = dateTimeHelper;
            this._forumSettings = forumSettings;
            this._customerSettings = customerSettings;
            this._customerService = customerService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message index model</returns>
        public virtual PrivateMessageIndexModel PreparePrivateMessageIndexModel()
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

            return model;
        }

        /// <summary>
        /// Prepare the inbox model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual IList<PrivateMessageModel> PrepareInboxModel()
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

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual IList<PrivateMessageModel> PrepareConverstationsModel(int toCustomerId = 0)
        {

            var model = new List<PrivateMessageModel>();
            var allmessage = _forumService.GetConverstation(_storeContext.CurrentStore.Id,
                 toCustomerId, _workContext.CurrentCustomer.Id, null, null, null, string.Empty);

            foreach (var pm in allmessage)
                model.Add(PreparePrivateMessageModel(pm));

            return model;
        }


        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual List<PrivateMessageModel> PrepareVendorModel()
        {

            var model = new List<PrivateMessageModel>();

            if (!_workContext.CurrentCustomer.IsGuest())
            {
                if (!_workContext.CurrentCustomer.IsVendor())
                {
                    //load and cache all vendor with online status 
                    model = _cacheManager.Get(string.Format(ModelCacheEventConsumer.ONLINE_VENDOR_KEY, _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id),
                    //as we cache per Store
                    //let's cache just for 3 minute
                    3, () =>
                    {
                        var allRoles = _customerService.GetAllCustomerRoles().Where(x => x.Name == SystemCustomerRoleNames.Vendors).Select(x => x.Id).ToArray();

                        var customers = _customerService.GetAllCustomers(StoreId: _storeContext.CurrentStore.Id, customerRoleIds: allRoles);
                        var customersOnline = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes), allRoles);
                        foreach (var pm in customers.OrderBy(x => x.Email))
                            model.Add(PreparePrivateMessageModel(pm, customersOnline));

                        return model;
                    });
                }
            }
            return model;
        }


        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message list model</returns>
        public virtual PrivateMessageListModel PrepareConversationModel(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();

            var list = _forumService.GetAllPrivateMessages(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id, 0, null, false, null, string.Empty, page, pageSize);
            foreach (var pm in list)
                messages.Add(PreparePrivateMessageModel(pm));

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { page = page, tab = tab }
            };

            var model = new PrivateMessageListModel
            {
                Messages = messages,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Private message model</returns>
        public virtual PrivateMessageModel PreparePrivateMessageModel(PrivateMessage pm)
        {
            if (pm == null)
                throw new ArgumentNullException("pm");

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromCustomerId = pm.FromCustomer.Id,
                CustomerFromName = pm.FromCustomer.FormatUserName() == null ? pm.FromCustomer.Email : pm.FromCustomer.FormatUserName(),
                AllowViewingFromProfile = _customerSettings.AllowViewingProfiles && pm.FromCustomer != null && !pm.FromCustomer.IsGuest(),
                ToCustomerId = pm.ToCustomer.Id,
                CustomerToName = pm.ToCustomer.FormatUserName() == null ? pm.ToCustomer.Email : pm.ToCustomer.FormatUserName(),
                AllowViewingToProfile = _customerSettings.AllowViewingProfiles && pm.ToCustomer != null && !pm.ToCustomer.IsGuest(),
                Message = pm.FormatPrivateMessageText(),
                CreatedOn = _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc),
                IsRead = pm.IsRead,
                AlignLeft = pm.FromCustomer.Id != _workContext.CurrentCustomer.Id,
                Deleted = pm.IsDeletedByAuthor || pm.IsDeletedByRecipient,
            };

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

        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>Private message model</returns>
        public virtual PrivateMessageModel PreparePrivateMessageModel(Customer pm, IPagedList<Customer> customerList)
        {
            if (pm == null)
                throw new ArgumentNullException("pm");

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromCustomerId = pm.Id,
                CustomerFromName = pm.FormatUserName() == null ? pm.Email : pm.FormatUserName(),
                ToCustomerId = pm.Id,
                CustomerToName = pm.FormatUserName() == null ? pm.Email : pm.FormatUserName(),
                Message = "",
                CreatedOn = _dateTimeHelper.ConvertToUserTime(pm.CreatedOnUtc, DateTimeKind.Utc),
                AlignLeft = pm.Id != _workContext.CurrentCustomer.Id,
                Online = customerList.Any(x => x.Id == pm.Id)
            };

            return model;
        }


        public virtual PrivateMessage PreparePrivateMessage(int vendorId = 0)
        {


            var customers = _cacheManager.Get(string.Format(ModelCacheEventConsumer.CHAT_ALL_VENDOR_CUSTOMER_KEY, _storeContext.CurrentStore.Id)
                   , () =>
                    {
                        var allRoles = _customerService.GetAllCustomerRoles().Where(x => x.Name == SystemCustomerRoleNames.Vendors).Select(x => x.Id).ToArray();
                        return _customerService.GetAllCustomers(StoreId: _storeContext.CurrentStore.Id, customerRoleIds: allRoles);
                    });

            var customer = customers.Where(x => x.VendorId == vendorId).FirstOrDefault();

            var nowUtc = DateTime.UtcNow;
            var privateMessage = new PrivateMessage
            {
                StoreId = _storeContext.CurrentStore.Id,
                ToCustomerId = customer.Id,
                ToCustomer = customer,
                FromCustomerId = _workContext.CurrentCustomer.Id,
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = nowUtc
            };


            return privateMessage;
        }
        #endregion
    }
}
