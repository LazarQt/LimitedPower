using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.Core.Extensions;
using LimitedPower.Model;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources
{

    public abstract class RatingGeneratorBase<T>
    {
        // own
        protected string BasePath { get; set; }
        protected string Set { get; set; }
        protected string SetFile => Path.Combine(BasePath, $"{Set}.json");

        // override

        protected abstract ReviewContributor[] ReviewContributors { get; set; }

        protected Dictionary<string, string> CardNameSubstitutions { get; set; }

        protected abstract IRatingCalculator<T> CreateRatingCalculator();

        protected RatingGeneratorBase(string basePath, string set, Dictionary<string, string> cardNameSubstitutions)
        {
            BasePath = basePath;
            Set = set;
            CardNameSubstitutions = cardNameSubstitutions;
        }

        protected abstract List<RawRating<T>> GetRawRatings();

        protected virtual string GetSearchTerm(string term) => term.StripBacksideName();

        public void Process()
        {
            // load cards
            var cards = JsonConvert.DeserializeObject<List<Card>>(File.ReadAllText(SetFile));
            if (cards == null)
            {
                throw new Exception($"could not load file {SetFile}");
            }

            // remove old ratings
            cards.ForEach(c => c.Ratings.RemoveAll(r => ReviewContributors.Contains(r.ReviewContributor)));

            // get raw ratings
            var rawRatings = GetRawRatings();

            // add new ratings
            var ratingCalculator = CreateRatingCalculator();
            foreach (var card in cards)
            {
                // setup search term
                var searchTerm = GetSearchTerm(card.Name);
                // substitute if any
                if (CardNameSubstitutions.ContainsKey(searchTerm)) searchTerm = CardNameSubstitutions[searchTerm];

                var cardRatings = rawRatings.Where(c => c.CardName == searchTerm).ToList();
                foreach (var cardRating in cardRatings)
                {
                    card.Ratings.Add(new LimitedPowerRating(ratingCalculator.Calculate(cardRating.RawValue), string.Empty, cardRating.ReviewContributor));
                }

                if (cardRatings.Count != ReviewContributors.Length)
                {
                    File.AppendAllText(Path.Combine(BasePath, $"{Set}-missing.json"), $"Missing ratings from {ReviewContributors.Select(r => r)} for card {searchTerm}");
                }
            }

            // write ratings back to original file
            File.WriteAllText(SetFile, JsonConvert.SerializeObject(cards));
        }
    }
}
