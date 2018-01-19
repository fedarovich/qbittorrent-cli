using System.Collections.Generic;
using System.Linq;

namespace QBittorrent.Client
{
    public class AddTorrentFilesRequest : AddTorrentRequest
    {
        public AddTorrentFilesRequest() : this(Enumerable.Empty<string>())
        {
        }

        public AddTorrentFilesRequest(IEnumerable<string> torrentFiles)
        {
            TorrentFiles = new List<string>(torrentFiles);
        }

        /// <summary>
        /// The list of torrent torrentFiles.
        /// </summary>
        public ICollection<string> TorrentFiles { get; }
    }
}
