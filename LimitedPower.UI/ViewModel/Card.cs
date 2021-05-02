using System;
using System.Linq;
using LimitedPower.Model;

namespace LimitedPower.UI.ViewModel
{
    public class Card : Model.Card
    {
        public bool ShowBack { get; set; }

        public string CardFaceName => !ShowBack ? CardFaces[0].Name : CardFaces[1].Name;
        public bool IsDFC => CardFaces?.Count > 1;

        public string ImageUrl => $"img/set/{SetCode}/{ArenaId}-{(ShowBack ? 1 : 0)}.jpg";
        public double TotalRating(bool live) => GetTotalRating(live);

        private double GetTotalRating(bool live)
        {
            if (!Ratings.Any()) return 0.0;
            if (!live || SetCode == "ita")
            {
                return Ratings.Where(o => o.ReviewContributor != ReviewContributor.SeventeenLands).Average(r => r.Rating);
            }
            else
            {
                return Ratings.Where(o => o.ReviewContributor == ReviewContributor.SeventeenLands).Average(r => r.Rating);
            }
        }

        public int ManaValue => CardFaces[0].ManaCost.Count(f => f == '{'); // each occurrence in bracket means 1 mana value

        public string Grade(bool liveData) => GetGrade(liveData);

        private string GetGrade(bool liveData)
        {
            // Special Case
            if (TotalRating(liveData) > 97)
            {
                if (!liveData)
                {
                    return "S";
                }
            }

            var ratings = new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" };
            return ratings[Convert.ToInt32(Math.Floor(TotalRating(liveData) * ratings.Length / 100))];
        }

        public void Flip()
        {
            if (IsDFC) ShowBack = !ShowBack;
        }
    }
}
