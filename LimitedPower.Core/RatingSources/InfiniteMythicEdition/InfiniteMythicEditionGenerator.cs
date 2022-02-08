using System;
using System.Collections.Generic;
using LimitedPower.Core.Extensions;

namespace LimitedPower.Core.RatingSources.InfiniteMythicEdition
{
    public class InfiniteMythicEditionGenerator : RatingGeneratorBase<string>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.Lolaman, ReviewContributor.Ham, ReviewContributor.Scottynada };

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
                {ReviewContributor.Lolaman, -3},
                {ReviewContributor.Ham, -2},
                {ReviewContributor.Scottynada, -1}
            };
            foreach (var card in cards)
            {
                if (card.TypeLine.ToLower().Contains("basic")) continue;
                var name = card.Name;
                if (name.Contains(","))
                {
                    name = name.Substring(0, name.IndexOf(",", StringComparison.Ordinal));
                }
                if (name.Contains("/"))
                {
                    name = name.Substring(0, name.IndexOf("/", StringComparison.Ordinal) - 1);
                }

                var pos = csv.IndexOf(name);

                var ratingName = card.Name;
                if (ratingName.Contains("/"))
                {
                    ratingName = ratingName.Substring(0, ratingName.IndexOf("/", StringComparison.Ordinal) - 1);
                }


                foreach (var r in reviewerDict)
                {
                    result.Add(new RawRating<string>
                    {
                        ReviewContributor = r.Key,
                        RawValue = csv[pos + r.Value],
                        CardName = ratingName
                    });
                }
            }

            return result;
        }
    }
}
