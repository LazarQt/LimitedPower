using System;
using System.Collections.Generic;

namespace LimitedPower.Model
{
    public static class FlagExtensions
    {
        public static RarityStatus GetRarity(this string rarity)
        {
            // Todo: Hard coded rarity values, fight me
            var rarityMap = new Dictionary<string, RarityStatus>
            {
                {"common", RarityStatus.Common },
                {"uncommon", RarityStatus.Uncommon },
                {"rare", RarityStatus.Rare },
                {"mythic", RarityStatus.Mythic },
            };
            return rarityMap[rarity];
        }

        public static ColorWheel CreateColorWheel(this List<string> colors) => CreateColorWheel(colors.ToArray());

        public static int GetManaValue(this string manaCost)
        {
            int cost = 0;
            while (manaCost.Length > 0)
            {
                var closeBracketPos = manaCost.IndexOf('}');
                var costElement = manaCost.Substring(1, closeBracketPos - 1);
                if (int.TryParse(costElement.Replace("{", string.Empty).Replace("}", string.Empty), out int number))
                {
                    cost += number;
                }
                else
                {
                    cost++;
                }

                manaCost = manaCost.Remove(0, closeBracketPos + 1);
            }

            return cost;
        }

        public static ColorWheel CreateColorWheel(this string[] colors)
        {
            // Todo: Hard coded values...again. 
            var colorMap = new Dictionary<string, ColorWheel>
            {
                {"W", ColorWheel.White },
                {"U", ColorWheel.Blue },
                {"B", ColorWheel.Black},
                {"R", ColorWheel.Red },
                {"G", ColorWheel.Green },
                {"C", ColorWheel.Colorless },
            };

            var colorWheel = ColorWheel.None;
            if (colors == null) return colorWheel;
            foreach (var color in colors)
            {
                colorWheel |= colorMap[color];
            }
            return colorWheel;
        }
    }

    [Flags]
    public enum ColorWheel
    {
        None = 0,
        White = 2,
        Blue = 4,
        Black = 8,
        Red = 16,
        Green = 32,
        Colorless = 64
    }

    [Flags]
    public enum RarityStatus
    {
        Common = 0,
        Uncommon = 2,
        Rare = 4,
        Mythic = 8
    }
}
