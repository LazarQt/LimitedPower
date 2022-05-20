using LimitedPower.Remote.Model;
using System.Collections.Generic;

namespace LimitedPower.Core
{
    public class Card : ScryfallCard
    {
        public List<LimitedPowerRating> Ratings { get; set; }
        public double CondensedRating { get; set; }
    }
}
