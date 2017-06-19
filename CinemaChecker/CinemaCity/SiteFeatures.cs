using Newtonsoft.Json;
using System.Collections.Generic;

namespace CinemaChecker.CinemaCity
{
    public class SiteFeatures
    {
        [JsonProperty("BD")]
        public List<Feature> Features { get; private set; }
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("n")]
        public string Name { get; private set; }
    }
}
