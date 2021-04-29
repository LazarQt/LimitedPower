using System.Collections.Generic;
using System.Linq;
using LimitedPower.Model;

namespace LimitedPower.Core.RatingSources.Drifter
{
    public class DrifterGenerator : GoogleDocGeneratorBase
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.Drifter };

        public DrifterGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions, string[] args) : base(basePath, set, cardNameSubstitutions, args) { }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+", "S" });

        protected override List<RawRating<string>> GetRawRatings()
        {

            var cardRatings = GetRows();
            cardRatings = cardRatings.Where(c => (string)c[1] != "★").ToList();

            // populate list
            return PopulateRatings(cardRatings, new Dictionary<ReviewContributor, int>()
            {
                {ReviewContributor.Drifter, 1},
            }, 0);
        }
    }
}
