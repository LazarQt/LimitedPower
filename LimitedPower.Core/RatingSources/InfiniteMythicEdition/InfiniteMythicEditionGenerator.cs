using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.ViewModel;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.InfiniteMythicEdition
{
    public class InfiniteMythicEditionGenerator : RatingGeneratorBase
    {
        public string SpreadsheetId { get; set; }

        public InfiniteMythicEditionGenerator(string basePath, string set, string spreadsheetId) : base(basePath, set)
        {
            SpreadsheetId = spreadsheetId;
        }

        public override void RateCards()
        {
            LoadFile();

            // review source setup
            var reviewSources = new[] { ReviewSource.InfiniteMythicEditionJustLolaman, ReviewSource.InfiniteMythicEditionM0bieus, ReviewSource.InfiniteMythicEditionScottynada };

            // load reviews from website
            var gdocsHelper = new GoogleDocsHelper();
            // ordelolaman, mobieus, scotty, cardname
            var cardRatings = gdocsHelper.GetRows(SpreadsheetId, new[] { "Sheet1!E2:H276", "Sheet1!M2:P64" });

            // setup rating calculation
            //var calc = new RatingCalculator(cardRatings.OrderByDescending(r => r.MyRating).Select(e => e.MyRating).ToList());
            var calc = new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" });

            // remove old ratings
            Cards.ForEach(c => c.Ratings.RemoveAll(r => reviewSources.Contains(r.ReviewSource)));

            var subs = new Dictionary<string, string>()
            {
                // correct, wrong
                {"Mage Hunters' Onslaught", "Mage Hunter's Onslaught"},
                {"Plumb the Forbidden", "Plumb the Forgotten"},
                //{"Academic Dispute", "Academic Debate"},
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

                var cardRating = cardRatings.FirstOrDefault(c => (string)c[3] == searchTerm);
                if (cardRating == null)
                {
                    Fails.Add(new RatingFailure(ReviewSource.InfiniteMythicEditionJustLolaman, card.ArenaId));
                }
                else
                {
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate((string)cardRating[0]), string.Empty, ReviewSource.InfiniteMythicEditionJustLolaman));
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate((string)cardRating[1]), string.Empty, ReviewSource.InfiniteMythicEditionM0bieus));
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate((string)cardRating[2]), string.Empty, ReviewSource.InfiniteMythicEditionScottynada));
                }
            }

            if (Fails.Any())
            {
                File.WriteAllText(JsonConvert.SerializeObject(Fails), Path.Combine(BasePath, $"{ReviewSource.InfiniteMythicEditionJustLolaman}-fails.json"));
            }

            WriteFile();
        }
    }
}
