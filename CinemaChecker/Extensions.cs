using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaChecker
{
    static class Extensions
    {
        /*
        var data = dt.Select(g => new
        {
            Season = g.season,
            AverageTemp = g.temp
        }).OrderByWeight(a => a.Season, x =>
        {
            if (x == "WINTER") return 1;
            if (x == "SPRING") return 2;
            if (x == "SUMMER") return 3;
            if (x == "AUTUMN") return 4;
            return 99;
        });
        */
        public static IOrderedEnumerable<TSource> OrderByWeight<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, int> weighting) where TKey : IComparable
        {
            Dictionary<TSource, int> order = new Dictionary<TSource, int>();
            foreach (TSource item in source)
            {
                if (!order.ContainsKey(item)) order.Add(item, weighting(keySelector(item)));
            }
            return source.OrderBy(s => order[s]);
        }
    }
}
