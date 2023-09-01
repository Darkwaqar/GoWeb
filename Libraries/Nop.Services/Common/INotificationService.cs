using Nop.Core;
using Nop.Core.Domain.Fcm;
using System;

namespace Nop.Services.Common
{
    /// <summary>
    /// Notification service interface
    /// </summary>
    public partial interface INotificationService
    {

        /// <summary>
        /// Deletes an notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void DeletesNotification(Notification notification);

        /// <summary>
        /// Gets an notification by notification identifier
        /// </summary>
        /// <param name="notificationId">Notification identifier</param>
        /// <returns>Notification</returns>
        Notification GetNotificationById(int notificationId);

        /// <summary>
        /// Inserts an notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void InsertNotification(Notification notification);

        /// <summary>
        /// Updates the notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void UpdateNotification(Notification notification);


        /// <summary>
        /// Gets all Notifications
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="SearchId">Search identifier</param>
        /// <param name="SearchBrand">Brand; null to load all Notifications</param>
        /// <param name="SearchCarrier">Carrier; null to load all Notifications </param>
        /// <param name="SearchOS">OS; null to load all customers</param>
        /// <param name="SearchLongitude">Longitude; 0 to load all customers</param>
        /// <param name="SearchLatitude">Latitude; 0 to load all customers</param>
        /// <param name="ShowHidden">Show Hidden; false to gett all Notifications</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Notifications</returns>
        IPagedList<Notification> GetAllNotifications(string Package = null, DateTime? CreatedOnUtc = null, DateTime? UpdatedOnUtc = null, string SearchId = null,
            string SearchBrand = null, string SearchCarrier = null, FcmApplicationType SearchFcmApplicationType = 0, decimal SearchLongitude = 0, decimal SearchLatitude = 0,
            bool ShowHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}
