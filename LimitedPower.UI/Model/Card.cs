using System;
using System.Linq;

namespace LimitedPower.UI.Model
{
    public class Card : ViewModel.Card
    {
        public bool ShowBack { get; set; }

        public string CardFaceName => !ShowBack ? CardFaces[0].Name : CardFaces[1].Name;
        public bool IsDFC => CardFaces?.Count > 1;
        public string ImageUrl => $"img/stx/{ArenaId}-{(ShowBack ? 1 : 0)}.png";
        public double TotalRating => !Ratings.Any() ? 0.0 : Ratings.Average(r => r.Rating);

        public string Grade => GetGrade();

        private string GetGrade()
        {
            // Special Case
            if (TotalRating > 97) return "S";

            var ratings = new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" };
            return ratings[Convert.ToInt32(Math.Floor(TotalRating * ratings.Length / 100))];
        }

        public void Flip()
        {
            if (IsDFC) ShowBack = !ShowBack;
        }
    }
}
