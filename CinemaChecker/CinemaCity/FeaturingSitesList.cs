using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    class FeaturingSitesList
    {
        const string FeatureCodeUrl = "https://www.cinema-city.pl/pgm-feat?code={0}&max=365";
        const string FeatureUrl = "https://www.cinema-city.pl/pgm-feat?max=365";

        // Filtered information showing only the selected movie information
        public FeaturingSitesList(string SeanceID)
        {
            SeanceInfoList = Program.GetRawStringAsyncNoCache(string.Format(FeatureCodeUrl, SeanceID))
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<SiteFeatures>>(t.Result);
                });
        }
        // Full, unfiltered information about every featured movie by every site
        public FeaturingSitesList()
        {
            SeanceInfoList = Program.GetRawStringAsyncNoCache(FeatureUrl)
                .ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<SiteFeatures>>(t.Result);
                });
        }

        public List<SiteFeatures> Get()
        {
            SeanceInfoList.Wait();
            return SeanceInfoList.Result;
        }

        internal Task<List<SiteFeatures>> SeanceInfoList;
    }
}
