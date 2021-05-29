using System;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.Model;

namespace LimitedPower.UI.ViewModel
{
    public class Card : Model.Card
    {
        public bool ShowBack { get; set; }

        public string CardFaceName => !ShowBack ? CardFaces[0].Name : CardFaces[1].Name;
        public bool IsDFC => CardFaces?.Count > 1;

        public bool Castable(string colors)
        {
            List<bool> castables = new List<bool>();
            foreach (var side in CardFaces)
            {
                var castable = true;
                var requirements = side.ManaCost.Split("}{").Select(t => t.Replace("{","").Replace("}",""));
                var hybridReq = 0;
                foreach (var requirement in requirements)
                {
                    if (int.TryParse(requirement, out _)) continue;
                    if (LongestCommonSubstring(requirement.ToLower(), colors.ToLower()) <= 0) castable = false;
                    if (requirement.Contains("/"))
                    {
                        if (colors.Length >= 2)
                        {
                            var a = requirement.Split("/");
                            Array.Sort(a);
                            var s1 = string.Join("", a);

                            var b = colors.ToCharArray();
                            Array.Sort(b);
                            var s2 = string.Join("", b);
                            if (s1.ToLower() != s2.ToLower()) hybridReq += 1;
                        }
                        else
                        {
                            hybridReq += 1;
                        }
                    }
                }

                if (hybridReq >= 3) castable = false;

                castables.Add(castable);
            }

            return castables.Any(c => c);
        }

        public int LongestCommonSubstring(string str1, string str2)
        {
            if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2))
                return 0;

            int[,] num = new int[str1.Length, str2.Length];
            int maxlen = 0;

            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                        }
                    }
                }
            }

            return maxlen;
        }


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

        public int ManaValue => GetManaValue();

        public int GetManaValue()
        {
            var face = CardFaces[0];
            var x = face.ManaCost.Split("}{").Select(u => u.Replace("{","").Replace("}",""));
            var total = 0;
            foreach (var c in x)
            {
                if (int.TryParse(c, out int myNum))
                {
                    total += myNum;
                }
                else
                {
                    total += 1;
                }
            }

            return total;
        }

        public string Grade(bool liveData) => GetGrade(liveData);

        private string GetGrade(bool liveData)
        {
            // Special Case
            if (TotalRating(liveData) > 97) return "S";
            //{
            //    if (!liveData)
            //    {
            //        return "S";
            //    }
            //}

            var ratings = new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" };
            return ratings[Convert.ToInt32(Math.Floor(TotalRating(liveData) * ratings.Length / 100))];
        }

        public void Flip()
        {
            if (IsDFC) ShowBack = !ShowBack;
        }
    }
}
