using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Affiliates;

namespace Nop.Services.Affiliates
{
    /// <summary>
    /// Coupon service interface
    /// </summary>
    public partial interface ICouponService
    {
        /// <summary>
        /// Deletes a coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        void DeleteCoupon(Coupon coupon);

        /// <summary>
        /// Gets a coupon
        /// </summary>
        /// <param name="couponId">Coupon identifier</param>
        /// <returns>Coupon entry</returns>
        Coupon GetCouponById(int couponId);


        /// <summary>
        /// Gets coupon by identifier
        /// </summary>
        /// <param name="couponIds">coupon identifiers</param>
        /// <returns>Coupons</returns>
        IList<Coupon> GetCouponsByIds(int[] couponIds);


        /// <summary>
        /// Gets a coupon
        /// </summary>
        /// <param name="couponCouponCode">Coupon code</param>
        /// <returns>Coupon entry</returns>
        Coupon GetCouponByCouponCode(string couponCouponCode);

        /// <summary>
        /// Gets all coupons
        /// </summary>
        /// <param name="purchasedWithAffiliateId">Associated affiliate ID; null to load all records</param>
        /// <param name="affiliateId">The affiliate ID in which the coupon was used; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="isCouponActivated">Value indicating whether coupon is activated; null to load all records</param>
        /// <param name="couponCouponCode">Coupon coupon code; null to load all records</param>
        /// <param name="recipientName">Recipient name; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Coupons</returns>
        IPagedList<Coupon> GetAllCoupons(int? affiliateId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool? isCouponActivated = null, string couponCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        void InsertCoupon(Coupon coupon);

        /// <summary>
        /// Updates the coupon
        /// </summary>
        /// <param name="coupon">Coupon</param>
        void UpdateCoupon(Coupon coupon);
        

        /// <summary>
        /// Generate new coupon code
        /// </summary>
        /// <returns>Result</returns>
        string GenerateCouponCode();


        /// <summary>
        /// Delete coupon usage history
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        void DeleteCouponUsageHistory(Affiliate affiliate);

        /// <summary>
        /// Gets a coupon remaining amount
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Coupon remaining amount</returns>
        decimal GetCouponRemainingAmount(Coupon coupon);

        /// <summary>
        /// Gets a coupon usage history entries
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Result</returns>
        IList<CouponUsageHistory> GetCouponUsageHistory(Coupon coupon);

        /// <summary>
        /// Gets a coupon usage history entries
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Result</returns>
        IList<CouponUsageHistory> GetCouponUsageHistory(Affiliate affiliate);

        /// <summary>
        /// Inserts a coupon usage history entry
        /// </summary>
        /// <param name="couponUsageHistory">Coupon usage history entry</param>
        void InsertCouponUsageHistory(CouponUsageHistory couponUsageHistory);

        /// <summary>
        /// Is coupon valid
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Result</returns>
        bool IsCouponValid(Coupon coupon);
    }
}
