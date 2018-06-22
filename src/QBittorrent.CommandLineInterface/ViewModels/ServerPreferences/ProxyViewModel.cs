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

        [Display(Name = "Use proxy for peer connections")]
        public bool? ProxyPeerConnections => _wrappedObject.ProxyPeerConnections;

        [Display(Name = "Disable connections not supported by proxies")]
        public bool? ForceProxy => _wrappedObject.ForceProxy;

        [Display(Name = "Proxy requires authentication")]
        public bool? ProxyAuthenticationEnabled => _wrappedObject.ProxyAuthenticationEnabled;

        [Display(Name = "Proxy username")]
        public string ProxyUsername => _wrappedObject.ProxyUsername;

        [Display(Name = "Proxy password")]
        public string ProxyPassword => _wrappedObject.ProxyPassword;
    }
}
