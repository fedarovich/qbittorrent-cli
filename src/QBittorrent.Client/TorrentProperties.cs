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
        [JsonProperty("save_path")]
        [Display(Name = "Save path")]
        public string SavePath { get; set; }

        [JsonProperty("creation_date")]
        [JsonConverter(typeof(UnixTimeToDateTimeOffsetConverter))]
        [Display(Name = "Created at")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTimeOffset? CreationDate { get; set; }

        [JsonProperty("piece_size")]
        [Display(Name = "Piece size")]
        [DisplayFormat(DataFormatString = "{0:N} bytes")]
        public long PieceSize { get; set; }

        [JsonProperty("comment")]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
    }
}
