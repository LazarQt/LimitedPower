using System;
using System.Collections.Generic;

namespace LimitedPower.Core.RatingSources.Deathsie
{
    public class DeathsieGenerator : GoogleDocGeneratorBase
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.Deathsie };

        public DeathsieGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions, string[] args) : base(basePath, set, cardNameSubstitutions, args) { }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D", "C", "B", "A", "S" });

        protected override string ModifySearchTerm(Card card)
        {
            if (card.Name.Contains("//"))
            {
                return card.Name.Substring(0, card.Name.IndexOf("//", StringComparison.Ordinal) - 1);
            }

            return base.ModifySearchTerm(card);
        }

        protected override List<RawRating<string>> GetRawRatings()
        {
            // load reviews from website
            // index 0 = rating
            // index 2 = card name
            var cardRatings = GetRows();

            foreach (var c in cardRatings)
            {
                // trim middle section 
                if (c.Count == 3)
                {
                    c.RemoveAt(1);
                }
            }

            // populate list
            return PopulateRatings(cardRatings, new Dictionary<ReviewContributor, int>()
            {
                {ReviewContributor.Deathsie, 0},
            }, 1);
        }

    }
}
