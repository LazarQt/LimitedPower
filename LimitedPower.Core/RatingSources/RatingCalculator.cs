using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.RatingSources
{
    public class RatingCalculator
    {
        public double MinRating { get; set; }
        public double NewMaxRating { get; set; }

        public RatingCalculator(List<double> ratingsDesc)
        {
            MinRating = ratingsDesc.Last();
            NewMaxRating = ratingsDesc.First() - MinRating;
        }

        public double Calculate(double input)
        {
            return 100 / NewMaxRating * (input - MinRating);
        }
    }
}
