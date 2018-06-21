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
    }
}
