

using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public partial interface IPopupService
    {
        /// <summary>
        /// Inserts a popup
        /// </summary>
        /// <param name="popup">Popup</param>        
        void InsertPopupActive(PopupActive popup);
        /// <summary>
        /// Gets active banner for customer
        /// </summary>
        /// <returns>BannerActive</returns>
        PopupActive GetActivePopupByCustomerId(int customerId);

        /// <summary>
        /// Move popup to archive
        /// </summary>
        void MovepopupToArchive(int id, int customerId);

    }
}
