using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Caching;

namespace CinemaChecker.Cinema.CinemaCity
{
    class CinemaCity : CinemaBase
    {
        public override string CinemaCode => "CC";


        string API_SitesEndpoint => "https://www.cinema-city.pl/pgm-sites";

        string API_RepertoireEndpoint => "https://www.cinema-city.pl/pgm-list-byfeat?si={0}&max=365";

        string API_UpcomingEndpoint => "https://www.cinema-city.pl/pgm-feats?ft=2&si=0&vt=0&udays=365&pi=0";

        string API_SeancesEndpoint => "https://www.cinema-city.pl/pgm-list-byfeat?si={0}&code={1}&max=365";

        public override async Task<List<Cinema.Site>> GetSites()
        {
            var request = API_SitesEndpoint;
            var ret = MemoryCache.Default.Get(request);
            if (ret == null)
            {
                var items = await Program.GetRawStringAsync(request)
                .ContinueWith(t => JsonConvert.DeserializeObject<List<Site>>(t.Result).Cast<Cinema.Site>().ToList());
                MemoryCache.Default.Add(request, items, new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                });
                return items;
            }
            else return ret as List<Cinema.Site>;
        }

        public override async Task<List<Cinema.Movie>> GetRepertoir(string Id)
        {
            try
            {
                var site = (await GetSites()).Single(s => s.Id == Id);
                return await GetRepertoir(site);
            }
            catch (InvalidOperationException) { }
            return new List<Cinema.Movie>();
        }

        public override async Task<List<Cinema.Movie>> GetRepertoir(Cinema.Site site)
        {
            var request = string.Format(API_RepertoireEndpoint, site.Id);
            var ret = MemoryCache.Default.Get(request);
            if (ret == null)
            {
                var items = await Program.GetRawStringAsync(request)
                .ContinueWith(t => JsonConvert.DeserializeObject<List<Movie>>(t.Result).Cast<Cinema.Movie>().ToList());
                MemoryCache.Default.Add(request, items, new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                });
                return items;
            }
            else return ret as List<Cinema.Movie>;
        }

        public override async Task<List<Cinema.UpcomingMovie>> GetUpcoming(string Id)
        {
            return await GetUpcoming(null as Site);
        }

        public override async Task<List<Cinema.UpcomingMovie>> GetUpcoming(Cinema.Site site)
        {
            var request = API_UpcomingEndpoint;
            var ret = MemoryCache.Default.Get(request);
            if (ret == null)
            {
                var items = await Program.GetRawStringAsync(request)
                .ContinueWith(t => JsonConvert.DeserializeObject<List<UpcomingMovie>>(t.Result).Cast<Cinema.UpcomingMovie>().ToList());
                MemoryCache.Default.Add(request, items, new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                });
                return items;
            }
            else return ret as List<Cinema.UpcomingMovie>;
        }

        public override async Task<List<Cinema.SeanceInfo>> FindSeance(string SiteId, string MovieId)
        {
            try
            {
                var site = (await GetSites()).Single(s => s.Id == SiteId);
                var movie = (await GetRepertoir(site)).Single(s => s.Id == MovieId);
                return await FindSeance(site, movie);
            }
            catch (InvalidOperationException) { }
            return new List<Cinema.SeanceInfo>();
        }

        public override async Task<List<Cinema.SeanceInfo>> FindSeance(Cinema.Site site, Cinema.Movie movie)
        {
            var request = string.Format(API_SeancesEndpoint, site.Id, movie.Id);
            var ret = MemoryCache.Default.Get(request);
            if (ret == null)
            {
                var items = await Program.GetRawStringAsync(request)
                    .ContinueWith(t => JsonConvert.DeserializeObject<List<SeanceInfo>>(t.Result).Cast<Cinema.SeanceInfo>().ToList());
                MemoryCache.Default.Add(request, items, new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                });
                return items;
            }
            else return ret as List<Cinema.SeanceInfo>;
        }

        public override async Task<Cinema.Movie> GetMovie(string SiteId, string MovieId)
        {
            try
            {
                var site = (await GetSites()).Single(s => s.Id == SiteId);
                var movie = (await GetRepertoir(site)).Single(s => s.Id == MovieId);
                return movie;
            }
            catch (InvalidOperationException) { }
            return null;
        }
    }
}
