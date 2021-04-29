using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.RatingSources
{
    public interface IRatingCalculator<in T>
    {
        public double Calculate(T input);
    }

    public abstract class RatingCalculatorBase<T> : IRatingCalculator<T>
    {
        public double MinRating { get; set; }
        public double NewMaxRating { get; set; }

        protected RatingCalculatorBase(double min, double max)
        {
            MinRating = min;
            NewMaxRating = max - MinRating;
        }

        public abstract double Calculate(T input);
    }

    public class IntegerCalculator : RatingCalculatorBase<int>
    {
        public IntegerCalculator(double min, double max) : base(min, max) { }

        public override double Calculate(int input) => 100 / NewMaxRating * (input - MinRating);
    }

    public class DoubleCalculator : RatingCalculatorBase<double>
    {
        public DoubleCalculator(double min, double max) : base(min, max) { }

        public override double Calculate(double input) => 100 / NewMaxRating * (input - MinRating);
    }

    public class RatingTransformer : IRatingCalculator<string>
    {
        private List<string> Ratings { get; }
            
        public RatingTransformer(string[] ratingsFromWorstToBest)
        {
            Ratings = ratingsFromWorstToBest.ToList();
        }

        public double Calculate(string input) => 100.0 / (Ratings.Count - 1) * Ratings.IndexOf(input);
    }
}
