using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LimitedPower.UI.Services
{
    public interface ICardService
    {
        Task<List<Model.Card>> GetCards();
    }
    public class CardService : ICardService
    {
        public HttpClient Http { get; set; }

        public CardService(HttpClient http)
        {
            Http = http;
        }

        private List<Model.Card> _cards = new();
        private bool _isInitialized = false;

        public async Task<List<Model.Card>> GetCards()
        {
            if (!_isInitialized)
            {
                _cards = JsonConvert.DeserializeObject<List<Model.Card>>(await Http.GetStringAsync("rating-data/stx.json"));
            }
            return _cards;
        }
    }
}
