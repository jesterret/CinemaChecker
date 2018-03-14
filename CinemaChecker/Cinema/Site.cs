using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema
{
    [DebuggerDisplay("{Id}", Name = "{Name}")]
    public abstract class Site
    {
        public abstract string Id { get; }

        public abstract string Name { get; }
    }
}
