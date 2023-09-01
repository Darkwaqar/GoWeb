using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// Represents a coupon
    /// </summary>
    public partial class Coupon : BaseEntity
    {
        private ICollection<CouponUsageHistory> _CouponUsageHistory;

        /// <summary>
        /// Gets or sets the coupon type identifier
        /// </summary>
        public int CouponTypeId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether coupon is activated
        /// </summary>
        public bool IsCouponActivated { get; set; }

        /// <summary>
        /// Gets or sets a coupon coupon code
        /// </summary>
        public string CouponCouponCode { get; set; }

        /// <summary>
        /// Gets or sets a recipient name
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// Gets or sets a recipient email
        /// </summary>
        public string RecipientEmail { get; set; }

        /// <summary>
        /// Gets or sets a sender name
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets a sender email
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets a message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recipient is notified
        /// </summary>
        public bool IsRecipientNotified { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the coupon type
        /// </summary>
        public CouponType CouponType
        {
            get
            {
                return (CouponType)this.CouponTypeId;
            }
            set
            {
                this.CouponTypeId = (int)value;
            }
        }
        
        /// <summary>
        /// Gets or sets the coupon usage history
        /// </summary>
        public virtual ICollection<CouponUsageHistory> CouponUsageHistory
        {
            get { return _CouponUsageHistory ?? (_CouponUsageHistory = new List<CouponUsageHistory>()); }
            protected set { _CouponUsageHistory = value; }
        }

    }
}
