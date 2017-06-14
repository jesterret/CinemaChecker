using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    class PosterInfo
    {
        [JsonProperty("posterSrc")]
        private string Poster;
        [JsonProperty("featureTitle")]
        public string Title { get; set; }
        [JsonProperty("code")]
        public string Code { get; private set; }
        [JsonProperty("movieUrl")]
        public Uri TrailerUrl { get; private set; }

        public string PosterImage => string.Format("https://n.cinema-city.pl/xmedia-cw/repo/feats/posters/{0}", Poster);
    }
    class PostersRequestData
    {
        [JsonProperty("posters")]
        public List<PosterInfo> Posters { get; private set; }
    }
    class PostersRequest
    {
        [JsonProperty("data")]
        private PostersRequestData Data { get; set; }

        public List<PosterInfo> Posters => Data.Posters;
    }
}
