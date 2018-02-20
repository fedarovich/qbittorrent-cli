namespace QBittorrent.Client
{
    /// <summary>
    /// Encapsulates the query parameters to get the list of torrents.
    /// </summary>
    public class TorrentListQuery
    {
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        public TorrentListFilter Filter { get; set; } = TorrentListFilter.All;

        /// <summary>
        /// Gets or sets the category to filter by.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the field to sort by.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sorting must be performed in the descending order.
        /// </summary>
        public bool ReverseSort { get; set; }

        /// <summary>
        /// Gets or sets the maximal number of torrents to return.
        /// </summary>
        /// <seealso cref="Offset"/>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the offset from the beginning of the torrent list.
        /// </summary>
        /// <seealso cref="Limit" />
        public int? Offset { get; set; }
    }
}
