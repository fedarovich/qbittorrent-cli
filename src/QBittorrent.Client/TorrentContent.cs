using Newtonsoft.Json;
using QBittorrent.Client.Converters;

namespace QBittorrent.Client
{
    /// <summary>
    /// Represents torrent content file info.
    /// </summary>
    public class TorrentContent
    {
        /// <summary>
        /// File name (including relative path)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// File progress (percentage/100)
        /// </summary>
        [JsonProperty("progress")]
        public double Progress { get; set; }

        /// <summary>
        /// File priority
        /// </summary>
        [JsonProperty("priority")]
        public TorrentContentPriority Priority { get; set; }

        /// <summary>
        /// True if file is seeding/complete
        /// </summary>
        [JsonProperty("is_seed")]
        public bool IsSeeding { get; set; }

        /// <summary>
        /// The range of the file.
        /// </summary>
        [JsonProperty("piece_range")]
        [JsonConverter(typeof(ArrayToRangeConverter))]
        public Range PieceRange { get; set; }
    }
}
