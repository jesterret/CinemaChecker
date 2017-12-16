using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class UpcomingMovie : Movie
    {
        public abstract DateTime Premiere { get; }
    }
}
