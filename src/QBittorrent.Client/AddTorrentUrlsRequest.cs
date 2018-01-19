using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QBittorrent.Client
{
    public class AddTorrentUrlsRequest : AddTorrentRequest
    {
        public AddTorrentUrlsRequest() : this(Enumerable.Empty<Uri>())
        {
        }

        public AddTorrentUrlsRequest(IEnumerable<Uri> urls)
        {
            TorrentUrls = new List<Uri>(urls);
        }

        /// <summary>
        /// The list of torrent torrentFiles.
        /// </summary>
        public ICollection<Uri> TorrentUrls { get; }
    }
}
