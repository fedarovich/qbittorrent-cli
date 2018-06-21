using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

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
            if (string.IsNullOrWhiteSpace(Url))
            {
                Url = GeneralSettings.Url;
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                UserName = GeneralSettings.Username;
            }

            if (Password == null && !AskForPassword)
            {
                Password = GeneralSettings.Password;
            }
        }
    }
}
