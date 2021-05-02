using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LimitedPower.Model;
using LimitedPower.UI.Extensions;
using Microsoft.AspNetCore.Components;

namespace LimitedPower.UI.Component
{
    public class TopCommons : CardList
    {

        protected override void SortCards()
        {
            var sortedByRating = Cards.Where(x => x.Rarity == RarityStatus.Common).OrderByDescending(c => c.TotalRating(Session.LiveData)).ToList();
            var identities = new List<ColorWheel>()
            {
                ColorWheel.White,
                ColorWheel.Blue,
                ColorWheel.Black,
                ColorWheel.Red,
                ColorWheel.Green,
                ColorWheel.None,
                ColorWheel.Red | ColorWheel.White,
                ColorWheel.Blue | ColorWheel.Red,
                ColorWheel.Green | ColorWheel.Blue,
                ColorWheel.White | ColorWheel.Black,
                ColorWheel.Green | ColorWheel.Black,
            };
            foreach (var identity in identities)
            {
                CardCategories.Add(identity.Gn(), sortedByRating.Where(c => c.ColorIdentity == identity).Take(5).ToList());
            }
        }
    }

    public class CombatTricks : CardList
    {
        protected override void SortCards()
        {
            CardCategories.Clear();
            var exceptions = new[]
            {
                76427, 76433, 76442, 76446, 76461, 76474, 76495, 76546, 76587, 76592, 76599, 76620, 76636, 76649, 77488,
                77491, 77519, 77492, 77512, 77505, 77498, 77517, 77514, 77537, 77525, 77540,
            };
            var cards = Cards;
            if (!IncludeSpecialCards)
            {
                cards = cards.Where(c => c.SetCode == "stx").ToList();
            }
            if (ShowFilter && ColorFilter != ColorWheel.None)
            {
                cards = cards.Where(c => ColorFilter.HasFlag(c.ColorIdentity)).ToList();
            }
            cards = cards
                .Where(o => !exceptions.Contains(o.ArenaId) && o.CardFaces.Any(cf => cf.TypeLine.Contains("Instant") || cf.OracleText.ToLower().Contains("flash")))
                .OrderBy(c => c.ManaValue)
                .ThenBy(x => Convert.ToInt32(x.CollectorNumber))
                .ToList();
            var cGroups = cards.GroupBy(x => x.ManaValue);
            foreach (var group in cGroups)
            {
                CardCategories.Add(group.Key.ToString(CultureInfo.InvariantCulture), group.ToList());
            }
        }
    }
}
