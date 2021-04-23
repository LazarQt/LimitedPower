using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.ViewModel;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.DraftaholicsAnonymous
{
    public class DraftaholicsAnonymousGenerator : RatingGeneratorBase
    {
        public DraftaholicsAnonymousGenerator(string basePath, string set) : base(basePath, set)
        {
        }

        public override void RateCards()
        {
            LoadFile();

            // review source setup
            var reviewSource = ReviewSource.DraftaholicsAnonymous;

            // load reviews from website
            string doc;
            using (var client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                doc = client.DownloadString(Uri.EscapeUriString($"https://apps.draftaholicsanonymous.com/p1p1/{Set.ToUpper()}/results?ajax"));
            }
            var cardRatings = JsonConvert.DeserializeObject<DraftaholicsAnonymousRoot>(doc)?.Data;
            if (cardRatings == null) throw new Exception("ratings are null");

            // setup rating calculation
            var calc = new RatingCalculator(cardRatings.OrderByDescending(r => r.Elo).Select(e => (double)e.Elo).ToList());

            // remove old ratings
            Cards.ForEach(c => c.Ratings.RemoveAll(r => r.ReviewSource == reviewSource));

            var subs = new Dictionary<string, string>()
            {
                // correct, wrong
                {"Mage Hunters' Onslaught", "Mage Hunter's Onslaught"},
                {"Plumb the Forbidden", "Plumb the Forgotten"},
                {"Academic Dispute", "Academic Debate"},
            };

            // add new ratings
            foreach (var card in Cards)
            {
                // setup search term
                var searchTerm = card.Name;
                // remove backside of card name
                if (searchTerm.Contains("//")) searchTerm = searchTerm.Substring(0, searchTerm.IndexOf("//", StringComparison.Ordinal) - 1);
                // substitute if any
                if (subs.ContainsKey(searchTerm)) searchTerm = subs[searchTerm];

                var cardRating = cardRatings.FirstOrDefault(c => c.Name == searchTerm);
                if (cardRating == null)
                {
                    Fails.Add(new RatingFailure(reviewSource, card.ArenaId));
                }
                else
                {
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate(cardRating.Elo), string.Empty, reviewSource));
                }
            }

            if (Fails.Any())
            {
                File.WriteAllText(JsonConvert.SerializeObject(Fails), Path.Combine(BasePath, $"{reviewSource}-fails.json"));
            }

            WriteFile();
        }
    }
}
