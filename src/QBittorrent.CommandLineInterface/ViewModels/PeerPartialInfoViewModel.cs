using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct PeerPartialInfoViewModel
    {
        private readonly PeerPartialInfo _wrappedObject;

        public PeerPartialInfoViewModel(PeerPartialInfo wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        public string Endpoint =>
            _wrappedObject.Address != null && _wrappedObject.Port != null
                ? new IPEndPoint(_wrappedObject.Address, _wrappedObject.Port.Value).ToString()
                : "n/a";

        public string Client => _wrappedObject.Client;

        [DisplayFormat(DataFormatString = "{0:P0}", NullDisplayText = "n/a")]
        public double? Progress => _wrappedObject.Progress;

        [Display(Name = "Download speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public int? DownloadSpeed => _wrappedObject.DownloadSpeed;

        [Display(Name = "Upload speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public int? UploadSpeed => _wrappedObject.UploadSpeed;

        [Display(Name = "Downloaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? Downloaded => _wrappedObject.Downloaded;

        [Display(Name = "Uploaded")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? Uploaded => _wrappedObject.Uploaded;

        [Display(Name = "Connection Flags")]
        public string ConnectionType => _wrappedObject.ConnectionType;

        public string Flags => _wrappedObject.Flags;

        [Display(Name = "Flags Description")]
        public string FlagsDescription => _wrappedObject.FlagsDescription;

        [DisplayFormat(DataFormatString = "{0:P0}", NullDisplayText = "<Unknown>")]
        public double? Relevance => _wrappedObject.Relevance;

        public IReadOnlyList<string> Files => _wrappedObject.Files;

        public string Country => _wrappedObject.Country;

        [Display(Name = "Country Code")]
        public string CountryCode => _wrappedObject.CountryCode;

        [Newtonsoft.Json.JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData => _wrappedObject.AdditionalData;
    }
}
