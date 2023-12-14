using System.Collections.Generic;
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
            public class Min : MultiTorrentCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to change priority of all torrents.")]
                public override IList<string> Hashes { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll ? client.ChangeTorrentPriorityAsync(TorrentPriorityChange.Minimal) : client.ChangeTorrentPriorityAsync(Hashes, TorrentPriorityChange.Minimal));
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Sets maximal torrent priority.")]
            public class Max : MultiTorrentCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to change priority of all torrents.")]
                public override IList<string> Hashes { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll ? client.ChangeTorrentPriorityAsync(TorrentPriorityChange.Maximal) : client.ChangeTorrentPriorityAsync(Hashes, TorrentPriorityChange.Maximal));
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Increases torrent priority.")]
            public class Up : MultiTorrentCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to change priority of all torrents.")]
                public override IList<string> Hashes { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll ? client.ChangeTorrentPriorityAsync(TorrentPriorityChange.Increase) : client.ChangeTorrentPriorityAsync(Hashes, TorrentPriorityChange.Increase));
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Decreases torrent priority.")]
            public class Down : MultiTorrentCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to change priority of all torrents.")]
                public override IList<string> Hashes { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll ? client.ChangeTorrentPriorityAsync(TorrentPriorityChange.Decrease) : client.ChangeTorrentPriorityAsync(Hashes, TorrentPriorityChange.Decrease));
                    return ExitCodes.Success;
                }
            }
        }
    }
}
