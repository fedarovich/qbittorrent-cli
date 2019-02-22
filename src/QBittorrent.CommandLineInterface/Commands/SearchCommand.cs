using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Searches torrents on the trackers.")]
    public partial class SearchCommand : AuthenticatedCommandBase
    {
        protected override Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
        {
            throw new NotImplementedException();
        }
    }
}
