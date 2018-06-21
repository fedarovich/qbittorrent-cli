using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class ClientCommandBase
    {
        [Option("--url <SERVER_URL>", "QBittorrent Server URL", CommandOptionType.SingleValue)]
        public string Url { get; set; }

        [Option("--username <USERNAME>", "User name", CommandOptionType.SingleValue)]
        public string UserName { get; set; }

        [Option("--password <PASSWORD>", "User password", CommandOptionType.SingleValue)]
        public string Password { get; set; }

        [Option("--ask-for-password", "Ask the user to enter a password in a secure way.", CommandOptionType.NoValue)]
        public bool AskForPassword { get; set; }

        protected GeneralSettings GeneralSettings { get; }

        protected NetworkSettings NetworkSettings { get; }

        protected ClientCommandBase()
        {
            GeneralSettings = SettingsService.Instance.GetGeneral();
            NetworkSettings = SettingsService.Instance.GetNetwork();
        }

        protected QBittorrentClient CreateClient()
        {
#if NETFRAMEWORK || NETCOREAPP2_0
            var handler = new HttpClientHandler
            {
                Proxy = GetProxy(),
                UseDefaultCredentials = NetworkSettings.UseDefaultCredentials,
                Credentials = GetCredentials(),
                PreAuthenticate = true
            };

            if (NetworkSettings.IgnoreCertificateErrors)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
            }
            
#else           
            var handler = new SocketsHttpHandler
            {
                Proxy = GetProxy(),
                Credentials = GetCredentials(),
                PreAuthenticate = true
            };

            if (NetworkSettings.IgnoreCertificateErrors)
            {
                handler.SslOptions.RemoteCertificateValidationCallback = (message, cert, chain, error) => true;
            }
#endif
            return new QBittorrentClient(new Uri(Url, UriKind.Absolute), handler, true);
        }

        private IWebProxy GetProxy()
        {
            if (NetworkSettings.Proxy == null)
                return null;

            var proxy = new WebProxy
            {
                Address = NetworkSettings.Proxy.Address,
                BypassProxyOnLocal = NetworkSettings.Proxy.BypassLocal,
            };

            if (NetworkSettings.Proxy.Bypass?.Any() == true)
            {
                proxy.BypassList = NetworkSettings.Proxy.Bypass.ToArray();
            }

            if (string.IsNullOrEmpty(NetworkSettings.Proxy.Username))
            {
                proxy.Credentials = null;
                proxy.UseDefaultCredentials = true;
            }
            else
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(NetworkSettings.Proxy.Username, NetworkSettings.Proxy.Password ?? "");
            }

            return proxy;
        }

        private ICredentials GetCredentials()
        {
            var cache = new CredentialCache();
            foreach (var cred in NetworkSettings.Credentials)
            {
                cache.Add(cred.Url, cred.AuthType.ToString(), cred.ToCredential());
            }

#if !(NETFRAMEWORK || NETCOREAPP2_0)
            if (NetworkSettings.UseDefaultCredentials)
            {
                return new CredentialCacheWithDefault(cache);
            }
#endif
            return cache;
        }

        protected async Task AuthenticateAsync(QBittorrentClient client)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                if (Password == null && AskForPassword)
                {
                    Password = Prompt.GetPassword("Please, enter your password: ");
                }

                await client.LoginAsync(UserName, Password);
            }
        }

        private class CredentialCacheWithDefault : ICredentials
        {
            private readonly CredentialCache _cache;

            public CredentialCacheWithDefault(CredentialCache cache)
            {
                _cache = cache;
            }

            public NetworkCredential GetCredential(Uri uri, string authType)
            {
                return _cache.GetCredential(uri, authType) ?? CredentialCache.DefaultNetworkCredentials;
            }
        }
    }
}
