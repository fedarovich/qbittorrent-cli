using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct QueueViewModel
    {
        private readonly Preferences _wrappedObject;

        public QueueViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Queueing enabled")]
        public bool? QueueingEnabled => _wrappedObject.QueueingEnabled;

        [Display(Name = "Maximum active downloads")]
        public int? MaxActiveDownloads => _wrappedObject.MaxActiveDownloads;

        [Display(Name = "Maximum active torrents")]
        public int? MaxActiveTorrents => _wrappedObject.MaxActiveTorrents;

        [Display(Name = "Maximum active uploads")]
        public int? MaxActiveUploads => _wrappedObject.MaxActiveUploads;

        [Display(Name = "Do not count slow torrents in these limits")]
        public bool? DoNotCountSlowTorrents => _wrappedObject.DoNotCountSlowTorrents;

        [Display(Name = "Slow download rate threshold")]
        [DisplayFormat(DataFormatString = "{0:N} KiB/s", NullDisplayText = "n/a")]
        public int? SlowTorrentDownloadRateThreshold => _wrappedObject.SlowTorrentDownloadRateThreshold;

        [Display(Name = "Slow upload rate threshold")]
        [DisplayFormat(DataFormatString = "{0:N} KiB/s", NullDisplayText = "n/a")]
        public int? SlowTorrentUploadRateThreshold => _wrappedObject.SlowTorrentUploadRateThreshold;

        [Display(Name = "Torrent inactivity timeout")]
        [DisplayFormat(DataFormatString = "{0:N} s", NullDisplayText = "n/a")]
        public int? SlowTorrentInactiveTime => _wrappedObject.SlowTorrentInactiveTime;
    }
}
