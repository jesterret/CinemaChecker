using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    class SeanceList
    {
        const string SiteMovies = "https://cinema-city.pl/pgm-site?si={0}";

        internal Task<List<SeanceInfo>> SeanceInfoList;
        public SeanceList(long SiteID)
        {
            SeanceInfoList = Program.GetRawStringAsync(string.Format(SiteMovies, SiteID))
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<SeanceInfo>>(t.Result);
                });
        }

        public List<SeanceInfo> Get()
        {
            SeanceInfoList.Wait();
            return SeanceInfoList.Result;
        }
    }
}
