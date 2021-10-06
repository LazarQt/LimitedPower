using System;
using System.Collections.Generic;

namespace LimitedPower.Companion.Model
{
    public class Draft
    {
        public int Id { get; set; }
        public string Set { get; set; }
        public string Event { get; set; }
        public string Entry { get; set; }
        public DateTime Date { get; set; }
        public string Deck { get; set; }
        public int? NewGoldStatus { get; set; }
        public int? NewGemStatus { get; set; }
        public int? NewPackStatus { get; set; }
        [Generated]
        public List<Match> Matches { get; set; }
    }
}
