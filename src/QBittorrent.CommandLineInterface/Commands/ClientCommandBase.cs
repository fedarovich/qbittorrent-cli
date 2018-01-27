using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [HelpOption]
    public abstract class ClientCommandBase
    {
        [Option("--url <SERVER_URL>", "QBittorrent Server URL", CommandOptionType.SingleValue)]
        public string Url { get; set; } = "http://localhost:8080";

        [Option("--username <USERNAME>", "User name", CommandOptionType.SingleValue)]
        public string UserName { get; set; }

        [Option("--password <PASSWORD>", "User password", CommandOptionType.SingleValue)]
        public string Password { get; set; }

        protected QBittorrentClient CreateClient()
        {
            var client = new QBittorrentClient(new Uri(Url, UriKind.Absolute));
            return client;
        }

        protected async Task AuthenticateAsync(QBittorrentClient client)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                await client.LoginAsync(UserName, Password);
            }
        }
    }
}
