using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Appointments;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Appointments
{
    /// <summary>
    /// Appointment attribute service
    /// </summary>
    public partial class AppointmentAttributeService : IAppointmentAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// </remarks>
        private const string AppointmentATTRIBUTES_ALL_KEY = "Nop.Appointmentattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Appointment attribute ID
        /// </remarks>
        private const string AppointmentATTRIBUTES_BY_ID_KEY = "Nop.Appointmentattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        private const string AppointmentATTRIBUTEVALUES_BY_ID_KEY = "Nop.Appointmentattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string AppointmentATTRIBUTEVALUES_ALL_KEY = "Nop.Appointmentattributevalue.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string AppointmentATTRIBUTES_PATTERN_KEY = "Nop.Appointmentattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string AppointmentATTRIBUTEVALUES_PATTERN_KEY = "Nop.Appointmentattributevalue.";
        #endregion

        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<AppointmentAttribute> _AppointmentAttributeRepository;
        private readonly IRepository<AppointmentAttributeValue> _AppointmentAttributeValueRepository;
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
        /// <param name="AppointmentAttributeRepository">Appointment attribute repository</param>
        /// <param name="mediator">Mediator</param>
        public AppointmentAttributeService(ICacheManager cacheManager,
            IRepository<AppointmentAttribute> AppointmentAttributeRepository,
            IRepository<AppointmentAttributeValue> AppointmentAttributeValueRepository,
            IStoreMappingService storeMappingService,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IAclService aclService)
        {
            this._cacheManager = cacheManager;
            this._AppointmentAttributeRepository = AppointmentAttributeRepository;
            this._AppointmentAttributeValueRepository = AppointmentAttributeValueRepository;
            this._storeMappingService = storeMappingService;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
        }

        #endregion

        #region Methods

        #region Appointment attributes

        /// <summary>
        /// Deletes a Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        public virtual void DeleteAppointmentAttribute(AppointmentAttribute AppointmentAttribute)
        {
            if (AppointmentAttribute == null)
                throw new ArgumentNullException("AppointmentAttribute");

             _AppointmentAttributeRepository.Delete(AppointmentAttribute);

             _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityDeleted(AppointmentAttribute);
        }

        /// <summary>
        /// Gets all Appointment attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Appointment attributes</returns>
        public virtual IList<AppointmentAttribute> GetAllAppointmentAttributes(int storeId = 0, bool ignorAcl = false)
        {
            string key = string.Format(AppointmentATTRIBUTES_ALL_KEY, storeId, ignorAcl);
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _AppointmentAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                var AppointmentAttributes = query.ToList();
                if (storeId > 0)
                {
                    //store mapping
                    AppointmentAttributes = AppointmentAttributes.Where(ca => _storeMappingService.Authorize(ca)).ToList();
                }
                if (!ignorAcl)
                {
                    //acl mapping
                    AppointmentAttributes = AppointmentAttributes.Where(ca => _aclService.Authorize(ca)).ToList();
                }
                return AppointmentAttributes;
            });
        }

        /// <summary>
        /// Gets a Appointment attribute 
        /// </summary>
        /// <param name="AppointmentAttributeId">Appointment attribute identifier</param>
        /// <returns>Appointment attribute</returns>
        public virtual AppointmentAttribute GetAppointmentAttributeById(int AppointmentAttributeId)
        {
            if (AppointmentAttributeId == 0)
                return null;

            string key = string.Format(AppointmentATTRIBUTES_BY_ID_KEY, AppointmentAttributeId);
            return _cacheManager.Get(key, () => _AppointmentAttributeRepository.GetById(AppointmentAttributeId));
        }

        /// <summary>
        /// Inserts a Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        public virtual void InsertAppointmentAttribute(AppointmentAttribute AppointmentAttribute)
        {
            if (AppointmentAttribute == null)
                throw new ArgumentNullException("AppointmentAttribute");

             _AppointmentAttributeRepository.Insert(AppointmentAttribute);

             _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityInserted(AppointmentAttribute);
        }

        /// <summary>
        /// Updates the Appointment attribute
        /// </summary>
        /// <param name="AppointmentAttribute">Appointment attribute</param>
        public virtual void UpdateAppointmentAttribute(AppointmentAttribute AppointmentAttribute)
        {
            if (AppointmentAttribute == null)
                throw new ArgumentNullException("AppointmentAttribute");

             _AppointmentAttributeRepository.Update(AppointmentAttribute);

             _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
             _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
             _eventPublisher.EntityUpdated(AppointmentAttribute);
        }

        #endregion

        #region Appointment attribute values

        /// <summary>
        /// Deletes a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        public virtual void DeleteAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue)
        {
            if (AppointmentAttributeValue == null)
                throw new ArgumentNullException("AppointmentAttributeValue");

            _AppointmentAttributeValueRepository.Delete(AppointmentAttributeValue);

            _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(AppointmentAttributeValue);
        }

        /// <summary>
        /// Gets Appointment attribute values by Appointment attribute identifier
        /// </summary>
        /// <param name="AppointmentAttributeId">The Appointment attribute identifier</param>
        /// <returns>Appointment attribute values</returns>
        public virtual IList<AppointmentAttributeValue> GetAppointmentAttributeValues(int AppointmentAttributeId)
        {
            string key = string.Format(AppointmentATTRIBUTEVALUES_ALL_KEY, AppointmentAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _AppointmentAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.AppointmentAttributeId == AppointmentAttributeId
                            select cav;
                var AppointmentAttributeValues = query.ToList();
                return AppointmentAttributeValues;
            });
        }

        /// <summary>
        /// Gets a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValueId">Appointment attribute value identifier</param>
        /// <returns>Appointment attribute value</returns>
        public virtual AppointmentAttributeValue GetAppointmentAttributeValueById(int AppointmentAttributeValueId)
        {
            if (AppointmentAttributeValueId == 0)
                return null;

            string key = string.Format(AppointmentATTRIBUTEVALUES_BY_ID_KEY, AppointmentAttributeValueId);
            return _cacheManager.Get(key, () => _AppointmentAttributeValueRepository.GetById(AppointmentAttributeValueId));
        }

        /// <summary>
        /// Inserts a Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        public virtual void InsertAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue)
        {
            if (AppointmentAttributeValue == null)
                throw new ArgumentNullException("AppointmentAttributeValue");

            _AppointmentAttributeValueRepository.Insert(AppointmentAttributeValue);

            _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(AppointmentAttributeValue);
        }

        /// <summary>
        /// Updates the Appointment attribute value
        /// </summary>
        /// <param name="AppointmentAttributeValue">Appointment attribute value</param>
        public virtual void UpdateAppointmentAttributeValue(AppointmentAttributeValue AppointmentAttributeValue)
        {
            if (AppointmentAttributeValue == null)
                throw new ArgumentNullException("AppointmentAttributeValue");

            _AppointmentAttributeValueRepository.Update(AppointmentAttributeValue);

            _cacheManager.RemoveByPattern(AppointmentATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AppointmentATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(AppointmentAttributeValue);
        }

        #endregion

        #endregion
    }
}
