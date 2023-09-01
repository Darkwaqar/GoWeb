using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Nop.Services.Customers
{
    public partial class CustomerReminderService : ICustomerReminderService
    {
        #region Fields

        private readonly IRepository<CustomerReminder> _customerReminderRepository;
        private readonly IRepository<CustomerReminderHistory> _customerReminderHistoryRepository;
        private readonly IRepository<ReminderConditionEntity> _reminderCondtionEntityRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IStoreService _storeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IProductService _productService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public CustomerReminderService(
            IRepository<CustomerReminder> customerReminderRepository,
            IRepository<CustomerReminderHistory> customerReminderHistoryRepository,
            IRepository<ReminderConditionEntity> reminderCondtionEntityRepository,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<GenericAttribute> gaRepository,
            IEventPublisher eventPublisher,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer,
            IStoreService storeService,
            IProductService productService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ILanguageService languageService,
            IGenericAttributeService genericAttributeService)
        {
            this._customerReminderRepository = customerReminderRepository;
            this._customerReminderHistoryRepository = customerReminderHistoryRepository;
            this._reminderCondtionEntityRepository = reminderCondtionEntityRepository;
            this._customerRepository = customerRepository;
            this._orderRepository = orderRepository;
            this._gaRepository = gaRepository;
            this._eventPublisher = eventPublisher;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._tokenizer = tokenizer;
            this._queuedEmailService = queuedEmailService;
            this._storeService = storeService;
            this._customerAttributeParser = customerAttributeParser;
            this._productService = productService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual bool SendEmail(Customer customer, CustomerReminder customerReminder, int reminderlevelId)
        {
            var reminderLevel = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId);
            var emailAccount = _emailAccountService.GetEmailAccountById(reminderLevel.EmailAccountId);
            var store = customer.ShoppingCartItems.Count > 0 ? _storeService.GetStoreById(customer.ShoppingCartItems.FirstOrDefault().StoreId) : (_storeService.GetAllStores()).FirstOrDefault();

            //retrieve message template data
            var bcc = reminderLevel.BccEmailAddresses;
            var languages = _languageService.GetAllLanguages();
            var langId = customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, store.Id);
            if (langId > 0)
                langId = languages.FirstOrDefault().Id;

            var language = languages.FirstOrDefault(x => x.Id == langId);
            if (language == null)
                language = languages.FirstOrDefault();

            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddShoppingCartTokens(tokens, customer, store, language.Id);

            string subject = _tokenizer.Replace(reminderLevel.Subject, tokens, false);
            string body = _tokenizer.Replace(reminderLevel.Body, tokens, true);

            //limit name length
            var toName = CommonHelper.EnsureMaximumLength(customer.GetFullName(), 300);
            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = customer.Email,
                ToName = toName,
                ReplyTo = string.Empty,
                ReplyToName = string.Empty,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subject,
                Body = body,
                AttachmentFilePath = "",
                AttachmentFileName = "",
                AttachedDownloadId = 0,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
            };

            _queuedEmailService.InsertQueuedEmail(email);
            //activity log
            _customerActivityService.InsertActivity(string.Format("CustomerReminder.{0}", customerReminder.ReminderRule.ToString()), _localizationService.GetResource(string.Format("ActivityLog.{0}", customerReminder.ReminderRule.ToString())), customer, customerReminder.Name);

            return true;
        }

        protected virtual bool SendEmail(Customer customer, Order order, CustomerReminder customerReminder, int reminderlevelId)
        {
            var reminderLevel = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId);
            var emailAccount = _emailAccountService.GetEmailAccountById(reminderLevel.EmailAccountId);
            var store = _storeService.GetStoreById(customer.RegisteredInStoreId);
            if (order != null)
            {
                store = _storeService.GetStoreById(order.StoreId);
            }
            if (store == null)
            {
                store = (_storeService.GetAllStores()).FirstOrDefault();
            }

            //retrieve message template data
            var bcc = reminderLevel.BccEmailAddresses;
            var languageId = _workContext.WorkingLanguage.Id;
            if (languageId == 0)
            {
                if (order != null)
                {
                    languageId = order.CustomerLanguageId;
                }
                if (languageId == 0)
                {
                    var customerLanguageId = customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId);
                    if (customerLanguageId > 0)
                        languageId = customerLanguageId;
                }
                if (languageId == 0)
                {
                    languageId = (_languageService.GetAllLanguages()).FirstOrDefault().Id;
                }
            }

            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddShoppingCartTokens(tokens, customer, store, languageId);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);

            string subject = _tokenizer.Replace(reminderLevel.Subject, tokens, false);
            string body = _tokenizer.Replace(reminderLevel.Body, tokens, true);

            //limit name length
            var toName = CommonHelper.EnsureMaximumLength(customer.GetFullName(), 300);
            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = customer.Email,
                ToName = toName,
                ReplyTo = string.Empty,
                ReplyToName = string.Empty,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subject,
                Body = body,
                AttachmentFilePath = "",
                AttachmentFileName = "",
                AttachedDownloadId = 0,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
            };

            _queuedEmailService.InsertQueuedEmail(email);
            //activity log
            _customerActivityService.InsertActivity(string.Format("CustomerReminder.{0}", customerReminder.ReminderRule.ToString()), string.Format("ActivityLog.{0}", customerReminder.ReminderRule.ToString()), customer, customerReminder.Name);

            return true;
        }


        #region Conditions
        protected bool CheckConditions(CustomerReminder customerReminder, Customer customer)
        {
            if (customerReminder.Conditions.Count == 0)
                return true;


            bool cond = false;
            foreach (var item in customerReminder.Conditions)
            {
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Category)
                {
                    cond = ConditionCategory(item, customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Product)
                {
                    cond = ConditionProducts(item, customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Manufacturer)
                {
                    cond = ConditionManufacturer(item, customer.ShoppingCartItems.Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart).Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerTag)
                {
                    cond = ConditionCustomerTag(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerRole)
                {
                    cond = ConditionCustomerRole(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerRegisterField)
                {
                    cond = ConditionCustomerRegister(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomCustomerAttribute)
                {
                    cond = ConditionCustomerAttribute(item, customer);
                }
            }

            return cond;
        }
        protected bool CheckConditions(CustomerReminder customerReminder, Customer customer, Order order)
        {
            if (customerReminder.Conditions.Count == 0)
                return true;


            bool cond = false;
            foreach (var item in customerReminder.Conditions)
            {
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Category)
                {
                    cond = ConditionCategory(item, order.OrderItems.Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Product)
                {
                    cond = ConditionProducts(item, order.OrderItems.Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.Manufacturer)
                {
                    cond = ConditionManufacturer(item, order.OrderItems.Select(x => x.ProductId).ToList());
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerTag)
                {
                    cond = ConditionCustomerTag(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerRole)
                {
                    cond = ConditionCustomerRole(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomerRegisterField)
                {
                    cond = ConditionCustomerRegister(item, customer);
                }
                if (item.ConditionType == CustomerReminderConditionTypeEnum.CustomCustomerAttribute)
                {
                    cond = ConditionCustomerAttribute(item, customer);
                }
            }

            return cond;
        }
        protected bool ConditionCategory(CustomerReminder.ReminderCondition condition, ICollection<int> products)
        {
            bool cond = false;
            if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
            {
                cond = true;
                foreach (var item in condition.Entity.Where(x => x.EntityName == "category"))
                {
                    foreach (var product in products)
                    {
                        var pr = _productService.GetProductById(product);
                        if (pr != null)
                        {
                            if (pr.ProductCategories.Where(x => x.CategoryId == item.EntityId).Count() == 0)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
            {
                foreach (var item in condition.Entity.Where(x => x.EntityName == "category"))
                {
                    foreach (var product in products)
                    {
                        var pr = _productService.GetProductById(product);
                        if (pr != null)
                        {
                            if (pr.ProductCategories.Where(x => x.CategoryId == item.EntityId).Count() > 0)
                                return true;
                        }
                    }
                }
            }

            return cond;
        }
        protected bool ConditionManufacturer(CustomerReminder.ReminderCondition condition, ICollection<int> products)
        {
            bool cond = false;
            if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
            {
                cond = true;
                foreach (var item in condition.Entity.Where(x => x.EntityName == "manufacturer"))
                {
                    foreach (var product in products)
                    {
                        var pr = _productService.GetProductById(product);
                        if (pr != null)
                        {
                            if (pr.ProductManufacturers.Where(x => x.ManufacturerId == item.EntityId).Count() == 0)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
            {
                foreach (var item in condition.Entity.Where(x => x.EntityName == "manufacturer"))
                {
                    foreach (var product in products)
                    {
                        var pr = _productService.GetProductById(product);
                        if (pr != null)
                        {
                            if (pr.ProductManufacturers.Where(x => x.ManufacturerId == item.EntityId).Count() > 0)
                                return true;
                        }
                    }
                }
            }

            return cond;
        }
        protected bool ConditionProducts(CustomerReminder.ReminderCondition condition, ICollection<int> products)
        {
            bool cond = true;
            if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
            {
                cond = products.ContainsAll(condition.Entity.Where(x => x.EntityName == "product").Select(x => x.EntityId));
            }
            if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
            {
                cond = products.ContainsAny(condition.Entity.Where(x => x.EntityName == "product").Select(x => x.EntityId));
            }

            return cond;
        }
        protected bool ConditionCustomerRole(CustomerReminder.ReminderCondition condition, Customer customer)
        {
            bool cond = false;
            if (customer != null)
            {
                var customerRoles = customer.CustomerRoles;
                if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
                {
                    cond = customerRoles.Select(x => x.Id).ContainsAll(condition.Entity.Where(x => x.EntityName == "customerRole").Select(x => x.EntityId));
                }
                if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
                {
                    cond = customerRoles.Select(x => x.Id).ContainsAny(condition.Entity.Where(x => x.EntityName == "customerRole").Select(x => x.EntityId));
                }
            }
            return cond;
        }
        protected bool ConditionCustomerTag(CustomerReminder.ReminderCondition condition, Customer customer)
        {
            bool cond = false;
            if (customer != null)
            {
                var customerTags = customer.CustomerTags;
                if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
                {
                    cond = customerTags.Select(x => x.Id).ContainsAll(condition.Entity.Where(x => x.EntityName == "customerTag").Select(x => x.EntityId));
                }
                if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
                {
                    cond = customerTags.Select(x => x.Id).ContainsAny(condition.Entity.Where(x => x.EntityName == "customerTag").Select(x => x.EntityId));
                }
            }
            return cond;
        }
        protected bool ConditionCustomerRegister(CustomerReminder.ReminderCondition condition, Customer customer)
        {
            bool cond = false;
            if (customer != null)
            {
                if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
                {
                    cond = true;
                    foreach (var item in condition.CustomerRegistration)
                    {
                        if (customer.GetAttribute<string>(item.RegisterField) != item.RegisterValue)
                            cond = false;
                    }
                }
                if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
                {
                    foreach (var item in condition.CustomerRegistration)
                    {
                        if (customer.GetAttribute<string>(item.RegisterField) == item.RegisterValue)
                            cond = true;
                    }
                }
            }
            return cond;
        }
        protected bool ConditionCustomerAttribute(CustomerReminder.ReminderCondition condition, Customer customer)
        {
            bool cond = false;
            if (customer != null)
            {
                if (condition.Condition == CustomerReminderConditionEnum.AllOfThem)
                {
                    var customCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    if (customCustomerAttributes != null)
                    {
                        if (!String.IsNullOrEmpty(customCustomerAttributes))
                        {
                            var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(customCustomerAttributes);
                            cond = true;
                            foreach (var item in condition.CustomCustomerAttributes)
                            {
                                var _fieldsString = item.RegisterField.Split(':');
                                var _fields = Array.ConvertAll(_fieldsString, int.Parse);
                                if (_fields.Count() > 1)
                                {
                                    if (selectedValues.Where(x => x.CustomerAttributeId == _fields.FirstOrDefault() && x.Id == _fields.LastOrDefault()).Count() == 0)
                                        cond = false;
                                }
                                else
                                    cond = false;
                            }
                        }
                    }
                }
                if (condition.Condition == CustomerReminderConditionEnum.OneOfThem)
                {

                    var customCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    if (customCustomerAttributes != null)
                    {
                        if (!String.IsNullOrEmpty(customCustomerAttributes))
                        {
                            var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(customCustomerAttributes);
                            foreach (var item in condition.CustomCustomerAttributes)
                            {
                                var _fieldsString = item.RegisterField.Split(':');
                                var _fields = Array.ConvertAll(_fieldsString, int.Parse);
                                if (_fields.Count() > 1)
                                {
                                    if (selectedValues.Where(x => x.CustomerAttributeId == _fields.FirstOrDefault() && x.Id == _fields.LastOrDefault()).Count() > 0)
                                        cond = true;
                                }
                            }
                        }
                    }
                }
            }
            return cond;
        }
        #endregion

        #region History

        protected virtual void UpdateHistory(Customer customer, CustomerReminder customerReminder, int reminderlevelId, CustomerReminderHistory history)
        {
            if (history != null)
            {
                history.Levels.Add(new CustomerReminderHistory.HistoryLevel()
                {
                    Level = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level,
                    ReminderLevelId = reminderlevelId,
                    SendDate = DateTime.UtcNow,
                });
                if (customerReminder.Levels.Max(x => x.Level) ==
                    customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level)
                {
                    history.Status = (int)CustomerReminderHistoryStatusEnum.CompletedReminder;
                    history.EndDate = DateTime.UtcNow;
                }
                _customerReminderHistoryRepository.Update(history);
            }
            else
            {
                history = new CustomerReminderHistory();
                history.CustomerId = customer.Id;
                history.Status = (int)CustomerReminderHistoryStatusEnum.Started;
                history.StartDate = DateTime.UtcNow;
                history.CustomerReminderId = customerReminder.Id;
                history.ReminderRuleId = customerReminder.ReminderRuleId;
                history.Levels.Add(new CustomerReminderHistory.HistoryLevel()
                {
                    Level = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level,
                    ReminderLevelId = reminderlevelId,
                    SendDate = DateTime.UtcNow,
                });

                _customerReminderHistoryRepository.Insert(history);
            }

        }

        protected virtual void UpdateHistory(Order order, CustomerReminder customerReminder, int reminderlevelId, CustomerReminderHistory history)
        {
            if (history != null)
            {
                history.Levels.Add(new CustomerReminderHistory.HistoryLevel()
                {
                    Level = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level,
                    ReminderLevelId = reminderlevelId,
                    SendDate = DateTime.UtcNow,
                });
                if (customerReminder.Levels.Max(x => x.Level) ==
                    customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level)
                {
                    history.Status = (int)CustomerReminderHistoryStatusEnum.CompletedReminder;
                    history.EndDate = DateTime.UtcNow;
                }
                _customerReminderHistoryRepository.Update(history);
            }
            else
            {
                history = new CustomerReminderHistory();
                history.BaseOrderId = order.Id;
                history.CustomerId = order.CustomerId;
                history.Status = (int)CustomerReminderHistoryStatusEnum.Started;
                history.StartDate = DateTime.UtcNow;
                history.CustomerReminderId = customerReminder.Id;
                history.ReminderRuleId = customerReminder.ReminderRuleId;
                history.Levels.Add(new CustomerReminderHistory.HistoryLevel()
                {
                    Level = customerReminder.Levels.FirstOrDefault(x => x.Id == reminderlevelId).Level,
                    ReminderLevelId = reminderlevelId,
                    SendDate = DateTime.UtcNow,
                });

                _customerReminderHistoryRepository.Insert(history);
            }

        }
        protected virtual void CloseHistoryReminder(CustomerReminder customerReminder, CustomerReminderHistory history)
        {
            history.Status = (int)CustomerReminderHistoryStatusEnum.CompletedReminder;
            history.EndDate = DateTime.UtcNow;
            _customerReminderHistoryRepository.Update(history);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets customer reminder
        /// </summary>
        /// <param name="id">Customer reminder identifier</param>
        /// <returns>Customer reminder</returns>
        public virtual CustomerReminder GetCustomerReminderById(int id)
        {
            return _customerReminderRepository.GetById(id);
        }


        /// <summary>
        /// Gets all customer reminders
        /// </summary>
        /// <returns>Customer reminders</returns>
        public virtual IList<CustomerReminder> GetCustomerReminders()
        {
            var query = from p in _customerReminderRepository.Table
                        orderby p.DisplayOrder
                        select p;
            return query.ToList();
        }

        /// <summary>
        /// Inserts a customer reminder
        /// </summary>
        /// <param name="CustomerReminder">Customer reminder</param>
        public virtual void InsertCustomerReminder(CustomerReminder customerReminder)
        {
            if (customerReminder == null)
                throw new ArgumentNullException("customerReminder");

            _customerReminderRepository.Insert(customerReminder);

            //event notification
            _eventPublisher.EntityInserted(customerReminder);

        }

        /// <summary>
        /// Delete a customer reminder
        /// </summary>
        /// <param name="customerReminder">Customer reminder</param>
        public virtual void DeleteCustomerReminder(CustomerReminder customerReminder)
        {
            if (customerReminder == null)
                throw new ArgumentNullException("customerReminder");

            _customerReminderRepository.Delete(customerReminder);

            //event notification
            _eventPublisher.EntityDeleted(customerReminder);

        }

        /// <summary>
        /// Updates the customer reminder
        /// </summary>
        /// <param name="CustomerReminder">Customer reminder</param>
        public virtual void UpdateCustomerReminder(CustomerReminder customerReminder)
        {
            if (customerReminder == null)
                throw new ArgumentNullException("customerReminder");

            _customerReminderRepository.Update(customerReminder);

            //event notification
            _eventPublisher.EntityUpdated(customerReminder);
        }



        public virtual IPagedList<SerializeCustomerReminderHistory> GetAllCustomerReminderHistory(int customerReminderId, int pageIndex = 0, int pageSize = 2147483647)
        {
            var query = from h in _customerReminderHistoryRepository.Table
                        from l in h.Levels
                        select new SerializeCustomerReminderHistory() { CustomerId = h.CustomerId, Id = h.Id, CustomerReminderId = h.CustomerReminderId, Level = l.Level, SendDate = l.SendDate, OrderId = h.OrderId };

            query = from p in query
                    where p.CustomerReminderId == customerReminderId
                    orderby p.Level
                    select p;
            return new PagedList<SerializeCustomerReminderHistory>(query, pageIndex, pageSize);
        }

        #endregion

        #region Tasks

        public virtual void Task_AbandonedCart(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.AbandonedCart
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.AbandonedCart
                                    select cr).ToList();
            }

            foreach (var reminder in customerReminder)
            {
                var customers = (from cu in _customerRepository.Table
                                 where cu.ShoppingCartItems.Any() && cu.LastUpdateCartDateUtc > reminder.LastUpdateDate && cu.Active && !cu.Deleted
                                 && (!String.IsNullOrEmpty(cu.Email))
                                 select cu).ToList();

                foreach (var customer in customers)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.CustomerId == customer.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();
                    if (history.Any())
                    {
                        var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                        if (activereminderhistory != null)
                        {
                            var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                            var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                            if (reminderLevel != null)
                            {
                                if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                {
                                    var send = SendEmail(customer, reminder, reminderLevel.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                                }
                            }
                            else
                            {
                                CloseHistoryReminder(reminder, activereminderhistory);
                            }
                        }
                        else
                        {
                            if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                            {
                                var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                if (level != null)
                                {

                                    if (DateTime.UtcNow > customer.LastUpdateCartDateUtc.Value.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                                    {
                                        if (CheckConditions(reminder, customer))
                                        {
                                            var send = SendEmail(customer, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(customer, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                        if (level != null)
                        {

                            if (DateTime.UtcNow > customer.LastUpdateCartDateUtc.Value.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                            {
                                if (CheckConditions(reminder, customer))
                                {
                                    var send = SendEmail(customer, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Task_RegisteredCustomer(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.RegisteredCustomer
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.RegisteredCustomer
                                    select cr).ToList();
            }
            foreach (var reminder in customerReminder)
            {
                var customers = (from cu in _customerRepository.Table
                                 where cu.CreatedOnUtc > reminder.LastUpdateDate && cu.Active && !cu.Deleted
                                 && (!String.IsNullOrEmpty(cu.Email))
                                 && !cu.IsSystemAccount
                                 select cu).ToList();

                foreach (var customer in customers)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.CustomerId == customer.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();
                    if (history.Any())
                    {
                        var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                        if (activereminderhistory != null)
                        {
                            var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                            var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                            if (reminderLevel != null)
                            {
                                if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                {
                                    var send = SendEmail(customer, reminder, reminderLevel.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                                }
                            }
                            else
                            {
                                CloseHistoryReminder(reminder, activereminderhistory);
                            }
                        }
                        else
                        {
                            if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                            {
                                var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                if (level != null)
                                {

                                    if (DateTime.UtcNow > customer.CreatedOnUtc.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                                    {
                                        if (CheckConditions(reminder, customer))
                                        {
                                            var send = SendEmail(customer, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(customer, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                        if (level != null)
                        {

                            if (DateTime.UtcNow > customer.CreatedOnUtc.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                            {
                                if (CheckConditions(reminder, customer))
                                {
                                    var send = SendEmail(customer, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Task_LastActivity(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.LastActivity
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.LastActivity
                                    select cr).ToList();
            }
            foreach (var reminder in customerReminder)
            {
                var customers = (from cu in _customerRepository.Table
                                 where cu.LastActivityDateUtc < reminder.LastUpdateDate && cu.Active && !cu.Deleted
                                 && (!String.IsNullOrEmpty(cu.Email))
                                 select cu).ToList();

                foreach (var customer in customers)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.CustomerId == customer.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();
                    if (history.Any())
                    {
                        var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                        if (activereminderhistory != null)
                        {
                            var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                            var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                            if (reminderLevel != null)
                            {
                                if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                {
                                    var send = SendEmail(customer, reminder, reminderLevel.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                                }
                            }
                            else
                            {
                                CloseHistoryReminder(reminder, activereminderhistory);
                            }
                        }
                        else
                        {
                            if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                            {
                                var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                if (level != null)
                                {

                                    if (DateTime.UtcNow > customer.LastActivityDateUtc.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                                    {
                                        if (CheckConditions(reminder, customer))
                                        {
                                            var send = SendEmail(customer, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(customer, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                        if (level != null)
                        {
                            if (DateTime.UtcNow > customer.LastActivityDateUtc.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes))
                            {
                                if (CheckConditions(reminder, customer))
                                {
                                    var send = SendEmail(customer, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Task_LastPurchase(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.LastPurchase
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.LastPurchase
                                    select cr).ToList();
            }
            foreach (var reminder in customerReminder)
            {
                var customers = (from cu in _customerRepository.Table
                                 where cu.LastPurchaseDateUtc < reminder.LastUpdateDate || cu.LastPurchaseDateUtc == null
                                 && (!String.IsNullOrEmpty(cu.Email)) && cu.Active && !cu.Deleted
                                 && !cu.IsSystemAccount
                                 select cu).ToList();

                foreach (var customer in customers)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.CustomerId == customer.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();
                    if (history.Any())
                    {
                        var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                        if (activereminderhistory != null)
                        {
                            var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                            var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                            if (reminderLevel != null)
                            {
                                if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                {
                                    var send = SendEmail(customer, reminder, reminderLevel.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                                }
                            }
                            else
                            {
                                CloseHistoryReminder(reminder, activereminderhistory);
                            }
                        }
                        else
                        {
                            if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                            {
                                var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                if (level != null)
                                {
                                    DateTime lastpurchaseDate = customer.LastPurchaseDateUtc.HasValue ? customer.LastPurchaseDateUtc.Value.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes) : DateTime.MinValue;
                                    if (DateTime.UtcNow > lastpurchaseDate)
                                    {
                                        if (CheckConditions(reminder, customer))
                                        {
                                            var send = SendEmail(customer, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(customer, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                        if (level != null)
                        {
                            DateTime lastpurchaseDate = customer.LastPurchaseDateUtc.HasValue ? customer.LastPurchaseDateUtc.Value.AddDays(level.Day).AddHours(level.Hour).AddMinutes(level.Minutes) : DateTime.MinValue;
                            if (DateTime.UtcNow > lastpurchaseDate)
                            {
                                if (CheckConditions(reminder, customer))
                                {
                                    var send = SendEmail(customer, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Task_Birthday(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.Birthday
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.Birthday
                                    select cr).ToList();
            }

            foreach (var reminder in customerReminder)
            {
                int day = 0;
                if (reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null)
                    day = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault().Day;

                string dateDDMM = DateTime.Now.AddDays(day).ToString("-MM-dd");

                var query = _customerRepository.Table;
                query = query.Where(x => x.Active && !x.Deleted);
                var customers = query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "Customer" &&
                        z.Attribute.Key == SystemCustomerAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateDDMM)))
                    .Select(z => z.Customer);

                //var customers = (from cu in _customerRepository.Table
                //                 where (!String.IsNullOrEmpty(cu.Email)) && 
                //                 && (cu.GetAttribute<string>(SystemCustomerAttributeNames.DateOfBirth)==dateDDMM)
                //                 select cu).ToList();

                foreach (var customer in customers)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.CustomerId == customer.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();
                    if (history.Any())
                    {
                        var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                        if (activereminderhistory != null)
                        {
                            var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                            var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                            if (reminderLevel != null)
                            {
                                if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                {
                                    var send = SendEmail(customer, reminder, reminderLevel.Id);
                                    if (send)
                                        UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                                }
                            }
                            else
                            {
                                CloseHistoryReminder(reminder, activereminderhistory);
                            }
                        }
                        else
                        {
                            if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                            {
                                var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                if (level != null)
                                {
                                    if (CheckConditions(reminder, customer))
                                    {
                                        var send = SendEmail(customer, reminder, level.Id);
                                        if (send)
                                            UpdateHistory(customer, reminder, level.Id, null);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                        if (level != null)
                        {
                            if (CheckConditions(reminder, customer))
                            {
                                var send = SendEmail(customer, reminder, level.Id);
                                if (send)
                                    UpdateHistory(customer, reminder, level.Id, null);
                            }
                        }
                    }
                }

                var activehistory = (from hc in _customerReminderHistoryRepository.Table
                                     where hc.CustomerReminderId == reminder.Id && hc.Status == (int)CustomerReminderHistoryStatusEnum.Started
                                     select hc).ToList();

                foreach (var activereminderhistory in activehistory)
                {
                    var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                    var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                    var customer = _customerRepository.Table.FirstOrDefault(x => x.Id == activereminderhistory.CustomerId);
                    if (reminderLevel != null && customer != null && customer.Active && !customer.Deleted)
                    {
                        if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                        {
                            var send = SendEmail(customer, reminder, reminderLevel.Id);
                            if (send)
                                UpdateHistory(customer, reminder, reminderLevel.Id, activereminderhistory);
                        }
                    }
                    else
                    {
                        CloseHistoryReminder(reminder, activereminderhistory);
                    }
                }
            }

        }

        public virtual void Task_CompletedOrder(int id = 0)
        {
            var dateNow = DateTime.UtcNow.Date;
            var datetimeUtcNow = DateTime.UtcNow;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.CompletedOrder
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.CompletedOrder
                                    select cr).ToList();
            }

            foreach (var reminder in customerReminder)
            {
                int day = 0;
                if (reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null)
                    day = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault().Day;

                var orders = (from or in _orderRepository.Table
                              where or.OrderStatusId == (int)OrderStatus.Complete
                              && or.CreatedOnUtc >= reminder.LastUpdateDate && or.CreatedOnUtc >= dateNow.AddDays(-day)
                              select or).ToList();

                foreach (var order in orders)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.BaseOrderId == order.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();

                    Customer customer = _customerRepository.Table.FirstOrDefault(x => x.Id == order.CustomerId && x.Active && !x.Deleted);
                    if (customer != null)
                    {
                        if (history.Any())
                        {
                            var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                            if (activereminderhistory != null)
                            {
                                var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                                var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                                if (reminderLevel != null)
                                {
                                    if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                    {
                                        var send = SendEmail(customer, order, reminder, reminderLevel.Id);
                                        if (send)
                                            UpdateHistory(order, reminder, reminderLevel.Id, activereminderhistory);
                                    }
                                }
                                else
                                {
                                    CloseHistoryReminder(reminder, activereminderhistory);
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                                {
                                    var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                    if (level != null)
                                    {
                                        if (CheckConditions(reminder, customer, order))
                                        {
                                            var send = SendEmail(customer, order, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(order, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                            if (level != null)
                            {
                                if (CheckConditions(reminder, customer, order))
                                {
                                    var send = SendEmail(customer, order, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(order, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }

                var activehistory = (from hc in _customerReminderHistoryRepository.Table
                                     where hc.CustomerReminderId == reminder.Id && hc.Status == (int)CustomerReminderHistoryStatusEnum.Started
                                     select hc).ToList();

                foreach (var activereminderhistory in activehistory)
                {
                    var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                    var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                    var order = _orderRepository.Table.FirstOrDefault(x => x.Id == activereminderhistory.BaseOrderId);
                    var customer = _customerRepository.Table.FirstOrDefault(x => x.Id == order.CustomerId && x.Active && !x.Deleted);
                    if (reminderLevel != null && order != null && customer != null)
                    {
                        if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                        {
                            var send = SendEmail(customer, order, reminder, reminderLevel.Id);
                            if (send)
                                UpdateHistory(order, reminder, reminderLevel.Id, activereminderhistory);
                        }
                    }
                    else
                    {
                        CloseHistoryReminder(reminder, activereminderhistory);
                    }
                }

            }

        }
        public virtual void Task_UnpaidOrder(int id = 0)
        {
            var datetimeUtcNow = DateTime.UtcNow;
            var dateNow = DateTime.UtcNow.Date;
            var customerReminder = new List<CustomerReminder>();
            if (id == 0)
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Active && datetimeUtcNow >= cr.StartDateTimeUtc && datetimeUtcNow <= cr.EndDateTimeUtc
                                    && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.UnpaidOrder
                                    select cr).ToList();
            }
            else
            {
                customerReminder = (from cr in _customerReminderRepository.Table
                                    where cr.Id == id && cr.ReminderRuleId == (int)CustomerReminderRuleEnum.UnpaidOrder
                                    select cr).ToList();
            }

            foreach (var reminder in customerReminder)
            {
                int day = 0;
                if (reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null)
                    day = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault().Day;

                var orders = (from or in _orderRepository.Table
                              where or.PaymentStatusId == (int)PaymentStatus.Pending
                              && or.CreatedOnUtc >= reminder.LastUpdateDate && or.CreatedOnUtc >= dateNow.AddDays(-day)
                              select or).ToList();

                foreach (var order in orders)
                {
                    var history = (from hc in _customerReminderHistoryRepository.Table
                                   where hc.BaseOrderId == order.Id && hc.CustomerReminderId == reminder.Id
                                   select hc).ToList();

                    Customer customer = _customerRepository.Table.FirstOrDefault(x => x.Id == order.CustomerId && x.Active && !x.Deleted);
                    if (customer != null)
                    {
                        if (history.Any())
                        {
                            var activereminderhistory = history.FirstOrDefault(x => x.HistoryStatus == CustomerReminderHistoryStatusEnum.Started);
                            if (activereminderhistory != null)
                            {
                                var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                                var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                                if (reminderLevel != null)
                                {
                                    if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                                    {
                                        var send = SendEmail(customer, order, reminder, reminderLevel.Id);
                                        if (send)
                                            UpdateHistory(order, reminder, reminderLevel.Id, activereminderhistory);
                                    }
                                }
                                else
                                {
                                    CloseHistoryReminder(reminder, activereminderhistory);
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow > history.Max(x => x.EndDate).AddDays(reminder.RenewedDay) && reminder.AllowRenew)
                                {
                                    var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                                    if (level != null)
                                    {
                                        if (CheckConditions(reminder, customer, order))
                                        {
                                            var send = SendEmail(customer, order, reminder, level.Id);
                                            if (send)
                                                UpdateHistory(order, reminder, level.Id, null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var level = reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() != null ? reminder.Levels.OrderBy(x => x.Level).FirstOrDefault() : null;
                            if (level != null)
                            {
                                if (CheckConditions(reminder, customer, order))
                                {
                                    var send = SendEmail(customer, order, reminder, level.Id);
                                    if (send)
                                        UpdateHistory(order, reminder, level.Id, null);
                                }
                            }
                        }
                    }
                }
                var activehistory = (from hc in _customerReminderHistoryRepository.Table
                                     where hc.CustomerReminderId == reminder.Id && hc.Status == (int)CustomerReminderHistoryStatusEnum.Started
                                     select hc).ToList();

                foreach (var activereminderhistory in activehistory)
                {
                    var lastLevel = activereminderhistory.Levels.OrderBy(x => x.SendDate).LastOrDefault();
                    var reminderLevel = reminder.Levels.FirstOrDefault(x => x.Level > lastLevel.Level);
                    var order = _orderRepository.Table.FirstOrDefault(x => x.Id == activereminderhistory.BaseOrderId);
                    var customer = _customerRepository.Table.FirstOrDefault(x => x.Id == order.CustomerId && x.Active && !x.Deleted);
                    if (reminderLevel != null && order != null && customer != null)
                    {
                        if (order.PaymentStatusId == (int)PaymentStatus.Pending)
                        {
                            if (DateTime.UtcNow > lastLevel.SendDate.AddDays(reminderLevel.Day).AddHours(reminderLevel.Hour).AddMinutes(reminderLevel.Minutes))
                            {
                                var send = SendEmail(customer, order, reminder, reminderLevel.Id);
                                if (send)
                                    UpdateHistory(order, reminder, reminderLevel.Id, activereminderhistory);
                            }
                        }
                        else
                            CloseHistoryReminder(reminder, activereminderhistory);

                    }
                    else
                    {
                        CloseHistoryReminder(reminder, activereminderhistory);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Delete a reminder condition entitity
        /// </summary>
        /// <param name="reminderConditionEntity">reminder condition Entity</param>
        public virtual void DeleteConditionEntitiy(ReminderConditionEntity reminderConditionEntity)
        {
            if (reminderConditionEntity == null)
                throw new ArgumentNullException("reminderConditionEntity");

            _reminderCondtionEntityRepository.Delete(reminderConditionEntity);

            //event notification
            _eventPublisher.EntityDeleted(reminderConditionEntity);
        }
    }

    public class SerializeCustomerReminderHistory
    {
        public int Id { get; set; }
        public int CustomerReminderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime SendDate { get; set; }
        public int Level { get; set; }
        public int OrderId { get; set; }
    }
}
