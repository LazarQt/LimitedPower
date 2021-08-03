using System.Linq;

namespace LimitedPower.Model
{
    public static class ModelExtensions
    {
        public static string ColorGroup(this ViewCard card) => card.Colors.Count != 1 ? string.Empty : card.Colors.First();
    }
}
