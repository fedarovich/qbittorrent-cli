using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct WebInterfaceViewModel
    {
        private readonly Preferences _wrappedObject;

        public WebInterfaceViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Language")]
        public string Locale => _wrappedObject.Locale;

        [Display(Name = "Address")]
        public string WebUIAddress => _wrappedObject.WebUIAddress;

        [Display(Name = "Port")]
        public int? WebUIPort => _wrappedObject.WebUIPort;

        [Display(Name = "Domain")]
        public string WebUIDomain => _wrappedObject.WebUIDomain;

        [Display(Name = "Use UPnP / NAT-PMP")]
        public bool? WebUIUpnp => _wrappedObject.WebUIUpnp;

        [Display(Name = "Use HTTPS")]
        public bool? WebUIHttps => _wrappedObject.WebUIHttps;

        [Display(Name = "SSL Certificate")]
        public string WebUISslCertificate => _wrappedObject.WebUISslCertificate;

        [Display(Name = "SSL Key")]
        [DisplayFormat(DataFormatString = "**********")]
        public string WebUISslKey => _wrappedObject.WebUISslKey;

        [Display(Name = "Alt. Web UI")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? AlternativeWebUI => _wrappedObject.AlternativeWebUIEnabled;

        [Display(Name = "Alt. Web UI Path")]
        public string AlternativeWebUIPath => _wrappedObject.AlternativeWebUIPath;
    }
}
