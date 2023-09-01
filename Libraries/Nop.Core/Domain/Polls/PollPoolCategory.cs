namespace Nop.Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll category mapping
    /// </summary>
    public partial class PollPollCategory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the poll identifier
        /// </summary>
        public int PollId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int PollCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the poll is featured
        /// </summary>
        public bool IsFeaturedPoll { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets the category
        /// </summary>
        public virtual PollCategory PollCategory { get; set; }

        /// <summary>
        /// Gets the poll
        /// </summary>
        public virtual Poll Poll { get; set; }

    }

}
