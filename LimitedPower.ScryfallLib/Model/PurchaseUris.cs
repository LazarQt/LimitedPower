using Newtonsoft.Json;

namespace LimitedPower.Remote.Model
{
    public class PurchaseUris
    {
        [JsonProperty("tcgplayer")]
        public string Tcgplayer { get; set; }

        [JsonProperty("cardmarket")]
        public string Cardmarket { get; set; }

        [JsonProperty("cardhoarder")]
        public string Cardhoarder { get; set; }
    }
}