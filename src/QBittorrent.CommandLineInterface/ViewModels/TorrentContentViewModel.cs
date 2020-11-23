using System;
using System.ComponentModel.DataAnnotations;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct TorrentContentViewModel
    {
        private readonly TorrentContent _wrappedObject;

        public TorrentContentViewModel(TorrentContent wrappedObject, int id)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
            Id = id;
        }

        [Display(Name = "Id")]
        public int Id { get; }

        [Display(Name = "Name")]
        public string Name => _wrappedObject.Name;

        [Display(Name = "Size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes")]
        public long Size => _wrappedObject.Size;

        [Display(Name = "Progress")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public double Progress => _wrappedObject.Progress;

        [Display(Name = "Priority")]
        public TorrentContentPriority Priority => _wrappedObject.Priority;

        [Display(Name = "Seeding")]
        public bool IsSeeding => _wrappedObject.IsSeeding;

        [Display(Name = "Piece Range")]
        public Client.Range PieceRange => _wrappedObject.PieceRange;
    }
}
