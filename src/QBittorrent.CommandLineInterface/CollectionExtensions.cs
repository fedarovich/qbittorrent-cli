using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.CommandLineInterface
{
    public static class CollectionExtensions
    {
        public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }

#if NET46
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer) => new HashSet<T>(enumerable, comparer);
#endif
    }
}
