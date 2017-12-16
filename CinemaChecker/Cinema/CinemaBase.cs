using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;

namespace CinemaChecker.Cinema
{
    public abstract class CinemaBase
    {
        public static Dictionary<string, CinemaBase> CinemaSystems => _cinemaSystems;
        public static CinemaBase GetCinema(string CinemaCode)
        {
            var code = CinemaCode ?? throw new ArgumentNullException(nameof(CinemaCode));
            if (CinemaSystems.Where(cin => cin.Key.Equals(code, StringComparison.OrdinalIgnoreCase)).Select(cin => cin.Value).SingleOrDefault() is CinemaBase system)
                return system;

            return null;
        }

        public abstract Task<List<Site>> GetSites();
        public abstract Task<List<Movie>> GetRepertoir(string Id);
        public abstract Task<List<Movie>> GetRepertoir(Site site);
        public abstract Task<List<UpcomingMovie>> GetUpcoming(string Id);
        public abstract Task<List<UpcomingMovie>> GetUpcoming(Site site);
        public abstract Task<List<SeanceInfo>> FindSeance(string SiteId, string MovieId);
        public abstract Task<List<SeanceInfo>> FindSeance(Site site, Movie movie);
        public abstract Task<Movie> GetMovie(string SiteId, string MovieId);

        public abstract string CinemaCode { get; }

        private readonly static Dictionary<string, CinemaBase> _cinemaSystems = typeof(CinemaBase).Assembly.GetTypes()
                                                                        .Where(t => t.IsSubclassOf(typeof(CinemaBase)) && !t.IsAbstract)
                                                                        .Select(t => Activator.CreateInstance(t) as CinemaBase)
                                                                        .ToDictionary(x => x.CinemaCode);
    }
}
