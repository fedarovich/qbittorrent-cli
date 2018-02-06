using System;

namespace QBittorrent.Client
{
    [Flags]
    public enum TorrentLogSeverity
    {
        Normal = 1,
        Info = 2,
        Warning = 4,
        Critical = 8,

        All = Normal | Info | Warning | Critical
    }
}
