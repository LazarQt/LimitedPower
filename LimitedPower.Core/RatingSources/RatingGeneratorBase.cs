using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.Core.Extensions;
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

        protected virtual string ModifySearchTerm(Card card) => card.Layout == "modal_dfc" ? card.Name.StripBacksideName() : card.Name;

        protected virtual string SanitizeReviewCard(string cardName) => cardName
            .Trim()
            .Replace('_', ' ')
            .Replace("’", "'");

        public void Process()
        {
            // load cards
            var cards = GetCardsFile();
            if (cards == null)
            {
                throw new Exception($"could not load file {SetFile}");
            }

            // remove old ratings
            cards.ForEach(c => c.Ratings?.RemoveAll(r => ReviewContributors.Contains(r.ReviewContributor)));

            // get raw ratings
            var rawRatings = GetRawRatings();
            // trim excess whitespace
            rawRatings.ForEach(x => x.CardName = SanitizeReviewCard(x.CardName));

            // add new ratings
            var ratingCalculator = CreateRatingCalculator();
            foreach (var card in cards)
            {
                // setup search term + remove white spaces
                var searchTerm = ModifySearchTerm(card);
                // substitute if any
                if (CardNameSubstitutions != null && CardNameSubstitutions.ContainsKey(searchTerm))
                {
                    searchTerm = CardNameSubstitutions[searchTerm];
                }

                var cardRatings = rawRatings.Where(c => c.CardName.ToLower() == searchTerm.ToLower()
                                                        || c.CardName.ToLower() == searchTerm.ToLower().Replace("// ", "")).ToList();
                foreach (var cardRating in cardRatings)
                {
                    card.Ratings.Add(new LimitedPowerRating(ratingCalculator.Calculate(cardRating.RawValue), string.Empty, cardRating.ReviewContributor));
                }

                if (cardRatings.Count != ReviewContributors.Length)
                {
                    File.AppendAllText(Path.Combine(BasePath, $"{Set}-missing.txt"),
                        $"Missing ratings from {ReviewContributors.Aggregate(string.Empty, (current, c) => current + c)} for card {searchTerm}"
                        + Environment.NewLine);
                }
            }

            // write ratings back to original file
            File.WriteAllText(SetFile, JsonConvert.SerializeObject(cards));
        }

        protected List<Card> GetCardsFile() => JsonConvert.DeserializeObject<List<Card>>(File.ReadAllText(SetFile));

    }
}
