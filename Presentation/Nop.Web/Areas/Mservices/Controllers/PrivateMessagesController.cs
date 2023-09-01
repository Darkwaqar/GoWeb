using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Factories;
using Nop.Web.Models.PrivateMessages;
using System.Linq;
using Nop.Web.Areas.MServices.Controllers;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public partial class PrivateMessagesController : BaseController
    {
        #region Fields

        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ForumSettings _forumSettings;

        #endregion

        #region Constructors

        public PrivateMessagesController(IPrivateMessagesModelFactory privateMessagesModelFactory,
            IForumService forumService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ForumSettings forumSettings)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
            this._forumService = forumService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumSettings = forumSettings;
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel();
            return View(model);
        }

        //inbox tab
        [HttpPost]
        public virtual ActionResult Inbox()
        {
            var model = _privateMessagesModelFactory.PrepareInboxModel();
            return Json(new
            {
                updateinboxsection = true,
                updateinboxsectionhtml = model,
                updateCustomerId = true,
                customerId = model.FirstOrDefault()?.ToCustomerId,
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),

            });
        }

        //sent items tab
        [HttpPost]
        public virtual ActionResult Conversations(int toCustomerId = 0)
        {
            var model = _privateMessagesModelFactory.PrepareConverstationsModel(toCustomerId);
            return Json(new
            {
                updateconversationsection = true,
                updateconversationsectionhtml = model,
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),
                scrollToBottom = true,
                updateCustomerId = true,
                customerId = toCustomerId,
            });
        }

        //sent items tab
        public virtual ActionResult Vendors()
        {
            var model = _privateMessagesModelFactory.PrepareVendorModel();
            return View(model);
        }
        
        [HttpPost]
        public virtual ActionResult DeleteMessage(int privateMessageId = 0)
        {
            PrivateMessage pm = _forumService.GetPrivateMessageById(privateMessageId);

            if (pm != null)
            {
                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }

            var conversations = _privateMessagesModelFactory.PrepareConverstationsModel(pm.FromCustomerId != _workContext.CurrentCustomer.Id ? pm.FromCustomerId : pm.ToCustomerId);
            return Json(new
            {
                updateconversationsection = true,
                updateconversationsectionhtml = conversations,
                updatetotalsection = true,
                totalsectionhtml = conversations.Where(x => !x.IsRead).Count(),
                sendcustomerupdate = true,
            });
        }

        [HttpPost]
        public virtual ActionResult ViewMessage(int privateMessageId = 0)
        {
            var customer = _workContext.CurrentCustomer;
            if (customer.IsVendor() || customer.IsAdmin())
            {
                PrivateMessage pm = _forumService.GetPrivateMessageById(privateMessageId);
                if (pm != null)
                {
                    return Json(new
                    {
                        updateMessage = true,
                        updateMessagehtml = pm.Text,
                        privateMessageId = privateMessageId,
                    });
                }
            }
            return Json(new
            {
                updateMessage = false,
                updateMessagehtml = "",
                privateMessageId = privateMessageId,
            });
        }

        [HttpPost]
        public virtual ActionResult DeleteAllMessageFromCustomerId(int fromCustomerId = 0)
        {
            var allmessage = _forumService.GetConverstation(_storeContext.CurrentStore.Id,
                  fromCustomerId, _workContext.CurrentCustomer.Id, null, null, null, string.Empty);
            foreach (var pm in allmessage)
            {
                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }

            var model = _privateMessagesModelFactory.PrepareInboxModel();
            var conversations = _privateMessagesModelFactory.PrepareConverstationsModel(fromCustomerId != _workContext.CurrentCustomer.Id ? fromCustomerId : _workContext.CurrentCustomer.Id);
            return Json(new
            {
                updateinboxsection = true,
                updateinboxsectionhtml = model,
                updateCustomerId = true,
                customerId = 0,
                updateconversationsection = true,
                updateconversationsectionhtml = conversations,
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),
                sendcustomerupdate = true,
            });

        }

        [HttpPost]
        public virtual ActionResult ReadAllMessageFromCustomerId(int fromCustomerId = 0)
        {
            var allmessage = _forumService.GetConverstation(_storeContext.CurrentStore.Id,
                  fromCustomerId, _workContext.CurrentCustomer.Id, null, null, null, string.Empty);
            foreach (var pm in allmessage)
            {
                if (!pm.IsRead && pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsRead = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }

            var model = _privateMessagesModelFactory.PrepareInboxModel();
            var conversations = _privateMessagesModelFactory.PrepareConverstationsModel(fromCustomerId != _workContext.CurrentCustomer.Id ? fromCustomerId : _workContext.CurrentCustomer.Id);
            return Json(new
            {
                updateconversationsection = true,
                updateconversationsectionhtml = conversations,
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),
            });

        }

        [HttpPost]
        public virtual ActionResult SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return InvokeHttp400("Private message is disable");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return InvokeHttp400("Guest count not Send Message");
            }
            //first PM
            Customer toCustomer = _customerService.GetCustomerById(model.ToCustomerId);

            if (toCustomer == null || toCustomer.IsGuest())
            {
                return InvokeHttp400("Guest count not Send Message");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var text = model.Message;
                    if (_forumSettings.PMTextMaxLength > 0 && text.Length > _forumSettings.PMTextMaxLength)
                    {
                        text = text.Substring(0, _forumSettings.PMTextMaxLength);
                    }

                    var nowUtc = DateTime.UtcNow;

                    var privateMessage = new PrivateMessage
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        ToCustomerId = toCustomer.Id,
                        FromCustomerId = _workContext.CurrentCustomer.Id,
                        Text = text,
                        IsDeletedByAuthor = false,
                        IsDeletedByRecipient = false,
                        IsRead = false,
                        CreatedOnUtc = nowUtc
                    };

                    _forumService.InsertPrivateMessage(privateMessage);

                    //activity log
                    _customerActivityService.InsertActivity("PublicStore.SendPM", _localizationService.GetResource("ActivityLog.PublicStore.SendPM"), toCustomer.Email);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var conversations = _privateMessagesModelFactory.PrepareConverstationsModel(toCustomer.Id);
            var inbox = _privateMessagesModelFactory.PrepareInboxModel();
            return Json(new
            {
                updateinboxsection = true,
                updateinboxsectionhtml = inbox,
                updateCustomerId = true,
                customerId = toCustomer.Id,
                updateconversationsection = true,
                updateconversationsectionhtml = conversations,
                updatetotalsection = true,
                totalsectionhtml = conversations.Where(x => !x.IsRead).Count(),
                sendcustomerupdate = true,
                scrollToBottom = true,
            });
        }

        public virtual ActionResult ViewPM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return InvokeHttp400("Private message is disable");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return InvokeHttp400("Guest count not Send Message");
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.ToCustomerId != _workContext.CurrentCustomer.Id && pm.FromCustomerId != _workContext.CurrentCustomer.Id)
                {
                    return RedirectToRoute("PrivateMessages");
                }

                if (!pm.IsRead && pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsRead = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            else
            {
                return RedirectToRoute("PrivateMessages");
            }

            var model = _privateMessagesModelFactory.PreparePrivateMessageModel(pm);
            return View(model);
        }

        public virtual ActionResult DeletePM(int privateMessageId)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return InvokeHttp400("Private message is disable");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return InvokeHttp400("Guest count not Send Message");
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        #endregion
    }
}