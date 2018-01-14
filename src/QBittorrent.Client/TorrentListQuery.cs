using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.Client
{
    public class TorrentListQuery
    {
        public TorrentListFilter Filter { get; set; } = TorrentListFilter.All;

        public string Category { get; set; }

        public string SortBy { get; set; }

        public bool ReverseSort { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }
    }
}
