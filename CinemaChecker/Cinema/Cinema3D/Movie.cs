using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    class Movie : Cinema.Movie
    {
        public override string Id => _id;
        public override string PosterImage => _image;
        public override string Title => _title;
        public override uint Duration => _duration;

        [JsonConstructor]
        Movie(string slug, string imageFile, string title, uint duration)
        {
            _id = slug;
            _image = imageFile;
            _title = title;
            _duration = duration;
        }
        
        private string _id;
        private string _image;
        private string _title;
        private uint _duration;
    }
}
