using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static T FirstOf<T, TKey>(this IEnumerable<T> original, Func<T, TKey> predicate)
        {
            return original.OrderBy(predicate).First();
        }

        public static T LastOf<T, TKey>(this IEnumerable<T> original, Func<T, TKey> predicate)
        {
            return original.OrderBy(predicate).Last();
        }
        
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
