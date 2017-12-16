using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.CinemaCity
{
    class Seance : Cinema.Seance
    {
        bool _valid;
        string _id;
        DateTime _date;
        
        internal Seance(string Id, DateTime Date)
        {
            _id = Id;
            _date = Date;
            _valid = true;
        }

        public override DateTime Date => _date;

        public override string Id => _id;

        public override bool IsValid => _valid;
    }
}
