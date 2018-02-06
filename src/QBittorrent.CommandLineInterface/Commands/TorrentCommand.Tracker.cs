using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("tracker", typeof(Tracker))]
    public partial class TorrentCommand
    {
        [Command(Description = "Gets or adds torrent trackers.")]
        [Subcommand("list", typeof(List))]
        [Subcommand("add", typeof(Add))]
        public class Tracker : ClientRootCommandBase
        {
            [Command(Description = "Shows the torrent trackers.")]
            public class List : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var trackers = await client.GetTorrentTrackersAsync(Hash);
                    foreach (var tracker in trackers)
                    {
                        console.PrintObject(tracker);
                        console.WriteLine();
                    }
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Adds trackers to the torrent.")]
            public class Add : TorrentSpecificCommandBase
            {
                [Argument(1, "<URL_1 URL_2 ... URL_N>", Description = "URLs of the trackers to add.")]
                [Required]
                public List<string> Trackers { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.AddTrackersAsync(Hash, Trackers.Select(url => new Uri(url, UriKind.Absolute)));
                    return ExitCodes.Success;
                }
            }
        }
    }
}
