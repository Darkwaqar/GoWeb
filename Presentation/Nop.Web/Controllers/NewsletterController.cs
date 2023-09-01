using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    public partial class NewsletterController : BasePublicController
    {
        private readonly INewsletterModelFactory _newsletterModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;

        public NewsletterController(INewsletterModelFactory newsletterModelFactory,
            ILocalizationService localizationService,
            IWorkContext workContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkflowMessageService workflowMessageService,
            IStoreContext storeContext,
            CustomerSettings customerSettings)
        {
            this._newsletterModelFactory = newsletterModelFactory;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._workflowMessageService = workflowMessageService;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
        }

        [ChildActionOnly]
        public virtual ActionResult NewsletterBox()
        {
            if (_customerSettings.HideNewsletterBlock)
                return Content("");

            var model = _newsletterModelFactory.PrepareNewsletterBoxModel();
            return PartialView(model);
        }

        //available even when a store is closed
        [StoreClosed(true)]
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult SubscribeNewsletter(string email, bool subscribe)
        {
            string result;
            bool success = false;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                    }
                }
                else if (subscribe)
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        StoreId = _storeContext.CurrentStore.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                }
                success = true;
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }

        //available even when a store is closed
        [StoreClosed(true)]
        public virtual ActionResult SubscriptionActivation(Guid token, bool active)
        {
            var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByGuid(token);
            if (subscription == null)
                return RedirectToRoute("HomePage");

            if (active)
            {
                subscription.Active = true;
                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
            }
            else
                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

            var model = _newsletterModelFactory.PrepareSubscriptionActivationModel(active);
            return View(model);
        }


        [ChildActionOnly]
        public virtual ActionResult VendorSubsriptionBox(int vendorId = 0)
        {
            var currentCustomer = _workContext.CurrentCustomer;
            var model = new NewsLetterSubscription();
            if (currentCustomer != null)
            {
                model = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndVendorId(currentCustomer.Email, vendorId);
                if (model == null)
                {
                    model = new NewsLetterSubscription
                    {
                        Email = _workContext.CurrentCustomer.Email,
                        Active = false,
                        VendorId = vendorId,
                    };
                }
            }
            else
            {
                model = new NewsLetterSubscription
                {
                    Email = "",
                    Active = false,
                    VendorId = vendorId,
                };
            }
            return PartialView(model);
        }

        //available even when a store is closed
        [StoreClosed(true)]
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult SubscribeVendorNewsletter(string email = "", int vendorId = 0)
        {
            string result;
            bool success = false;
            string active = "UnSubscribe";
            var currentCustomer = _workContext.CurrentCustomer;

            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
                active = "Subscribe";
            }

            if (currentCustomer != null && !currentCustomer.IsGuest())
            {
                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndVendorId(currentCustomer.Email, vendorId);
                if (subscription != null)
                {
                    if (subscription.Active)
                    {
                        subscription.Active = false;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                        active = "Subscribe";
                        _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, _workContext.WorkingLanguage.Id);
                    }
                    else
                    {
                        subscription.Active = true;
                        _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscription);
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                        active = "UnSubscribe";
                        _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                    }
                }
                else
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = currentCustomer.Email,
                        Active = true,
                        StoreId = _storeContext.CurrentStore.Id,
                        VendorId = vendorId,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    active = "UnSubscribe";
                }
            }
            else
            {
               var subscription = new NewsLetterSubscription
                {
                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                    Email = email,
                    Active = true,
                    StoreId = _storeContext.CurrentStore.Id,
                    VendorId = vendorId,
                    CreatedOnUtc = DateTime.UtcNow
                };
                _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                active = "UnSubscribe";
            }

            success = true;
            return Json(new
            {
                Success = success,
                Active = active,
                Result = result
            });
        }
    }
}
