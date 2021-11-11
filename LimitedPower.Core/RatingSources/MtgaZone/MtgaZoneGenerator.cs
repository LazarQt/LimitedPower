using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.RatingSources.MtgaZone
{
    public class MtgaZoneGenerator : GoogleDocGeneratorBase
    {
        protected sealed override ReviewContributor[] ReviewContributors { get; set; }

        private readonly Dictionary<ReviewContributor, int> _contributors;

        public MtgaZoneGenerator(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions, args)
        {
            var contributorArgs = args[3].Split(',');
            _contributors = new Dictionary<ReviewContributor, int>();
            foreach (var c in contributorArgs)
            {
                var split = c.Split(':');
                if (Enum.TryParse(split[0], out ReviewContributor contributorEnum))
                {
                    _contributors.Add(contributorEnum, Convert.ToInt32(split[1]));
                }
            }

            ReviewContributors = _contributors.Select(c => c.Key).ToArray();
        }

        protected override IRatingCalculator<string> CreateRatingCalculator() =>
            new RatingTransformer(new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+", "S" });

        protected override List<RawRating<string>> GetRawRatings() => PopulateRatings(GetRows(), _contributors, 0);
    }
}
