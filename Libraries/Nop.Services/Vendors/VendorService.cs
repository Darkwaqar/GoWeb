using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Services.Events;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service
    /// </summary>
    public partial class VendorService : IVendorService
    {

        #region Fields

        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorNote> _vendorNoteRepository;
        //extended
        private readonly IRepository<MobileAppSetting> _mobileAppSettingRepository;
        private readonly IRepository<VendorBanner> _vendorBannerRepository;
        private readonly IRepository<SocialLinks> _socialLinksRepository;
        private readonly IRepository<VendorPaymentInfo> _vendorPaymentInfoRepository;
        private readonly IRepository<VendorPayout> _vendorPayoutRepository;
        private readonly IRepository<VendorReview> _vendorReviewRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IRepository<VendorMappedCategory> _vendorMappedCategoryRepository;

        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="vendorRepository">Vendor repository</param>
        /// <param name="vendorNoteRepository">Vendor note repository</param>
        /// <param name="eventPublisher">Event published</param>
        public VendorService(IRepository<Vendor> vendorRepository,
            IRepository<VendorNote> vendorNoteRepository,
            IEventPublisher eventPublisher,
            IRepository<MobileAppSetting> mobileAppSettingRepository,
            IRepository<VendorBanner> vendorBannerRepository,
            IRepository<SocialLinks> socailLinksRepository,
            IRepository<VendorPaymentInfo> vendorPaymentInfoRepository,
            IRepository<VendorPayout> vendorPayoutRepository,
            IRepository<VendorReview> vendorReviewRepository,
            IRepository<VendorMappedCategory> vendorMappedCategoryRepository,
            ICacheManager cacheManager,
            CatalogSettings catalogSettings,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreContext storeContext)
        {
            this._vendorRepository = vendorRepository;
            this._vendorNoteRepository = vendorNoteRepository;
            this._eventPublisher = eventPublisher;
            this._mobileAppSettingRepository = mobileAppSettingRepository;
            this._vendorBannerRepository = vendorBannerRepository;
            this._socialLinksRepository = socailLinksRepository;
            this._vendorPaymentInfoRepository = vendorPaymentInfoRepository;
            this._vendorPayoutRepository = vendorPayoutRepository;
            this._vendorReviewRepository = vendorReviewRepository;
            this._vendorMappedCategoryRepository = vendorMappedCategoryRepository;
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
            this._storeMappingRepository = storeMappingRepository;
            this._storeContext = storeContext;
        }


        #endregion

        #region VendorServices
        #region Methods

        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        public virtual Vendor GetVendorById(int vendorId)
        {
            if (vendorId == 0)
                return null;

            return _vendorRepository.GetById(vendorId);
        }

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void DeleteVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            vendor.Deleted = true;
            UpdateVendor(vendor);

            //event notification
            _eventPublisher.EntityDeleted(vendor);
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendorsLimitedToStore(string name = "",
            int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            if ((storeId > 0 && !_catalogSettings.IgnoreStoreLimitations))
            {
               
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    
                    //Store mapping
                    query = from m in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = "Vendor" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                            from sm in m_sm.DefaultIfEmpty()
                            where !m.LimitedToStores || storeId == sm.StoreId
                            select m;
                }
                //only distinct manufacturers (group by ID)
                query = from m in query
                        group m by m.Id
                            into mGroup
                        orderby mGroup.Key
                        select mGroup.FirstOrDefault();
                query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id);
            }

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetAllVendors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            if (!showHidden && (!_catalogSettings.IgnoreAcl || !_catalogSettings.IgnoreStoreLimitations))
            {
                if (!_catalogSettings.IgnoreStoreLimitations)
                {
                    //Store mapping
                    var currentStoreId = _storeContext.CurrentStore.Id;
                    query = from m in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = m.Id, c2 = "Vendor" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into m_sm
                            from sm in m_sm.DefaultIfEmpty()
                            where !m.LimitedToStores || currentStoreId == sm.StoreId
                            select m;
                }
                //only distinct vendor (group by ID)
                query = from c in query
                        group c by c.Id
                        into cGroup
                        orderby cGroup.Key
                        select cGroup.FirstOrDefault();
                query = query.OrderBy(pc => pc.DisplayOrder).ThenBy(pc => pc.Id);
            }

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void InsertVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Insert(vendor);

            //event notification
            _eventPublisher.EntityInserted(vendor);
        }

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        public virtual void UpdateVendor(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException("vendor");

            _vendorRepository.Update(vendor);

            //event notification
            _eventPublisher.EntityUpdated(vendor);
        }



        /// <summary>
        /// Gets a vendor note note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        public virtual VendorNote GetVendorNoteById(int vendorNoteId)
        {
            if (vendorNoteId == 0)
                return null;

            return _vendorNoteRepository.GetById(vendorNoteId);
        }

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        public virtual void DeleteVendorNote(VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException("vendorNote");

            _vendorNoteRepository.Delete(vendorNote);

            //event notification
            _eventPublisher.EntityDeleted(vendorNote);
        }

        #endregion

        #endregion

        #region Extended Services

        #region SeachVendor

        public virtual IPagedList<Vendor> SearchVendors(string search = "",
               int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _vendorRepository.Table;
            if (!String.IsNullOrWhiteSpace(search))
                query = query.Where(v => v.Name.Contains(search) || v.Url.Contains(search) || v.MetaKeywords.Contains(search) || v.MetaTitle.Contains(search) || v.MetaDescription.Contains(search) || v.AdminComment.Contains(search));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        /// <summary>
        /// Gets a vendor by Category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        public virtual IPagedList<Vendor> GetVendorByCategoryID(int categoryId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {


            var query = from vmc in _vendorMappedCategoryRepository.Table
                        join v in _vendorRepository.Table on vmc.VendorId equals v.Id
                        where vmc.CategoryId == categoryId &&
                              !v.Deleted &&
                              (showHidden || v.Active)
                        orderby vmc.DisplayOrder, vmc.Id
                        select v;

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);
            return vendors;
        }

        #endregion

        #region GetAllFeaturedVendors
        public IPagedList<Vendor> GetAllFeaturedVendors(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from v in _vendorRepository.Table
                        orderby v.DisplayOrder, v.Name
                        where v.Active &&
                        !v.Deleted &&
                        v.IsFeatured
                        select v;

            var vendors = new PagedList<Vendor>(query, pageIndex, pageSize);

            return vendors;
        }
        #endregion

        #region Banner

        /// <summary>
        /// Gets a vendor banners by vendor identifier
        /// </summary>
        /// <param name="vendorId">The vendor identifier</param>
        /// <returns>Product pictures</returns>
        public virtual IList<VendorBanner> GetVendorBannersByVendorId(int vendorId)
        {
            var query = from pp in _vendorBannerRepository.Table
                        where pp.VendorId == vendorId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;
            var VendorBanners = query.ToList();
            return VendorBanners;
        }

        /// <summary>
        /// Updates a vendor banner
        /// </summary>
        /// <param name="productPicture">Vendor banner</param>
        public virtual void UpdateVendorBanner(VendorBanner vendorBanner)
        {
            if (vendorBanner == null)
                throw new ArgumentNullException("vendorBanner");

            _vendorBannerRepository.Update(vendorBanner);

            //event notification
            _eventPublisher.EntityUpdated(vendorBanner);
        }


        public void DeleteVendorBanner(VendorBanner vendorBanner)
        {
            if (vendorBanner == null)
                throw new ArgumentNullException("vendor banner");

            _vendorBannerRepository.Delete(vendorBanner);
            _eventPublisher.EntityDeleted(vendorBanner);
        }

        public VendorBanner GetVendorBannerById(int vendorbannerId)
        {
            if (vendorbannerId == 0)
                return null;

            return _vendorBannerRepository.GetById(vendorbannerId);
        }



        public void InsertVendorBanner(VendorBanner vendorBanner)
        {
            if (vendorBanner == null)
                throw new ArgumentNullException("vendor banner");

            _vendorBannerRepository.Insert(vendorBanner);

            //event notification		
            _eventPublisher.EntityInserted(vendorBanner);
        }
        #endregion

        #region MobileAppSetting

        public MobileAppSetting GetMobileAppSetting(int VendorId)
        {
            return _mobileAppSettingRepository.Table.FirstOrDefault(x => x.VendorId == VendorId);
        }

        public void InsertMobileAppSetting(MobileAppSetting MobileAppSetting)
        {
            _mobileAppSettingRepository.Insert(MobileAppSetting);
        }

        public void UpdateMobileAppSetting(MobileAppSetting MobileAppSetting)
        {
            _mobileAppSettingRepository.Update(MobileAppSetting);
            _eventPublisher.EntityUpdated(MobileAppSetting);
        }




        #endregion

        #region SocialLinks
        public SocialLinks GetSocialLinks(int VendorId)
        {
            return _socialLinksRepository.Table.FirstOrDefault(x => x.VendorId == VendorId);
        }

        public void InsertSocialLinks(SocialLinks SocialLinks)
        {
            _socialLinksRepository.Insert(SocialLinks);
        }

        public void UpdateSocialLinks(SocialLinks SocialLinks)
        {
            _socialLinksRepository.Update(SocialLinks);
            _eventPublisher.EntityUpdated(SocialLinks);
        }


        #endregion

        #region VendorPaymentinfo

        public VendorPaymentInfo GetVendorPaymentInfo(int VendorId)
        {
            return _vendorPaymentInfoRepository.Table.FirstOrDefault(x => x.VendorId == VendorId);
        }

        public void InsertVendorPaymentInfo(VendorPaymentInfo VendorPaymentInfo)
        {
            _vendorPaymentInfoRepository.Insert(VendorPaymentInfo);
        }

        public void UpdateVendorPaymentInfo(VendorPaymentInfo VendorPaymentInfo)
        {
            _vendorPaymentInfoRepository.Update(VendorPaymentInfo);
            _eventPublisher.EntityUpdated(VendorPaymentInfo);
        }


        #endregion

        #region Payouts

        public IPagedList<VendorPayout> GetVendorPayouts(int VendorId, PayoutStatus? PayoutStatus, int PageNumber = 1, int PageSize = int.MaxValue)
        {
            var key = "VENDOR-PAYOUT-{0}-{1}";
            key = PayoutStatus.HasValue ? string.Format(key, VendorId, PayoutStatus.Value) : string.Format(key, VendorId, "all");

            return _cacheManager.Get(key, () =>
            {
                var query = _vendorPayoutRepository.Table.Where(x => x.VendorId == VendorId);
                if (PayoutStatus.HasValue)
                    query = query.Where(x => x.PayoutStatus == PayoutStatus.Value);

                var records = query.ToList();
                var paged = new PagedList<VendorPayout>(records, PageNumber - 1, PageSize);
                return paged;
            });
        }

        public VendorPayout GetVendorPayout(int VendorPayoutId)
        {
            return _vendorPayoutRepository.Table.FirstOrDefault(x => x.Id == VendorPayoutId);
        }

        public IList<VendorPayout> GetVendorPayoutsByOrder(int OrderId)
        {
            var query = _vendorPayoutRepository.Table.Where(x => x.OrderId == OrderId);
            return query.ToList();
        }

        public void SaveVendorPayout(VendorPayout VendorPayout)
        {
            if (VendorPayout.Id == 0)
                _vendorPayoutRepository.Insert(VendorPayout);
            else {
                _vendorPayoutRepository.Update(VendorPayout);
                _eventPublisher.EntityUpdated(VendorPayout);
            }
                
        }

        public void DeleteVendorPayout(VendorPayout VendorPayout)
        {
            _vendorPayoutRepository.Delete(VendorPayout);
        }




        #endregion

        #region Review
        public VendorReview GetVendorReview(int VendorReviewId)
        {
            return _vendorReviewRepository.Table.FirstOrDefault(x => x.Id == VendorReviewId);
        }

        public VendorReview GetVendorReview(int VendorId, int CustomerId, int OrderId, int ProductId)
        {
            var query = from tr in _vendorReviewRepository.Table
                        where tr.VendorId == VendorId && tr.CustomerId == CustomerId && tr.ProductId == ProductId && tr.OrderId == OrderId
                        select tr;
            return query.FirstOrDefault();
        }

        public IPagedList<VendorReview> GetVendorReviews(int? VendorId, int? CustomerId, bool? IsApproved, int PageNumber = 1, int PageSize = int.MaxValue)
        {
            var query = _vendorReviewRepository.Table;
            if (VendorId.HasValue)
                query = query.Where(x => x.VendorId == VendorId);

            if (CustomerId.HasValue)
                query = query.Where(x => x.CustomerId == CustomerId);

            if (IsApproved.HasValue)
                query = query.Where(x => x.IsApproved == IsApproved);
            
            //reverse sort
            var records = query.OrderByDescending(r => r.CreatedOnUTC).ToList();
            var paged = new PagedList<VendorReview>(records, PageNumber - 1, PageSize);
            return paged;
        }

        public void SaveVendorReview(VendorReview VendorReview)
        {
            if (VendorReview.Id == 0)
                _vendorReviewRepository.Insert(VendorReview);
            else
                _vendorReviewRepository.Update(VendorReview);
        }

        public void DeleteVendorReview(VendorReview VendorReview)
        {
            _vendorReviewRepository.Delete(VendorReview);
        }

        public bool IsVendorReviewDone(int VendorId, int CustomerId, int OrderId, int ProductId)
        {
            return
              _vendorReviewRepository.Table.Any(
                  x => x.OrderId == OrderId && x.VendorId == VendorId && x.ProductId == ProductId && x.CustomerId == CustomerId);
        }

        public bool CanCustomerReviewVendor(int VendorId, int CustomerId, Order Order)
        {
            var vendorIds = Order.OrderItems.ToList().Select(m => m.Product.VendorId);
            return vendorIds.Contains(VendorId) && CustomerId == Order.Customer.Id;
        }


        public Dictionary<Order, List<Product>> GetProductsWithPendingReviews(IList<Order> Orders, int CustomerId)
        {
            var dict = new Dictionary<Order, List<Product>>();
            var customerReviews = GetVendorReviews(null, CustomerId, null);

            foreach (var order in Orders)
            {
                var orderReviews = customerReviews.Where(cr => cr.OrderId == order.Id);
                if (!orderReviews.Any())
                    dict.Add(order, order.OrderItems.Select(oi => oi.Product).ToList());
                else
                {
                    var orderProductIds = order.OrderItems.Select(oi => oi.Product.Id).ToList();
                    var reviewedProductIds = orderReviews.Select(or => or.ProductId).ToList();
                    if (orderProductIds.Count == reviewedProductIds.Count)
                        continue; //all products reviewed for this order

                    var pendingForReviewIds = orderProductIds.Except(reviewedProductIds);
                    var pendingProducts = order.OrderItems.Where(oi => pendingForReviewIds.Contains(oi.ProductId))
                                            .Select(oi => oi.Product).ToList();

                    dict.Add(order, pendingProducts);
                }
            }
            return dict;
        }



        #endregion


        #endregion
    }
}