using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    public class Cinema3D : CinemaBase
    {
        public override string CinemaCode => "C3";

        
        string API_SitesEndpoint => "http://system.v67926.tld.pl/api/v1/cinema";

        string API_RepertoireEndpoint => "http://system.v67926.tld.pl/api/v1/movies/now/{0}";

        string API_UpcomingEndpoint => "http://system.v67926.tld.pl/api/v1/movies/teasers/{0}";

        string API_SeancesEndpoint => "http://system.v67926.tld.pl/api/v1/movie/{1}/seances/{0}";

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
            try
            {
                var site = (await GetSites()).Single(s => s.Id == Id);
                return await GetUpcoming(site);
            }
            catch (InvalidOperationException) { }
            return new List<Cinema.UpcomingMovie>();
        }

        public override async Task<List<Cinema.UpcomingMovie>> GetUpcoming(Cinema.Site site)
        {
            var request = string.Format(API_UpcomingEndpoint, site.Id);
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
                items.ForEach(it => it.Movie = movie);
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
