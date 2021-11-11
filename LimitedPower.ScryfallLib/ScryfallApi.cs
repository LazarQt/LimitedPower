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

        public ScryfallCard FuzzySearch(string name)
        {
            var req = new RestRequest("/cards/named?fuzzy="+name, Method.GET);
            var res = _client.Execute(req);
            return JsonConvert.DeserializeObject<ScryfallCard>(res.Content);
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

        public List<ScryfallCard> GetLands() => GetLandsRecursively(
            "cards/search?q=t%3Aland+is%3Afirstprinting+-t%3Abasic&unique=cards&as=grid&order=released&dir=asc");

        private List<ScryfallCard> GetLandsRecursively(string req, List<ScryfallCard> lands = null)
        {
            var landRequest = new RestRequest(req, Method.GET);
            var landResponse = _client.Execute(landRequest);
            var search = JsonConvert.DeserializeObject<CardsSearch>(landResponse.Content);

            lands ??= new List<ScryfallCard>();
            if (search is {Data: { }}) lands.AddRange(search.Data);

            return search is {HasMore: true} ? GetLandsRecursively(search.NextPage, lands) : lands;
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
