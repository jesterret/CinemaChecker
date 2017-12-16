using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    public abstract class Site
    {
        public abstract string Id { get; }

        public abstract string Name { get; }
    }
}
