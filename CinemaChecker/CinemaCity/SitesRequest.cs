using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.CinemaCity
{
    public class CinemaSite
    {
        const string SiteMovies = "https://cinema-city.pl/pgm-site?si={0}";

        [JsonProperty("n")]
        public string Name { get; private set; }
        [JsonProperty("id")]
        public int Id { get; private set; }

        public List<PresentationData> Presents()
        {
            if (Presentations == null)
            {
                Presentations = Program.GetRawStringAsync(string.Format(SiteMovies, Id)).ContinueWith(t =>
                {
                    return JsonConvert.DeserializeObject<List<PresentationData>>(t.Result);
                });
                Presentations.Wait();
            }
            return Presentations.Result;
        }
        
        Task<List<PresentationData>> Presentations;
    }
}
