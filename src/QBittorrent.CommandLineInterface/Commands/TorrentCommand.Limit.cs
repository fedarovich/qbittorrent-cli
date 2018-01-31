using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("limit", typeof(Limit))]
    public partial class TorrentCommand
    {
        [Subcommand("download", typeof(Download))]
        [Subcommand("upload", typeof(Upload))]
        [Command(Description = "Gets or sets download and upload speed limits.")]
        public class Limit : ClientRootCommandBase
        {
            [Command(Description = "Gets or sets torrent download speed limit.")]
            public class Download : TorrentSpecificCommandBase
            {
                [Range(0, int.MaxValue)]
                [Option("-s|--set <VALUE>", "The download speed limit in bytes/s to set. Pass 0 to remove the limit.", CommandOptionType.SingleValue)]
                public int? Set { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Set.HasValue)
                    {
                        await client.SetTorrentDownloadLimitAsync(Hash, Set.Value);
                    }
                    else
                    {
                        var limit = await client.GetTorrentDownloadLimitAsync(Hash);
                        if (limit == null || limit < 0)
                        {
                            console.WriteLine("n/a");
                        }
                        else if (limit == 0)
                        {
                            console.WriteLine("unlimited");
                        }
                        else
                        {
                            console.WriteLine($"{limit:N0} bytes/s");
                        }
                    }
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Gets or sets torrent upload speed limit.")]
            public class Upload : TorrentSpecificCommandBase
            {
                [Range(0, int.MaxValue)]
                [Option("-s|--set <VALUE>", "The upload speed limit in bytes/s to set. Pass 0 to remove the limit.", CommandOptionType.SingleValue)]
                public int? Set { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Set.HasValue)
                    {
                        await client.SetTorrentUploadLimitAsync(Hash, Set.Value);
                    }
                    else
                    {
                        var limit = await client.GetTorrentUploadLimitAsync(Hash);
                        if (limit == null || limit < 0)
                        {
                            console.WriteLine("n/a");
                        }
                        else if (limit == 0)
                        {
                            console.WriteLine("unlimited");
                        }
                        else
                        {
                            console.WriteLine($"{limit:N0} bytes/s");
                        }
                    }
                    return ExitCodes.Success;
                }
            }
        }
    }
}
