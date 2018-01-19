using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using QBittorrent.Client.Converters;

namespace QBittorrent.Client
{
    public class TorrentProperties
    {
        /// <summary>
        /// Torrent save path
        /// </summary>
        [JsonProperty("save_path")]
        [Display(Name = "Save path")]
        public string SavePath { get; set; }

        /// <summary>
        /// Torrent creation date
        /// </summary>
        [JsonProperty("creation_date")]
        [JsonConverter(typeof(UnixTimeToNullableDateTimeConverter))]
        [Display(Name = "Created at")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Torrent comment
        /// </summary>
        [JsonProperty("comment")]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Torrent total size (bytes)
        /// </summary>
        [JsonProperty("total_size")]
        [Display(Name = "Size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? Size { get; set; }

        /// <summary>
        /// Total data wasted for torrent (bytes)
        /// </summary>
        [JsonProperty("total_wasted")]
        [Display(Name = "Total wasted")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? TotalWasted { get; set; }

        /// <summary>
        /// Total data uploaded for torrent (bytes)
        /// </summary>
        [JsonProperty("total_uploaded")]
        [Display(Name = "Total Uploaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? TotalUploaded { get; set; }

        /// <summary>
        /// Total data uploaded this session (bytes)
        /// </summary>
        [JsonProperty("total_uploaded_session")]
        [Display(Name = "Total uploaded this session")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? TotalUploadedInSession { get; set; }

        /// <summary>
        /// Total data downloaded for torrent (bytes)
        /// </summary>
        [JsonProperty("total_downloaded")]
        [Display(Name = "Total downloaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? TotalDownloaded { get; set; }

        /// <summary>
        /// Total data downloaded this session (bytes)
        /// </summary>
        [JsonProperty("total_downloaded_session")]
        [Display(Name = "Total downloaded this session")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? TotalDownloadedInSession { get; set; }

        /// <summary>
        /// Torrent upload limit (bytes/s)
        /// </summary>
        [JsonProperty("up_limit")]
        [Display(Name = "Upload limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "None")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? UploadLimit { get; set; }

        /// <summary>
        /// Torrent download limit (bytes/s)
        /// </summary>
        [JsonProperty("dl_limit")]
        [Display(Name = "Download limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "None")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? DownloadLimit { get; set; }

        /// <summary>
        /// Torrent elapsed time
        /// </summary>
        [JsonProperty("time_elapsed")]
        [Display(Name = "Tile elapsed")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan? TimeElapsed { get; set; }

        /// <summary>
        /// Torrent elapsed time while complete
        /// </summary>
        [JsonProperty("seeding_time")]
        [Display(Name = "Seeding time")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan? SeedingTime { get; set; }

        /// <summary>
        /// Torrent connection count
        /// </summary>
        [JsonProperty("nb_connections")]
        [Display(Name = "Connections")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? ConnectionCount { get; set; }

        /// <summary>
        /// Torrent connection count limit
        /// </summary>
        [JsonProperty("nb_connections_limit")]
        [Display(Name = "Connection Limit")]
        [DisplayFormat(NullDisplayText = "Unlimited")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? ConnectionLimit { get; set; }

        /// <summary>
        /// Torrent share ratio
        /// </summary>
        [JsonProperty("share_ratio")]
        [Display(Name = "Share ratio")]
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double ShareRatio { get; set; }

        /// <summary>
        /// When this torrent was added
        /// </summary>
        [JsonProperty("addition_date")]
        [JsonConverter(typeof(UnixTimeToNullableDateTimeConverter))]
        [Display(Name = "Addition date")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? AdditionDate { get; set; }

        /// <summary>
        /// Torrent completion date
        /// </summary>
        [JsonProperty("completion_date ")]
        [JsonConverter(typeof(UnixTimeToNullableDateTimeConverter))]
        [Display(Name = "Completion date")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Torrent creator
        /// </summary>
        [JsonProperty("created_by")]
        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Torrent average download speed (bytes/second)
        /// </summary>
        [JsonProperty("dl_speed_avg")]
        [Display(Name = "Avg. download speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? AverageDownloadSpeed { get; set; }

        /// <summary>
        /// Torrent download speed (bytes/second)
        /// </summary>
        [JsonProperty("dl_speed")]
        [Display(Name = "Download speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? DownloadSpeed { get; set; }

        /// <summary>
        /// Torrent average upload speed (bytes/second)
        /// </summary>
        [JsonProperty("up_speed_avg")]
        [Display(Name = "Avg. upload speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? AverageUploadSpeed { get; set; }

        /// <summary>
        /// Torrent upload speed (bytes/second)
        /// </summary>
        [JsonProperty("up_speed")]
        [Display(Name = "Upload speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public long? UploadSpeed { get; set; }

        /// <summary>
        /// Torrent ETA
        /// </summary>
        [JsonProperty("eta")]
        [Display(Name = "ETA")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan? EstimatedTime { get; set; }

        /// <summary>
        /// Last seen complete date
        /// </summary>
        [JsonProperty("last_seen")]
        [Display(Name = "Last seen")]
        [JsonConverter(typeof(UnixTimeToNullableDateTimeConverter))]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? LastSeen { get; set; }

        /// <summary>
        /// Number of peers connected to
        /// </summary>
        [JsonProperty("peers")]
        [Display(Name = "Peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? Peers { get; set; }

        /// <summary>
        /// Number of peers in the swarm
        /// </summary>
        [JsonProperty("peers_total")]
        [Display(Name = "Total peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? TotalPeers { get; set; }

        /// <summary>
        /// Number of seeds connected to
        /// </summary>
        [JsonProperty("seeds")]
        [Display(Name = "Seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? Seeds { get; set; }

        /// <summary>
        /// Number of seeds in the swarm
        /// </summary>
        [JsonProperty("seeds_total")]
        [Display(Name = "Total seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? TotalSeeds { get; set; }

        /// <summary>
        /// Torrent piece size
        /// </summary>
        [JsonProperty("piece_size")]
        [Display(Name = "Piece size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? PieceSize { get; set; }

        /// <summary>
        /// Number of pieces owned
        /// </summary>
        [JsonProperty("pieces_have")]
        [Display(Name = "Owned pieces")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? OwnedPieces { get; set; }

        /// <summary>
        /// Number of pieces of the torrent
        /// </summary>
        [JsonProperty("pieces_num")]
        [Display(Name = "Total pieces")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(NegativeToNullConverter))]
        public int? TotalPieces { get; set; }

        /// <summary>
        /// Number of seconds until the next announce
        /// </summary>
        [JsonProperty("reannounce")]
        [Display(Name = "Time until reannounce")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [JsonConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan? Reannounce { get; set; }
    }
}
