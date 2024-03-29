﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.RatingSources.DraftaholicsAnonymous
{
    public class DraftaholicsAnonymousGenerator : RatingGeneratorBase<int>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.DraftaholicsAnonymous };

        private int _minRating;
        private int _maxRating;

        public DraftaholicsAnonymousGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions) : base(basePath, set, cardNameSubstitutions) { }

        protected override List<RawRating<int>> GetRawRatings()
        {
            // load reviews from website
            string doc;
            using (var client = new System.Net.WebClient()) // WebClient class inherits IDisposable
            {
                doc = client.DownloadString(Uri.EscapeUriString($"https://apps.draftaholicsanonymous.com/p1p1/{Set.ToUpper()}/results?ajax"));
            }
            var cardRatings = JsonConvert.DeserializeObject<DraftaholicsAnonymousRoot>(doc)?.Data;
            if (cardRatings == null) throw new Exception("ratings are null");

            // populate list
            var result = new List<RawRating<int>>();
            foreach (var r in cardRatings)
            {
                result.Add(new RawRating<int>
                {
                    ReviewContributor = ReviewContributor.DraftaholicsAnonymous,
                    RawValue = r.Elo,
                    CardName = r.Name
                });
            }

            // initialize rating calculator private fields
            var orderedResults = cardRatings.OrderByDescending(x => x.Elo).ToList();
            _minRating = orderedResults.Last().Elo;
            _maxRating = orderedResults.First().Elo;

            return result;
        }

        protected override IRatingCalculator<int> CreateRatingCalculator() => new IntegerCalculator(_minRating, _maxRating);
    }
}
