using System;

namespace Nop.Core.Domain.SMS
{
    /// <summary>
    /// Represents an number account
    /// </summary>
    public partial class NumberAccount : BaseEntity
    {
        /// <summary>
        /// Gets or sets an number address
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets an number display name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets an number Account Sid
        /// </summary>
        public string AccountSid { get; set; }
        /// <summary>
        /// Gets or sets an number AuthToken
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets a friendly number account name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.DisplayName))
                    return this.Number + " (" + this.DisplayName + ")";
                return this.Number;
            }
        }

       
    }
}
