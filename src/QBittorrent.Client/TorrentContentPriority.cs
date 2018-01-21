using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.Client
{
    public enum TorrentContentPriority
    {
        Skip = 0,
        Normal = 1,
        High = 6,
        Maximal = 7
    }
}
