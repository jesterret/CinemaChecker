using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    public class Site : Cinema.Site
    {
        private string _id;
        private string _name;
        private string _location;

        [JsonConstructor]
        Site(string slug, string name, string location)
        {
            _id = slug;
            _name = name;
            _location = location;
        }

        public override string Id => _id;

        public override string Name => $"{_name} - {_location}";
    }
}
