using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class ServerCommand
    {
        [Subcommand(typeof(IpFilter))]
        public partial class Settings
        {
            [Command("ip-filter", "ipfilter", Description = "Manages IP filter.", ExtendedHelpText = ExtendedHelp)]
            [Subcommand(typeof(Add))]
            [Subcommand(typeof(Delete))]
            [Subcommand(typeof(Clear))]
            [Subcommand(typeof(List))]
            public class IpFilter : SettingsCommand<IpFilterViewModel>
            {
                [Option("-e|--enabled <BOOL>", "Enables/disables IP filter", CommandOptionType.SingleValue, Inherited = false)]
                public bool? IpFilterEnabled { get; set; }

                [Option("-f|--filter <PATH>", "Filter path (.dat, .p2p, .p2b)", CommandOptionType.SingleValue, Inherited = false)]
                public string IpFilterPath { get; set; }

                [Option("-t|--filter-trackers <BOOL>", "Apply filter to trackers", CommandOptionType.SingleValue, Inherited = false)]
                public bool? IpFilterTrackers { get; set; }

                protected override IReadOnlyDictionary<string, Func<object, object>> CustomFormatters =>
                    new Dictionary<string, Func<object, object>>
                    {
                        [nameof(IpFilterViewModel.BannedIpAddresses)] =
                            value => value is IEnumerable<string> list ? string.Join(Environment.NewLine, list) : null
                    };

                [Command(Description = "Adds IP addresses to the ban-list.")]
                public class Add : AuthenticatedCommandBase
                {
                    [Argument(0, "IP_1 IP_2 ... IP_N", "IP addresses to add.")]
                    [Required]
                    [IpAddressValidation]
                    public List<string> Addresses { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        var banList = prefs.BannedIpAddresses ?? new List<string>();
                        bool modified = false;
                        foreach (var address in Addresses)
                        {
                            if (!banList.Contains(address))
                            {
                                banList.Add(address);
                                modified = true;
                            }
                        }

                        if (modified)
                        {
                            prefs = new Preferences {BannedIpAddresses = banList};
                            await client.SetPreferencesAsync(prefs);
                        }

                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes IP addresses from the ban-list.")]
                public class Delete : AuthenticatedCommandBase
                {
                    [Argument(0, "IP_1 IP_2 ... IP_N", "IP addresses to add.")]
                    [Required]
                    [IpAddressValidation]
                    public List<string> Addresses { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        var banList = prefs.BannedIpAddresses ?? new List<string>();

                        bool modified = false;
                        foreach (var address in Addresses)
                        {
                            modified |= banList.Remove(address);
                        }

                        if (modified)
                        {
                            prefs = new Preferences { BannedIpAddresses = banList };
                            await client.SetPreferencesAsync(prefs);
                        }

                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes all IP addresses from the ban-list.")]
                public class Clear : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = new Preferences { BannedIpAddresses = new string[0] };
                        await client.SetPreferencesAsync(prefs);
                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Lists all manually banned IP addresses.")]
                public class List : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        foreach (var address in prefs.BannedIpAddresses ?? Enumerable.Empty<string>())
                        {
                            console.WriteLineColored(address, ColorScheme.Current.Normal);
                        }
                        return ExitCodes.Success;
                    }
                }
            }
        }
    }
}
