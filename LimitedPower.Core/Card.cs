using System.Collections.Generic;
using LimitedPower.Remote.Model;

namespace LimitedPower.Core
{
    public class Card : ScryfallCard
    {
        public List<LimitedPowerRating> Ratings { get; set; }
    }
}
