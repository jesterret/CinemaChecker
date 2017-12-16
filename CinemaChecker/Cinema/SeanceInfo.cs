using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class SeanceInfo
    {
        public abstract string Id { get; }

        public abstract string Type { get; }

        public abstract List<Seance> Seances { get; }

        public abstract Movie Movie { get; set; }

        public abstract bool Is2D { get; }

        public abstract bool Is3D { get; }

        public abstract bool IsSub { get; }

        public abstract bool IsDub { get; }
    }
}
