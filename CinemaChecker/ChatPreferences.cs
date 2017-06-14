using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CinemaChecker
{
    public class PreferenceWeights : System.IComparable
    {
        int WeightIMAX = 0;
        int Weight4DX = 0;
        int Weight3D = 0;
        int WeightSubs = 0;
        int WeightDub = 0;
        public int CompareTo(object obj)
        {
            return 0;
        }
    }
    public class ChatPreferences
    {
        public ChatPreferences()
        {

        }

        public void Add(long siteId)
        {
            TrackedSites.Add(siteId);
        }
        public void Add(Regex titleRegex)
        {
            TrackedTitles.Add(titleRegex);
        }
        public bool Contains(long siteId)
        {
            return TrackedSites.Contains(siteId);
        }
        public bool Contains(Regex titleRegex)
        {
            return TrackedTitles.Contains(titleRegex);
        }
        public HashSet<long> GetSites()
        {
            return TrackedSites;
        }

        public PreferenceWeights Weights;
        HashSet<long> TrackedSites = new HashSet<long>();
        HashSet<Regex> TrackedTitles = new HashSet<Regex>();
    }
}
