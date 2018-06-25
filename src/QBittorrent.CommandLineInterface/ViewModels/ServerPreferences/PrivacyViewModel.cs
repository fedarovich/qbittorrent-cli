using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct PrivacyViewModel
    {
        private readonly Preferences _wrappedObject;

        public PrivacyViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Enable DHT")]
        public bool? DHT => _wrappedObject.DHT;

        public bool? DHTSameAsBT => _wrappedObject.DHTSameAsBT;

        [Display(Name = "DHT port")]
        public int? DHTPort => _wrappedObject.DHTPort;

        [Display(Name = "Enable PeX (peer exchange)")]
        public bool? PeerExchange => _wrappedObject.PeerExchange;

        [Display(Name = "Enable LPD (local peer discovery)")]
        public bool? LocalPeerDiscovery => _wrappedObject.LocalPeerDiscovery;

        [Display(Name = "Encryption mode")]
        public Encryption? Encryption => _wrappedObject.Encryption;

        [Display(Name = "Enable anonymous mode")]
        public bool? AnonymousMode => _wrappedObject.AnonymousMode;
    }
}
