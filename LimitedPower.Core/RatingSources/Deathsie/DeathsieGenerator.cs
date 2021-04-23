using System;
using System.IO;
using System.Linq;
using LimitedPower.ViewModel;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.Deathsie
{
    public class DeathsieGenerator : RatingGeneratorBase
    {
        public string SpreadsheetId { get; set; }

        public DeathsieGenerator(string basePath, string set, string spreadsheetId) : base(basePath, set)
        {
            SpreadsheetId = spreadsheetId;
        }

        public override void RateCards()
        {
            LoadFile();

            // review source setup
            var reviewSources = new[] { ReviewSource.Deathsie };

            // load reviews from website
            var gdocsHelper = new GoogleDocsHelper();
            var cardRatings = gdocsHelper.GetRows(SpreadsheetId, new[] {
                "Deathsie!E37:G65",
                "Deathsie!J37:L63",
                "Deathsie!O37:Q64",
                "Deathsie!T37:V64",
                "Deathsie!Y37:AA64",
                "Deathsie!E73:G90",
                "Deathsie!J73:L89",
                "Deathsie!O73:Q90",
                "Deathsie!T73:V90",
                "Deathsie!Y73:AA89",
                "Deathsie!E98:G113",
                "Deathsie!J98:L114",
                "Deathsie!P98:Q111",
                "Deathsie!E133:G143",
                "Deathsie!E150:G161",
                "Deathsie!E167:G177",
                "Deathsie!J133:L144",
                "Deathsie!J150:L161",
                "Deathsie!J167:L171",
            });
            foreach (var c in cardRatings)
            {
                // trim middle section 
                if (c.Count == 3)
                {
                    c.RemoveAt(1);
                }
            }

            // setup rating calculation
            var calc = new RatingTransformer(new[] { "F", "D", "C", "B", "A", "S" });

            // remove old ratings
            Cards.ForEach(c => c.Ratings.RemoveAll(r => reviewSources.Contains(r.ReviewSource)));

            // add new ratings
            foreach (var card in Cards)
            {
                // setup search term
                var searchTerm = card.Name;
                // remove backside of card name
                if (searchTerm.Contains("//")) searchTerm = searchTerm.Substring(0, searchTerm.IndexOf("//", StringComparison.Ordinal) - 1);

                var cardRating = cardRatings.FirstOrDefault(c => (string)c[1] == searchTerm);
                if (cardRating == null)
                {
                    Fails.Add(new RatingFailure(ReviewSource.Deathsie, card.ArenaId));
                }
                else
                {
                    card.Ratings.Add(new LimitedPowerRating(calc.Calculate((string)cardRating[0]), string.Empty, ReviewSource.Deathsie));
                }
            }

            if (Fails.Any())
            {
                File.WriteAllText(JsonConvert.SerializeObject(Fails), Path.Combine(BasePath, $"{ReviewSource.Deathsie}-fails.json"));
            }

            WriteFile();
        }

    }
}
