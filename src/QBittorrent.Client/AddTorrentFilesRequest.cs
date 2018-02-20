using System.Collections.Generic;
using System.Linq;

namespace QBittorrent.Client
{
    /// <summary>
    /// Request to add new torrents using torrent files.
    /// </summary>
    /// <seealso cref="QBittorrent.Client.AddTorrentRequest" />
    public class AddTorrentFilesRequest : AddTorrentRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddTorrentFilesRequest"/> class.
        /// </summary>
        public AddTorrentFilesRequest() : this(Enumerable.Empty<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddTorrentFilesRequest"/> class.
        /// </summary>
        /// <param name="torrentFiles">The torrent files' paths.</param>
        public AddTorrentFilesRequest(IEnumerable<string> torrentFiles)
        {
            TorrentFiles = new List<string>(torrentFiles);
        }

        /// <summary>
        /// The list of torrent files' paths to add.
        /// </summary>
        public ICollection<string> TorrentFiles { get; }
    }
}
