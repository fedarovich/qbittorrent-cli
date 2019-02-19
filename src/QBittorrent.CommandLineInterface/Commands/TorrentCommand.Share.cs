using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using NJsonSchema.Infrastructure;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Share))]
    public partial class TorrentCommand
    {
        [Command("share", "sharing", Description = "Manages torrent sharing limits.")]
        public class Share : TorrentSpecificCommandBase
        {
            [Option("-r|--ratio <VALUE>", "Set the ratio limit (number|GLOBAL|NONE)", CommandOptionType.SingleValue)]
            public double? RatioLimit { get; set; }

            [Option("-s|--seeding-time <VALUE>", "Set the share limit ([d.]HH:mm[:ss]|GLOBAL|NONE)", CommandOptionType.SingleValue)]
            public TimeSpan? SeedingTimeLimit { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var partialData = await client.GetPartialDataAsync();
                if (!partialData.TorrentsChanged.TryGetValue(Hash, out var info))
                {
                    console.WriteLineColored($"No torrent matching hash {Hash} is found.", ColorScheme.Current.Warning);
                    return ExitCodes.NotFound;
                }

                if (RatioLimit != null || SeedingTimeLimit != null)
                {
                    await client.SetShareLimitsAsync(Hash,
                        RatioLimit ?? info.RatioLimit ?? ShareLimits.Ratio.Global,
                        SeedingTimeLimit ?? info.SeedingTimeLimit ?? ShareLimits.SeedingTime.Global);
                    return ExitCodes.Success;
                }

                // TODO: Print current state.

                return ExitCodes.Success;
            }
        }
    }
}
