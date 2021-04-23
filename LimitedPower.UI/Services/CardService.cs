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
                //var sl = JsonConvert.DeserializeObject<List<SeventeenLandsCard>>(await Http.GetStringAsync("rating-data/stx-sl.json"));
                //var calc = new RatingCalculator(cardRatings.OrderByDescending(r => r.MyRating).Select(e => e.MyRating).ToList());
            }
            return _cards;
        }
    }
}
