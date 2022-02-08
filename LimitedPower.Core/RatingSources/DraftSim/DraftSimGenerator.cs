using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LimitedPower.Core.RatingSources.DraftSim
{
    public class DraftSimGenerator : RatingGeneratorBase<double>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.DraftSim };

        private string[] UrlParts { get; }
        private double _minRating;
        private double _maxRating;

        public DraftSimGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions)
        {
            UrlParts = args[0].Split(',');
        }

        protected override List<RawRating<double>> GetRawRatings()
        {
            // load reviews from website
            List<DraftSimData> cardRatings = new List<DraftSimData>();
            foreach (var url in UrlParts)
            {
                cardRatings.AddRange(RetrieveDraftSimData(url));
            }

            // remove duplicates 
            cardRatings = cardRatings.GroupBy(x => x.Name).Select(x => x.First()).ToList();

            // SPECIAL CASE: remove some lands, might implement ignore list later
            var ignore = new List<string> {
                "Snow-Covered_Plains_2",
                "Snow-Covered_Island_2",
                "Snow-Covered_Swamp_2",
                "Snow-Covered_Mountain_2",
                "Snow-Covered_Forest_2",
                 };
            cardRatings = cardRatings.Where(c => !ignore.Contains(c.Name)).ToList();

            // populate list
            var result = new List<RawRating<double>>();
            foreach (var r in cardRatings)
            {
                result.Add(new RawRating<double>
                {
                    ReviewContributor = ReviewContributor.DraftSim,
                    RawValue = r.MyRating,
                    CardName = r.Name
                });
            }

            // initialize rating calculator private fields
            var orderedResults = cardRatings.OrderByDescending(x => x.MyRating).ToList();
            _minRating = orderedResults.Last().MyRating;
            _maxRating = orderedResults.First().MyRating;

            return result;
        }

        private List<DraftSimData> RetrieveDraftSimData(string url)
        {
            string doc;
            using (var client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                doc = client.DownloadString(Uri.EscapeUriString($"https://draftsim.com/generated/{url}.js"));
            }

            var start = doc.IndexOf('[');
            var end = doc.LastIndexOf(']');
            var strippedDoc = doc.Substring(start, end - start + 1);
            var replace = new[]
            {
                // ReSharper disable StringLiteralTypo
                "name",
                "castingcost1",
                "castingcost2",
                "type",
                "rarity",
                "myrating",
                "image",
                "cmc",
                "colors",
                "creaturesort",
                "colorsort"
                // ReSharper restore StringLiteralTypo
            };
            foreach (var r in replace)
            {
                strippedDoc = strippedDoc.Replace($"{r}:", $"\"{r}\":");
            }

            var cardRatings = JsonConvert.DeserializeObject<List<DraftSimData>>(JArray.Parse(strippedDoc).ToString());
            if (cardRatings == null) throw new Exception("ratings are null");
            return cardRatings;
        }

        protected override IRatingCalculator<double> CreateRatingCalculator() => new DoubleCalculator(_minRating, _maxRating);
    }
}
