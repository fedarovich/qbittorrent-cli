using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
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
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [JsonProperty("size")]
        [Display(Name = "Size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes")]
        public long Size { get; set; }

        /// <summary>
        /// File progress (percentage/100)
        /// </summary>
        [JsonProperty("progress")]
        [Display(Name = "Progress")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double Progress { get; set; }

        /// <summary>
        /// File priority
        /// </summary>
        [JsonProperty("priority")]
        [Display(Name = "Priority")]
        public TorrentContentPriority Priority { get; set; }

        /// <summary>
        /// True if file is seeding/complete
        /// </summary>
        [JsonProperty("is_seed")]
        [Display(Name = "Seeding")]
        public bool IsSeeding { get; set; }

        /// <summary>
        /// The range of the file.
        /// </summary>
        [JsonProperty("piece_range")]
        [Display(Name = "Piece Range")]
        [JsonConverter(typeof(ArrayToRangeConverter))]
        public Range PieceRange { get; set; }
    }
}
