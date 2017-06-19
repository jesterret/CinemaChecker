using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public static IEnumerable<TResult> SelectTwo<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
        {
            return source.Select((item, index) => new { item, index })
            .GroupBy(x => x.index / 2)
            .Select(g => g.Select(i => i.item).ToArray())
            .Where(h => h.Count() == 2)
            .Select(a => selector(a[0], a[1]));
        }

        public static IEnumerable<T[]> Partition<T>(this IEnumerable<T> sequence, int partitionSize)
        {
            Contract.Requires(sequence != null);
            Contract.Requires(partitionSize > 0);

            var buffer = new T[partitionSize];
            var n = 0;
            foreach (var item in sequence)
            {
                buffer[n] = item;
                n += 1;
                if (n == partitionSize)
                {
                    yield return buffer;
                    buffer = new T[partitionSize];
                    n = 0;
                }
            }
            //partial leftovers
            if (n > 0)
            {
                var retbuf = new T[n];
                Array.Copy(buffer, retbuf, n);
                yield return retbuf;
            }
        }
    }
}
