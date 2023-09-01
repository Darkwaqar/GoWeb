using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Interactive;
using Nop.Services.Events;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Messages
{
    public partial class InteractiveFormAttributeService : IInteractiveFormAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// </remarks>
        private const string INTERACTIVE_FORMATTRIBUTES_ALL_KEY = "Nop.interactivefromattribute.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : InteractiveFrom attribute ID
        /// </remarks>
        private const string INTERACTIVE_FORMATTRIBUTES_BY_ID_KEY = "Nop.interactivefromattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        private const string INTERACTIVE_FORMATTRIBUTEVALUES_BY_ID_KEY = "Nop.interactivefromattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string INTERACTIVE_FORMATTRIBUTEVALUES_ALL_KEY = "Nop.interactivefromattributevalue.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY = "Nop.interactivefromattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY = "Nop.interactivefromattributevalue.";
        #endregion

        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<InteractiveFormAttribute> _InteractiveFromAttributeRepository;
        private readonly IRepository<InteractiveFormAttributeValue> _InteractiveFromAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _workContext;
        private readonly IAclService _aclService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="InteractiveFromAttributeRepository">InteractiveForm attribute repository</param>
        /// <param name="mediator">Mediator</param>
        public InteractiveFormAttributeService(ICacheManager cacheManager,
            IRepository<InteractiveFormAttribute> InteractiveFromAttributeRepository,
            IRepository<InteractiveFormAttributeValue> InteractiveFromAttributeValueRepository,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            IAclService aclService)
        {
            this._cacheManager = cacheManager;
            this._InteractiveFromAttributeRepository = InteractiveFromAttributeRepository;
            this._InteractiveFromAttributeValueRepository = InteractiveFromAttributeValueRepository;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._aclService = aclService;
        }

        #endregion

        #region Methods

        #region InteractiveForm attributes

        /// <summary>
        /// Deletes a InteractiveFrom attribute
        /// </summary>
        /// <param name="InteractiveFromAttribute">InteractiveForm attribute</param>
        public virtual void DeleteInteractiveFormAttribute(InteractiveFormAttribute InteractiveFromAttribute)
        {
            if (InteractiveFromAttribute == null)
                throw new ArgumentNullException("InteractiveFromAttribute");

            _InteractiveFromAttributeRepository.Delete(InteractiveFromAttribute);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(InteractiveFromAttribute);
        }

        /// <summary>
        /// Gets all InteractiveFrom attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>InteractiveForm attributes</returns>
        public virtual IList<InteractiveFormAttribute> GetAllInteractiveFormAttributes(int InteractiveFormId = 0)
        {
            string key = string.Format(INTERACTIVE_FORMATTRIBUTES_ALL_KEY, InteractiveFormId);
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _InteractiveFromAttributeRepository.Table
                            where ca.InteractiveFormId == InteractiveFormId
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                var InteractiveFromAttributes = query.ToList();
                return InteractiveFromAttributes;
            });
        }

        /// <summary>
        /// Gets a InteractiveFrom attribute 
        /// </summary>
        /// <param name="InteractiveFromAttributeId">InteractiveForm attribute identifier</param>
        /// <returns>InteractiveForm attribute</returns>
        public virtual InteractiveFormAttribute GetInteractiveFormAttributeById(int InteractiveFromAttributeId)
        {
            if (InteractiveFromAttributeId == 0)
                return null;

            string key = string.Format(INTERACTIVE_FORMATTRIBUTES_BY_ID_KEY, InteractiveFromAttributeId);
            return _cacheManager.Get(key, () => _InteractiveFromAttributeRepository.GetById(InteractiveFromAttributeId));
        }

        /// <summary>
        /// Inserts a InteractiveFrom attribute
        /// </summary>
        /// <param name="InteractiveFromAttribute">InteractiveForm attribute</param>
        public virtual void InsertInteractiveFormAttribute(InteractiveFormAttribute InteractiveFromAttribute)
        {
            if (InteractiveFromAttribute == null)
                throw new ArgumentNullException("InteractiveFromAttribute");

            _InteractiveFromAttributeRepository.Insert(InteractiveFromAttribute);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(InteractiveFromAttribute);
        }

        /// <summary>
        /// Updates the InteractiveFrom attribute
        /// </summary>
        /// <param name="InteractiveFromAttribute">InteractiveForm attribute</param>
        public virtual void UpdateInteractiveFormAttribute(InteractiveFormAttribute InteractiveFromAttribute)
        {
            if (InteractiveFromAttribute == null)
                throw new ArgumentNullException("InteractiveFromAttribute");

            _InteractiveFromAttributeRepository.Update(InteractiveFromAttribute);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(InteractiveFromAttribute);
        }

        #endregion

        #region InteractiveForm attribute values

        /// <summary>
        /// Deletes a InteractiveFrom attribute value
        /// </summary>
        /// <param name="InteractiveFromAttributeValue">InteractiveForm attribute value</param>
        public virtual void DeleteInteractiveFormAttributeValue(InteractiveFormAttributeValue InteractiveFromAttributeValue)
        {
            if (InteractiveFromAttributeValue == null)
                throw new ArgumentNullException("InteractiveFromAttributeValue");

            _InteractiveFromAttributeValueRepository.Delete(InteractiveFromAttributeValue);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(InteractiveFromAttributeValue);
        }

        /// <summary>
        /// Gets InteractiveFrom attribute values by InteractiveFrom attribute identifier
        /// </summary>
        /// <param name="InteractiveFromAttributeId">The InteractiveFrom attribute identifier</param>
        /// <returns>InteractiveForm attribute values</returns>
        public virtual IList<InteractiveFormAttributeValue> GetInteractiveFormAttributeValues(int InteractiveFromAttributeId)
        {
            string key = string.Format(INTERACTIVE_FORMATTRIBUTEVALUES_ALL_KEY, InteractiveFromAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _InteractiveFromAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.InteractiveFormAttributeId == InteractiveFromAttributeId
                            select cav;
                var InteractiveFromAttributeValues = query.ToList();
                return InteractiveFromAttributeValues;
            });
        }

        /// <summary>
        /// Gets a InteractiveFrom attribute value
        /// </summary>
        /// <param name="InteractiveFromAttributeValueId">InteractiveForm attribute value identifier</param>
        /// <returns>InteractiveForm attribute value</returns>
        public virtual InteractiveFormAttributeValue GetInteractiveFormAttributeValueById(int InteractiveFromAttributeValueId)
        {
            if (InteractiveFromAttributeValueId == 0)
                return null;

            string key = string.Format(INTERACTIVE_FORMATTRIBUTEVALUES_BY_ID_KEY, InteractiveFromAttributeValueId);
            return _cacheManager.Get(key, () => _InteractiveFromAttributeValueRepository.GetById(InteractiveFromAttributeValueId));
        }

        /// <summary>
        /// Inserts a InteractiveFrom attribute value
        /// </summary>
        /// <param name="InteractiveFromAttributeValue">InteractiveForm attribute value</param>
        public virtual void InsertInteractiveFormAttributeValue(InteractiveFormAttributeValue InteractiveFromAttributeValue)
        {
            if (InteractiveFromAttributeValue == null)
                throw new ArgumentNullException("InteractiveFromAttributeValue");

            _InteractiveFromAttributeValueRepository.Insert(InteractiveFromAttributeValue);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(InteractiveFromAttributeValue);
        }

        /// <summary>
        /// Updates the InteractiveFrom attribute value
        /// </summary>
        /// <param name="InteractiveFromAttributeValue">InteractiveForm attribute value</param>
        public virtual void UpdateInteractiveFormAttributeValue(InteractiveFormAttributeValue InteractiveFromAttributeValue)
        {
            if (InteractiveFromAttributeValue == null)
                throw new ArgumentNullException("InteractiveFromAttributeValue");

            _InteractiveFromAttributeValueRepository.Update(InteractiveFromAttributeValue);

            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(INTERACTIVE_FORMATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(InteractiveFromAttributeValue);
        }

        #endregion

        #endregion
    }
}
