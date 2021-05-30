using System.Collections.Generic;

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
        public List<CardFace> CardFaces { get; set; } = new List<CardFace>();

        public List<LimitedPowerRating> Ratings = new List<LimitedPowerRating>();
    }
}
