using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Coupon service
    /// </summary>
    public partial class CouponService : ICouponService
    {
        #region Fields
        
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CouponUsageHistory> _couponUsageHistoryRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="couponRepository">Coupon context</param>
        /// <param name="eventPublisher">Event published</param>
        public CouponService(IRepository<Coupon> couponRepository,
            IEventPublisher eventPublisher,
            IRepository<CouponUsageHistory> couponUsageHistoryRepository)
        {
            this._couponRepository = couponRepository;
            this._eventPublisher = eventPublisher;
            this._couponUsageHistoryRepository = couponUsageHistoryRepository;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes a coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        public virtual void DeleteCoupon(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException("coupon");

            _couponRepository.Delete(coupon);

            //event notification
            _eventPublisher.EntityDeleted(coupon);
        }

        /// <summary>
        /// Gets a coupon
        /// </summary>
        /// <param name="couponId">Coupon identifier</param>
        /// <returns>Coupon entry</returns>
        public virtual Coupon GetCouponById(int couponId)
        {
            if (couponId == 0)
                return null;

            return _couponRepository.GetById(couponId);
        }


        /// <summary>
        /// Get coupons by identifiers
        /// </summary>
        /// <param name="couponIds">Coupon identifiers</param>
        /// <returns>Coupons</returns>
        public virtual IList<Coupon> GetCouponsByIds(int[] couponIds)
        {
            if (couponIds == null || couponIds.Length == 0)
                return new List<Coupon>();

            var query = from p in _couponRepository.Table
                        where couponIds.Contains(p.Id)
                        select p;
            var coupons = query.ToList();
            //sort by passed identifiers
            var sortedCoupons = new List<Coupon>();
            foreach (int id in couponIds)
            {
                var coupon = coupons.Find(x => x.Id == id);
                if (coupon != null)
                    sortedCoupons.Add(coupon);
            }
            return sortedCoupons;
        }


        /// <summary>
        /// Gets a coupon
        /// </summary>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>Coupon entry</returns>
        public virtual Coupon GetCouponByCouponCode(string couponCouponCode) {

            if (couponCouponCode == null)
                throw new ArgumentNullException("couponCouponCode");

            var query = _couponRepository.Table;
            query = query.Where(gc => gc.CouponCouponCode == couponCouponCode);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets all coupons
        /// </summary>
        /// <param name="purchasedWithAffiliateId">Associated affiliate ID; null to load all records</param>
        /// <param name="usedWithAffiliateId">The affiliate ID in which the coupon was used; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="isCouponActivated">Value indicating whether coupon is activated; null to load all records</param>
        /// <param name="couponCouponCode">Coupon coupon code; nullto load all records</param>
        /// <param name="recipientName">Recipient name; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Coupons</returns>
        public virtual IPagedList<Coupon> GetAllCoupons( int? affiliateId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, 
            bool? isCouponActivated = null, string couponCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _couponRepository.Table;
            if (affiliateId.HasValue)
                query = query.Where(gc => gc.CouponUsageHistory.Any(history => history.AffiliateId == affiliateId));
            if (createdFromUtc.HasValue)
                query = query.Where(gc => createdFromUtc.Value <= gc.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(gc => createdToUtc.Value >= gc.CreatedOnUtc);
            if (isCouponActivated.HasValue)
                query = query.Where(gc => gc.IsCouponActivated == isCouponActivated.Value);
            if (!String.IsNullOrEmpty(couponCouponCode))
                query = query.Where(gc => gc.CouponCouponCode == couponCouponCode);
            if (!String.IsNullOrWhiteSpace(recipientName))
                query = query.Where(c => c.RecipientName.Contains(recipientName));
            query = query.OrderByDescending(gc => gc.CreatedOnUtc);

            var coupons = new PagedList<Coupon>(query, pageIndex, pageSize);
            return coupons;
        }

        /// <summary>
        /// Inserts a coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        public virtual void InsertCoupon(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException("coupon");

            _couponRepository.Insert(coupon);

            //event notification
            _eventPublisher.EntityInserted(coupon);
        }

        /// <summary>
        /// Updates the coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        public virtual void UpdateCoupon(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException("coupon");

            _couponRepository.Update(coupon);

            //event notification
            _eventPublisher.EntityUpdated(coupon);
        }

        /// <summary>
        /// Generate new coupon code
        /// </summary>
        /// <returns>Result</returns>
        public virtual string GenerateCouponCode()
        {
            int length = 13;
            string result = Guid.NewGuid().ToString();
            if (result.Length > length)
                result = result.Substring(0, length);
            return result;
        }

        /// <summary>
        /// Delete coupon usage history
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public virtual void DeleteCouponUsageHistory(Affiliate affiliate)
        {
            var couponUsageHistory = GetCouponUsageHistory(affiliate);

            _couponUsageHistoryRepository.Delete(couponUsageHistory);

            var query = _couponRepository.Table;

            var couponIds = couponUsageHistory.Select(gcuh => gcuh.CouponId).ToArray();
            var coupons = query.Where(bp => couponIds.Contains(bp.Id)).ToList();

            //event notification
            foreach (var coupon in coupons)
            {
                _eventPublisher.EntityUpdated(coupon);
            }
        }

        /// <summary>
        /// Gets a coupon remaining amount
        /// </summary>
        /// <returns>Coupon remaining amount</returns>
        public virtual decimal GetCouponRemainingAmount(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException(nameof(coupon));

            var result = coupon.Amount;

            foreach (var gcuh in GetCouponUsageHistory(coupon))
                result -= gcuh.UsedValue;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Gets a coupon usage history entries
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Result</returns>
        public virtual IList<CouponUsageHistory> GetCouponUsageHistory(Coupon coupon)
        {
            if (coupon is null)
                throw new ArgumentNullException(nameof(coupon));

            return _couponUsageHistoryRepository.Table.Where(gcuh => gcuh.CouponId == coupon.Id).ToList();
        }

        /// <summary>
        /// Gets a coupon usage history entries
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Result</returns>
        public virtual IList<CouponUsageHistory> GetCouponUsageHistory(Affiliate affiliate)
        {
            if (affiliate is null)
                throw new ArgumentNullException(nameof(affiliate));

            return _couponUsageHistoryRepository.Table.Where(gcuh => gcuh.AffiliateId == affiliate.Id).ToList();
        }

        /// <summary>
        /// Inserts a coupon usage history entry
        /// </summary>
        /// <param name="couponUsageHistory">Coupon usage history entry</param>
        public virtual void InsertCouponUsageHistory(CouponUsageHistory couponUsageHistory)
        {
            if (couponUsageHistory is null)
                throw new ArgumentNullException(nameof(couponUsageHistory));

            _couponUsageHistoryRepository.Insert(couponUsageHistory);

            //event notification
            _eventPublisher.EntityInserted(couponUsageHistory);
        }

        /// <summary>
        /// Is coupon valid
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Result</returns>
        public virtual bool IsCouponValid(Coupon coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException(nameof(coupon));

            if (!coupon.IsCouponActivated)
                return false;

            var remainingAmount = GetCouponRemainingAmount(coupon);
            if (remainingAmount > decimal.Zero)
                return true;

            return false;
        }

        #endregion
    }
}
