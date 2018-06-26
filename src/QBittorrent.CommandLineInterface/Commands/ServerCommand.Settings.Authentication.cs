using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class ServerCommand
    {
        [Subcommand("authentication", typeof(Authentication))]
        public partial class Settings
        {
            [Command(Description = "Manages authentication settings.")]
            [Subcommand("whitelist", typeof(Whitelist))]
            public class Authentication : SettingsCommand<AuthenticationViewModel>
            {
                private Encoding _encoding;

                public Authentication()
                {
                    CustomFormatters[nameof(AuthenticationViewModel.BypassAuthenticationSubnetWhitelist)] =
                        value => (value is IList<string> list)
                            ? string.Join(Environment.NewLine, list)
                            : string.Empty;
                }

                [Option("-u|--server-username <USERNAME>", "qBittorrent web interface username.", CommandOptionType.SingleValue, Inherited = false)]
                [MinLength(1)]
                public string WebUIUsername { get; set; }

                [Option("-p|--server-password <PASSWORD>", "qBittorrent web interface password.", CommandOptionType.SingleValue, Inherited = false)]
                [NoAutoSet]
                public string WebUIPassword { get; set; }

                [Option("-P|--ask-server-password", "Ask for qBittorrent web interface password.", CommandOptionType.NoValue, Inherited = false)]
                [NoAutoSet]
                public bool AskForServerPassword { get; set; }

                [Option("-e|--password-encoding <ENCODING>", "Encoding used on server to calculate password hash. (ASCII by default)", CommandOptionType.SingleValue, Inherited = false)]
                [Ignore]
                public string PasswordEncoding { get; set; }
                
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

                    _encoding = PasswordEncoding != null
                        ? Encoding.GetEncoding(PasswordEncoding)
                        : Encoding.ASCII;

                    return Task.CompletedTask;
                }

                protected override void CustomFillPrefences(Preferences preferences)
                {
                    if (WebUIPassword != null)
                    {
                        using (var md5 = MD5.Create())
                        {
                            var bytes = _encoding.GetBytes(WebUIPassword);
                            var hash = string.Concat(md5.ComputeHash(bytes).Select(b => b.ToString("X2")));
                            preferences.WebUIPasswordHash = hash;
                        }
                    }
                }

                [Command(Description = "Manages authentication bypass whitelist.")]
                [Subcommand("list", typeof(List))]
                [Subcommand("add", typeof(Add))]
                [Subcommand("delete", typeof(Delete))]
                [Subcommand("clear", typeof(Clear))]
                public class Whitelist : ClientRootCommandBase
                {
                    [Command(Description = "Adds IP subnets to authentication bypass whitelist.")]
                    public class Add : AuthenticatedCommandBase
                    {
                        [Argument(0, "NET_1 NET2 ... NET_N", "IP subnets to add.")]
                        [IpNetworkValidation]
                        [Required]
                        public IList<string> Networks { get; set; }

                        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                        {
                            var prefs = await client.GetPreferencesAsync();
                            var currentNetworks = prefs.BypassAuthenticationSubnetWhitelist;
                            bool modified = false;
                            foreach (var networl in Networks)
                            {
                                if (!currentNetworks.Contains(networl))
                                {
                                    currentNetworks.Add(networl);
                                    modified = true;
                                }
                            }

                            if (modified)
                            {
                                prefs = new Preferences { BypassAuthenticationSubnetWhitelist = currentNetworks };
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
                            var currentNetwoks = prefs.BypassAuthenticationSubnetWhitelist;

                            bool modified = false;
                            foreach (var network in Networks)
                            {
                                modified |= currentNetwoks.Remove(network);
                            }

                            if (modified)
                            {
                                prefs = new Preferences { BypassAuthenticationSubnetWhitelist = currentNetwoks };
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
                                foreach (var network in prefs.BypassAuthenticationSubnetWhitelist)
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
