using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class Movie
    {
        public abstract string Id { get; }

        public abstract string Title { get; }

        public abstract string PosterImage { get; }

        public abstract uint Duration { get; }
    }
}
