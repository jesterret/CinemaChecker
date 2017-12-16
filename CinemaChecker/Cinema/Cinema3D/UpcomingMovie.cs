using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    public class UpcomingMovie : Cinema.UpcomingMovie
    {
        DateTime _premiere;
        string _id;
        string _image;
        string _title;

        [JsonConstructor]
        UpcomingMovie(string slug, string trailerFile, string title, DateTime premiere)
        {
            _id = slug;
            _image = trailerFile;
            _title = title;
            _premiere = premiere;
        }

        public override string Id => _id;
        public override string Title => _title;
        public override string PosterImage => _image;
        public override DateTime Premiere => _premiere;

        public override uint Duration => 0;
    }
}
