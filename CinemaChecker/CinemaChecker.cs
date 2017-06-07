using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaChecker
{
    class CinemaChecker
    {
        const string MovieListUrl = "https://www.cinema-city.pl/loadFunction?layoutId=10&layerId=1&exportCode=movies_filter";
        const string PresentationUrl = "https://www.cinema-city.pl/presentationsJSON";
        const string PortfolioXPath = "//*[@id='categoryfeatures_portfolio']";
        const string NodeClass = ".//li[contains(@class,'cat_0')]/a[@class ='featureInfo']";

        CinemaListing listings = null;

        public CinemaChecker()
        {
            PresentationTask = Program.GetRawStringAsync(PresentationUrl);
            MovieListTask = Program.GetRawStringAsync(MovieListUrl);
        }

        public IEnumerable<MovieInfo> GetMovies()
        {
            var Page = new HtmlAgilityPack.HtmlDocument();
            Page.LoadHtml(MovieListTask.Result);
            var node = Page.DocumentNode.SelectSingleNode(PortfolioXPath);
            var tests = node?.SelectNodes(NodeClass);
            foreach (var test in tests)
            { 
                yield return new MovieInfo(test);
            }
        }

        private void Up()
        {
            if (listings == null)
                listings = JsonConvert.DeserializeObject<CinemaListing>(PresentationTask.Result);
        }

        public List<CinemaListing.CinemaSite> GetSites()
        {
            Up();
            return listings.Sites;
        }

        public CinemaListing GetData()
        {
            Up();
            return listings;
        }
        
        Task<string> MovieListTask, PresentationTask;
    }
}
