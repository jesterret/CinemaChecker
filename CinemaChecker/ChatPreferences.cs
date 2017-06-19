using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CinemaChecker
{
    public class PreferenceWeights : System.IComparable
    {
        public int WeightIMAX = 5;
        public int Weight4DX = 4;
        public int Weight3D = 3;
        public int WeightSubs = 2;
        public int WeightDub = 0;
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

        public bool Add(long siteId)
        {
            return Sites.Add(siteId);
        }
        public bool Add(Regex titleRegex)
        {
            return Titles.Add(titleRegex);
        }
        public bool Remove(long siteId)
        {
            return Sites.Remove(siteId);
        }
        public bool Remove(Regex titleRegex)
        {
            return Titles.Remove(titleRegex);
        }
        public bool Contains(long siteId)
        {
            return Sites.Contains(siteId);
        }
        public bool Contains(Regex titleRegex)
        {
            return Titles.Contains(titleRegex);
        }
        public bool Contains(string title)
        {
            foreach(var Title in Titles)
            {
                if (Title.IsMatch(title))
                    return true;
            }
            return false;
        }

        //        public PreferenceWeights Weights { get; } = new PreferenceWeights();
        public HashSet<long> Sites { get; } = new HashSet<long>();
        public HashSet<Regex> Titles { get; } = new HashSet<Regex>();
    }
}
