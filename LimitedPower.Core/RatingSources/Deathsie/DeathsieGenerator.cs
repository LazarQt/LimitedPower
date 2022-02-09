using System.Collections.Generic;
using LimitedPower.Core.Extensions;

namespace LimitedPower.Core.RatingSources.Deathsie
{
    public class DeathsieGenerator : RatingGeneratorBase<string>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = {ReviewContributor.Deathsie};
        private readonly string _googleSheet;

        public DeathsieGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions)
        {
            _googleSheet = args[0];
        }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D", "C", "B", "A", "S" });

        protected override List<RawRating<string>> GetRawRatings()
        {
            var result = new List<RawRating<string>>();
            var cards = GetCardsFile();
            var csv = GetCsv($"https://www.google.com/url?q={_googleSheet.Replace("pubhtml#", "pub")}?output%3Dcsv",
                cards.CardsWithComma());
            csv.RemoveAll(c => c == string.Empty);

            foreach (var card in cards)
            {
                var cardName = card.Name;

                var pos = csv.DistanceIndex(cardName);

                var rawValuePos = pos - 2;
                if (!int.TryParse(csv[pos - 1], out _))
                {
                    rawValuePos += 1;
                }

                result.Add(new RawRating<string>
                {
                    ReviewContributor = ReviewContributor.Deathsie,
                    RawValue = csv[rawValuePos],
                    CardName = cardName
                });
            }

            return result;
        }

    }
}
