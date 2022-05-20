using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Core.Extensions
{
    public static class CardExtensions
    {
        /// <summary>
        /// Check if card is of Basic super type (Lands)
        /// </summary>
        /// <param name="c">Card to check</param>
        /// <returns>Returns true if card is a basic land</returns>
        public static bool IsBasic(this Card c) => c.TypeLine.ToLower().Contains(CardAttribute.Basic.ToLower());

        /// <summary>
        /// Filter cards with commas in name
        /// </summary>
        /// <param name="cards">List of cards to filter</param>
        /// <returns>Returns list with cards that have comma in name</returns>
        public static List<string> CardsWithComma(this IEnumerable<Card> cards) =>
            cards.ToList().Where(c => c.Name.Contains(",")).Select(o => o.Name).ToList();
    }
}
