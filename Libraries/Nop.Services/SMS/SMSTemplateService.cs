using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SMS;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.SMS
{
    public partial class SMSTemplateService: ISMSTemplateService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string SMSTEMPLATES_ALL_KEY = "Nop.smstemplate.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        private const string SMSTEMPLATES_BY_NAME_KEY = "Nop.smstemplate.name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SMSTEMPLATES_PATTERN_KEY = "Nop.smstemplate.";

        #endregion

        #region Fields

        private readonly IRepository<SMSTemplate> _smsTemplateRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="localizedEntityService">Localized entity service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="smsTemplateRepository">SMS template repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public SMSTemplateService(ICacheManager cacheManager,
            IRepository<StoreMapping> storeMappingRepository,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            IRepository<SMSTemplate> smsTemplateRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._storeMappingService = storeMappingService;
            this._smsTemplateRepository = smsTemplateRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a sms template
        /// </summary>
        /// <param name="smsTemplate">SMS template</param>
        public virtual void DeleteSMSTemplate(SMSTemplate smsTemplate)
        {
            if (smsTemplate == null)
                throw new ArgumentNullException("smsTemplate");

            _smsTemplateRepository.Delete(smsTemplate);

            _cacheManager.RemoveByPattern(SMSTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(smsTemplate);
        }

        /// <summary>
        /// Inserts a sms template
        /// </summary>
        /// <param name="smsTemplate">SMS template</param>
        public virtual void InsertSMSTemplate(SMSTemplate smsTemplate)
        {
            if (smsTemplate == null)
                throw new ArgumentNullException("smsTemplate");

            _smsTemplateRepository.Insert(smsTemplate);

            _cacheManager.RemoveByPattern(SMSTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(smsTemplate);
        }

        /// <summary>
        /// Updates a sms template
        /// </summary>
        /// <param name="smsTemplate">SMS template</param>
        public virtual void UpdateSMSTemplate(SMSTemplate smsTemplate)
        {
            if (smsTemplate == null)
                throw new ArgumentNullException("smsTemplate");

            _smsTemplateRepository.Update(smsTemplate);

            _cacheManager.RemoveByPattern(SMSTEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(smsTemplate);
        }

        /// <summary>
        /// Gets a sms template
        /// </summary>
        /// <param name="smsTemplateId">SMS template identifier</param>
        /// <returns>SMS template</returns>
        public virtual SMSTemplate GetSMSTemplateById(int smsTemplateId)
        {
            if (smsTemplateId == 0)
                return null;

            return _smsTemplateRepository.GetById(smsTemplateId);
        }

        /// <summary>
        /// Gets a sms template
        /// </summary>
        /// <param name="smsTemplateName">SMS template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>SMS template</returns>
        public virtual SMSTemplate GetSMSTemplateByName(string smsTemplateName, int storeId)
        {
            if (string.IsNullOrWhiteSpace(smsTemplateName))
                throw new ArgumentException("smsTemplateName");

            string key = string.Format(SMSTEMPLATES_BY_NAME_KEY, smsTemplateName, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _smsTemplateRepository.Table;
                query = query.Where(t => t.Name == smsTemplateName);
                query = query.OrderBy(t => t.Id);
                var templates = query.ToList();

                //store mapping
                if (storeId > 0)
                {
                    templates = templates
                        .Where(t => _storeMappingService.Authorize(t, storeId))
                        .ToList();
                }

                return templates.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all sms templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>SMS template list</returns>
        public virtual IList<SMSTemplate> GetAllSMSTemplates(int storeId)
        {
            string key = string.Format(SMSTEMPLATES_ALL_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _smsTemplateRepository.Table;
                query = query.OrderBy(t => t.Name);

                //Store mapping
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    query = from t in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = t.Id, c2 = "SMSTemplate" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
                            from sm in t_sm.DefaultIfEmpty()
                            where !t.LimitedToStores || storeId == sm.StoreId
                            select t;

                    //only distinct items (group by ID)
                    query = from t in query
                            group t by t.Id
                            into tGroup
                            orderby tGroup.Key
                            select tGroup.FirstOrDefault();
                    query = query.OrderBy(t => t.Name);
                }

                return query.ToList();
            });
        }

        /// <summary>
        /// Create a copy of sms template with all depended data
        /// </summary>
        /// <param name="smsTemplate">SMS template</param>
        /// <returns>SMS template copy</returns>
        public virtual SMSTemplate CopySMSTemplate(SMSTemplate smsTemplate)
        {
            if (smsTemplate == null)
                throw new ArgumentNullException("smsTemplate");

            var mtCopy = new SMSTemplate
            {
                Name = smsTemplate.Name,
                BccNumberAddresses = smsTemplate.BccNumberAddresses,
                Subject = smsTemplate.Subject,
                Body = smsTemplate.Body,
                IsActive = smsTemplate.IsActive,
                AttachedDownloadId = smsTemplate.AttachedDownloadId,
                NumberAccountId = smsTemplate.NumberAccountId,
                LimitedToStores = smsTemplate.LimitedToStores,
                DelayBeforeSend = smsTemplate.DelayBeforeSend,
                DelayPeriod = smsTemplate.DelayPeriod
            };

            InsertSMSTemplate(mtCopy);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                var bccEmailAddresses = smsTemplate.GetLocalized(x => x.BccNumberAddresses, lang.Id, false, false);
                if (!String.IsNullOrEmpty(bccEmailAddresses))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.BccNumberAddresses, bccEmailAddresses, lang.Id);

                var subject = smsTemplate.GetLocalized(x => x.Subject, lang.Id, false, false);
                if (!String.IsNullOrEmpty(subject))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Subject, subject, lang.Id);

                var body = smsTemplate.GetLocalized(x => x.Body, lang.Id, false, false);
                if (!String.IsNullOrEmpty(body))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Body, body, lang.Id);

                var emailAccountId = smsTemplate.GetLocalized(x => x.NumberAccountId, lang.Id, false, false);
                if (emailAccountId > 0)
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.NumberAccountId, emailAccountId, lang.Id);
            }

            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(smsTemplate);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(mtCopy, id);
            }

            return mtCopy;
        }

        #endregion
    }
}
