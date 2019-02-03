using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct RssFeedViewModel
    {
        private readonly RssFeed _wrappedObject;

        public RssFeedViewModel(string path, RssFeed wrappedObject)
        {
            Path = path;
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Path")]
        public string Path { get; }

        [Display(Name = "Name")]
        public string Name => _wrappedObject.Name;

        [Display(Name = "Title")]
        public string Title => _wrappedObject.Title;

        [Display(Name = "UID")]
        public Guid Uid => _wrappedObject.Uid;

        [Display(Name = "URL")]
        public Uri Url => _wrappedObject.Url;

        [Display(Name = "Last build date")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTimeOffset? LastBuildDate => _wrappedObject.LastBuildDate;

        [Display(Name = "Is loading")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? IsLoading => _wrappedObject.IsLoading;

        [Display(Name = "Has error")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? HasError => _wrappedObject.HasError;

        [Display(Name = "Articles")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public IEnumerable<RssArticleViewModel> Articles =>
            _wrappedObject.Articles?.Select(a => new RssArticleViewModel(a));
    }
}
