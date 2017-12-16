using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.CinemaCity
{
    class UpcomingMovie : Cinema.UpcomingMovie
    {
        DateTime _premiere = new DateTime();
        private string _id;
        private string _image;
        private string _title;

        [JsonConstructor]
        UpcomingMovie(string code, string fn, string n)
        {
            _id = code;
            _image = "https://www.cinema-city.pl/xmedia-cw/repo/feats/posters/" + fn;
            _title = n;
        }

        public override string Id => _id;
        public override string Title => _title;
        public override string PosterImage => _image;
        public override DateTime Premiere => _premiere;

        public override uint Duration => 0;
    }
}
