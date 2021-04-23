using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LimitedPower.Core.RatingSources.DraftSim
{
    public class DraftSimGenerator : RatingGeneratorBase
    {
        private string[] Urls { get; set; }

        public DraftSimGenerator(string basePath, string set, string[] urls) : base(basePath, set)
        {
            Urls = urls;
        }

        public override void RateCards()
        {
            LoadFile();

            // review source setup
            var reviewSource = ReviewSource.DraftSim;

            // load reviews from website
            List<DraftSimData> cardRatings = new List<DraftSimData>();
            foreach (var url in Urls)
            {
                cardRatings.AddRange(RetrieveDraftSimData(url));
            }

            // setup rating calculation
            var calc = new RatingCalculator(cardRatings.OrderByDescending(r => r.MyRating).Select(e => e.MyRating).ToList());

            // remove old ratings
            Cards.ForEach(c => c.Ratings.RemoveAll(r => r.ReviewSource == reviewSource));

            // add new ratings
            foreach (var card in Cards)
            {
                // setup search term
                var searchTerm = card.Name.Replace(' ', '_');
                // remove backside of card name
                if (searchTerm.Contains("//")) searchTerm = searchTerm.Substring(0, searchTerm.IndexOf("//", StringComparison.Ordinal) - 1);

                var cardRating = cardRatings.FirstOrDefault(c => c.Name == searchTerm);
                if (cardRating == null)
                {
                    Fails.Add(new RatingFailure(reviewSource, card.ArenaId));
                }
                else
                {
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate(cardRating.MyRating), string.Empty, reviewSource));
                }
            }

            if (Fails.Any())
            {
                File.WriteAllText(JsonConvert.SerializeObject(Fails), Path.Combine(BasePath, $"{reviewSource}-fails.json"));
            }

            WriteFile();
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
            };
            foreach (var r in replace)
            {
                strippedDoc = strippedDoc.Replace($"{r}:", $"\"{r}\":");
            }

            var json = JArray.Parse(strippedDoc).ToString();


            var cardRatings = JsonConvert.DeserializeObject<List<DraftSimData>>(json);
            if (cardRatings == null) throw new Exception("ratings are null");
            return cardRatings;
        }
    }
}
