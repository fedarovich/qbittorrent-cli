using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct IpFilterViewModel
    {
        private readonly Preferences _wrappedObject;

        public IpFilterViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Enabled")]
        public bool? IpFilterEnabled => _wrappedObject.IpFilterEnabled;

        [Display(Name = "Filter path")]
        public string IpFilterPath => _wrappedObject.IpFilterPath;

        [Display(Name = "Filter trackers")]
        public bool? IpFilterTrackers => _wrappedObject.IpFilterTrackers;

        [Display(Name = "Manually banned IP addresses")]
        public IList<string> BannedIpAddresses => _wrappedObject.BannedIpAddresses;
    }
}
