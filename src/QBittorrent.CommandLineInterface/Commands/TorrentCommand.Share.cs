using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels;

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

                var (properties, global) = await TaskHelper.WhenAll(
                    client.GetTorrentPropertiesAsync(Hash),
                    client.GetPreferencesAsync());
                var viewModel = new TorrentShareViewModel(properties, info);

                UIHelper.PrintObject(viewModel,
                    new Dictionary<string, Func<object, object>>
                    {
                        [nameof(viewModel.RatioLimit)] = FormatRatioLimit,
                        [nameof(viewModel.SeedingTimeLimit)] = FormatSeedingTimeLimit
                    });

                return ExitCodes.Success;

                object FormatRatioLimit(object arg)
                {
                    switch (arg)
                    {
                        case double value when Math.Abs(value - ShareLimits.Ratio.Global) < 0.0001:
                            var globalValue = global.MaxRatio >= 0 ? global.MaxRatio.Value.ToString("F3") : "Unlimited";
                            return $"Global ({globalValue})";
                        case double value when Math.Abs(value - ShareLimits.Ratio.Unlimited) < 0.0001:
                            return "Unlimited";
                        case double value:
                            return value.ToString("F3");
                        default:
                            return null;
                    }
                }

                object FormatSeedingTimeLimit(object arg)
                {
                    var time = arg as TimeSpan?;
                    if (time == ShareLimits.SeedingTime.Global)
                    {
                        var globalValue = global.MaxSeedingTime >= 0 ? global.MaxSeedingTime.Value.ToString() : "Unlimited";
                        return $"Global ({globalValue})";
                    }

                    return time == ShareLimits.SeedingTime.Unlimited ? "Unlimited" : time?.ToString();
                }
            }
        }
    }
}
