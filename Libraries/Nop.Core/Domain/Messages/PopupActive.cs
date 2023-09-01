using System;

namespace Nop.Core.Domain.Messages
{
    public partial class PopupActive : BaseEntity
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public int CustomerId { get; set; }
        public int CustomerActionId { get; set; }
        public int PopupTypeId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

}
