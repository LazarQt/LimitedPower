using System;
using System.Collections.Generic;

namespace LimitedPower.Core.RatingSources.InfiniteMythicEdition
{
    public class InfiniteMythicEditionGenerator : GoogleDocGeneratorBase
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } =
        {
            ReviewContributor.InfiniteMythicEditionJustLolaman,
            ReviewContributor.InfiniteMythicEditionM0bieus,
            ReviewContributor.InfiniteMythicEditionScottynada
        };

        public InfiniteMythicEditionGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions, string[] args) : base(basePath, set, cardNameSubstitutions, args) { }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" });

        protected override List<RawRating<string>> GetRawRatings()
        {
            // order: JustLolaman(0), M0bieus(1), Scottynada(2), card name index (3)

            // populate list
            return PopulateRatings(GetRows(), new Dictionary<ReviewContributor, int>()
            {
                {ReviewContributor.InfiniteMythicEditionJustLolaman, 0},
                {ReviewContributor.InfiniteMythicEditionM0bieus, 1},
                {ReviewContributor.InfiniteMythicEditionScottynada, 2}
            }, 3);
        }

        protected override string ModifySearchTerm(Card card)
        {
            if (card.Name.Contains("//"))
            {
                return card.Name.Substring(0, card.Name.IndexOf("//", StringComparison.Ordinal) - 1);
            }

            return base.ModifySearchTerm(card);
        }
    }
}
