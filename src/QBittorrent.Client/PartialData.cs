using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace QBittorrent.Client
{
    public class PartialData
    {
        [JsonProperty("rid")]
        public int ResponseId { get; set; }

        [JsonProperty("full_update")]
        public bool FullUpdate { get; set; }

        [JsonProperty("torrents")]
        public IReadOnlyDictionary<string, TorrentPartialInfo> TorrentsChanged { get; set; }

        [JsonProperty("torrents_removed")]
        public IReadOnlyList<string> TorrentsRemoved { get; set; }

        [JsonProperty("categories")]
        public IReadOnlyList<string> CategoriesAdded { get; set; }

        [JsonProperty("categories_removed")]
        public IReadOnlyList<string> CategoriesRemoved { get; set; }

        [JsonProperty("queueing")]
        public bool Queueing { get; set; }

        [JsonProperty("server_state")]
        public GlobalTransferInfo ServerState { get; set; }
    }
}
