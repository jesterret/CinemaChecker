using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    public class PresentationInfo
    {

        [JsonProperty("sold")]
        public bool IsSoldOut { get; private set; }
        [JsonProperty("is3d")]
        public bool Is3D { get; private set; }
        [JsonProperty("sub")]
        public bool Subtitles { get; private set; }
        [JsonProperty("dub")]
        public bool Dubbing { get; private set; }
        [JsonProperty("vn")]
        public string Venue { get; private set; }
        [JsonProperty("time")]
        public string Time { get; private set; }
        [JsonProperty("vt")]
        public int VenueType { get; private set; }

        public bool IsIMAX => Venue == "IMAX";
    }
    public class FeatureInfo
    {
        [JsonProperty("date")]
        public string Date { get; private set; }
        [JsonProperty("P")]
        public List<PresentationInfo> Presentations { get; private set; }
    }
    public class PresentationData
    {
        [JsonProperty("BD")]
        public List<FeatureInfo> Features { get; private set; }
        [JsonProperty("code")]
        public string Code { get; private set; }
        [JsonProperty("dur")]
        public int Duration { get; private set; }
        [JsonProperty("n")]
        public string Name { get; private set; }
    }
}
