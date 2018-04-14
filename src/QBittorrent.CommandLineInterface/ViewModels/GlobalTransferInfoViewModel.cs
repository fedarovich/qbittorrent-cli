using System;
using System.ComponentModel.DataAnnotations;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels
{
    public readonly struct GlobalTransferInfoViewModel
    {
        private readonly GlobalTransferInfo _wrappedObject;

        public GlobalTransferInfoViewModel(GlobalTransferInfo wrappedObject)
        {
            _wrappedObject = wrappedObject ?? throw new ArgumentNullException(nameof(wrappedObject));
        }

        [Display(Name = "Download speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public long? DownloadSpeed => _wrappedObject.DownloadSpeed;

        [Display(Name = "Downloaded data")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? DownloadedData => _wrappedObject.DownloadedData;

        [Display(Name = "Download speed limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public long? DownloadSpeedLimit => _wrappedObject.DownloadSpeedLimit;

        [Display(Name = "Upload speed")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public long? UploadSpeed => _wrappedObject.UploadSpeed;

        [Display(Name = "Uploaded data")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes", NullDisplayText = "n/a")]
        public long? UploadedData => _wrappedObject.UploadedData;

        [Display(Name = "Upload speed limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s", NullDisplayText = "n/a")]
        public long? UploadSpeedLimit => _wrappedObject.UploadSpeedLimit;

        [Display(Name = "DHT nodes")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public long? DhtNodes => _wrappedObject.DhtNodes;

        [Display(Name = "Connection status")]
        public ConnectionStatus? ConnectionStatus => _wrappedObject.ConnectionStatus;
    }
}
