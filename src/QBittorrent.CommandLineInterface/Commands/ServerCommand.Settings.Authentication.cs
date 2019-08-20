using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class ServerCommand
    {
        [Subcommand(typeof(Authentication))]
        public partial class Settings
        {
            [Command("authentication", "auth", Description = "Manages authentication settings.")]
            [Subcommand(typeof(Whitelist))]
            public class Authentication : SettingsCommand<AuthenticationViewModel>
            {
                [Option("-u|--server-username <USERNAME>", "qBittorrent web interface username.", CommandOptionType.SingleValue, Inherited = false)]
                [MinLength(3)]
                public string WebUIUsername { get; set; }

                [Option("-p|--server-password <PASSWORD>", "qBittorrent web interface password.", CommandOptionType.SingleValue, Inherited = false)]
                [MinLength(6)]
                public string WebUIPassword { get; set; }

                [Option("-P|--ask-server-password", "Ask for qBittorrent web interface password.", CommandOptionType.NoValue, Inherited = false)]
                [NoAutoSet]
                public bool AskForServerPassword { get; set; }
                
                [Option("-l|--bypass-local <BOOL>", "Bypass authentication on localhost", CommandOptionType.SingleValue, Inherited = false)]
                public bool? BypassLocalAuthentication { get; set; }

                [Option("-w|--bypass-whitelist <BOOL>", "Bypass authentication on whitelist", CommandOptionType.SingleValue, Inherited = false)]
                public bool? BypassAuthenticationSubnetWhitelistEnabled { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (AskForServerPassword)
                    {
                        WebUIPassword = console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your web interface password: ");
                    }

                    return Task.CompletedTask;
                }

                protected override IReadOnlyDictionary<string, Func<object, object>> CustomFormatters =>
                    new Dictionary<string, Func<object, object>>
                    {
                        [nameof(AuthenticationViewModel.BypassAuthenticationSubnetWhitelist)] =
                            value => (value is IList<string> list)
                                ? string.Join(Environment.NewLine, list)
                                : string.Empty
                    };

                [Command(Description = "Manages authentication bypass whitelist.")]
                [Subcommand(typeof(List))]
                [Subcommand(typeof(Add))]
                [Subcommand(typeof(Delete))]
                [Subcommand(typeof(Clear))]
                public class Whitelist : ClientRootCommandBase
                {
                    [Command("whitelist", "white-list", "wl", Description = "Adds IP subnets to authentication bypass whitelist.")]
                    public class Add : AuthenticatedCommandBase
                    {
                        [Argument(0, "NET_1 NET2 ... NET_N", "IP subnets to add.")]
                        [IpNetworkValidation]
                        [Required]
                        public IList<string> Networks { get; set; }

                        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                        {
                            var prefs = await client.GetPreferencesAsync();
                            var currentNetworks = (prefs.BypassAuthenticationSubnetWhitelist ?? Enumerable.Empty<string>())
                                .Select(IPNetwork.Parse)
                                .ToHashSet();
                            bool modified = false;
                            foreach (var network in Networks.Select(IPNetwork.Parse))
                            {
                                if (!currentNetworks.Contains(network))
                                {
                                    currentNetworks.Add(network);
                                    modified = true;
                                }
                            }

                            if (modified)
                            {
                                prefs = new Preferences
                                {
                                    BypassAuthenticationSubnetWhitelist = currentNetworks.Select(n => n.ToString()).ToList()
                                };
                                await client.SetPreferencesAsync(prefs);
                            }

                            return ExitCodes.Success;
                        }
                    }

                    [Command(Description = "Deletes IP subnets from authentication bypass whitelist.")]
                    public class Delete : AuthenticatedCommandBase
                    {
                        [Argument(0, "NET_1 NET2 ... NET_N", "IP subnets to add.")]
                        [IpNetworkValidation]
                        [Required]
                        public IList<string> Networks { get; set; }

                        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                        {
                            var prefs = await client.GetPreferencesAsync();
                            var currentNetworks = (prefs.BypassAuthenticationSubnetWhitelist ?? Enumerable.Empty<string>())
                                .Select(IPNetwork.Parse)
                                .ToHashSet();

                            bool modified = false;
                            foreach (var network in Networks.Select(IPNetwork.Parse))
                            {
                                modified |= currentNetworks.Remove(network);
                            }

                            if (modified)
                            {
                                prefs = new Preferences
                                {
                                    BypassAuthenticationSubnetWhitelist = currentNetworks.Select(n => n.ToString()).ToList()
                                };
                                await client.SetPreferencesAsync(prefs);
                            }

                            return ExitCodes.Success;
                        }
                    }

                    [Command(Description = "Deletes all IP subnets from authentication bypass whitelist.")]
                    public class Clear : AuthenticatedCommandBase
                    {
                        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                        {
                            var prefs = new Preferences { BypassAuthenticationSubnetWhitelist = new string[0] };
                            await client.SetPreferencesAsync(prefs);
                            return ExitCodes.Success;
                        }
                    }

                    [Command(Description = "Shows authentication bypass whitelist.")]
                    public class List : AuthenticatedCommandBase
                    {
                        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                        {
                            var prefs = await client.GetPreferencesAsync();
                            if (prefs.BypassAuthenticationSubnetWhitelist != null)
                            {
                                foreach (var network in prefs.BypassAuthenticationSubnetWhitelist ?? Enumerable.Empty<string>())
                                {
                                    console.WriteLine(network);
                                }
                            }

                            return ExitCodes.Success;
                        }
                    }
                }
            }
        }
    }
}
