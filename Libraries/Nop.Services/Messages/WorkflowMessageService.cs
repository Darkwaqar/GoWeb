using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Core.Domain.Appointments;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Vendors;
using Nop.Services.SMS;
using Nop.Core.Domain.SMS;
using Nop.Services.Common;
using Nop.Core.Domain.Contact;
using Nop.Core.Domain.Affiliates;

namespace Nop.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly IContactUsService _contactUsService;
        private readonly IVendorService _vendorService;
        private readonly INumberAccountService _numberAccountService;
        private readonly ISMSTemplateService _smsTemplateService;
        private readonly IQueuedSMSService _queuedSMSService;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService,
            ILanguageService languageService,
            ITokenizer tokenizer,
            IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService,
            IStoreContext storeContext,
            CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher,
            HttpContextBase httpContext,
            ICustomerService customerService,
            IWebHelper webHelper,
            IContactUsService contactUsService,
            IVendorService vendorService,
            INumberAccountService numberAccountService,
            ISMSTemplateService smsTemplateService,
            IQueuedSMSService queuedSMSService)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._commonSettings = commonSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._eventPublisher = eventPublisher;
            this._httpContext = httpContext;
            this._customerService = customerService;
            this._webHelper = webHelper;
            this._contactUsService = contactUsService;
            this._vendorService = vendorService;
            this._numberAccountService = numberAccountService;
            this._smsTemplateService = smsTemplateService;
            this._queuedSMSService = queuedSMSService;
        }

        #endregion

        #region Utilities

        protected virtual MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int storeId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, storeId);

            //no template found
            if (messageTemplate == null)
                return null;

            //ensure it's active
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        protected virtual SMSTemplate GetActiveSMSTemplate(string messageTemplateName, int storeId)
        {
            var smsTemplate = _smsTemplateService.GetSMSTemplateByName(messageTemplateName, storeId);

            //no template found
            if (smsTemplate == null)
                return null;

            //ensure it's active
            var isActive = smsTemplate.IsActive;
            if (!isActive)
                return null;

            return smsTemplate;
        }

        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;

        }

        protected virtual NumberAccount GetNumberAccountOfMessageTemplate(SMSTemplate messageTemplate, int languageId)
        {
            var numberAccountId = messageTemplate.GetLocalized(mt => mt.NumberAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (numberAccountId == 0)
                numberAccountId = messageTemplate.NumberAccountId;

            var numberAccount = _numberAccountService.GetNumberAccountById(numberAccountId);
            if (numberAccount == null)
                numberAccount = _numberAccountService.GetNumberAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (numberAccount == null)
                numberAccount = _numberAccountService.GetAllNumberAccounts().FirstOrDefault();
            return numberAccount;

        }

        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }
            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
            return language.Id;
        }

        #endregion

        #region Methods

        #region Customer workflow

        /// <summary>
        /// Sends 'New customer' notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerRegisteredNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerRegisteredNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toEmail = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                SendNotification(smsTemplate, numberAccount, languageId, tokens, toEmail, toName);
            }

            return 0;
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerWelcomeMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerWelcomeMessage, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerEmailValidationMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerEmailValidationMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an email re-validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerEmailRevalidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerEmailRevalidationMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);


                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                //email to re-validate
                var toEmail = customer.EmailToRevalidate;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerEmailRevalidationMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                //todo add the emailtorevalidate as a token

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                ////email to re-validate
                //var toEmail = customer.EmailToRevalidate;
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerPasswordRecoveryMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);


                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerPasswordRecoveryMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);


                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }

            return 0;
        }

        /// <summary>
        /// Send an email token validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="store">Store instance</param>
        /// <param name="token">Token</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerEmailTokenValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerEmailTokenValidationMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerEmailTokenValidationMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }

            return 0;
        }

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPlacedVendorNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPlacedVendorNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = vendor.ContactNumber;
                var toName = vendor.Name;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order paid notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPaidStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPaidStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPaidStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order paid notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPaidCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPaidCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, attachmentFilePath, attachmentFileName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPaidCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName, attachmentFilePath, attachmentFileName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order paid notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPaidVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = vendor.ContactNumber;
                var toName = vendor.Name;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order placed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPlacedCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, attachmentFilePath, attachmentFileName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPlacedCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName, attachmentFilePath, attachmentFileName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a shipment sent notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendShipmentSentCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ShipmentSentCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
                _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.ShipmentSentCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
                _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;

        }

        /// <summary>
        /// Sends a shipment delivered notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
                _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
                _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCompletedCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderCompletedCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, attachmentFilePath, attachmentFileName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderCompletedCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName, attachmentFilePath, attachmentFileName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order cancelled notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderCancelledCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderCancelledCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order cancelled notification to an admin
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCancelledStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId);
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var customer = _customerService.GetCustomerById(order.CustomerId);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderCancelledStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderCancelledStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var emailAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = emailAccount.Number;
                var toName = emailAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, emailAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order cancel notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCancelledVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeService.GetStoreById(order.StoreId);
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var customer = _customerService.GetCustomerById(order.CustomerId);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderCancelledVendorNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                SendNotification(messageTemplate, emailAccount,
                   languageId, tokens,
                   toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderCancelledVendorNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId, vendor.Id);
                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = vendor.ContactNumber;
                var toName = vendor.Name;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }

            return 0;
        }

        /// <summary>
        /// Sends an order refunded notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderRefundedStoreOwnerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddOrderRefundedTokens(tokens, order, refundedAmount);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddOrderRefundedTokens(tokens, order, refundedAmount);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends an order refunded notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderRefundedCustomerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderRefundedCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddOrderRefundedTokens(tokens, order, refundedAmount);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderRefundedCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddOrderRefundedTokens(tokens, order, refundedAmount);
                _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a new order note added notification to a customer
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewOrderNoteAddedCustomerNotification(OrderNote orderNote, int languageId)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");

            var order = orderNote.Order;

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderNoteTokens(tokens, orderNote);
                _messageTokenProvider.AddOrderTokens(tokens, orderNote.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, orderNote.Order.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = order.BillingAddress.Email;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderNoteTokens(tokens, orderNote);
                _messageTokenProvider.AddOrderTokens(tokens, orderNote.Order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, orderNote.Order.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = order.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a store owner
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendRecurringPaymentCancelledStoreOwnerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var store = _storeService.GetStoreById(recurringPayment.InitialOrder.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a customer
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendRecurringPaymentCancelledCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var store = _storeService.GetStoreById(recurringPayment.InitialOrder.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = recurringPayment.InitialOrder.BillingAddress.Email;
                var toName = string.Format("{0} {1}",
                    recurringPayment.InitialOrder.BillingAddress.FirstName, recurringPayment.InitialOrder.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = recurringPayment.InitialOrder.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}",
                    recurringPayment.InitialOrder.BillingAddress.FirstName, recurringPayment.InitialOrder.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a "Recurring payment failed" notification to a customer
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendRecurringPaymentFailedCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            var store = _storeService.GetStoreById(recurringPayment.InitialOrder.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = recurringPayment.InitialOrder.BillingAddress.Email;
                var toName = string.Format("{0} {1}",
                    recurringPayment.InitialOrder.BillingAddress.FirstName, recurringPayment.InitialOrder.BillingAddress.LastName);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var emailAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
                _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = recurringPayment.InitialOrder.BillingAddress.PhoneNumber;
                var toName = string.Format("{0} {1}",
                    recurringPayment.InitialOrder.BillingAddress.FirstName, recurringPayment.InitialOrder.BillingAddress.LastName);
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, emailAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }
            //cant make sms request because not a user and number not avalible 
            return 0;
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }

            //cant make sms request because not a user and number not avalible 
            return 0;
        }

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendProductEmailAFriendMessage(Customer customer, int languageId,
            Product product, string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.EmailAFriendMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddProductTokens(tokens, product, languageId);
            tokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("EmailAFriend.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        }

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendWishlistEmailAFriendMessage(Customer customer, int languageId,
             string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.WishlistToFriendMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            tokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("Wishlist.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        }

        #endregion

        #region Return requests

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends 'New Return Request' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewReturnRequestCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewReturnRequestCustomerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.Email :
                    returnRequest.Customer.Email;
                var toName = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.FirstName :
                    returnRequest.Customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewReturnRequestCustomerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.PhoneNumber :
                    returnRequest.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.FirstName :
                    returnRequest.Customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }

            return 0;
        }

        /// <summary>
        /// Sends 'Return Request status changed' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var store = _storeService.GetStoreById(orderItem.Order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification, store.Id);
            if (messageTemplate == null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                string toEmail = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.Email :
                    returnRequest.Customer.Email;
                var toName = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.FirstName :
                    returnRequest.Customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification, store.Id);
            if (smsTemplate == null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
                _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, orderItem);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                string toNumber = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.PhoneNumber :
                    returnRequest.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = returnRequest.Customer.IsGuest() ?
                    orderItem.Order.BillingAddress.FirstName :
                    returnRequest.Customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumTopicMessage(Customer customer, ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewForumTopicMessage, store.Id);
            if (messageTemplate != null)
            { //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
                _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewForumTopicMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
                _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumPostMessage(Customer customer, ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewForumPostMessage, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
                _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic, friendlyForumTopicPageIndex, forumPost.Id);
                _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewForumPostMessage, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
                _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic, friendlyForumTopicPageIndex, forumPost.Id);
                _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
                throw new ArgumentNullException("privateMessage");

            var store = _storeService.GetStoreById(privateMessage.StoreId) ?? _storeContext.CurrentStore;

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.PrivateMessageNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddPrivateMessageTokens(tokens, privateMessage);
                _messageTokenProvider.AddCustomerTokens(tokens, privateMessage.ToCustomer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = privateMessage.ToCustomer.Email;
                var toName = privateMessage.ToCustomer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.PrivateMessageNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddPrivateMessageTokens(tokens, privateMessage);
                _messageTokenProvider.AddCustomerTokens(tokens, privateMessage.ToCustomer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = privateMessage.ToCustomer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = privateMessage.ToCustomer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Sends 'New vendor account submitted' message to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewVendorAccountApplyStoreOwnerNotification(Customer customer, Vendor vendor, int languageId, string senderEmail,
            string senderName, string attrInfo, string attrXml)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);


                string fromEmail;
                string fromName;
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                _messageTokenProvider.AddVendorTokens(tokens, vendor);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                //store in database
                if (_commonSettings.StoreInDatabaseVendorAccountForm)
                {
                    var contactus = new ContactUs()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        CustomerId = customer.Id,
                        StoreId = _storeContext.CurrentStore.Id,
                        VendorId = 0,
                        Email = senderEmail,
                        FullName = senderName,
                        Subject = "Apply For Vendor Account",
                        Enquiry = "",
                        EmailAccountId = emailAccount.Id,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        ContactAttributeDescription = attrInfo,
                        ContactAttributesXml = attrXml
                    };
                    _contactUsService.InsertContactUs(contactus);
                }
                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                   fromEmail: fromEmail,
                   fromName: fromName,
                   replyToEmailAddress: senderEmail,
                   replyToName: senderName);
            }
            //no number avalible from the sending sms 

            return 0;
        }

        /// <summary>
        /// Sends 'Vendor information changed' message to a store owner
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendVendorInformationChangeNotification(Vendor vendor, int languageId)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.VendorInformationChangeNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddVendorTokens(tokens, vendor);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.VendorInformationChangeNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddVendorTokens(tokens, vendor);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            var order = giftCard.PurchasedWithOrderItem != null ? giftCard.PurchasedWithOrderItem.Order : null;
            var store = order != null ? _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore : _storeContext.CurrentStore;

            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddGiftCardTokens(tokens, giftCard);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = giftCard.RecipientEmail;
                var toName = giftCard.RecipientName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddGiftCardTokens(tokens, giftCard);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                //TODO add giftCard with RecipientNumber number requried to send giftcard to message
                //var toNumber = giftCard.RecipientNumber;
                //var toName = giftCard.RecipientName;

                //return SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a coupon notification
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCouponNotification(Coupon coupon, int languageId)
        {
            if (coupon == null)
                throw new ArgumentNullException("coupon");

            var store = _storeContext.CurrentStore;

            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCouponTokens(tokens, coupon);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = coupon.RecipientEmail;
                var toName = coupon.RecipientName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCouponTokens(tokens, coupon);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                //TODO add giftCard with RecipientNumber number requried to send giftcard to message
                //var toNumber = giftCard.RecipientNumber;
                //var toName = giftCard.RecipientName;

                //return SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendProductReviewNotificationMessage(ProductReview productReview, int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ProductReviewNotification, store.Id);
            if (messageTemplate == null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddProductReviewTokens(tokens, productReview);
                _messageTokenProvider.AddCustomerTokens(tokens, productReview.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.ProductReviewNotification, store.Id);
            if (smsTemplate == null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddProductReviewTokens(tokens, productReview);
                _messageTokenProvider.AddCustomerTokens(tokens, productReview.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }


        /// <summary>
        /// Sends a vendor review notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="product">Product</param>
        /// <param name="vendorReview">Vendor review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendVendorReviewNotificationMessage(Customer customer, Vendor vendor, Product product, VendorReview vendorReview,
            int languageId)
        {
            if (vendorReview == null)
                throw new ArgumentNullException("vendorReview");

            var store = _storeContext.CurrentStore;
            var language = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddVendorReviewTokens(tokens, vendor, product, vendorReview);

                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                _messageTokenProvider.AddVendorTokens(tokens, vendor);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;
                SendNotification(messageTemplate, emailAccount,
                   languageId, tokens,
                   toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var emailAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddVendorReviewTokens(tokens, vendor, product, vendorReview);

                if (customer != null)
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);

                _messageTokenProvider.AddVendorTokens(tokens, vendor);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = vendor.ContactNumber;
                var toName = vendor.Name;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, emailAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendProductAppointmentNotificationMessage(ProductAppointment productAppointment, int languageId)
        {
            if (productAppointment == null)
                throw new ArgumentNullException("productAppointment");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ProductReviewNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddProductAppointmentTokens(tokens, productAppointment);
                _messageTokenProvider.AddCustomerTokens(tokens, productAppointment.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.ProductReviewNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddProductAppointmentTokens(tokens, productAppointment);
                _messageTokenProvider.AddCustomerTokens(tokens, productAppointment.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendQuantityBelowStoreOwnerNotification(Product product, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="combination">Attribute combination</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendQuantityBelowStoreOwnerNotification(ProductAttributeCombination combination, int languageId)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var product = combination.Product;

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                _messageTokenProvider.AddAttributeCombinationTokens(tokens, combination, languageId);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var product = combination.Product;

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                _messageTokenProvider.AddAttributeCombinationTokens(tokens, combination, languageId);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        /// <summary>
        /// Sends a "new VAT submitted" notification to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewVatSubmittedStoreOwnerNotification(Customer customer,
            string vatName, string vatAddress, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                tokens.Add(new Token("VatValidationResult.Name", vatName));
                tokens.Add(new Token("VatValidationResult.Address", vatAddress));

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                tokens.Add(new Token("VatValidationResult.Name", vatName));
                tokens.Add(new Token("VatValidationResult.Address", vatAddress));

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException("blogComment");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.BlogCommentNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);
                _messageTokenProvider.AddCustomerTokens(tokens, blogComment.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.BlogCommentNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);
                _messageTokenProvider.AddCustomerTokens(tokens, blogComment.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewsCommentNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddNewsCommentTokens(tokens, newsComment);
                _messageTokenProvider.AddCustomerTokens(tokens, newsComment.Customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.NewsCommentNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddNewsCommentTokens(tokens, newsComment);
                _messageTokenProvider.AddCustomerTokens(tokens, newsComment.Customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = numberAccount.Number;
                var toName = numberAccount.DisplayName;

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a 'Back in stock' notification message to a customer
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var store = _storeService.GetStoreById(subscription.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.BackInStockNotification, store.Id);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, subscription.Customer);
                _messageTokenProvider.AddBackInStockTokens(tokens, subscription);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var customer = subscription.Customer;
                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.BackInStockNotification, store.Id);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //tokens
                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, subscription.Customer);
                _messageTokenProvider.AddBackInStockTokens(tokens, subscription);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var customer = subscription.Customer;
                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendContactUsMessage(Customer customer, int languageId, string senderEmail,
            string senderName, string subject, string body, string attrInfo, string attrXml)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ContactUsMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                    _httpContext.Server.HtmlEncode(senderName), _httpContext.Server.HtmlEncode(senderEmail), body);
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddContactTokens(tokens, senderEmail, senderName, body, attrInfo);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            //store in database
            if (_commonSettings.StoreInDatabaseContactUsForm)
            {
                var contactus = new ContactUs()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    StoreId = _storeContext.CurrentStore.Id,
                    VendorId = 0,
                    Email = senderEmail,
                    FullName = senderName,
                    Subject = String.IsNullOrEmpty(subject) ? "Contact Us (form)" : subject,
                    Enquiry = body,
                    EmailAccountId = emailAccount.Id,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    ContactAttributeDescription = attrInfo,
                    ContactAttributesXml = attrXml
                };
                _contactUsService.InsertContactUs(contactus);
            }

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }

        /// <summary>
        /// Sends "Appointment" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendAppointmentMessage(Customer customer, int languageId, string senderEmail,
            string senderName, string subject, string body, string attrInfo, string attrXml)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ContactUsMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                    _httpContext.Server.HtmlEncode(senderName), _httpContext.Server.HtmlEncode(senderEmail), body);
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddContactTokens(tokens, senderEmail, senderName, body, attrInfo);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            //store in database
            if (_commonSettings.StoreInDatabaseAppointmentForm)
            {
                var contactus = new ContactUs()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    StoreId = _storeContext.CurrentStore.Id,
                    VendorId = 0,
                    Email = senderEmail,
                    FullName = senderName,
                    Subject = String.IsNullOrEmpty(subject) ? "Appointment" : subject,
                    Enquiry = body,
                    EmailAccountId = emailAccount.Id,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    ContactAttributeDescription = attrInfo,
                    ContactAttributesXml = attrXml
                };
                _contactUsService.InsertContactUs(contactus);
            }

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }

        /// <summary>
        /// Sends "contact vendor" message
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendContactVendorMessage(Customer customer, Vendor vendor, int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ContactVendorMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                    _httpContext.Server.HtmlEncode(senderName), _httpContext.Server.HtmlEncode(senderEmail), body);
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddContactTokens(tokens, senderEmail, senderName, body, "");

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = vendor.Email;
            var toName = vendor.Name;

            //store in database
            if (_commonSettings.StoreInDatabaseContactUsForm)
            {
                var contactus = new ContactUs()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    StoreId = _storeContext.CurrentStore.Id,
                    VendorId = vendor.Id,
                    Email = senderEmail,
                    FullName = senderName,
                    Subject = String.IsNullOrEmpty(subject) ? "Contact Us (form)" : subject,
                    Enquiry = body,
                    EmailAccountId = emailAccount.Id,
                    IpAddress = _webHelper.GetCurrentIpAddress()
                };
                _contactUsService.InsertContactUs(contactus);
            }

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                SendNotification(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
            }
            return 0;
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="smsTemplateId">Message template identifier</param>
        /// <param name="sendToNumber">Send to Number</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued SMS  identifier</returns>
        public virtual int SendTestSMS(int smsTemplateId, string sendToNumber, List<Token> tokens, int languageId)
        {
            var smsTemplate = _smsTemplateService.GetSMSTemplateById(smsTemplateId);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);
                if (!String.IsNullOrEmpty(sendToNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, sendToNumber, null);
            }
            return 0;
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized(mt => mt.BccEmailAddresses, languageId);
            if (String.IsNullOrEmpty(subject))
                subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="smsTemplate">SMS template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(SMSTemplate smsTemplate,
            NumberAccount numberAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToNumberAddress = null, string replyToName = null,
            string fromNumber = null, string fromName = null, string subject = null)
        {
            if (smsTemplate == null)
                throw new ArgumentNullException("smsTemplate");

            if (numberAccount == null)
                throw new ArgumentNullException("numberAccount");

            //retrieve localized message template data
            var bcc = smsTemplate.GetLocalized(mt => mt.BccNumberAddresses, languageId);
            if (String.IsNullOrEmpty(subject))
                subject = smsTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = smsTemplate.GetLocalized(mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var sms = new QueuedSMS
            {
                Priority = QueuedSMSPriority.High,
                From = !string.IsNullOrEmpty(fromNumber) ? fromNumber : numberAccount.Number,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : numberAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToNumberAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = smsTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                NumberAccountId = numberAccount.Id,
                DontSendBeforeDateUtc = !smsTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(smsTemplate.DelayPeriod.ToHours(smsTemplate.DelayBeforeSend.Value)))
            };

            _queuedSMSService.InsertQueuedSMS(sms);
            return sms.Id;
        }

        #endregion

        #region Customer Action Event

        /// <summary>
        /// Sends a customer action event - Add to cart notification to a customer
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        /// <param name="ShoppingCartItem">Item</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerActionEvent_AddToCart_Notification(CustomerAction action, ShoppingCartItem cartItem, int languageId, Customer customer)
        {
            if (cartItem == null)
                throw new ArgumentNullException("cartItem");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(action.MessageTemplateId);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                var product = EngineContext.Current.Resolve<IProductService>().GetProductById(cartItem.ProductId);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toEmail))
                    toEmail = emailAccount.Email;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = _smsTemplateService.GetSMSTemplateById(action.MessageTemplateId);
            if (smsTemplate != null)
            {
                //email account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                var product = EngineContext.Current.Resolve<IProductService>().GetProductById(cartItem.ProductId);
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }


        /// <summary>
        /// Sends a customer action event - Add to cart notification to a customer
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        /// <param name="Order">Order</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerActionEvent_AddToOrder_Notification(CustomerAction action, Order order, Customer customer, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(action.MessageTemplateId);

            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = string.Empty;
                var toName = string.Empty;

                if (order.BillingAddress != null)
                {
                    toEmail = order.BillingAddress.Email;
                    toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                }
                else
                {
                    if (order.ShippingAddress != null)
                    {
                        toEmail = order.ShippingAddress.Email;
                        toName = string.Format("{0} {1}", order.ShippingAddress.FirstName, order.ShippingAddress.LastName);
                    }
                }

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }
            var smsTemplate = _smsTemplateService.GetSMSTemplateById(action.MessageTemplateId);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = string.Empty;
                var toName = string.Empty;

                if (order.BillingAddress != null)
                {
                    toNumber = order.BillingAddress.PhoneNumber;
                    toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
                }
                else
                {
                    if (order.ShippingAddress != null)
                    {
                        toNumber = order.ShippingAddress.PhoneNumber;
                        toName = string.Format("{0} {1}", order.ShippingAddress.FirstName, order.ShippingAddress.LastName);
                    }
                }

                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }

        /// <summary>
        /// Sends a customer action event - Add to cart notification to a customer
        /// </summary>
        /// <param name="CustomerAction">Customer action</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerActionEvent_Notification(CustomerAction action, int languageId, Customer customer)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = _messageTemplateService.GetMessageTemplateById(action.MessageTemplateId);
            if (messageTemplate != null)
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toEmail))
                    toEmail = emailAccount.Email;

                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = _smsTemplateService.GetSMSTemplateById(action.MessageTemplateId);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddStoreTokens(tokens, store);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();

                if (!String.IsNullOrEmpty(toNumber))
                    toNumber = numberAccount.Number;

                SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;


        }

        #endregion

        #region Auction notification

        public virtual int SendAuctionEndedCustomerNotificationWin(Product product, int languageId, Bid bid)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var customer = _customerService.GetCustomerById(bid.CustomerId);
            if (customer != null)
            {
                int storeId = bid.StoreId;
                if (storeId == 0)
                {
                    storeId = _storeContext.CurrentStore.Id;
                }
                var store = _storeService.GetStoreById(storeId);
                languageId = EnsureLanguageIsActive(languageId, store.Id);
                var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionWin, store.Id);
                if (messageTemplate != null)
                {

                    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);
                    _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                    _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                    //event notification
                    _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                    var toEmail = customer.Email;
                    var toName = customer.GetFullName();
                    SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
                }

                var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionWin, store.Id);
                if (smsTemplate != null)
                {
                    //number account
                    var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                    _messageTokenProvider.AddCustomerTokens(tokens, customer);
                    _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                    _messageTokenProvider.AddStoreTokens(tokens, store);

                    //event notification
                    _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                    var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    var toName = customer.GetFullName();

                    if (!String.IsNullOrEmpty(toNumber))
                        SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
                }
            }
            return 0;
        }

        public virtual int SendAuctionEndedCustomerNotificationLost(Product product, int languageId, Bid bid)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var customerwin = _customerService.GetCustomerById(bid.CustomerId);
            if (customerwin != null)
            {
                int storeId = bid.StoreId;
                if (storeId == 0)
                {
                    storeId = _storeContext.CurrentStore.Id;
                }
                languageId = EnsureLanguageIsActive(languageId, storeId);
                var store = _storeService.GetStoreById(storeId);
                var bids = EngineContext.Current.Resolve<IAuctionService>().GetBidsByProductId(bid.ProductId).Where(x => x.CustomerId != bid.CustomerId).GroupBy(x => x.CustomerId);
                var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionLost, storeId);
                if (messageTemplate != null)
                {
                    var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                    _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                    _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                    foreach (var item in bids)
                    {
                        var customer = _customerService.GetCustomerById(item.Key);

                        _messageTokenProvider.AddCustomerTokens(tokens, customer);

                        //event notification
                        _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                        var toEmail = customer.Email;
                        var toName = customer.GetFullName();
                        SendNotification(messageTemplate, emailAccount,
                           languageId, tokens,
                           toEmail, toName);
                    }
                }

                var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionLost, storeId);
                if (smsTemplate != null)
                {
                    var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                    var tokens = new List<Token>();
                    _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                    _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                    _messageTokenProvider.AddStoreTokens(tokens, store);
                    foreach (var item in bids)
                    {
                        var customer = _customerService.GetCustomerById(item.Key);

                        _messageTokenProvider.AddCustomerTokens(tokens, customer);

                        //event notification
                        _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                        var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                        var toName = customer.GetFullName();

                        if (!String.IsNullOrEmpty(toNumber))
                            SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
                    }
                }
            }
            return 0;
        }

        public virtual int SendAuctionEndedCustomerNotificationBin(Product product, int customerId, int languageId, int storeId)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (storeId == 0)
            {
                storeId = _storeContext.CurrentStore.Id;
            }
            var store = _storeService.GetStoreById(storeId);
            languageId = EnsureLanguageIsActive(languageId, store.Id);
            var bids = EngineContext.Current.Resolve<IAuctionService>().GetBidsByProductId(product.Id).Where(x => x.CustomerId != customerId).GroupBy(x => x.CustomerId);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionEnded, storeId);
            if (messageTemplate != null)
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                foreach (var item in bids)
                {
                    var customer = _customerService.GetCustomerById(item.Key);
                    if (customer != null)
                    {
                        _messageTokenProvider.AddCustomerTokens(tokens, customer);

                        //event notification
                        _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                        var toEmail = customer.Email;
                        var toName = customer.GetFullName();

                        SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
                    }
                }
            }
            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerNotificationAuctionEnded, storeId);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddProductTokens(tokens, product, languageId);
                _messageTokenProvider.AddStoreTokens(tokens, store);

                foreach (var item in bids)
                {
                    var customer = _customerService.GetCustomerById(item.Key);
                    if (customer != null)
                    {
                        _messageTokenProvider.AddCustomerTokens(tokens, customer);

                        //event notification
                        _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                        var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                        var toName = customer.GetFullName();
                        if (!String.IsNullOrEmpty(toNumber))
                            SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
                    }
                }
            }

            return 0;
        }
        public virtual int SendAuctionEndedStoreOwnerNotification(Product product, int languageId, Bid bid)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var tokens = new List<Token>();
            MessageTemplate messageTemplate = null;
            EmailAccount emailAccount = null;

            if (bid != null)
            {
                var customer = _customerService.GetCustomerById(bid.CustomerId);

                int storeId = bid.StoreId;
                if (storeId == 0)
                {
                    storeId = _storeContext.CurrentStore.Id;
                }
                var store = _storeService.GetStoreById(storeId);

                languageId = EnsureLanguageIsActive(languageId, store.Id);

                messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.StoreOwnerNotificationAuctionEnded, storeId);
                if (messageTemplate == null)
                    return 0;

                emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                _messageTokenProvider.AddStoreTokens(tokens, _storeService.GetStoreById(storeId), emailAccount);
            }
            else
            {
                var store = (_storeService.GetAllStores()).FirstOrDefault();
                var language = EnsureLanguageIsActive(languageId, store.Id);
                messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.StoreOwnerNotificationAuctionExpired, languageId);
                if (messageTemplate == null)
                    return 0;

                emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
                _messageTokenProvider.AddProductTokens(tokens, product, language);
            }

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }


        /// <summary>
        /// Send outbid notification to a customer
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product</param>
        /// <param name="Bid">Bid</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOutBidCustomerNotification(Product product, int languageId, Bid bid)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var customer = _customerService.GetCustomerById(bid.CustomerId);

            int storeId = bid.StoreId;
            if (storeId == 0)
            {
                storeId = _storeContext.CurrentStore.Id;
            }
            var store = _storeService.GetStoreById(storeId);
            var language = EnsureLanguageIsActive(languageId, storeId);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.CustomerNotificationBidUp, storeId);
            if (messageTemplate != null)
            {
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = customer.GetFullName();
                SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }

            var smsTemplate = GetActiveSMSTemplate(MessageTemplateSystemNames.CustomerNotificationBidUp, storeId);
            if (smsTemplate != null)
            {
                //number account
                var numberAccount = GetNumberAccountOfMessageTemplate(smsTemplate, languageId);

                var tokens = new List<Token>();
                _messageTokenProvider.AddAuctionTokens(tokens, product, bid);
                _messageTokenProvider.AddCustomerTokens(tokens, customer);
                _messageTokenProvider.AddStoreTokens(tokens, store);

                //event notification
                _eventPublisher.SMSTokensAdded(smsTemplate, tokens);

                var toNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                var toName = customer.GetFullName();
                if (!String.IsNullOrEmpty(toNumber))
                    SendNotification(smsTemplate, numberAccount, languageId, tokens, toNumber, toName);
            }
            return 0;
        }
        #endregion

        #endregion
    }
}
