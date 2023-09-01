using System.Collections.Generic;
using Nop.Core.Domain.Media;

namespace Nop.Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll answer
    /// </summary>
    public partial class PollAnswer : BaseEntity
    {
        private ICollection<PollVotingRecord> _pollVotingRecords;

        /// <summary>
        /// Gets or sets the poll identifier
        /// </summary>
        public int PollId { get; set; }

        /// <summary>
        /// Gets or sets the poll answer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current number of votes
        /// </summary>
        public int NumberOfVotes { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public virtual Picture Picture { get; set; }

        /// <summary>
        /// Gets or sets the poll
        /// </summary>
        public virtual Poll Poll { get; set; }

        /// <summary>
        /// Gets or sets the poll voting records
        /// </summary>
        public virtual ICollection<PollVotingRecord> PollVotingRecords
        {
            get { return _pollVotingRecords ?? (_pollVotingRecords = new List<PollVotingRecord>()); }
            protected set { _pollVotingRecords = value; }
        }
    }
}