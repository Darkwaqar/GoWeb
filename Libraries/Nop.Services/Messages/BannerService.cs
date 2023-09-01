using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Banners;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Messages
{
    public partial class BannerService : IBannerService
    {
        #region Fields

        private readonly IRepository<Banner> _bannerRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<BannerPicture> _bannerPictureRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="bannerRepository">Banner repository</param>
        /// <param name="mediator">Mediator</param>
        public BannerService(IRepository<Banner> bannerRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<BannerPicture> bannerPictureRepository,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings)
        {
            this._bannerRepository = bannerRepository;
            this._eventPublisher = eventPublisher;
            this._storeMappingRepository = storeMappingRepository;
            this._bannerPictureRepository = bannerPictureRepository;
            this._catalogSettings=catalogSettings;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Inserts a banner
        /// </summary>
        /// <param name="banner">Banner</param>        
        public virtual void InsertBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");

            _bannerRepository.Insert(banner);

            //event notification
            _eventPublisher.EntityInserted(banner);
        }

        /// <summary>
        /// Updates a banner
        /// </summary>
        /// <param name="banner">Banner</param>
        public virtual void UpdateBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");

            _bannerRepository.Update(banner);

            //event notification
            _eventPublisher.EntityUpdated(banner);
        }

        /// <summary>
        /// Deleted a banner
        /// </summary>
        /// <param name="banner">Banner</param>
        public virtual void DeleteBanner(Banner banner)
        {
            if (banner == null)
                throw new ArgumentNullException("banner");

            _bannerRepository.Delete(banner);

            //event notification
            _eventPublisher.EntityDeleted(banner);
        }

        /// <summary>
        /// Gets a banner by identifier
        /// </summary>
        /// <param name="bannerId">Banner identifier</param>
        /// <returns>Banner</returns>
        public virtual Banner GetBannerById(int bannerId)
        {
            return _bannerRepository.GetById(bannerId);
        }

        /// <summary>
        /// Gets all banners
        /// </summary>
        /// <param name="bannerName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="VendorId">Vendor identifier; 0 if you want to get all records</param>
        /// <param name="bannerID">Category identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Banners</returns>
        public virtual IPagedList<Banner> GetAllBanners(string bannerName = "", int storeId = 0, int vendorId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {

            var query = _bannerRepository.Table;

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                //Store mapping
                query = from c in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = c.Id, c2 = "Banner" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into c_sm
                        from sm in c_sm.DefaultIfEmpty()
                        where !c.LimitedToStores || storeId == sm.StoreId
                        select c;
            }

            if (!showHidden)
                query = query.Where(c => c.Published);

            if (!String.IsNullOrWhiteSpace(bannerName))
                query = query.Where(c => c.Name.Contains(bannerName));
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);
            if (vendorId != 0)
            {
                query = query.Where(c => c.VendorId == vendorId);
            }
            var banners = query.ToList();
            //paging
            return new PagedList<Banner>(banners, pageIndex, pageSize);
        }


        #region Banner pictures

        /// <summary>
        /// Deletes a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        public virtual void DeleteBannerPicture(BannerPicture bannerPicture)
        {
            if (bannerPicture == null)
                throw new ArgumentNullException("bannerPicture");

            _bannerPictureRepository.Delete(bannerPicture);

            //event notification
            _eventPublisher.EntityDeleted(bannerPicture);
        }

        /// <summary>
        /// Gets a banner pictures by banner identifier
        /// </summary>
        /// <param name="bannerId">The banner identifier</param>
        /// <returns>Banner pictures</returns>
        public virtual IList<BannerPicture> GetBannerPicturesByBannerId(int bannerId)
        {
            var query = from pp in _bannerPictureRepository.Table
                        where pp.BannerId == bannerId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;
            var bannerPictures = query.ToList();
            return bannerPictures;
        }

        /// <summary>
        /// Gets a banner picture
        /// </summary>
        /// <param name="bannerPictureId">Banner picture identifier</param>
        /// <returns>Banner picture</returns>
        public virtual BannerPicture GetBannerPictureById(int bannerPictureId)
        {
            if (bannerPictureId == 0)
                return null;

            return _bannerPictureRepository.GetById(bannerPictureId);
        }

        /// <summary>
        /// Inserts a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        public virtual void InsertBannerPicture(BannerPicture bannerPicture)
        {
            if (bannerPicture == null)
                throw new ArgumentNullException("bannerPicture");

            _bannerPictureRepository.Insert(bannerPicture);

            //event notification
            _eventPublisher.EntityInserted(bannerPicture);
        }

        /// <summary>
        /// Updates a banner picture
        /// </summary>
        /// <param name="bannerPicture">Banner picture</param>
        public virtual void UpdateBannerPicture(BannerPicture bannerPicture)
        {
            if (bannerPicture == null)
                throw new ArgumentNullException("bannerPicture");

            _bannerPictureRepository.Update(bannerPicture);

            //event notification
            _eventPublisher.EntityUpdated(bannerPicture);
        }

        /// <summary>
        /// Get the IDs of all banner images 
        /// </summary>
        /// <param name="bannersIds">Banners IDs</param>
        /// <returns>All picture identifiers grouped by banner ID</returns>
        public IDictionary<int, int[]> GetBannersImagesIds(int[] bannersIds)
        {
            return _bannerPictureRepository.Table.Where(p => bannersIds.Contains(p.BannerId))
                .GroupBy(p => p.BannerId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        #endregion
        #endregion
    }
}
