using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct AuthenticationViewModel
    {
        private readonly Preferences _wrappedObject;

        public AuthenticationViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Username")]
        public string WebUIUsername => _wrappedObject.WebUIUsername;

        [Display(Name = "Password")]
        [DisplayFormat(DataFormatString = "**********")]
        public string WebUIPasswordHash => _wrappedObject.WebUIPasswordHash;

        [Display(Name = "Bypass authentication on localhost")]
        public bool? BypassLocalAuthentication => _wrappedObject.BypassLocalAuthentication;

        [Display(Name = "Bypass authentication on whitelist")]
        public bool? BypassAuthenticationSubnetWhitelistEnabled => _wrappedObject.BypassAuthenticationSubnetWhitelistEnabled;

        [Display(Name = "Network whitelist")]
        public IList<string> BypassAuthenticationSubnetWhitelist => _wrappedObject.BypassAuthenticationSubnetWhitelist;
    }
}
