using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Share))]
    public partial class TorrentCommand
    {
        [Command("share", "sharing", "seeding", Description = "Manages torrent sharing limits.")]
        public class Share : TorrentSpecificCommandBase
        {
            [Option("-r|--ratio-limit <VALUE>", "Set the ratio limit (number|GLOBAL|NONE)", CommandOptionType.SingleValue)]
            [ShareRatioLimitValidation]
            public string RatioLimit { get; set; }

            [Option("-t|--time-limit <VALUE>", "Set the seeding time limit ([d.]HH:mm[:ss]|GLOBAL|NONE)", CommandOptionType.SingleValue)]
            [ShareSeedingTimeLimitValidation]
            public string SeedingTimeLimit { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var partialData = await client.GetPartialDataAsync();
                if (!partialData.TorrentsChanged.TryGetValue(Hash, out var info))
                {
                    console.WriteLineColored($"No torrent matching hash {Hash} is found.", ColorScheme.Current.Warning);
                    return ExitCodes.NotFound;
                }

                var ratioLimit = GetRatioLimit();
                var seedingTimeLimit = GetSeedingTimeLimit();

                if (ratioLimit != null || seedingTimeLimit != null)
                {
                    await client.SetShareLimitsAsync(Hash,
                        ratioLimit ?? info.RatioLimit ?? ShareLimits.Ratio.Global,
                        seedingTimeLimit ?? info.SeedingTimeLimit ?? ShareLimits.SeedingTime.Global);
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
                            var globalValue = global.MaxRatio >= 0 ? global.MaxRatio.Value.ToString("F3") : "None";
                            return $"Global ({globalValue})";
                        case double value when Math.Abs(value - ShareLimits.Ratio.Unlimited) < 0.0001:
                            return "None";
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
                        var globalValue = global.MaxSeedingTime >= 0 ? global.MaxSeedingTime.Value.ToString() : "None";
                        return $"Global ({globalValue})";
                    }

                    return time == ShareLimits.SeedingTime.Unlimited ? "None" : time?.ToString();
                }
            }

            private double? GetRatioLimit()
            {
                if ("GLOBAL".Equals(RatioLimit, StringComparison.OrdinalIgnoreCase))
                    return ShareLimits.Ratio.Global;

                if ("NONE".Equals(RatioLimit, StringComparison.OrdinalIgnoreCase))
                    return ShareLimits.Ratio.Unlimited;

                if (double.TryParse(RatioLimit, NumberStyles.Float, CultureInfo.InvariantCulture, out var ratioLimit))
                    return ratioLimit;

                return double.TryParse(RatioLimit, out ratioLimit) ? ratioLimit : (double?) null;
            }

            private TimeSpan? GetSeedingTimeLimit()
            {
                if ("GLOBAL".Equals(SeedingTimeLimit, StringComparison.OrdinalIgnoreCase))
                    return ShareLimits.SeedingTime.Global;

                if ("NONE".Equals(SeedingTimeLimit, StringComparison.OrdinalIgnoreCase))
                    return ShareLimits.SeedingTime.Unlimited;

                return TimeSpan.TryParse(SeedingTimeLimit, out var seedingTimeLimit) ? seedingTimeLimit : (TimeSpan?)null;
            }
        }
    }
}
