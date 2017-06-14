using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.Timers;

namespace CinemaChecker
{
    class Program
    {
        const long PrivateChatID = 0x28d07dc;
        const string Bot_SaySelectPreferedCinema = "Select prefered cinema site";

        static System.Timers.Timer CheckerTimer = new System.Timers.Timer(1000 * 60 * 60);
        static CinemaChecker checker = new CinemaChecker();
        static Dictionary<long, HashSet<long>> ChatTrackedSites = new Dictionary<long, HashSet<long>>();

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

        }
    }
}
