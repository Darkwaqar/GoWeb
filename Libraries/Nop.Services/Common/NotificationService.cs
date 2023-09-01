using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Fcm;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Common
{
    public partial class NotificationService : INotificationService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address ID
        /// </remarks>
        private const string NOTIFICATION_BY_ID_KEY = "Nop.notification.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string NOTIFICATION_PATTERN_KEY = "Nop.notification.";

        #endregion

        #region Fields

        private readonly IRepository<Notification> _notificationRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;



        #endregion

        #region Ctor
        public NotificationService(ICacheManager cacheManager,
            IRepository<Notification> notificationRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._notificationRepository = notificationRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion


        /// <summary>
        /// Deletes an notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public void DeletesNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            _notificationRepository.Delete(notification);

            //cache
            _cacheManager.RemoveByPattern(NOTIFICATION_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(notification);
        }



        /// <summary>
        /// Gets an notification by notification identifier
        /// </summary>
        /// <param name="notificationId">notification identifier</param>
        /// <returns>Notification</returns>
        public Notification GetNotificationById(int notificationId)
        {
            if (notificationId == 0)
                return null;

            string key = string.Format(NOTIFICATION_BY_ID_KEY, notificationId);
            return _cacheManager.Get(key, () => _notificationRepository.GetById(notificationId));
        }


        /// <summary>
        /// Inserts an notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public void InsertNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            notification.CreatedOnUtc = DateTime.UtcNow;

            _notificationRepository.Insert(notification);

            //cache
            _cacheManager.RemoveByPattern(NOTIFICATION_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(notification);
        }

        /// <summary>
        /// Updates the notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public void UpdateNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            notification.UpdatedOnUtc = DateTime.UtcNow;
            _notificationRepository.Update(notification);

            //cache
            _cacheManager.RemoveByPattern(NOTIFICATION_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(notification);
        }


        /// <summary>
        /// Gets all Notifications
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="SearchId">Search identifier</param>
        /// <param name="SearchBrand">Brand; null to load all Notifications</param>
        /// <param name="SearchCarrier">Carrier; null to load all Notifications </param>
        /// <param name="SearchOS">OS; null to load all Notifications</param>
        /// <param name="SearchLongitude">Longitude; 0 to load all Notifications</param>
        /// <param name="SearchLatitude">Latitude; 0 to load all Notifications</param>
        /// <param name="ShowHidden">Show Hidden; false to gett all Notifications</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Notifications</returns>
        public IPagedList<Notification> GetAllNotifications(string Package = null, DateTime? CreatedOnUtc = null, DateTime? UpdatedOnUtc = null, string SearchId = null, string SearchBrand = null,
            string SearchCarrier = null, FcmApplicationType SearchFcmApplicationType = 0, decimal SearchLongitude = 0, decimal SearchLatitude = 0, bool ShowHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _notificationRepository.Table;
            if (CreatedOnUtc.HasValue)
                query = query.Where(c => CreatedOnUtc.Value <= c.CreatedOnUtc);
            if (UpdatedOnUtc.HasValue)
                query = query.Where(c => UpdatedOnUtc.Value >= c.CreatedOnUtc);
           
            query = query.OrderByDescending(c => c.Id);
            var notifications = new PagedList<Notification>(query, pageIndex, pageSize);

            return notifications;
        }
    }
}
