using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using QBittorrent.Client.Converters;

namespace QBittorrent.Client
{
    /// <summary>
    /// Represents the torrent tracker information.
    /// </summary>
    public class TorrentTracker
    {
        /// <summary>
        /// Tracker URL
        /// </summary>
        [JsonProperty("url")]
        [Display(Name = "URL")]
        public Uri Url { get; set; }

        /// <summary>
        /// Tracker status (translated string)
        /// </summary>
        [JsonProperty("status")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Number of peers for current torrent reported by the tracker
        /// </summary>
        [JsonProperty("num_peers")]
        [Display(Name = "Peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? Peers { get; set; }

        /// <summary>
        /// Tracker message (there is no way of knowing what this message is - it's up to tracker admins)
        /// </summary>
        [JsonProperty("msg")]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}
