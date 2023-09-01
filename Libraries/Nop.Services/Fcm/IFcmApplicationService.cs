using Nop.Core;
using Nop.Core.Domain.Fcm;
using System.Collections.Generic;

namespace Nop.Services.Fcm
{
    public partial interface IFcmApplicationService
    {
        /// <summary>
        /// Inserts a fcmapplication
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>
        void InsertFcmApplication(FcmApplication fcmApplication);

        /// <summary>
        /// Updates a fcmapplication
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>
        void UpdateFcmApplication(FcmApplication fcmApplication);

        /// <summary>
        /// Deleted a fcmapplication
        /// </summary>
        /// <param name="fcmApplication">Fcm Application</param>
        void DeleteFcmApplication(FcmApplication fcmApplication);

        /// <summary>
        /// Deleted a fcmapplications
        /// </summary>
        /// <param name="fcmApplications">Fcm Applications</param>
        void DeleteFcmApplications(IList<FcmApplication> fcmApplications);

        /// <summary>
        /// Gets a fcmapplication by identifier
        /// </summary>
        /// <param name="fcmApplicationId">Fcm Application identifier</param>
        /// <returns>Fcm Application</returns>
        FcmApplication GetFcmApplicationById(int fcmApplicationId);

        /// <summary>
        /// Get fcmapplications by identifiers
        /// </summary>
        /// <param name="fcmApplicationIds">fcmapplication identifiers</param>
        /// <returns>Fcm Applications</returns>
        IList<FcmApplication> GetFcmApplicationsByIds(int[] fcmApplicationIds);

        /// <summary>
        /// Search fcmapplications
        /// </summary>
        /// <param name="fromFcm">From Fcm</param>
        /// <param name="toFcm">To Fcm</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent fcms</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only fcms for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort fcmapplication descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Fcm Applications</returns>
        IPagedList<FcmApplication> SearchApplications(int SearchVendorId = 0, int SearchId = 0, string SearchPackage = null, string SearchAppKey = null,
            string SearchName = null, FcmApplicationType SearchFcmApplicationType = 0, int pageIndex = 0, int pageSize = int.MaxValue);


        /// <summary>
        /// Gets all fcmapplication
        /// </summary>
        /// <returns>FcmApplication</returns>
        IList<FcmApplication> GetAllFcmApplication();
    }
}
