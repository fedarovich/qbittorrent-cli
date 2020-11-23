using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("peer", "peers", Description = "Manages the peers.")]
    [Subcommand(typeof(Ban))]
    public class PeerCommand : ClientRootCommandBase
    {
        [Command(Description = "Bans the peers.")]
        public class Ban : AuthenticatedCommandBase
        {
            [Argument(0, "<PEER_1 PEER_2 ... PEER_N>", "The peers to ban. Each peer must be in format IP:PORT.")]
            [Required]
            [IpEndpointValidation]
            public IList<string> Peers { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.BanPeersAsync(Peers);
                return ExitCodes.Success;
            }
        }
    }
}
