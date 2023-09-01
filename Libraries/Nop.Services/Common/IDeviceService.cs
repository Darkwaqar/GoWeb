using Nop.Core;
using Nop.Core.Domain.Fcm;
using System;

namespace Nop.Services.Common
{
    /// <summary>
    /// Device service interface
    /// </summary>
    public partial interface IDeviceService
    {
        /// <summary>
        /// Deletes an device
        /// </summary>
        /// <param name="device">Device</param>
        void DeletesDevice(Device device);

        /// <summary>
        /// Gets an device by device identifier
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <returns>Device</returns>
        Device GetDeviceById(int deviceId);

        /// <summary>
        /// Inserts an device
        /// </summary>
        /// <param name="device">Device</param>
        void InsertDevice(Device device);

        /// <summary>
        /// Updates the device
        /// </summary>
        /// <param name="device">Device</param>
        void UpdateDevice(Device device);


        /// <summary>
        /// Gets all Devices
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="SearchId">Search identifier</param>
        /// <param name="SearchBrand">Brand; null to load all Devices</param>
        /// <param name="SearchCarrier">Carrier; null to load all Devices </param>
        /// <param name="SearchOS">OS; null to load all customers</param>
        /// <param name="SearchLongitude">Longitude; 0 to load all customers</param>
        /// <param name="SearchLatitude">Latitude; 0 to load all customers</param>
        /// <param name="ShowHidden">Show Hidden; false to gett all Devices</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Devices</returns>
        IPagedList<Device> GetAllDevices(string Package = null, DateTime? CreatedOnUtc = null, DateTime? UpdatedOnUtc = null, string SearchId = null,
            string SearchBrand = null, string SearchCarrier = null, FcmApplicationType SearchFcmApplicationType = 0, decimal SearchLongitude = 0, decimal SearchLatitude = 0,
            bool ShowHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}
