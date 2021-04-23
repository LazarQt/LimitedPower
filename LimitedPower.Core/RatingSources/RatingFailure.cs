using LimitedPower.ViewModel;

namespace LimitedPower.Core.RatingSources
{
    public class RatingFailure
    {
        public ReviewSource ReviewSource { get; set; }
        public int ArenaId { get; set; }
        public RatingFailure(ReviewSource source, int arenaId)
        {
            ReviewSource = source;
            ArenaId = arenaId;
        }
    }
}
