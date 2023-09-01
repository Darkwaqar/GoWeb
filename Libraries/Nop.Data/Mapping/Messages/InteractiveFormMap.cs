using Nop.Core.Domain.Interactive;

namespace Nop.Data.Mapping.Messages
{
    public partial class InteractiveFormMap : NopEntityTypeConfiguration<InteractiveForm>
    {
        public InteractiveFormMap()
        {
            this.ToTable("InteractiveForm");
            this.HasKey(ea => ea.Id);
        }
    }
}
