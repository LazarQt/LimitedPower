using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.RatingSources
{
    public class RatingTransformer
    {
        private List<string> Ratings { get; set; }
        public RatingTransformer(string[] ratingsFromWorstToBest)
        {
            Ratings = ratingsFromWorstToBest.ToList();
        }

        public double Calculate(string input) => 100.0 / (Ratings.Count - 1) * Ratings.IndexOf(input);
    }
}
