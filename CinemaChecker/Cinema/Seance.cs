using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class Seance
    {
        public abstract bool IsValid { get; }

        public abstract string Id { get; }

        public abstract DateTime Date { get; }

        public List<string> Attributes { get; } = new List<string>();
    }
}
