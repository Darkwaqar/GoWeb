using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Contact;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Contact attribute service
    /// </summary>
    public partial class ContactAttributeService : IContactAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// </remarks>
        private const string CONTACTATTRIBUTES_ALL_KEY = "Nop.contactattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : contact attribute ID
        /// </remarks>
        private const string CONTACTATTRIBUTES_BY_ID_KEY = "Nop.contactattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        private const string CONTACTATTRIBUTEVALUES_BY_ID_KEY = "Nop.contactattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string CONTACTATTRIBUTEVALUES_ALL_KEY = "Nop.contactattributevalue.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CONTACTATTRIBUTES_PATTERN_KEY = "Nop.contactattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CONTACTATTRIBUTEVALUES_PATTERN_KEY = "Nop.contactattributevalue.";
        #endregion

        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ContactAttribute> _contactAttributeRepository;
        private readonly IRepository<ContactAttributeValue> _contactAttributeValueRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="contactAttributeRepository">Contact attribute repository</param>
        /// <param name="mediator">Mediator</param>
        public ContactAttributeService(ICacheManager cacheManager,
            IRepository<ContactAttribute> contactAttributeRepository,
            IRepository<ContactAttributeValue> contactAttributeValueRepository,
            IStoreMappingService storeMappingService,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IAclService aclService)
        {
            this._cacheManager = cacheManager;
            this._contactAttributeRepository = contactAttributeRepository;
            this._contactAttributeValueRepository = contactAttributeValueRepository;
            this._storeMappingService = storeMappingService;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
        }

        #endregion

        #region Methods

        #region Contact attributes

        /// <summary>
        /// Deletes a contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        public virtual void DeleteContactAttribute(ContactAttribute contactAttribute)
        {
            if (contactAttribute == null)
                throw new ArgumentNullException("contactAttribute");

             _contactAttributeRepository.Delete(contactAttribute);

             _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityDeleted(contactAttribute);
        }

        /// <summary>
        /// Gets all contact attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Contact attributes</returns>
        public virtual IList<ContactAttribute> GetAllContactAttributes(int storeId = 0, bool ignorAcl = false)
        {
            string key = string.Format(CONTACTATTRIBUTES_ALL_KEY, storeId, ignorAcl);
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _contactAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                var contactAttributes = query.ToList();
                if (storeId > 0)
                {
                    //store mapping
                    contactAttributes = contactAttributes.Where(ca => _storeMappingService.Authorize(ca)).ToList();
                }
                if (!ignorAcl)
                {
                    //acl mapping
                    contactAttributes = contactAttributes.Where(ca => _aclService.Authorize(ca)).ToList();
                }
                return contactAttributes;
            });
        }

        /// <summary>
        /// Gets a contact attribute 
        /// </summary>
        /// <param name="contactAttributeId">Contact attribute identifier</param>
        /// <returns>Contact attribute</returns>
        public virtual ContactAttribute GetContactAttributeById(int contactAttributeId)
        {
            if (contactAttributeId == 0)
                return null;

            string key = string.Format(CONTACTATTRIBUTES_BY_ID_KEY, contactAttributeId);
            return _cacheManager.Get(key, () => _contactAttributeRepository.GetById(contactAttributeId));
        }

        /// <summary>
        /// Inserts a contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        public virtual void InsertContactAttribute(ContactAttribute contactAttribute)
        {
            if (contactAttribute == null)
                throw new ArgumentNullException("contactAttribute");

             _contactAttributeRepository.Insert(contactAttribute);

             _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityInserted(contactAttribute);
        }

        /// <summary>
        /// Updates the contact attribute
        /// </summary>
        /// <param name="contactAttribute">Contact attribute</param>
        public virtual void UpdateContactAttribute(ContactAttribute contactAttribute)
        {
            if (contactAttribute == null)
                throw new ArgumentNullException("contactAttribute");

             _contactAttributeRepository.Update(contactAttribute);

             _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityUpdated(contactAttribute);
        }

        #endregion

        #region Contact attribute values

        /// <summary>
        /// Deletes a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        public virtual void DeleteContactAttributeValue(ContactAttributeValue contactAttributeValue)
        {
            if (contactAttributeValue == null)
                throw new ArgumentNullException("contactAttributeValue");

            _contactAttributeValueRepository.Delete(contactAttributeValue);

            _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(contactAttributeValue);
        }

        /// <summary>
        /// Gets contact attribute values by contact attribute identifier
        /// </summary>
        /// <param name="contactAttributeId">The contact attribute identifier</param>
        /// <returns>Contact attribute values</returns>
        public virtual IList<ContactAttributeValue> GetContactAttributeValues(int contactAttributeId)
        {
            string key = string.Format(CONTACTATTRIBUTEVALUES_ALL_KEY, contactAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _contactAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.ContactAttributeId == contactAttributeId
                            select cav;
                var contactAttributeValues = query.ToList();
                return contactAttributeValues;
            });
        }

        /// <summary>
        /// Gets a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValueId">Contact attribute value identifier</param>
        /// <returns>Contact attribute value</returns>
        public virtual ContactAttributeValue GetContactAttributeValueById(int contactAttributeValueId)
        {
            if (contactAttributeValueId == 0)
                return null;

            string key = string.Format(CONTACTATTRIBUTEVALUES_BY_ID_KEY, contactAttributeValueId);
            return _cacheManager.Get(key, () => _contactAttributeValueRepository.GetById(contactAttributeValueId));
        }

        /// <summary>
        /// Inserts a contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        public virtual void InsertContactAttributeValue(ContactAttributeValue contactAttributeValue)
        {
            if (contactAttributeValue == null)
                throw new ArgumentNullException("contactAttributeValue");

            _contactAttributeValueRepository.Insert(contactAttributeValue);

            _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(contactAttributeValue);
        }

        /// <summary>
        /// Updates the contact attribute value
        /// </summary>
        /// <param name="contactAttributeValue">Contact attribute value</param>
        public virtual void UpdateContactAttributeValue(ContactAttributeValue contactAttributeValue)
        {
            if (contactAttributeValue == null)
                throw new ArgumentNullException("contactAttributeValue");

            _contactAttributeValueRepository.Update(contactAttributeValue);

            _cacheManager.RemoveByPattern(CONTACTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CONTACTATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(contactAttributeValue);
        }

        #endregion

        #endregion
    }
}
