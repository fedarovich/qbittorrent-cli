using System;
using Newtonsoft.Json;
using QBittorrent.Client.Converters;

namespace QBittorrent.Client
{
    public class TorrentLogItem
    {
        /// <summary>
        /// Message Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Message timestamp
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Log item severity.
        /// </summary>
        [JsonProperty("type")]
        public TorrentLogSeverity Severity { get; set; }
    }
}
