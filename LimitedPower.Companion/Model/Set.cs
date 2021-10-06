using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Companion.Model
{
    public class Set
    {
        public string Name { get; set; }
        public int StartGold { get; set; }
        public int StartGems { get; set; }
        public int StartPacks { get; set; }
        public List<Draft> Drafts { get; set; }
        [Generated]
        public List<Match> Matches => Drafts.SelectMany(d => d.Matches).ToList();

        public int Wins => Matches.Count(m => m.Outcome == Outcome.Win);
        public int Losses => Matches.Count(m => m.Outcome == Outcome.Loss);
        public double Winrate => Math.Round(100d / (Wins + Losses) * Wins, MidpointRounding.AwayFromZero);

        public double GetDeckWinrate(string deck) => Math.Round(100d / (GetDeckWins(deck) + GetDeckLosses(deck)) * GetDeckWins(deck), MidpointRounding.AwayFromZero);
        public double GetDeckWins(string deck) => GetDeckMatches(deck).Count(m => m.Outcome == Outcome.Win);
        public double GetDeckLosses(string deck) => GetDeckMatches(deck).Count(m => m.Outcome == Outcome.Loss);
        public IEnumerable<Match> GetDeckMatches(string deck) => Drafts.Where(d => d.Deck == deck).SelectMany(m => m.Matches);
    }
}
