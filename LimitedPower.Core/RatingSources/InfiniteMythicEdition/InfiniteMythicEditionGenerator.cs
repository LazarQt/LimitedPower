using LimitedPower.Core.Extensions;
using System.Collections.Generic;

namespace LimitedPower.Core.RatingSources.InfiniteMythicEdition
{
    public class InfiniteMythicEditionGenerator : RatingGeneratorBase<string>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } =
            {ReviewContributor.Lolaman, ReviewContributor.Scottynada};

        private readonly string _googleSheet;

        public InfiniteMythicEditionGenerator(string basePath, string set,
            Dictionary<string, string> cardNameSubstitutions, string[] args) : base(basePath, set,
            cardNameSubstitutions)
        {
            _googleSheet = args[0];
        }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" });

        protected override List<RawRating<string>> GetRawRatings()
        {
            var result = new List<RawRating<string>>();
            var cards = GetCardsFile();
            var csv = GetCsv($"https://www.google.com/url?q={_googleSheet.Replace("pubhtml#", "pub")}?output%3Dcsv",
                cards.CardsWithComma());
            csv.RemoveAll(c => c == string.Empty);

            var reviewerDict = new Dictionary<ReviewContributor, int>()
            {
                {ReviewContributor.Lolaman, -2},
                {ReviewContributor.Scottynada, -1}
            };

            foreach (var card in cards)
            {
                var cardName = card.Name;

                var pos = csv.DistanceIndex(cardName);

                foreach (var r in reviewerDict)
                {
                    result.Add(new RawRating<string>
                    {
                        ReviewContributor = r.Key,
                        RawValue = csv[pos + r.Value],
                        CardName = cardName
                    });
                }
            }

            return result;
        }
    }
}
