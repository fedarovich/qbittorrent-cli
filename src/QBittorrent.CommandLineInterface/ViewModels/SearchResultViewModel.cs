using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct SearchResultViewModel
    {
        private readonly SearchResult _wrappedObject;

        public SearchResultViewModel(SearchResult wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "File name")]
        public string FileName => _wrappedObject.FileName;

        [Display(Name = "File size")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public long? FileSize => _wrappedObject.FileSize;

        [Display(Name = "File URL")]
        public Uri FileUrl => _wrappedObject.FileUrl;

        [Display(Name = "Leechers")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public long? Leechers => _wrappedObject.Leechers;

        [Display(Name = "Seeds")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public long? Seeds => _wrappedObject.Seeds;

        [Display(Name = "Torrent site")]
        public Uri SiteUrl => _wrappedObject.SiteUrl;

        [Display(Name = "Description")]
        public Uri DescriptionUrl => _wrappedObject.DescriptionUrl;
    }
}
