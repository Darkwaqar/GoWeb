using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Catalog
{
    public partial class PollPollCategoryMap : NopEntityTypeConfiguration<PollPollCategory>
    {
        public PollPollCategoryMap()
        {
            this.ToTable("Poll_PollCategory_Mapping");
            this.HasKey(pc => pc.Id);
            
            this.HasRequired(pc => pc.PollCategory)
                .WithMany()
                .HasForeignKey(pc => pc.PollCategoryId);


            this.HasRequired(pc => pc.Poll)
                .WithMany(p => p.PollCategories)
                .HasForeignKey(pc => pc.PollId);
        }
    }
}