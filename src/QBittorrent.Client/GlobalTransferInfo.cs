using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using QBittorrent.Client.Converters;

namespace QBittorrent.Client
{
    /// <summary>
    /// Represents global transfer info.
    /// </summary>
    public class GlobalTransferInfo
    {
        /// <summary>
        /// Global download rate (bytes/s)
        /// </summary>
        [JsonProperty("dl_info_speed")]
        [Display(Name = "Download speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? DownloadSpeed { get; set; }

        /// <summary>
        /// Data downloaded this session (bytes)
        /// </summary>
        [JsonProperty("dl_info_data")]
        [Display(Name = "Downloaded data")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? DownloadedData { get; set; }

        /// <summary>
        /// Download rate limit (bytes/s)
        /// </summary>
        [JsonProperty("dl_rate_limit")]
        [Display(Name = "Download speed limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? DownloadSpeedLimit { get; set; }

        /// <summary>
        /// Global upload rate (bytes/s)
        /// </summary>
        [JsonProperty("up_info_speed")]
        [Display(Name = "Upload speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? UploadSpeed { get; set; }

        /// <summary>
        /// Data uploaded this session (bytes)
        /// </summary>
        [JsonProperty("up_info_data")]
        [Display(Name = "Uploaded data")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? UploadedData { get; set; }

        /// <summary>
        /// Upload rate limit (bytes/s)
        /// </summary>
        [JsonProperty("up_rate_limit")]
        [Display(Name = "Upload speed limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? UploadSpeedLimit { get; set; }

        /// <summary>
        /// DHT nodes connected to
        /// </summary>
        [JsonProperty("dht_nodes")]
        [Display(Name = "DHT nodes")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? DhtNodes { get; set; }

        /// <summary>
        /// Connection status
        /// </summary>
        [JsonProperty("connection_status")]
        [Display(Name = "Connection status")]
        public ConnectionStatus? ConnectionStatus { get; set; }
    }
}
