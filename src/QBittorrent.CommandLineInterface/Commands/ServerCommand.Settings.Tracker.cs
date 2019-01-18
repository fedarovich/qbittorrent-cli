using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class ServerCommand
    {
        [Subcommand(typeof(Tracker))]
        public partial class Settings
        {
            [Command(Description = "Manages additional trackers.", ExtendedHelpText = "\nAdditional trackers are automatically added to each new torrent.")]
            [Subcommand(typeof(Add))]
            [Subcommand(typeof(Delete))]
            [Subcommand(typeof(Clear))]
            [Subcommand(typeof(List))]
            public class Tracker : ClientRootCommandBase
            {
                [Command(Description = "Adds additional trackers.")]
                public class Add : AuthenticatedCommandBase
                {
                    [Argument(0, "URL_1 URL_2 ... URL_N", "The URLs of the trackers to be added.")]
                    [Required]
                    [Url]
                    public List<string> Trackers { get; set; } 

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        var currentTrackers = prefs.AdditinalTrackers ?? new List<string>();
                        bool modified = false;
                        foreach (var tracker in Trackers)
                        {
                            if (!currentTrackers.Contains(tracker))
                            {
                                currentTrackers.Add(tracker);
                                modified = true;
                            }
                        }

                        if (modified)
                        {
                            prefs = new Preferences {AdditinalTrackers = currentTrackers};
                            await client.SetPreferencesAsync(prefs);
                        }

                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes additional trackers.")]
                public class Delete : AuthenticatedCommandBase
                {
                    [Argument(0, "URL_1 URL_2 ... URL_N", "The URLs of the trackers to be removed.")]
                    [Required]
                    [Url]
                    public List<string> Trackers { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        var currentTrackers = prefs.AdditinalTrackers ?? new List<string>();
                        bool modified = false;
                        foreach (var tracker in Trackers)
                        {
                            modified |= currentTrackers.Remove(tracker);
                        }

                        if (modified)
                        {
                            prefs = new Preferences { AdditinalTrackers = currentTrackers };
                            await client.SetPreferencesAsync(prefs);
                        }

                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes all additional trackers.")]
                public class Clear : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = new Preferences { AdditinalTrackers = new string[0] };
                        await client.SetPreferencesAsync(prefs);
                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Shows the list of additional trackers.")]
                public class List : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        foreach (var tracker in prefs.AdditinalTrackers ?? Enumerable.Empty<string>())
                        {
                            console.WriteLineColored(tracker, ColorScheme.Current.Normal);
                        }

                        return ExitCodes.Success;
                    }
                }
            }
        }
    }
}
