using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.CinemaCity
{
    public class Site : Cinema.Site
    {
        private string _id;
        private string _name;

        [JsonConstructor]
        Site(string id, string n)
        {
            _id = id;
            _name = n;
        }
        public override string Id => _id;

        public override string Name => _name;
    }
}
