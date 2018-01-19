using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class AuthenticatedCommandBase : ClientCommandBase
    {
        public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
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
    }
}
