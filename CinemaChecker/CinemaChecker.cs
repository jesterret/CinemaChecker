using CinemaChecker.CinemaCity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker
{
    class CinemaChecker
    {
        const string PosterUrl = "https://cinema-city.pl/getPosters?filter=%7B%22hideNoImageFeatures%22:false%7D";
        const string SitesUrl = "https://cinema-city.pl/pgm-sites";

        public CinemaChecker()
        {
            SitesTask = Program.GetRawStringAsync(SitesUrl)
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<CinemaSite>>(t.Result);
                });
            PostersTask = Program.GetRawStringAsync(PosterUrl)
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<PostersRequest>(t.Result);
                });
        }

        public List<CinemaSite> GetSites()
        {
            SitesTask.Wait();
            return SitesTask.Result;
        }
        public List<PosterInfo> GetPosters()
        {
            PostersTask.Wait();
            return PostersTask.Result.Posters;
        }

        Task<PostersRequest> PostersTask;
        Task<List<CinemaSite>> SitesTask;
    }
}
