using Newtonsoft.Json;
using System.Collections.Generic;

namespace CinemaChecker.CinemaCity
{
    public class SeanceInfo
    {
        [JsonProperty("BD")]
        public List<Feature> Features { get; private set; }
        [JsonProperty("code")]
        public string Code { get; private set; }
        [JsonProperty("dur")]
        public int Duration { get; private set; }
        [JsonProperty("n")]
        public string Name { get; private set; }
    }
}
