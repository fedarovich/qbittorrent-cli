using System.Collections.Generic;
using System.Linq;

namespace QBittorrent.Client
{
    public class DownloadWithTorrentFilesRequest : DownloadRequest
    {
        public DownloadWithTorrentFilesRequest() : this(Enumerable.Empty<string>())
        {
        }

        public DownloadWithTorrentFilesRequest(IEnumerable<string> torrentFiles)
        {
            TorrentFiles = new List<string>(torrentFiles);
        }

        /// <summary>
        /// The list of torrent torrentFiles.
        /// </summary>
        public ICollection<string> TorrentFiles { get; }
    }
}
