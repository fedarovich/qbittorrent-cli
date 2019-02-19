using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BencodeNET.Torrents;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct TorrentShareViewModel
    {
        private readonly TorrentPartialInfo _partialInfo;
        private readonly TorrentProperties _properties;

        public TorrentShareViewModel(TorrentProperties properties, TorrentPartialInfo wrappedObject)
        {
            _properties = properties ?? throw new ArgumentNullException(nameof(properties));
            _partialInfo = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Ratio")]
        public double Ratio => _properties.ShareRatio;

        [Display(Name = "Ratio limit")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public double? RatioLimit => _partialInfo.RatioLimit;

        [Display(Name = "Seeding time")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? SeedingTime => _properties.SeedingTime;

        [Display(Name = "Seeding time limit")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TimeSpan? SeedingTimeLimit => _partialInfo.SeedingTimeLimit;
    }
}
