using LimitedPower.Model;

namespace LimitedPower.Core.RatingSources
{
    public class RawRating<T>
    {
        public ReviewContributor ReviewContributor { get; set; }
        public T RawValue { get; set; }
        public string CardName { get; set; }
    }
}
