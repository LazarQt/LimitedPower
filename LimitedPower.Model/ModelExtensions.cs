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
    }
}
