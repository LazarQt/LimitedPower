using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.Extensions
{
    public static class CardExtensions
    {
        public static bool IsBasic(this Card c) => c.TypeLine.ToLower().Contains(CardConst.Basic.ToLower());

        public static List<string> CardsWithComma(this IEnumerable<Card> cards) =>
            cards.ToList().Where(c => c.Name.Contains(",")).Select(o => o.Name).ToList();
    }
}
