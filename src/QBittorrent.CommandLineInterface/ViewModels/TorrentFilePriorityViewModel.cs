using QBittorrent.Client;
using System;
using System.ComponentModel.DataAnnotations;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct TorrentFilePriorityViewModel
    {
        private readonly TorrentContent _wrappedObject;

        public TorrentFilePriorityViewModel(TorrentContent wrappedObject, int id)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
            Id = id;
        }

        [Display(Name = "Id")]
        public int Id { get; }

        [Display(Name = "Priority")]
        public TorrentContentPriority Priority => _wrappedObject.Priority;
    }
}
