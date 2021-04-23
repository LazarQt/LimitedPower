using System;
using System.IO;
using System.Linq;
using LimitedPower.ViewModel;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.Drifter
{
    public class DrifterGenerator : RatingGeneratorBase
    {
        public string SpreadsheetId { get; set; }

        public DrifterGenerator(string basePath, string set, string spreadsheetId) : base(basePath, set)
        {
            SpreadsheetId = spreadsheetId;
        }

        public override void RateCards()
        {
            LoadFile();

            // review source setup
            var reviewSources = new[] { ReviewSource.Drifter };

            // load reviews from website
            var gdocsHelper = new GoogleDocsHelper();
            var cardRatings = gdocsHelper.GetRows(SpreadsheetId, new[] {
                "Drifter!A34:B381",
            });
            cardRatings = cardRatings.Where(c => (string)c[1] != "★").ToList();

            // setup rating calculation
            var calc = new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+", "S" });

            // remove old ratings
            Cards.ForEach(c => c.Ratings.RemoveAll(r => reviewSources.Contains(r.ReviewSource)));

            // add new ratings
            foreach (var card in Cards)
            {
                // setup search term
                var searchTerm = card.Name;
                // remove backside of card name
                if (searchTerm.Contains("//")) searchTerm = searchTerm.Substring(0, searchTerm.IndexOf("//", StringComparison.Ordinal) - 1);

                var cardRating = cardRatings.FirstOrDefault(c => (string)c[0] == searchTerm);
                if (cardRating == null)
                {
                    Fails.Add(new RatingFailure(ReviewSource.Drifter, card.ArenaId));
                }
                else
                {
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate((string)cardRating[1]), string.Empty, ReviewSource.Drifter));
                }
            }

            if (Fails.Any())
            {
                File.WriteAllText(JsonConvert.SerializeObject(Fails), Path.Combine(BasePath, $"{ReviewSource.Drifter}-fails.json"));
            }

            WriteFile();
        }
    }
}
