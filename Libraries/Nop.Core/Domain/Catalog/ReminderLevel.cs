namespace Nop.Core.Domain.Catalog
{
    public partial class ReminderLevel : BaseEntity
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public int EmailAccountId { get; set; }
        public string BccEmailAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
