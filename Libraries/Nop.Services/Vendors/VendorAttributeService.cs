using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor attribute service
    /// </summary>
    public partial class VendorAttributeService : IVendorAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// </remarks>
        private const string VENDORATTRIBUTES_ALL_KEY = "Nop.vendorattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        private const string VENDORATTRIBUTES_BY_ID_KEY = "Nop.vendorattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        private const string VENDORATTRIBUTEVALUES_BY_ID_KEY = "Nop.vendorattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        private const string VENDORATTRIBUTEVALUES_ALL_KEY = "Nop.vendorattributevalue.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string VENDORATTRIBUTES_PATTERN_KEY = "Nop.vendorattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string VENDORATTRIBUTEVALUES_PATTERN_KEY = "Nop.vendorattributevalue.";
        #endregion

        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<VendorAttribute> _vendorAttributeRepository;
        private readonly IRepository<VendorAttributeValue> _vendorAttributeValueRepository;
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
        /// <param name="vendorAttributeRepository">Vendor attribute repository</param>
        /// <param name="mediator">Mediator</param>
        public VendorAttributeService(ICacheManager cacheManager,
            IRepository<VendorAttribute> vendorAttributeRepository,
            IRepository<VendorAttributeValue> vendorAttributeValueRepository,
            IStoreMappingService storeMappingService,
            IEventPublisher eventPublisher,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IAclService aclService)
        {
            this._cacheManager = cacheManager;
            this._vendorAttributeRepository = vendorAttributeRepository;
            this._vendorAttributeValueRepository = vendorAttributeValueRepository;
            this._storeMappingService = storeMappingService;
            this._eventPublisher = eventPublisher;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._aclService = aclService;
        }

        #endregion

        #region Methods

        #region Vendor attributes

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void DeleteVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException("vendorAttribute");

            _vendorAttributeRepository.Delete(vendorAttribute);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttribute);
        }

        /// <summary>
        /// Gets all vendor attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Vendor attributes</returns>
        public virtual IList<VendorAttribute> GetAllVendorAttributes(int storeId = 0, bool ignorAcl = false)
        {
            string key = string.Format(VENDORATTRIBUTES_ALL_KEY, storeId, ignorAcl);
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _vendorAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;
                var vendorAttributes = query.ToList();
                if (storeId > 0)
                {
                    //store mapping
                    vendorAttributes = vendorAttributes.Where(ca => _storeMappingService.Authorize(ca)).ToList();
                }
                if (!ignorAcl)
                {
                    //acl mapping
                    vendorAttributes = vendorAttributes.Where(ca => _aclService.Authorize(ca)).ToList();
                }
                return vendorAttributes;
            });
        }

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        public virtual VendorAttribute GetVendorAttributeById(int vendorAttributeId)
        {
            if (vendorAttributeId == 0)
                return null;

            string key = string.Format(VENDORATTRIBUTES_BY_ID_KEY, vendorAttributeId);
            return _cacheManager.Get(key, () => _vendorAttributeRepository.GetById(vendorAttributeId));
        }

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void InsertVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException("vendorAttribute");

            _vendorAttributeRepository.Insert(vendorAttribute);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(vendorAttribute);
        }

        /// <summary>
        /// Updates the vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void UpdateVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException("vendorAttribute");

            _vendorAttributeRepository.Update(vendorAttribute);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(vendorAttribute);
        }

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void DeleteVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException("vendorAttributeValue");

            _vendorAttributeValueRepository.Delete(vendorAttributeValue);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttributeValue);
        }

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        public virtual IList<VendorAttributeValue> GetVendorAttributeValues(int vendorAttributeId)
        {
            string key = string.Format(VENDORATTRIBUTEVALUES_ALL_KEY, vendorAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _vendorAttributeValueRepository.Table
                            orderby cav.DisplayOrder, cav.Id
                            where cav.VendorAttributeId == vendorAttributeId
                            select cav;
                var vendorAttributeValues = query.ToList();
                return vendorAttributeValues;
            });
        }

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        public virtual VendorAttributeValue GetVendorAttributeValueById(int vendorAttributeValueId)
        {
            if (vendorAttributeValueId == 0)
                return null;

            string key = string.Format(VENDORATTRIBUTEVALUES_BY_ID_KEY, vendorAttributeValueId);
            return _cacheManager.Get(key, () => _vendorAttributeValueRepository.GetById(vendorAttributeValueId));
        }

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void InsertVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException("vendorAttributeValue");

            _vendorAttributeValueRepository.Insert(vendorAttributeValue);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(vendorAttributeValue);
        }

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void UpdateVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException("vendorAttributeValue");

            _vendorAttributeValueRepository.Update(vendorAttributeValue);

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}
