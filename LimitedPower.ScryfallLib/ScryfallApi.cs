using System;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.Remote.Model;
using Newtonsoft.Json;
using RestSharp;

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

        private CardsSearch PerformCardsSearch(string setSearchUri)
        {
            var cardsSearchRequest = new RestRequest(new Uri(setSearchUri).PathAndQuery, Method.GET);
            IRestResponse cardsSearchResponse = _client.Execute(cardsSearchRequest);
            return JsonConvert.DeserializeObject<CardsSearch>(cardsSearchResponse.Content);
        }

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

            if (set == null)
            {
                throw new Exception($"could not get cards for set {setCode}");
            }

            // get card search from set 
            var cardsSearch = PerformCardsSearch(set.SearchUri);

            var result = new List<ScryfallCard>();

            while (cardsSearch != null)
            {
                foreach (var sourceCard in cardsSearch.Data)
                {
                    var printedSize = set.PrintedSize;
                    if (printedSize == 0 && set.Digital) printedSize = set.CardCount; // fallback for digital only sets
                    if (Parameters.ContainsKey("PrintedSize")) printedSize = Convert.ToInt32(Parameters["PrintedSize"]);
                    if (set.Digital && printedSize == 0) printedSize = set.CardCount;
                    if (printedSize == 0) printedSize = set.CardCount;
                    if (!sourceCard.Booster || result.Any(o => o.Name == sourceCard.Name) || Convert.ToInt32(sourceCard.CollectorNumber) > printedSize)
                    {
                        continue;
                    }
                    result.Add(sourceCard);
                }

                cardsSearch = cardsSearch.HasMore ? PerformCardsSearch(cardsSearch.NextPage) : null;
            }

            return result;
        }
    }
}
