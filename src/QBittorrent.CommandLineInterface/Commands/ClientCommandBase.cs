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

        protected Settings Settings { get; }

        protected ClientCommandBase()
        {
            Settings = SettingsService.Instance.Get();
        }

        protected QBittorrentClient CreateClient()
        {
            if (Settings.Proxy == null)
                return new QBittorrentClient(new Uri(Url, UriKind.Absolute));

            var proxy = new WebProxy
            {
                Address = Settings.Proxy.Address,
                BypassProxyOnLocal = Settings.Proxy.BypassLocal,
            };

            if (Settings.Proxy.Bypass?.Any() == true)
            {
                proxy.BypassList = Settings.Proxy.Bypass.ToArray();
            }

            if (string.IsNullOrEmpty(Settings.Proxy.Username))
            {
                proxy.Credentials = null;
                proxy.UseDefaultCredentials = true;
            }
            else
            {
                proxy.UseDefaultCredentials = false;
                proxy.Credentials = new NetworkCredential(Settings.Proxy.Username, Settings.Proxy.Password ?? "");
            }

#if NETFRAMEWORK || NETCOREAPP2_0
            var handler = new HttpClientHandler { Proxy = proxy };
#else           
            var handler = new SocketsHttpHandler { Proxy = proxy };
#endif
            return new QBittorrentClient(new Uri(Url, UriKind.Absolute), handler, true);
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
    }
}
