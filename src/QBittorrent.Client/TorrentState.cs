using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QBittorrent.Client
{
    public enum TorrentState
    {
        /// <summary>
        /// Some error occurred, applies to paused torrents
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Torrent is paused and has finished downloading
        /// </summary>
        [EnumMember(Value = "pausedUP")]
        PausedUpload,

        /// <summary>
        /// Torrent is paused and has NOT finished downloading
        /// </summary>
        [EnumMember(Value = "pausedDL")]
        PausedDownload,

        /// <summary>
        /// Queuing is enabled and torrent is queued for upload
        /// </summary>
        [EnumMember(Value = "queuedUP")]
        QueuedUpload,

        /// <summary>
        /// Queuing is enabled and torrent is queued for download
        /// </summary>
        [EnumMember(Value = "queuedDL")]
        QueuedDownload,

        /// <summary>
        /// Torrent is being seeded and data is being transferred
        /// </summary>
        [EnumMember(Value = "uploading")]
        Uploading,

        /// <summary>
        /// Torrent is being seeded, but no connection were made
        /// </summary>
        [EnumMember(Value = "stalledUP")]
        StalledUpload,

        /// <summary>
        /// Torrent has finished downloading and is being checked; 
        /// this status also applies to preallocation (if enabled) and checking resume data on qBt startup
        /// </summary>
        [EnumMember(Value = "checkingUP")]
        CheckingUpload,

        /// <summary>
        /// Torrent is being checked
        /// </summary>
        [EnumMember(Value = "checkingDL")]
        CheckingDownload,

        /// <summary>
        /// Torrent is being downloaded and data is being transferred
        /// </summary>
        [EnumMember(Value = "downloading")]
        Downloading,

        /// <summary>
        /// Torrent is being downloaded, but no connection were made
        /// </summary>
        [EnumMember(Value = "stalledDL")]
        StalledDownload,
        
        /// <summary>
        /// Torrent has just started downloading and is fetching metadata
        /// </summary>
        [EnumMember(Value = "metaDL")]
        FetchingMetadata
    }
}
