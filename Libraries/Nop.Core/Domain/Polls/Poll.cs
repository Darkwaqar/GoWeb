using System;
using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Polls
{
    /// <summary>
    /// Represents a poll
    /// </summary>
    public partial class Poll : BaseEntity
    {
        private ICollection<PollAnswer> _pollAnswers;
        private ICollection<PollPollCategory> _pollCategories;

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity should be shown on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the anonymous votes are allowed
        /// </summary>
        public bool AllowGuestsToVote { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the poll start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the poll end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the news comments
        /// </summary>
        public virtual ICollection<PollAnswer> PollAnswers
        {
            get { return _pollAnswers ?? (_pollAnswers = new List<PollAnswer>()); }
            protected set { _pollAnswers = value; }
        }
        
        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }



        /// <summary>
        /// Gets or sets the collection of PollPollCategory
        /// </summary>
        public virtual ICollection<PollPollCategory> PollCategories
        {
            get { return _pollCategories ?? (_pollCategories = new List<PollPollCategory>()); }
            protected set { _pollCategories = value; }
        }
    }
}