using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct AutoTorrentManagementViewModel
    {
        private readonly Preferences _wrappedObject;

        public AutoTorrentManagementViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Auto TMM enabled by default")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? AutoTMMEnabledByDefault => _wrappedObject.AutoTMMEnabledByDefault;

        [Display(Name = "Retain Auto TMM when category changes")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? AutoTMMRetainedWhenCategoryChanges => _wrappedObject.AutoTMMRetainedWhenCategoryChanges;

        [Display(Name = "Retain Auto TMM when default save path changes")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? AutoTMMRetainedWhenDefaultSavePathChanges => _wrappedObject.AutoTMMRetainedWhenDefaultSavePathChanges;

        [Display(Name = "Retain Auto TMM when category save path changes")]
        [DisplayFormat(NullDisplayText = "n/a")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? AutoTMMRetainedWhenCategorySavePathChanges => _wrappedObject.AutoTMMRetainedWhenCategorySavePathChanges;
    }
}
