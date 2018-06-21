using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct ConnectionViewModel
    {
        private readonly Preferences _wrappedObject;

        public ConnectionViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Enabled protocols")]
        public BittorrentProtocol? BittorrentProtocol => _wrappedObject.BittorrentProtocol;

        [Display(Name = "Incoming connections port")]
        public int? ListenPort => _wrappedObject.ListenPort;

        [Display(Name = "Use different port on each startup")]
        public bool? RandomPort => _wrappedObject.RandomPort;

        [Display(Name = "Use UPnP/NAT-PMP port forwarding")]
        public bool? UpnpEnabled => _wrappedObject.UpnpEnabled;

        [Display(Name = "Maximal number of connections")]
        [DisplayFormat(DataFormatString = "{0:0;unlimited}")]
        public int? MaxConnections => _wrappedObject.MaxConnections;

        [Display(Name = "Maximal number of connections per torrent")]
        [DisplayFormat(DataFormatString = "{0:0;unlimited}")]
        public int? MaxConnectionsPerTorrent => _wrappedObject.MaxConnectionsPerTorrent;

        [Display(Name = "Maximal number of upload slots")]
        [DisplayFormat(DataFormatString = "{0:0;unlimited}")]
        public int? MaxUploads => _wrappedObject.MaxUploads;

        [Display(Name = "Maximal number of upload slots per torrent")]
        [DisplayFormat(DataFormatString = "{0:0;unlimited}")]
        public int? MaxUploadsPerTorrent => _wrappedObject.MaxUploadsPerTorrent;

    }
}
