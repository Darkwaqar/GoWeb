using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Services.Events;
using System;
using Nop.Core;
using System.Linq;
using Nop.Core.Domain.Fcm;

namespace Nop.Services.Common
{
    public partial class DeviceService : IDeviceService
    {

        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address ID
        /// </remarks>
        private const string DEVICE_BY_ID_KEY = "Nop.device.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string DEVICE_PATTERN_KEY = "Nop.device.";

        #endregion

        #region Fields

        private readonly IRepository<Device> _deviceRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;



        #endregion

        #region Ctor
        public DeviceService(ICacheManager cacheManager,
            IRepository<Device> deviceRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._deviceRepository = deviceRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion


        /// <summary>
        /// Deletes an device
        /// </summary>
        /// <param name="device">Device</param>
        public void DeletesDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _deviceRepository.Delete(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(device);
        }



        /// <summary>
        /// Gets an device by device identifier
        /// </summary>
        /// <param name="deviceId">device identifier</param>
        /// <returns>Device</returns>
        public Device GetDeviceById(int deviceId)
        {
            if (deviceId == 0)
                return null;

            string key = string.Format(DEVICE_BY_ID_KEY, deviceId);
            return _cacheManager.Get(key, () => _deviceRepository.GetById(deviceId));
        }


        /// <summary>
        /// Inserts an device
        /// </summary>
        /// <param name="device">Device</param>
        public void InsertDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            device.CreatedOnUtc = DateTime.UtcNow;

            _deviceRepository.Insert(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(device);
        }

        /// <summary>
        /// Updates the device
        /// </summary>
        /// <param name="device">Device</param>
        public void UpdateDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            device.UpdatedOnUtc = DateTime.UtcNow;
            _deviceRepository.Update(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(device);
        }


        /// <summary>
        /// Gets all Devices
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="SearchId">Search identifier</param>
        /// <param name="SearchBrand">Brand; null to load all Devices</param>
        /// <param name="SearchCarrier">Carrier; null to load all Devices </param>
        /// <param name="SearchOS">OS; null to load all Devices</param>
        /// <param name="SearchLongitude">Longitude; 0 to load all Devices</param>
        /// <param name="SearchLatitude">Latitude; 0 to load all Devices</param>
        /// <param name="ShowHidden">Show Hidden; false to gett all Devices</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Devices</returns>
        public IPagedList<Device> GetAllDevices(string Package = null, DateTime? CreatedOnUtc = null, DateTime? UpdatedOnUtc = null, string SearchId = null, string SearchBrand = null,
            string SearchCarrier = null, FcmApplicationType SearchFcmApplicationType = 0, decimal SearchLongitude = 0, decimal SearchLatitude = 0, bool ShowHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _deviceRepository.Table;
            if (CreatedOnUtc.HasValue)
                query = query.Where(c => CreatedOnUtc.Value <= c.CreatedOnUtc);
            if (UpdatedOnUtc.HasValue)
                query = query.Where(c => UpdatedOnUtc.Value >= c.CreatedOnUtc);
            if (!String.IsNullOrWhiteSpace(Package))
                query = query.Where(c => c.Package.Equals(Package, StringComparison.Ordinal));
            if (!String.IsNullOrWhiteSpace(SearchId))
                query = query.Where(c => c.DeviceId.Contains(SearchId));

            if (!String.IsNullOrWhiteSpace(SearchBrand))
                query = query.Where(c => c.Brand.Contains(SearchBrand));

            if (!String.IsNullOrWhiteSpace(SearchCarrier))
                query = query.Where(c => c.Carrier.Contains(SearchCarrier));

            if (SearchFcmApplicationType != 0)
            {
                query = query.Where(qe => qe.DeviceOS == SearchFcmApplicationType);
            }

            if (SearchLongitude > 0)
            {
                query = query.Where(c => c.Longitude == SearchLongitude);
            }

            if (SearchLatitude > 0)
            {
                query = query.Where(c => c.Latitude == SearchLatitude);
            }

            if (!ShowHidden)
            {
                query = query.Where(c => c.Active);
            }
            query = query.OrderByDescending(c => c.Id);
            var devices = new PagedList<Device>(query, pageIndex, pageSize);

            return devices;
        }
    }
}
