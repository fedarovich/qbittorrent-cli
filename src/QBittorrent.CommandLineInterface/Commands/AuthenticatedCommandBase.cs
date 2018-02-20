using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class AuthenticatedCommandBase : ClientCommandBase
    {
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            LoadSettings();
            var client = CreateClient();
            try
            {
                await AuthenticateAsync(client);
                var result = await OnExecuteAuthenticatedAsync(client, app, console);
                return result;
            }
            finally
            {
                client.Dispose();
            }
        }

        protected abstract Task<int> OnExecuteAuthenticatedAsync(
            QBittorrentClient client,
            CommandLineApplication app,
            IConsole console);

        private void LoadSettings()
        {
            var settings = new Lazy<Settings>(() => SettingsService.Instance.Get(), false);
            if (string.IsNullOrWhiteSpace(Url))
            {
                Url = settings.Value.Url;
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                UserName = settings.Value.Username;
            }

            if (Password == null && !AskForPassword)
            {
                Password = settings.Value.Password;
            }
        }
    }
}
