using Nop.Core.Configuration;

namespace Nop.Core.Domain.SMS
{
    public class NumberAccountSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a store default number account identifier
        /// </summary>
        public int DefaultNumberAccountId { get; set; }
    }

}
