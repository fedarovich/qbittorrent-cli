using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct DownloadsViewModel
    {
        private readonly Client.Preferences _wrappedObject;

        public DownloadsViewModel(Client.Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Default save path")]
        public string SavePath => _wrappedObject.SavePath;

        [Display(Name = "Incompleted file path enabled")]
        public bool? TempPathEnabled => _wrappedObject.TempPathEnabled;
        
        [Display(Name = "Incompleted file path")]
        public string TempPath => _wrappedObject.TempPath;

        [Display(Name = "Copy .torrent files to")]
        [DisplayFormat(NullDisplayText = "<disabled>")]
        public string ExportDirectory => _wrappedObject.ExportDirectory;

        [Display(Name = "Copy finished .torrent files to")]
        [DisplayFormat(NullDisplayText = "<disabled>")]
        public string ExportDirectoryForFinished => _wrappedObject.ExportDirectoryForFinished;

        [Display(Name = "Preallocate all files")]
        public bool? PreallocateAll => _wrappedObject.PreallocateAll;

        [Display(Name = "Append .!qb extension to incomplete files")]
        public bool? AppendExtensionToIncompleteFiles => _wrappedObject.AppendExtensionToIncompleteFiles;

        [Display(Name = "Run external program for completed torrent")]
        public bool? AutorunEnabled => _wrappedObject.AutorunEnabled;

        [Display(Name = "External program command line")]
        public string AutorunProgram => _wrappedObject.AutorunProgram;

        [Display(Name = "Create subfolder for multi-file torrents")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? CreateTorrentSubfolder => _wrappedObject.CreateTorrentSubfolder;

        [Display(Name = "Add new torrents in paused state")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? AddTorrentPaused => _wrappedObject.AddTorrentPaused;

        [Display(Name = "Delete .torrent files after added")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public TorrentFileAutoDeleteMode? TorrentFileAutoDeleteMode => _wrappedObject.TorrentFileAutoDeleteMode;
    }
}
