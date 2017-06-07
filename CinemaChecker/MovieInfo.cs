using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker
{
    class MovieInfo : IEquatable<MovieInfo>
    {
        const string BaseUrl = "https://www.cinema-city.pl";
        const string TitleClass = ".//div[@class='featurePosterTitle']";
        const string PosterClass = ".//img[@class='catPoster']";
        const string PremiereDateClass = ".//div[@class='featurePrimer']";

        public MovieInfo(HtmlNode node)
        {
            Title = node.SelectSingleNode(TitleClass).InnerText;
            FeatureCode = node.GetAttributeValue("data-dcode", string.Empty);
            PremiereDateString = node.SelectSingleNode(PremiereDateClass).InnerText;
            ReservationUrl = new Uri(new Uri(BaseUrl), node.GetAttributeValue("href", string.Empty));
            PosterImage = node.SelectSingleNode(PosterClass).GetAttributeValue("data-src", string.Empty);
        }

        public Uri ReservationUrl { get; set; }
        public string Title { get; set; }
        public string PosterImage { get; set; }
        public string FeatureCode { get; set; }
        public string PremiereDateString { get; set; }
        public DateTime PremiereDate => DateTime.Parse(PremiereDateString);
        
        public bool Equals(MovieInfo other)
        {
            return Title == other.Title;
        }
        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}
