using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct TorrentPropertiesViewModel
    {
        private readonly TorrentProperties _wrappedObject;

        public TorrentPropertiesViewModel(TorrentProperties wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Save path")]
        public string SavePath => _wrappedObject.SavePath;

        [Display(Name = "Created at")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? CreationDate => _wrappedObject.CreationDate;

        [Display(Name = "Comment")]
        public string Comment => _wrappedObject.Comment;

        [Display(Name = "Size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? Size => _wrappedObject.Size;

        [Display(Name = "Total wasted")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? TotalWasted => _wrappedObject.TotalWasted;

        [Display(Name = "Total Uploaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? TotalUploaded => _wrappedObject.TotalUploaded;

        [Display(Name = "Total uploaded this session")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? TotalUploadedInSession => _wrappedObject.TotalUploadedInSession;

        [Display(Name = "Total downloaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? TotalDownloaded => _wrappedObject.TotalDownloaded;

        [Display(Name = "Total downloaded this session")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? TotalDownloadedInSession => _wrappedObject.TotalDownloadedInSession;

        [Display(Name = "Upload limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "None")]
        public long? UploadLimit => _wrappedObject.UploadLimit;

        [Display(Name = "Download limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "None")]
        public long? DownloadLimit => _wrappedObject.DownloadLimit;

        [Display(Name = "Tile elapsed")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? TimeElapsed => _wrappedObject.TimeElapsed;

        [Display(Name = "Seeding time")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? SeedingTime => _wrappedObject.SeedingTime;

        [Display(Name = "Connections")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? ConnectionCount => _wrappedObject.ConnectionCount;

        [Display(Name = "Connection Limit")]
        [DisplayFormat(NullDisplayText = "Unlimited")]
        public int? ConnectionLimit => _wrappedObject.ConnectionLimit;

        [Display(Name = "Share ratio")]
        [DisplayFormat(DataFormatString = "{0:F3}")]
        public double ShareRatio => _wrappedObject.ShareRatio;

        [Display(Name = "Addition date")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? AdditionDate => _wrappedObject.AdditionDate;

        [Display(Name = "Completion date")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? CompletionDate => _wrappedObject.CompletionDate;

        [Display(Name = "Created by")]
        public string CreatedBy => _wrappedObject.CreatedBy;

        [Display(Name = "Avg. download speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        public long? AverageDownloadSpeed => _wrappedObject.AverageDownloadSpeed;

        [Display(Name = "Download speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        public long? DownloadSpeed => _wrappedObject.DownloadSpeed;

        [Display(Name = "Avg. upload speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        public long? AverageUploadSpeed => _wrappedObject.AverageUploadSpeed;

        [Display(Name = "Upload speed")]
        [DisplayFormat(DataFormatString = "{0} bytes/s", NullDisplayText = "n/a")]
        public long? UploadSpeed => _wrappedObject.UploadSpeed;

        [Display(Name = "ETA")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? EstimatedTime => _wrappedObject.EstimatedTime;

        [Display(Name = "Last seen")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTime? LastSeen => _wrappedObject.LastSeen;

        [Display(Name = "Peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Peers => _wrappedObject.Peers;

        [Display(Name = "Total peers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? TotalPeers => _wrappedObject.TotalPeers;

        [Display(Name = "Seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? Seeds => _wrappedObject.Seeds;

        [Display(Name = "Total seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? TotalSeeds => _wrappedObject.TotalSeeds;

        [Display(Name = "Piece size")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? PieceSize => _wrappedObject.PieceSize;

        [Display(Name = "Owned pieces")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? OwnedPieces => _wrappedObject.OwnedPieces;

        [Display(Name = "Total pieces")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public int? TotalPieces => _wrappedObject.TotalPieces;

        [Display(Name = "Time until reannounce")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? Reannounce => _wrappedObject.Reannounce;
    }
}
