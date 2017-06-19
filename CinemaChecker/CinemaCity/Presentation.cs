using Newtonsoft.Json;

namespace CinemaChecker.CinemaCity
{
    public class Presentation
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
        [JsonProperty("attr")]
        public string Tags { get; private set; }
        [JsonProperty("code")]
        public string Code { get; private set; }
        [JsonProperty("vt")]
        public int VenueType { get; private set; }

        public bool IsIMAX => Venue == "IMAX";

        public string GetUrl(long SiteId)
        {
            return $"https://cinema-city.pl/ecom-tickets?siteId={SiteId}&prsntId={Code}";
        }
    }
}
