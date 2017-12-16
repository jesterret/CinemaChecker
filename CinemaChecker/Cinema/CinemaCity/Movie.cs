using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.CinemaCity
{
    class Movie : Cinema.Movie
    {
        public override string Id => _id;
        public override string PosterImage => _image;
        public override string Title => _title;
        public override uint Duration => _duration;

        [JsonConstructor]
        internal Movie(string code, string fn, string n, uint dur)
        {
            _id = code;
            _image = "https://www.cinema-city.pl/xmedia-cw/repo/feats/posters/" + fn;
            _title = n;
            _duration = dur;
        }
        
        private string _id;
        private string _image;
        private string _title;
        private uint _duration;
    }
}
