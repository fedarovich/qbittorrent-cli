using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class SmartCommand
    {
        public class Move : TorrentSpecificCommandBase
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var torrents = await client.GetTorrentListAsync();
                var torrent = torrents.SingleOrDefault(t => string.Equals(t.Hash, Hash, StringComparison.InvariantCultureIgnoreCase));
                if (torrent == null)
                    return ExitCodes.NotFound;
                
                throw new NotImplementedException();
            }
        }
    }
}
