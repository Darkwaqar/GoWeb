using System.Collections.Generic;
using Nop.Core.Domain.Polls;

namespace Nop.Services.Polls
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class PollCategoryExtensions
    {

        /// <summary>
        /// Returns a PollPollCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="PollId">Poll identifier</param>
        /// <param name="PollCategoryId">PollCategory identifier</param>
        /// <returns>A PollPollCategory that has the specified values; otherwise null</returns>
        public static PollPollCategory FindPollPollCategory(this IList<PollPollCategory> source,
            int PollId, int PollCategoryId)
        {
            foreach (var PollPollCategory in source)
                if (PollPollCategory.PollId == PollId && PollPollCategory.PollCategoryId == PollCategoryId)
                    return PollPollCategory;

            return null;
        }
    }
}
