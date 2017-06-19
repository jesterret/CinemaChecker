using Newtonsoft.Json;
using System.Collections.Generic;

namespace CinemaChecker.CinemaCity
{
    public class Feature
    {
        [JsonProperty("P")]
        public List<Presentation> Presentations { get; private set; }
        [JsonProperty("date")]
        public string Date { get; private set; }
    }
}
