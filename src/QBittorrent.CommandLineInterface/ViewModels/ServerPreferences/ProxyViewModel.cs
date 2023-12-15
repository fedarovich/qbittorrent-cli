using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct ProxyViewModel
    {
        private readonly Preferences _wrappedObject;

        public ProxyViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Proxy type")]
        public ProxyType? ProxyType => _wrappedObject.ProxyType;

        [Display(Name = "Address")]
        public string ProxyAddress => _wrappedObject.ProxyAddress;

        [Display(Name = "Port")]
        public int? ProxyPort => _wrappedObject.ProxyPort;

        [Display(Name = "Perform hostname lookup via proxy")]
        public bool? ProxyHostnameLookup => _wrappedObject.ProxyHostnameLookup;

        [Display(Name = "Use proxy for BitTorrent purposes")]
        public bool? ProxyBitTorrent => _wrappedObject.ProxyBittorrent;

        [Display(Name = "Use proxy for peer connections")]
        public bool? ProxyPeerConnections => _wrappedObject.ProxyPeerConnections;

        [Display(Name = "Use proxy for torrents only")]
        public bool? ProxyTorrentsOnly => _wrappedObject.ProxyTorrentsOnly;

        [Display(Name = "Use proxy for RSS purposes")]
        public bool? ProxyRss => _wrappedObject.ProxyRss;

        [Display(Name = "Use proxy for general purposes")]
        public bool? ProxyMisc => _wrappedObject.ProxyMisc;

        [Display(Name = "Disable connections not supported by proxies")]
        public bool? ForceProxy => _wrappedObject.ForceProxy;

        [Display(Name = "Proxy username")]
        public string ProxyUsername => _wrappedObject.ProxyUsername;

        [Display(Name = "Proxy password")]
        public string ProxyPassword => _wrappedObject.ProxyPassword;
    }
}
