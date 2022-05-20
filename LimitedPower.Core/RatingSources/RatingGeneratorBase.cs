using LimitedPower.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

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

        protected virtual string SanitizeReviewCard(string cardName) => cardName
            .Trim()
            .Replace('_', ' ')
            .Replace("’", "'");

        // processing

        public List<string> GetCsv(string url, List<string> cardsWithCommas)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            var resp = (HttpWebResponse)req.GetResponse();

            var sr = new StreamReader(resp.GetResponseStream() ?? throw new InvalidOperationException());
            var results = sr.ReadToEnd();
            sr.Close();

            var dict = new Dictionary<string, string>();
            foreach (var c in cardsWithCommas)
            {
                dict.Add(c, c.Replace(",", string.Empty));
            }

            foreach (var d in dict)
            {
                results = results.Replace(d.Key, d.Value);
            }

            return results.Replace("\r\n", string.Empty).Replace("\"", "").Split(",").ToList();
        }


        public void Process()
        {
            // load cards
            var cards = GetCardsFile();
            if (cards == null || !cards.Any())
            {
                throw new Exception($"could not load file {SetFile}");
            }

            // remove old ratings
            cards.ForEach(c => c.Ratings?.RemoveAll(r => ReviewContributors.Contains(r.ReviewContributor)));

            // get raw ratings
            var rawRatings = GetRawRatings().OrderBy(o => o.CardName).ToList();

            // trim excess whitespace1
            rawRatings.ForEach(x => x.CardName = SanitizeReviewCard(x.CardName));

            // add new ratings
            var ratingCalculator = CreateRatingCalculator();
            foreach (var card in cards)
            {
                // setup search term with levenshtein algorithm 
                var distanceIndex = rawRatings.Select(c => c.CardName).DistanceIndex(card.Name);
                var searchTerm = rawRatings[distanceIndex].CardName;

                var cardRatings = rawRatings.Where(c => c.CardName.ToLower() == searchTerm.ToLower()).ToList();
                foreach (var cardRating in cardRatings)
                {
                    card.Ratings ??= new List<LimitedPowerRating>();
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

        protected List<Card> GetCardsFile() =>
            (JsonConvert.DeserializeObject<List<Card>>(File.ReadAllText(SetFile)) ?? new List<Card>())
            .Where(c => !c.IsBasic()).ToList();

    }
}
