using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;

namespace CinemaChecker
{
    class Program
    {
        const long PrivateChatID = 0x28d07dc;

        static Timer CheckerTimer = new Timer(1000 * 60 * 60);

        static public string GetRawString(string Url)
        {
            var cl = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            return cl.DownloadString(Url);
        }
        static public Task<string> GetRawStringAsync(string Url)
        {
            var cl = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            return cl.DownloadStringTaskAsync(Url);
        }

        static TelegramManager manager;
        
        static void Main(string[] args)
        {
            CheckerTimer.Elapsed += PerformCinemaCheck;
            manager = new TelegramManager();
            Console.ReadLine();
            return;
        }

        private static void PerformCinemaCheck(object sender, ElapsedEventArgs e)
        {
            manager.Check();
        }
    }
}
