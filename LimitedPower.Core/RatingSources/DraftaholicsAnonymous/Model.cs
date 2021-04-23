using System.Collections.Generic;
using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.DraftaholicsAnonymous
{
    public class Card
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("set_id")]
        public int SetId { get; set; }

        [JsonProperty("set_name")]
        public string SetName { get; set; }

        [JsonProperty("image_small")]
        public string ImageSmall { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("image_large")]
        public string ImageLarge { get; set; }

        [JsonProperty("back_image_small")]
        public string BackImageSmall { get; set; }

        [JsonProperty("back_image")]
        public string BackImage { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("back_name")]
        public string BackName { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("elo")]
        public int Elo { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("exclude_from_p1p1")]
        public object ExcludeFromP1P1 { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("mana_cost")]
        public string ManaCost { get; set; }

        [JsonProperty("colors")]
        public string Colors { get; set; }
    }

    public class DraftaholicsAnonymousRoot
    {
        [JsonProperty("data")]
        public List<Card> Data { get; set; }
    }


}
