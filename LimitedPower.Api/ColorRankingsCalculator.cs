using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LimitedPower.Model;
using Newtonsoft.Json;

namespace LimitedPower.Api
{
    public class ColorRankingsCalculator
    {
        public Dictionary<string, double> GetRankings(string setCode, bool live, string callParams)
        {
            List<ViewCard> currentCards = GetCards($"Set/{setCode}.json", live);

            var allCards = new List<ViewCard>();

            string[] fileEntries = Directory.GetFiles("Set/");
            foreach (var entry in fileEntries)
            {
                allCards.AddRange(GetCards(entry, live));
            }

            var currentColorRanks = InitRanks();
            var allColorRanks = InitRanks();

            var currentRankings = Calculate(currentColorRanks, currentCards, live);
            var allRankings = Calculate(allColorRanks, allCards, live);

            var relativeRankings = new Dictionary<string, double>()
            {
                {"w", default},
                {"u", default},
                {"b", default},
                {"r", default},
                {"g", default}
            };

            foreach (var r in currentRankings)
            {
                var val = r.Value;
                var diffVal = allRankings[r.Key];
                relativeRankings[r.Key] = Math.Round(val - diffVal, 2);
            }

            return relativeRankings;
        }

        private List<ViewCard> GetCards(string path, bool live)
        {
            var cards = JsonConvert.DeserializeObject<List<ViewCard>>(File.ReadAllText($"{path}"));
            if (cards == null) return new List<ViewCard>();
            cards = live ? cards.OrderByDescending(c => c.LiveRating).ToList() : cards.OrderByDescending(c => c.InitialRating).ToList();
            return cards;
        }

        private Dictionary<string, List<ViewCard>> InitRanks()
        {
            return new Dictionary<string, List<ViewCard>>()
            {
                {"w", new List<ViewCard>()},
                {"u", new List<ViewCard>()},
                {"b", new List<ViewCard>()},
                {"r", new List<ViewCard>()},
                {"g", new List<ViewCard>()}
            };
        }

        private Dictionary<string, double> Calculate(Dictionary<string, List<ViewCard>> ranks, List<ViewCard> cards, bool live)
        {
            foreach (var color in ranks)
            {
                foreach (var card in cards)
                {
                    if (card.CanBeCastWithOnly(color.Key))
                    {
                        color.Value.Add(card);
                    }
                }
            }

            var rankings = new Dictionary<string, double>();
            foreach (var x in ranks)
            {
                var r = live ? x.Value.Average(u => u.LiveRating) : x.Value.Average(u => u.InitialRating);
                rankings.Add(x.Key, r);
            }

            return rankings;
        }
    }
}
