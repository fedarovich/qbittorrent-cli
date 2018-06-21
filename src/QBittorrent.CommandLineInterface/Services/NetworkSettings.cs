using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QBittorrent.CommandLineInterface.Converters;

namespace QBittorrent.CommandLineInterface.Services
{
    public class NetworkSettings
    {
        public bool UseDefaultCredentials { get; set; }

        public bool IgnoreCertificateErrors { get; set; }

        public IList<SiteCredentials> Credentials { get; set; } = new List<SiteCredentials>();

        public ProxySettings Proxy { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Credentials == null)
            {
                Credentials = new List<SiteCredentials>();
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (Credentials == null)
            {
                Credentials = new List<SiteCredentials>();
            }
        }

        public class SiteCredentials
        {
            public Uri Url { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public AuthType AuthType { get; set; }

            public string Username { get; set; }

            [JsonConverter(typeof(EncryptConverter))]
            public string Password { get; set; }

            public string Domain { get; set; }

            public NetworkCredential ToCredential() => new NetworkCredential(Username, Password, Domain ?? string.Empty);
        }

        public enum AuthType
        {
            Basic,
            Digest,
            Ntlm,
            Negotiate,
            Kerberos
        }
    }
}
