using System;
using System.Collections.Generic;

namespace LimitedPower.Core.RatingSources.Deathsie
{
    public class DeathsieGenerator : RatingGeneratorBase<string>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.Deathsie };
        private string GoogleSheet;

        public DeathsieGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions)
        {
            GoogleSheet = args[0];
        }

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
            var result = new List<RawRating<string>>();

            var csv = GetCsv($"https://www.google.com/url?q={GoogleSheet.Replace("pubhtml#", "pub")}?output%3Dcsv");
            csv.RemoveAll(c => c == string.Empty);
            var cards = GetCardsFile();

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

                if (name == "Undying Malice") name = "Undying Malace";

                var ratingName = card.Name;
                if (ratingName.Contains("/"))
                {
                    ratingName = ratingName.Substring(0, ratingName.IndexOf("/", StringComparison.Ordinal) - 1);
                }

                var pos = csv.IndexOf(name);

          
                    result.Add(new RawRating<string>
                    {
                        ReviewContributor = ReviewContributor.Deathsie,
                        RawValue = csv[pos - 2],
                        CardName = ratingName
                    });
                
            }

            return result;
        }

    }
}
