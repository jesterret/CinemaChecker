using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    public class Site
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("n")]
        public string Name { get; private set; }
    }

    class SiteList
    {
        const string SitesUrl = "https://cinema-city.pl/pgm-sites";

        internal Task<List<Site>> SiteRequest;
        public SiteList()
        {
            SiteRequest = Program.GetRawStringAsync(SitesUrl)
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<Site>>(t.Result);
                });
        }

        public List<Site> Get()
        {
            SiteRequest.Wait();
            return SiteRequest.Result;
        }
    }
}
