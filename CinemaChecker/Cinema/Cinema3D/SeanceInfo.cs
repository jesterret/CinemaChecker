using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker.Cinema.Cinema3D
{
    class SeanceInfo : Cinema.SeanceInfo
    {
        string _id;
        string _type;
        string _language;
        Movie _movie;
        List<Seance> _seances;

        [JsonConstructor]
        SeanceInfo(string guid, string audio, string type, string language, List<Seance> seances)
        {
            _id = guid;
            _type = type;
            _language = language;
            _seances = seances.Where(s => s.IsValid).ToList();
            _seances.ForEach(s =>
            {
                s.Attributes.Add(audio);
                s.Attributes.Add(_type);
                s.Attributes.Add(IsSub ? "SUB" : "DUB"); // To keep it consistent between C3D & CC
            });
        }

        public override string Id => _id;

        public override string Type => string.Format("{0} {1}", _language , _type);

        public override bool Is2D => _type == "2D";

        public override bool Is3D => _type == "3D";

        public override bool IsDub => _language == "Dubbing";

        public override bool IsSub => _language == "Napisy";

        public override Cinema.Movie Movie { get => _movie; set => _movie = value as Movie; }
        
        public override List<Cinema.Seance> Seances => _seances.Cast<Cinema.Seance>().ToList();
    }
}
