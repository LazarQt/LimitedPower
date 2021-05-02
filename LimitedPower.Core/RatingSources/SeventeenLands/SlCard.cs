using Newtonsoft.Json;

namespace LimitedPower.Core.RatingSources.SeventeenLands
{
    public class SlCard
    {
        [JsonProperty("seen_count")]
        public int SeenCount { get; set; }

        [JsonProperty("avg_seen")]
        public double AvgSeen { get; set; }

        [JsonProperty("pick_count")]
        public int PickCount { get; set; }

        [JsonProperty("avg_pick")]
        public double AvgPick { get; set; }

        [JsonProperty("game_count")]
        public int GameCount { get; set; }

        [JsonProperty("win_rate")]
        public double WinRate { get; set; }

        [JsonProperty("sideboard_game_count")]
        public int SideboardGameCount { get; set; }

        [JsonProperty("sideboard_win_rate")]
        public double SideboardWinRate { get; set; }

        [JsonProperty("opening_hand_game_count")]
        public int OpeningHandGameCount { get; set; }

        [JsonProperty("opening_hand_win_rate")]
        public double OpeningHandWinRate { get; set; }

        [JsonProperty("drawn_game_count")]
        public int DrawnGameCount { get; set; }

        [JsonProperty("drawn_win_rate")]
        public double DrawnWinRate { get; set; }

        [JsonProperty("ever_drawn_game_count")]
        public int EverDrawnGameCount { get; set; }

        [JsonProperty("ever_drawn_win_rate")]
        public double EverDrawnWinRate { get; set; }

        [JsonProperty("never_drawn_game_count")]
        public int NeverDrawnGameCount { get; set; }

        [JsonProperty("never_drawn_win_rate")]
        public double NeverDrawnWinRate { get; set; }

        [JsonProperty("drawn_improvement_win_rate")]
        public double DrawnImprovementWinRate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
