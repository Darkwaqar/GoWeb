
using Nop.Core;
using Nop.Core.Domain.Fcm;
using System.Collections.Generic;

namespace Nop.Services.Fcm
{
    public partial interface IFcmActionService
    {

        /// <summary>
        /// Inserts a fcmaction
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>
        void InsertFcmAction(FcmAction fcmAction);

        /// <summary>
        /// Updates a fcmaction
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>
        void UpdateFcmAction(FcmAction fcmAction);

        /// <summary>
        /// Deleted a fcmaction
        /// </summary>
        /// <param name="fcmAction">Fcm Action</param>
        void DeleteFcmAction(FcmAction fcmAction);

        /// <summary>
        /// Deleted a fcmactions
        /// </summary>
        /// <param name="fcmActions">Fcm Actions</param>
        void DeleteFcmActions(IList<FcmAction> fcmActions);

        /// <summary>
        /// Gets a fcmaction by identifier
        /// </summary>
        /// <param name="fcmActionId">Fcm Action identifier</param>
        /// <returns>Fcm Action</returns>
        FcmAction GetFcmActionById(int fcmActionId);

        /// <summary>
        /// Get fcmactions by identifiers
        /// </summary>
        /// <param name="fcmActionIds">fcmaction identifiers</param>
        /// <returns>Fcm Actions</returns>
        IList<FcmAction> GetFcmActionsByIds(int[] fcmActionIds);

        /// <summary>
        /// Search fcmactions
        /// </summary>
        /// <param name="SearchVendorId">Search by vendor identifier</param>
        /// <param name="SearchName">Search by name</param>
        /// <param name="Showhidden">Is Action status Active  ;false to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Fcm Actions</returns>
        IPagedList<FcmAction> SearchFcmActions(int SearchVendorId = 0 ,string SearchName = null, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all fcmaction
        /// </summary>
        /// <returns>FcmAction</returns>
        IList<FcmAction> GetAllFcmAction();
    }
}
