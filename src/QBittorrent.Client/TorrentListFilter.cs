using System;
using System.Collections.Generic;
using System.Text;

namespace QBittorrent.Client
{
    public enum TorrentListFilter
    {
        All,
        Downloading,
        Completed,
        Paused,
        Active,
        Inactive
    }
}
