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
            var cardRatings = GetLatestRatings(DateTime.Now, 7, GetCardsFile().Count);
            if (cardRatings == null) throw new Exception("ratings are null");

            // in some cases, cards are registered twice (e.g. same land with different art), skip those
            cardRatings = cardRatings.GroupBy(x => x.Name).Select(x => x.First()).ToList();

            // populate list
            var result = new List<RawRating<double>>();
            foreach (var r in cardRatings)
            {
                result.Add(new RawRating<double>
                {
                    ReviewContributor = ReviewContributor.SeventeenLands,
                    RawValue = r.AvgSeen,
                    CardName = r.Name.Replace("///","//")
                });
            }

            // initialize rating calculator private fields
            var orderedResults = cardRatings.OrderByDescending(x => x.AvgSeen).ToList();
            _minRating = orderedResults.Last().AvgSeen;
            _maxRating = orderedResults.First().AvgSeen;

            return result;
        }

        protected override IRatingCalculator<double> CreateRatingCalculator() => new DoubleCalculator(_maxRating, _minRating);

        private List<SlCard> GetLatestRatings(DateTime today, int daysBack, int expectedCardCount)
        {
            // load reviews from website
            using var client = new System.Net.WebClient();
            var lastWeek = today.AddDays(-daysBack);
            var url = $"https://www.17lands.com/card_ratings/data?" +
                      $"expansion={Set.ToUpper()}&format=PremierDraft&" +
                      $"start_date={lastWeek.Year}-{lastWeek.Month:00}-{lastWeek.Day:00}&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = client.DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonConvert.DeserializeObject<List<SlCard>>(doc);
            if (cardRatings != null && (!cardRatings.Any() || cardRatings.Count < expectedCardCount)) 
            {
                return GetLatestRatings(today, daysBack + 3, expectedCardCount);
            }
            return cardRatings;
        }
    }
}
