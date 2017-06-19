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
        public List<Site> GetSites()
        {
            return Sites.Get();
        }
        public List<Poster> GetPosters()
        {
            return Posters.Get().Posters;
        }
        public List<SiteFeatures>  GetFeatured(string SeanceID = null)
        {
            FeaturingSitesList req = null;
            if (SeanceID != null)
                req = new FeaturingSitesList(SeanceID);
            else
                req = new FeaturingSitesList();

            return req.Get();
        }

        public List<SeanceInfo> GetCinemaSeances(long SiteID)
        {
            var list = new SeanceList(SiteID);
            return list.Get();
        }

        PosterList Posters = new PosterList();
        SiteList Sites = new SiteList();
    }
}
