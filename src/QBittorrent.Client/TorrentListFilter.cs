namespace QBittorrent.Client
{
    /// <summary>
    /// The status filter for the torrent list.
    /// </summary>
    public enum TorrentListFilter
    {
        /// <summary>
        /// All torrents
        /// </summary>
        All,

        /// <summary>
        /// The torrents being downloaded
        /// </summary>
        Downloading,
        
        /// <summary>
        /// The completed torrents
        /// </summary>
        Completed,

        /// <summary>
        /// The paused torrents
        /// </summary>
        Paused,

        /// <summary>
        /// The active torrents
        /// </summary>
        Active,

        /// <summary>
        /// The inactive torrents
        /// </summary>
        Inactive
    }
}
