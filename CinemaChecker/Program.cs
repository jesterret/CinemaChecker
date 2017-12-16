using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaChecker
{
    class Program
    {
        public static ManualResetEvent ShouldQuit = new ManualResetEvent(false);
        static System.Timers.Timer CheckerTimer = new System.Timers.Timer(1000 * 60 * 60);

        static public string GetRawString(string Url)
        {
            return new WebClient()
            {
                Encoding = Encoding.UTF8
            }.DownloadString(Url);
        }
        static public Task<string> GetRawStringAsync(string Url)
        {
            return new WebClient()
            {
                Encoding = Encoding.UTF8
            }.DownloadStringTaskAsync(Url);

        }
        static public Task<string> GetRawStringAsyncNoCache(string Url)
        {
            return new WebClient()
            {
                Encoding = Encoding.UTF8
            }.DownloadStringTaskAsync(Url);
        }

        public static async void _testcinema<T>() where T : Cinema.CinemaBase
        {
            var c = Activator.CreateInstance<T>();
            var x = await c.GetSites();
            var site = x[0];
            var y = await c.GetRepertoir(site);
            var seances = await c.FindSeance(site, y[0]);
            var z = await c.GetUpcoming(site);
            return;
        }
        public static async void _testcc()
        {
            var c = new Cinema.CinemaCity.CinemaCity();
            var x = await c.GetSites();
            var site = x.Single(s => s.Name.Contains("Galeria Plaza"));
            var y = await c.GetRepertoir(site);
            var seances = await c.FindSeance(site, y[0]);
            var z = await c.GetUpcoming(site);
            return;
        }
        public static async void _testc3d()
        {
            var c = new Cinema.Cinema3D.Cinema3D();
            var x = await c.GetSites();
            var site = x.Single(s => s.Id.Contains("tarnow"));
            var y = await c.GetRepertoir(site);
            var seances = await c.FindSeance(site, y[0]);
            var z = await c.GetUpcoming(site);
            return;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            //_testcc();
            //_testc3d();
            using (var manager = new TelegramManager())
            {
                ShouldQuit.WaitOne();
                CheckerTimer.Elapsed += manager.Check;
            }
            Thread.Sleep(5000);
            return;
        }
    }
}
