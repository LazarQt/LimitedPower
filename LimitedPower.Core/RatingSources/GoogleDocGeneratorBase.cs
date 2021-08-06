using System.Collections.Generic;
using System.Linq;
using LimitedPower.Model;

namespace LimitedPower.Core.RatingSources
{
    public abstract class GoogleDocGeneratorBase : RatingGeneratorBase<string>
    {
        protected override ReviewContributor[] ReviewContributors { get; set; } =
        {
            ReviewContributor.InfiniteMythicEditionJustLolaman,
            ReviewContributor.InfiniteMythicEditionM0bieus,
            ReviewContributor.InfiniteMythicEditionScottynada
        };

        protected string SpreadsheetId { get; set; }
        protected string SpreadsheetName { get; set; }
        protected string[] SpreadsheetRanges { get; set; }

        protected GoogleDocGeneratorBase(string basePath, string set, Dictionary<string, string> cardNameSubstitutions,
            string[] args) : base(basePath, set, cardNameSubstitutions)
        {
            SpreadsheetId = args[0];
            SpreadsheetName = args[1];
            SpreadsheetRanges = args[2].Split(',');
        }

        protected List<RawRating<string>> PopulateRatings(List<List<object>> rawRatings, Dictionary<ReviewContributor, int> contributors, int nameIndex)
        {
            var result = new List<RawRating<string>>();
            foreach (var r in rawRatings)
            {
                // in older reviews there were fewer people, todo: fix permanently?
                if (nameIndex >= r.Count) continue;
                var cardName = (string)r[nameIndex];
                foreach (var contributor in contributors)
                {
                    result.Add(new RawRating<string>
                    {
                        ReviewContributor = contributor.Key,
                        RawValue = (string)r[contributor.Value],
                        CardName = cardName
                    });
                }
            }
            return result;
        }

        protected List<List<object>> GetRows() => new GoogleDocsHelper().GetRows(SpreadsheetId, SpreadsheetRanges.Select(r => $"{SpreadsheetName}!{r}").ToArray());

        protected abstract override IRatingCalculator<string> CreateRatingCalculator();

        protected abstract override List<RawRating<string>> GetRawRatings();
    }
}
