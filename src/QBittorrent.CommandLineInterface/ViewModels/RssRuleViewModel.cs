using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct RssRuleViewModel
    {
        private readonly RssAutoDownloadingRule _wrappedObject;

        public RssRuleViewModel(string name, RssAutoDownloadingRule wrappedObject)
        {
            Name = name;
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Name", Order = -1)]
        public string Name { get; }

        [Display(Name = "Enabled")]
        public bool Enabled => _wrappedObject.Enabled;

        [Display(Name = "Must contain")]
        public string MustContain => _wrappedObject.MustContain;

        [Display(Name = "Must not contain")]
        public string MustNotContain => _wrappedObject.MustNotContain;

        [Display(Name = "Use regular expression")]
        public bool UseRegex => _wrappedObject.UseRegex;

        [Display(Name = "Episode filter")]
        public string EpisodeFilter => _wrappedObject.EpisodeFilter;

        [Display(Name = "Use smart filter")]
        public bool SmartFilter => _wrappedObject.SmartFilter;

        [Display(Name = "Previously matched episodes")]
        public IReadOnlyList<string> PreviouslyMatchedEpisodes => _wrappedObject.PreviouslyMatchedEpisodes;

        [Display(Name = "Affected RSS feeds")]
        public IReadOnlyList<Uri> AffectedFeeds => _wrappedObject.AffectedFeeds;

        [Display(Name = "Ignore days")]
        public int IgnoreDays => _wrappedObject.IgnoreDays;

        [Display(Name = "Last match")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public DateTimeOffset? LastMatch => _wrappedObject.LastMatch;

        [Display(Name = "Add paused")]
        [DisplayFormat(NullDisplayText = "Auto")]
        public bool? AddPaused => _wrappedObject.AddPaused;

        [Display(Name = "Assigned category")]
        public string AssignedCategory => _wrappedObject.AssignedCategory;

        [Display(Name = "Save path")]
        public string SavePath => _wrappedObject.SavePath;
    }
}
