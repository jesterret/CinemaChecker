using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    class Seance : Cinema.Seance
    {
        bool _valid;
        string _id;
        DateTime _date;

        [JsonConstructor]
        Seance(bool active, string guid, DateTime start)
        {
            _valid = active;
            _id = guid;
            _date = start;
        }

        public override DateTime Date => _date;

        public override string Id => _id;

        public override bool IsValid => _valid;
    }
}
