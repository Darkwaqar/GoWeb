
namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CouponExtensions
    {
        /// <summary>
        /// Gets a coupon remaining amount
        /// </summary>
        /// <returns>Coupon remaining amount</returns>
        public static decimal GetCouponRemainingAmount(this Coupon coupon)
        {
            decimal result = coupon.Amount;

            foreach (var gcuh in coupon.CouponUsageHistory)
                result -= gcuh.UsedValue;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Is coupon valid
        /// </summary>
        /// <param name="coupon">Coupon</param>
        /// <returns>Result</returns>
        public static bool IsCouponValid(this Coupon coupon)
        {
            if (!coupon.IsCouponActivated)
                return false;

            decimal remainingAmount = coupon.GetCouponRemainingAmount();
            if (remainingAmount > decimal.Zero)
                return true;

            return false;
        }
    }
}
