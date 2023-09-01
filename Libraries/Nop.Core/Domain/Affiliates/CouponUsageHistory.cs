using System;

namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// Represents a coupon usage history entry
    /// </summary>
    public partial class CouponUsageHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the coupon identifier
        /// </summary>
        public int CouponId { get; set; }

        /// <summary>
        /// Gets or sets the Affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the used value (amount)
        /// </summary>
        public decimal UsedValue { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets the coupon
        /// </summary>
        public virtual Coupon Coupon { get; set; }

        /// <summary>
        /// Gets the coupon
        /// </summary>
        public virtual Affiliate UsedWithAffiliate { get; set; }

        /// <summary>
        /// Gets the The 34 character id of the Account this message is associated with.
        /// </summary>
        public string AccountSid { get; set; }
        /// <summary>
        /// Gets The phone number or Channel address that sent this message.
        /// </summary>
        public string FromSender { get; set; }
        /// <summary>
        /// Gets The phone number or Channel address of the recipient.
        /// </summary>
        public string ToRecipient { get; set; }
        /// <summary>
        /// Gets The city of the sender
        /// </summary>
        public string FromCity { get; set; }
        /// <summary>
        /// Gets The state or province of the sender.
        /// </summary>
        public string FromState { get; set; }
        /// <summary>
        /// Gets The postal code of the called sender.
        /// </summary>
        public string FromZip { get; set; }
        /// <summary>
        /// Gets The country of the called sender
        /// </summary>
        public string FromCountry { get; set; }
        /// <summary>
        /// Gets The city of the recipient.
        /// </summary>
        public string ToCity { get; set; }
        /// <summary>
        /// Gets The state or province of the recipient.
        /// </summary>
        public string ToState { get; set; }
        /// <summary>
        /// Gets The postal code of the recipient.	
        /// </summary>
        public string ToZip { get; set; }
        /// <summary>
        /// Gets The country of the recipient.
        /// </summary>
        public string ToCountry { get; set; }
    }
}
