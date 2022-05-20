using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LimitedPower.Core.RatingSources
{
    public class MtgdsGenerator : RatingGeneratorBase<double>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } = { ReviewContributor.MtgDs };
        private readonly string _googleSheet;
        private double _minRating;
        private double _maxRating;

        public MtgdsGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions)
        {

        }

        protected override IRatingCalculator<double> CreateRatingCalculator() => new DoubleCalculator(_minRating, _maxRating);

        protected override List<RawRating<double>> GetRawRatings()
        {
            var result = new List<RawRating<double>>();
            var cards = GetCardsFile();
            var csv = File.ReadAllLines("snc.csv")
                .Skip(1)
                .Select(v => v)
                .ToList();
            csv.RemoveAll(c => c == string.Empty);

            foreach (var csvLine in csv)
            {
                var csvSplit = csvLine.Split(";");
                var x = csvSplit[0];
                if (x == "Mountain" || x == "Forest" || x == "Island" || x == "Swamp" || x == "Plains") continue;
                var n = cards.FirstOrDefault(c => c.Name.StartsWith(csvSplit[0]));
                result.Add(new RawRating<double>
                {
                    ReviewContributor = ReviewContributor.MtgDs,
                    RawValue = Convert.ToDouble(csvSplit[4]),
                    CardName = n.Name
                });
            }

            // initialize rating calculator private fields
            var orderedResults = result.OrderByDescending(x => x.RawValue).ToList();
            _minRating = orderedResults.Last().RawValue;
            _maxRating = orderedResults.First().RawValue;

            return result;
        }

    }
}
