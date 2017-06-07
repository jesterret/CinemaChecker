using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CinemaChecker
{
    class CinemaListing
    {
        public class PresentationInfo
        {
            [JsonProperty("dt")]
            public string DateString { get; private set; }
            [JsonProperty("pc")]
            public int PresentationCode { get; private set; }
            [JsonProperty("tm")]
            private string Time { get; set; }
            [JsonProperty("dtticks")]
            public long DateTicks { get; private set; }
            [JsonProperty("db")]
            public bool IsDubbing { get; private set; }
            [JsonProperty("sb")]
            public bool IsSubTitles { get; private set; }
            [JsonProperty("td")]
            public bool Is3D { get; private set; }
            [JsonProperty("vt")]
            public int VenueType { get; private set; }
            [JsonProperty("at")]
            public int At { get; private set; }

            public DateTime Date => DateTime.ParseExact(DateString + ", " + Time, "dd/MM/yyyy dddd, HH:mm", new CultureInfo("pl-PL", true));
            public string ToUrl(long SiteID)
            {
                return $"https://www.cinema-city.pl/titanTicketing?siteId={SiteID}&presentationCode={PresentationCode}";
            }
            public string ToHtml(long SiteID)
            {
                return string.Format("<a href=\"{0}\">{1}{2}{3}</a>", ToUrl(SiteID), Date.ToString("ddd d, hh:mm"), VenueType == 2 ? " IMAX" : "" , Is3D ? " 3D" : "");
            }
        }
        public class FeatureInfo
        {
            [JsonProperty("pr")]
            public List<PresentationInfo> Presentations { get; private set; }
            [JsonProperty("dc")]
            public string DisplayCode { get; private set; }
            [JsonProperty("fn")]
            public string FeatureName { get; private set; }
        }
        public class CinemaSite
        {
            [JsonProperty("si")]
            public int SiteID { get; private set; }
            [JsonProperty("sn")]
            public string SiteName { get; private set; }
            [JsonProperty("fe")]
            public List<FeatureInfo> Features { get; private set; }
        }

        [JsonProperty("sites")]
        public List<CinemaSite> Sites { get; private set; }
        [JsonProperty("venueTypes")]
        public List<string> VenueTypes { get; private set; }
    }
}
