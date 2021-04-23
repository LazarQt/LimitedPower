using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.DraftSim
{
    public class DraftSimData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("castingcost1")]
        public string CastingCost1 { get; set; }

        [JsonProperty("castingcost2")]
        public string CastingCost2 { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("myrating")]
        public double MyRating { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("cmc")]
        public string Cmc { get; set; }

        [JsonProperty("creaturesort")]
        public string CreatureSort { get; set; }

        [JsonProperty("colorsort")]
        public string ColorSort { get; set; }
    }
}
