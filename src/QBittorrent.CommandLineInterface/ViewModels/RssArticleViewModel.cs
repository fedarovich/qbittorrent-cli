using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct RssArticleViewModel
    {
        private readonly RssArticle _wrappedObject;

        public RssArticleViewModel(RssArticle wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Title")]
        public string Title => _wrappedObject.Title;

        [Display(Name = "Id")]
        public string Id => _wrappedObject.Id;

        [Display(Name = "Date/Time")]
        public DateTimeOffset Date => _wrappedObject.Date;

        [Display(Name = "Author")]
        public string Author => _wrappedObject.Author;

        [Display(Name = "Description")]
        public string Description => _wrappedObject.Description;

        [Display(Name = "Torrent URI")]
        public Uri TorrentUri => _wrappedObject.TorrentUri;

        [Display(Name = "Link")]
        public Uri Link => _wrappedObject.Link;

        [Display(Name = "Is read?")]
        public bool IsRead => _wrappedObject.IsRead;
    }
}
