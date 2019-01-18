using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Priority))]
    public partial class TorrentCommand
    {
        [Command(Description = "Changes torrent priority.")]
        [Subcommand(typeof(Min))]
        [Subcommand(typeof(Max))]
        [Subcommand(typeof(Up))]
        [Subcommand(typeof(Down))]
        public class Priority : ClientRootCommandBase
        {
            [Command(Description = "Sets minimal torrent priority.")]
            public class Min : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.ChangeTorrentPriorityAsync(Hash, TorrentPriorityChange.Minimal);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Sets maximal torrent priority.")]
            public class Max : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.ChangeTorrentPriorityAsync(Hash, TorrentPriorityChange.Maximal);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Increases torrent priority.")]
            public class Up : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.ChangeTorrentPriorityAsync(Hash, TorrentPriorityChange.Increase);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Decreases torrent priority.")]
            public class Down : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.ChangeTorrentPriorityAsync(Hash, TorrentPriorityChange.Decrease);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
