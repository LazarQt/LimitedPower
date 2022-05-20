using LimitedPower.Core;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Tool
{
    public static class Extensions
    {
        public static string PrimarySet(this string sets) => sets.ToArray().First();

        public static string[] ToArray(this string sets) => sets.Split(',');

        public static LimitedPowerConfig GetSet(this List<LimitedPowerConfig> configurations, string i) =>
            configurations.FirstOrDefault(c => c.Set.Equals(i));
    }
}
