using Nop.Core;
using Nop.Core.Domain.Banners;
using System.Collections.Generic;
namespace Nop.Services.Messages
{
    public partial interface IBannerService
    {
        /// <summary>
        /// Inserts a banner
        /// </summary>
        /// <param name="banner">Banner</param>        
        void InsertBanner(Banner banner);

        /// <summary>
        /// Updates a banner
        /// </summary>
        /// <param name="banner">Banner</param>
        void UpdateBanner(Banner banner);

        /// <summary>
        /// Deleted a banner
        /// </summary>
        /// <param name="banner">Banner</param>
        void DeleteBanner(Banner banner);

        /// <summary>
        /// Gets a banner by identifier
        /// </summary>
        /// <param name="bannerId">Banner identifier</param>
        /// <returns>Banner</returns>
        Banner GetBannerById(int bannerId);

        /// <summary>
        /// Gets all banner
        /// </summary>
        /// <param name="bannerName">Banner name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="vendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param> 
        /// <returns>Banners</returns>
        IPagedList<Banner> GetAllBanners(string bannerName = "", int storeId = 0,
            int vendorId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        #region Banner pictures

        /// <summary>
        /// Deletes a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        void DeleteBannerPicture(BannerPicture bannerPicture);

        /// <summary>
        /// Gets a banner pictures by banner identifier
        /// </summary>
        /// <param name="bannerId">The banner identifier</param>
        /// <returns>Banner pictures</returns>
        IList<BannerPicture> GetBannerPicturesByBannerId(int bannerId);

        /// <summary>
        /// Gets a banner picture
        /// </summary>
        /// <param name="bannerPictureId">Banner picture identifier</param>
        /// <returns>Banner picture</returns>
        BannerPicture GetBannerPictureById(int bannerPictureId);

        /// <summary>
        /// Inserts a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        void InsertBannerPicture(BannerPicture bannerPicture);

        /// <summary>
        /// Updates a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        void UpdateBannerPicture(BannerPicture bannerPicture);

        /// <summary>
        /// Get the IDs of all banner images 
        /// </summary>
        /// <param name="bannersIds">Banners IDs</param>
        /// <returns>All picture identifiers grouped by banner ID</returns>
        IDictionary<int, int[]> GetBannersImagesIds(int[] bannersIds);

        #endregion

    }
}
