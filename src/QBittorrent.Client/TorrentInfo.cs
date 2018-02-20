using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QBittorrent.Client
{
    /// <summary>
    /// Represents main information about torrent.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class TorrentInfo
    {
        /// <summary>
        /// Torrent hash
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Torrent name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Total size (bytes) of files selected for download
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// Torrent progress (percentage/100)
        /// </summary>
        [JsonProperty("progress")]
        public double Progress { get; set; }

        /// <summary>
        /// Torrent download speed (bytes/s)
        /// </summary>
        [JsonProperty("dlspeed")]
        public long DownloadSpeed { get; set; }

        /// <summary>
        /// Torrent upload speed (bytes/s)
        /// </summary>
        [JsonProperty("upspeed")]
        public long UploadSpeed { get; set; }

        /// <summary>
        /// Torrent priority. Returns -1 if queuing is disabled or torrent is in seed mode
        /// </summary>
        [JsonProperty("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Number of seeds connected to
        /// </summary>
        [JsonProperty("num_seeds")]
        public int ConnectedSeeds { get; set; }

        /// <summary>
        /// Number of seeds in the swarm
        /// </summary>
        [JsonProperty("num_complete")]
        public int TotalSeeds { get; set; }

        /// <summary>
        /// Number of leechers connected to
        /// </summary>
        [JsonProperty("num_leechs")]
        public int ConnectedLeechers { get; set; }

        /// <summary>
        /// Number of leechers in the swarm
        /// </summary>
        [JsonProperty("num_incomplete")]
        public int TotalLeechers { get; set; }

        /// <summary>
        /// Torrent share ratio. Max ratio value: 9999.
        /// </summary>
        [JsonProperty("ratio")]
        public double Ratio { get; set; }

        /// <summary>
        /// Torrent ETA (seconds)
        /// </summary>
        [JsonProperty("eta")]
        public int EstimatedTime { get; set; }

        /// <summary>
        /// Torrent state
        /// </summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TorrentState State { get; set; }

        /// <summary>
        /// True if sequential download is enabled
        /// </summary>
        [JsonProperty("seq_dl")]
        public bool SequentialDownload { get; set; }

        /// <summary>
        /// True if first last piece are prioritized
        /// </summary>
        [JsonProperty("f_l_piece_prio")]
        public bool FirstLastPiecePrioritized { get; set; }

        /// <summary>
        /// Category of the torrent
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// True if super seeding is enabled
        /// </summary>
        [JsonProperty("super_seeding")]
        public bool SuperSeeding { get; set; }

        /// <summary>
        /// True if force start is enabled for this torrent
        /// </summary>
        [JsonProperty("force_start")]
        public bool ForceStart { get; set; }
    }
}
