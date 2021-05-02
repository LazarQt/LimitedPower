using System;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.Model;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.SeventeenLands
{
    public class SeventeenLandsGenerator : RatingGeneratorBase<double>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.SeventeenLands };

        private double _minRating;
        private double _maxRating;

        public SeventeenLandsGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions) : base(basePath, set, cardNameSubstitutions) { }

        protected override List<RawRating<double>> GetRawRatings()
        {
            // load reviews from website
            string doc;
            using (var client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {

                doc = client.DownloadString(Uri.EscapeUriString(
                    $"https://www.17lands.com/card_ratings/data?expansion={Set.ToUpper()}&format=PremierDraft&start_date=2021-04-24&end_date=2021-05-02"));
            }
            var cardRatings = JsonConvert.DeserializeObject<List<SlCard>>(doc);
            if (cardRatings == null) throw new Exception("ratings are null");

            // populate list
            var result = new List<RawRating<double>>();
            foreach (var r in cardRatings)
            {
                result.Add(new RawRating<double>
                {
                    ReviewContributor = ReviewContributor.SeventeenLands,
                    RawValue = r.AvgSeen,
                    CardName = r.Name
                });
            }

            // initialize rating calculator private fields
            var orderedResults = cardRatings.OrderByDescending(x => x.AvgSeen).ToList();
            _minRating = orderedResults.Last().AvgSeen;
            _maxRating = orderedResults.First().AvgSeen;

            return result;
        }

        protected override IRatingCalculator<double> CreateRatingCalculator() => new DoubleCalculator(_maxRating, _minRating);
    }
}
