using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common
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
    }
}
