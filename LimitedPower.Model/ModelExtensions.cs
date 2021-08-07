using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Model
{
    public static class ModelExtensions
    {
        public static string ColorGroup(this ViewCard card) => card.Colors.Count != 1 ? string.Empty : card.Colors.First();
        public static string GetString(this List<string> list)
        {
            list.Sort();
            return string.Join("", list);
        }

        public static bool CanBeCastWithOnly(this Card card, string color)
        {
            if (string.IsNullOrEmpty(card.ManaCost)) return true;
            var manaCost = card.ManaCost;
            manaCost = manaCost.Substring(1, manaCost.Length - 2);
            var manaCosts = manaCost.Split("}{");
            foreach (var mc in manaCosts)
            {
                if (int.TryParse(mc, out _)) continue;
                if (mc.ToLower().Contains(color.ToLower())) continue;
                return false;
            }

            return true;
        }
    }
}
