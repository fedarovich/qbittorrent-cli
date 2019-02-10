using System;
using System.ComponentModel.DataAnnotations;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct TorrentTrackerViewModel
    {
        private readonly TorrentTracker _wrappedObject;

        public TorrentTrackerViewModel(TorrentTracker wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "URL")]
        public Uri Url => _wrappedObject.Url;

        [Display(Name = "Status")]
        public string Status => _wrappedObject.Status;

        [Display(Name = "Peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Peers => _wrappedObject.Peers;

        [Display(Name = "Seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Seeds => _wrappedObject.Seeds;

        [Display(Name = "Leeches")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Leeches => _wrappedObject.Leeches;

        [Display(Name = "Completed downloads")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? CompletedDownloads => _wrappedObject.CompletedDownloads;

        [Display(Name = "Message")]
        public string Message => _wrappedObject.Message;

        [Display(Name = "Tier")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Tier => _wrappedObject.Tier;
    }
}
