﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace LimitedPower.ScryfallLib.Model
{
    class CardsSearch
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("total_cards")]
        public int TotalCards { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("next_page")]
        public string NextPage { get; set; }

        [JsonProperty("data")]
        public List<Card> Data { get; set; }
    }
}
