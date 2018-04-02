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

        [Display(Name = "Message")]
        public string Message => _wrappedObject.Message;
    }
}
