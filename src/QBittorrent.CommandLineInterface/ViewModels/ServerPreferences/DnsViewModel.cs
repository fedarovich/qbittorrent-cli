using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct DnsViewModel
    {
        private readonly Preferences _wrappedObject;

        public DnsViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Dynamic DNS enabled")]
        public bool? DynamicDnsEnabled => _wrappedObject.DynamicDnsEnabled;

        [Display(Name = "Service")]
        public DynamicDnsService? DynamicDnsService => _wrappedObject.DynamicDnsService;

        [Display(Name = "Domain name")]
        public string DynamicDnsDomain => _wrappedObject.DynamicDnsDomain;

        [Display(Name = "Username")]
        public string DynamicDnsUsername => _wrappedObject.DynamicDnsUsername;

        [Display(Name = "Password")]
        [DisplayFormat(DataFormatString = "**********")]
        public string DynamicDnsPassword => _wrappedObject.DynamicDnsPassword;
    }
}
