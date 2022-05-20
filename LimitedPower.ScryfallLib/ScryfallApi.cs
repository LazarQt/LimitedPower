using LimitedPower.Remote.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedPower.Remote
{
    public class ScryfallApi
    {
        readonly RestClient _client = new RestClient("https://api.scryfall.com");
        private Dictionary<string, object> Parameters { get; }

        public ScryfallApi(Dictionary<string, object> parameters = null)
        {
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Get Scryfall cards for given set of mtg sets
        /// </summary>
        /// <param name="setCodes">List of sets to get cards from</param>
        /// <returns>Scryfall card list</returns>
        public List<ScryfallCard> GetSourceCards(string[] setCodes)
        {
            var result = new List<ScryfallCard>();
            foreach (var setcode in setCodes)
            {
                result.AddRange(GetSourceCards(setcode));
            }
            return result;
        }

        private List<ScryfallCard> GetSourceCards(string setCode)
        {
            // get set
            var setRequest = new RestRequest($"sets/{setCode}", Method.GET);
            IRestResponse setResponse = _client.Execute(setRequest);
            var set = JsonConvert.DeserializeObject<Set>(setResponse.Content);
            if (set == null) throw new Exception($"could not get cards for set {setCode}");

            // get card search from set 
            var cardsSearch = PerformCardsSearch(set.SearchUri);

            // populate results
            var result = new List<ScryfallCard>();
            while (cardsSearch != null)
            {
                foreach (var sourceCard in cardsSearch.Data)
                {
                    var printedSize = set.PrintedSize;
                    if (printedSize == 0 && set.Digital) printedSize = set.CardCount; // fallback for digital only sets

                    // special case if no set count is given (sometimes error on scryfall side)
                    if (Parameters.ContainsKey(Argument.PrintedSize)) printedSize = Convert.ToInt32(Parameters[Argument.PrintedSize]);

                    if (set.Digital && printedSize == 0) printedSize = set.CardCount;
                    if (printedSize == 0) printedSize = set.CardCount;
                    if (!sourceCard.Booster || result.Any(o => o.Name == sourceCard.Name) || Convert.ToInt32(sourceCard.CollectorNumber) > printedSize)
                    {
                        continue;
                    }
                    result.Add(sourceCard);
                }
                // recursive search 
                cardsSearch = cardsSearch.HasMore ? PerformCardsSearch(cardsSearch.NextPage) : null;
            }

            return result;
        }

        private CardsSearch PerformCardsSearch(string setSearchUri)
        {
            var cardsSearchResponse = _client.Execute(new RestRequest(new Uri(setSearchUri).PathAndQuery, Method.GET));
            return JsonConvert.DeserializeObject<CardsSearch>(cardsSearchResponse.Content);
        }
    }
}
