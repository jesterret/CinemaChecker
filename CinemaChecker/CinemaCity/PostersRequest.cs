using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    public class Poster
    {
#pragma warning disable CS0649 // Field 'Poster.PosterFile' is never assigned to, and will always have its default value null 
        [JsonProperty("posterSrc")]
        private string PosterFile;
#pragma warning restore CS0649 // Field 'Poster.PosterFile' is never assigned to, and will always have its default value null 
        [JsonProperty("featureTitle")]
        public string Title { get; set; }
        [JsonProperty("code")]
        public string Code { get; private set; }
        [JsonProperty("movieUrl")]
        public Uri TrailerUrl { get; private set; }

        public string PosterImage => string.Format("https://cinema-city.pl/xmedia-cw/repo/feats/posters/{0}", PosterFile);
    }
    public class PostersRequestData
    {
        [JsonProperty("posters")]
        public List<Poster> PosterList { get; private set; }
    }
    public class PostersRequest
    {
        [JsonProperty("data")]
        internal PostersRequestData Data { get; set; }

        public List<Poster> Posters => Data.PosterList;
    }

    public class PosterList
    {
        const string PosterUrl = "https://cinema-city.pl/getPosters?filter=%7B%22hideNoImageFeatures%22:false%7D";

        internal Task<PostersRequest> PosterReq;
        public PosterList()
        {
            PosterReq = Program.GetRawStringAsync(PosterUrl)
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<PostersRequest>(t.Result);
                });
        }

        public PostersRequest Get()
        {
            PosterReq.Wait();
            return PosterReq.Result;
        }
    }
}
