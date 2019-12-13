using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
        [DisplayFormat(NullDisplayText = "n/a")]
        public string WebUISslCertificate => _wrappedObject.WebUISslCertificate;

        [Display(Name = "SSL Key")]
        [DisplayFormat(DataFormatString = "**********", NullDisplayText = "n/a")]
        public string WebUISslKey => _wrappedObject.WebUISslKey;

        [Display(Name = "SSL Certificate Path")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public string WebUISslCertificatePath => _wrappedObject.WebUISslCertificatePath?.Replace('/', Path.DirectorySeparatorChar);

        [Display(Name = "SSL Key Path")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public string WebUISslKeyPath => _wrappedObject.WebUISslKeyPath?.Replace('/', Path.DirectorySeparatorChar);

        [Display(Name = "Alt. Web UI")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public bool? AlternativeWebUI => _wrappedObject.AlternativeWebUIEnabled;

        [Display(Name = "Alt. Web UI Path")]
        public string AlternativeWebUIPath => _wrappedObject.AlternativeWebUIPath;
    }
}
