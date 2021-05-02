using System.Collections.Generic;
using Newtonsoft.Json;

namespace LimitedPower.Core
{
    public class ParserConfiguration
    {
        [JsonProperty("GeneratorName")]
        public string GeneratorName { get; set; }

        [JsonProperty("CardNameSubstitutions")]
        public Dictionary<string, string> CardNameSubstitutions { get; set; }

        [JsonProperty("Args")]
        public string[] Args { get; set; }
    }
}
