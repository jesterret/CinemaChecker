using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CinemaChecker.Cinema.CinemaCity
{
    class SeanceInfo : Cinema.SeanceInfo
    {
        string _id;
        string _type;
        Movie _movie;
        List<Cinema.Seance> _seances = new List<Cinema.Seance>();

        [JsonConstructor]
        SeanceInfo(string code, string fn, string n, uint dur, string attr, List<JObject> BD)
        {
            _id = code;
            _type = attr;
            _movie = new Movie(code, fn, n, dur);
            foreach(var item in BD)
            {
                var Date = item["date"].Value<string>();
                var P = item["P"].ToArray();
                foreach (var pres in P)
                {
                    var Time = pres["time"].Value<string>();
                    var SeanceDate = DateTime.ParseExact($"{Date} {Time}", "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfoByIetfLanguageTag("pl-PL"));
                    if (SeanceDate < DateTime.Now || pres["sold"].Value<bool>())
                        continue;

                    //SeanceDate.ToString("dd.MM.yyyy, HH:mm", CultureInfo.GetCultureInfoByIetfLanguageTag("pl-PL"));
                    var seanceId = pres["code"].Value<string>();
                    var s = new Seance(seanceId, SeanceDate);
                    s.Attributes.AddRange(pres["attr"].Value<string>().Split(new[] { ',' }));
                    _seances.Add(s as Cinema.Seance);
                }
            }
        }

        public override string Id => _id;

        public override string Type => _type;

        public override bool Is2D => _type.Contains("2D");

        public override bool Is3D => _type.Contains("3D");

        public override bool IsDub => _type.Contains("DUB");

        public override bool IsSub => _type.Contains("SUB");

        public override Cinema.Movie Movie { get => _movie; set => _movie = value as Movie; }

        public override List<Cinema.Seance> Seances => _seances;
    }
}
