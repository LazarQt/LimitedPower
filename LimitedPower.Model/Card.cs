﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Model
{
    public class Card
    {
        public int ArenaId { get; set; }
        public string Name { get; set; }
        public string SetCode { get; set; }
        public string CollectorNumber { get; set; }
        public List<string> Keywords;
        public string Rarity { get; set; }
        public List<string> Colors { get; set; }
        public List<string> ColorIdentity { get; set; }
        public List<string> ProducedMana { get; set; }
        public List<CardFace> CardFaces { get; set; }
        public List<LimitedPowerRating> Ratings { get; set; }
    }

    public class ViewCard : Card
    {
        public double LiveRating => Math.Round(Ratings
            .Where(r => r.ReviewContributor == ReviewContributor.SeventeenLands)
            .Average(x => x.Rating), 2);

        public double InitialRating => Math.Round(Ratings
            .Where(r => r.ReviewContributor != ReviewContributor.SeventeenLands)
            .Average(x => x.Rating), 2);

        public bool IsDFC => CardFaces.Count > 1;

        public string LiveGrade => GetStringGrade(LiveRating);
        public string InitialGrade => GetStringGrade(InitialRating);

        private string GetStringGrade(double rating)
        {
            if (rating > 99) return "S";
            var ratings = new[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A", "A+" };
            return ratings[Convert.ToInt32(Math.Floor(rating * ratings.Length / 100))];
        }

    }
}
