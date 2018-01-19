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
        /// Torrent piece size
        /// </summary>
        [JsonProperty("piece_size")]
        [Display(Name = "Piece size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? PieceSize { get; set; }

        /// <summary>
        /// Torrent comment
        /// </summary>
        [JsonProperty("comment")]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

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
    }
}
