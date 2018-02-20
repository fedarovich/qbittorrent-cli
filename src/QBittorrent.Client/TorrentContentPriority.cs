namespace QBittorrent.Client
{
    /// <summary>
    /// Torrent content file download priority
    /// </summary>
    public enum TorrentContentPriority
    {
        /// <summary>
        /// Do not download the file.
        /// </summary>
        Skip = 0,
        
        /// <summary>
        /// The normal priority.
        /// </summary>
        Normal = 1,
        
        /// <summary>
        /// The high priority.
        /// </summary>
        High = 6,

        /// <summary>
        /// The maximal priority.
        /// </summary>
        Maximal = 7
    }
}
