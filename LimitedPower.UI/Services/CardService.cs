using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LimitedPower.UI.Services
{
    public interface ICardService
    {
        Task<List<ViewModel.Card>> GetCards(string s);
    }

    public class CardService : ICardService
    {
        public HttpClient Http { get; set; }

        public CardService(HttpClient http)
        {
            Http = http;
        }

        private List<ViewModel.Card> _cards = new();
        private bool _isInitialized;
        private string _setLoaded = string.Empty;

        public async Task<List<ViewModel.Card>> GetCards(string setcode)
        {
            if (!_isInitialized || _setLoaded != setcode)
            {
                string x = "";
                try
                {
                    x = await Http.GetStringAsync($"rating-data/{setcode}.json");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                _cards = JsonConvert.DeserializeObject<List<ViewModel.Card>>(x);
                _isInitialized = true;
                _setLoaded = setcode;
            }
            return _cards;
        }
    }
}
