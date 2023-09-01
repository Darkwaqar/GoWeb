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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Models.PrivateMessages;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Services.Seo;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    public partial class PrivateMessagesController : BasePublicController
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
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public PrivateMessagesController(IPrivateMessagesModelFactory privateMessagesModelFactory,
            IForumService forumService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ForumSettings forumSettings,
            IProductService productService,
            IWebHelper webHelper)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
            this._forumService = forumService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._forumSettings = forumSettings;
            this._productService = productService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel();
            model.Inbox = this.RenderPartialViewToString("Inbox", model.InboxModel);
            model.Conversations = this.RenderPartialViewToString("Conversations", model.ConversationsModel);
            return PartialView(model);
        }

        //inbox tab
        [HttpPost]
        public virtual ActionResult Inbox()
        {
            var model = _privateMessagesModelFactory.PrepareInboxModel();
            return Json(new
            {
                updateinboxsection = true,
                updateinboxsectionhtml = this.RenderPartialViewToString("Inbox", model),
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
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", model),
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),
                scrollToBottom = true,
                updateCustomerId = true,
                customerId = toCustomerId,
            });
        }

        //sent items tab
        [ChildActionOnly]
        public virtual ActionResult Vendors()
        {
            var model = _privateMessagesModelFactory.PrepareVendorModel();
            return PartialView(model);
        }

        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult DeleteInboxPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsDeletedByRecipient = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult MarkUnread(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsRead = false;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToRoute("PrivateMessages");
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
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", conversations),
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
                updateinboxsectionhtml = this.RenderPartialViewToString("Inbox", model),
                updateCustomerId = true,
                customerId = 0,
                updateconversationsection = true,
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", conversations),
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
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", conversations),
                updatetotalsection = true,
                totalsectionhtml = model.Where(x => !x.IsRead).Count(),
            });

        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult DeleteSentPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                    int privateMessageId;
                    if (Int32.TryParse(id, out privateMessageId))
                    {
                        PrivateMessage pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                pm.IsDeletedByAuthor = true;
                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }

            }
            return RedirectToRoute("PrivateMessages", new { tab = "sent" });
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult SendPM(SendPrivateMessageModel model)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }
            //first PM
            Customer toCustomer = _customerService.GetCustomerById(model.ToCustomerId);

            if (toCustomer == null || toCustomer.IsGuest())
            {
                return RedirectToRoute("PrivateMessages");
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
                updateinboxsectionhtml = this.RenderPartialViewToString("Inbox", inbox),
                updateCustomerId = true,
                customerId = toCustomer.Id,
                updateconversationsection = true,
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", conversations),
                updatetotalsection = true,
                totalsectionhtml = conversations.Where(x => !x.IsRead).Count(),
                sendcustomerupdate = true,
                scrollToBottom = true,
            });
        }

        [HttpPost]
        public virtual ActionResult SendLink(int ProductId = 0)
        {
            if (!_forumSettings.AllowPrivateMessages)
            {
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }
            if (ProductId == 0)
            {
                return RedirectToRoute("HomePage");
            }


            var product = _productService.GetProductById(ProductId);
            string productUrl = Url.RouteUrl("Product", new { SeName = product.GetSeName() }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");

            var privateMessage = _privateMessagesModelFactory.PreparePrivateMessage(product.VendorId);

            if (ModelState.IsValid)
            {
                try
                {

                    privateMessage.Text = string.Format("Need To Know more about '{0}'",product.Name);
                    _forumService.InsertPrivateMessage(privateMessage);

                    //activity log
                    _customerActivityService.InsertActivity("PublicStore.SendPM", _localizationService.GetResource("ActivityLog.PublicStore.SendPM"), privateMessage.ToCustomer.Email);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var conversations = _privateMessagesModelFactory.PrepareConverstationsModel(privateMessage.ToCustomer.Id);
            var inbox = _privateMessagesModelFactory.PrepareInboxModel();
            return Json(new
            {
                updateinboxsection = true,
                updateinboxsectionhtml = this.RenderPartialViewToString("Inbox", inbox),
                updateCustomerId = true,
                customerId = privateMessage.ToCustomer.Id,
                updateconversationsection = true,
                updateconversationsectionhtml = this.RenderPartialViewToString("Conversations", conversations),
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
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
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
                return RedirectToRoute("HomePage");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
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
