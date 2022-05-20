using Newtonsoft.Json;
using System.Collections.Generic;

namespace LimitedPower.Core
{
    public class LimitedPowerConfig
    {
        public string Set { get; set; }
        public Dictionary<string, object> ScryfallApiArgs { get; set; }
        public List<ParserConfiguration> ParserConfigurations { get; set; }
    }

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
