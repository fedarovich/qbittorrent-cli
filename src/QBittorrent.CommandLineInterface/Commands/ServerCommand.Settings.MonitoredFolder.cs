using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    public partial class ServerCommand
    {
        [Subcommand("monitored-folder", typeof(MonitoredFolder))]
        public partial class Settings
        {
            [Command(Description = "Manages monitored folders.")]
            [Subcommand("list", typeof(List))]
            [Subcommand("add", typeof(Add))]
            [Subcommand("delete", typeof(Delete))]
            [Subcommand("clear", typeof(Clear))]
            public class MonitoredFolder : ClientRootCommandBase
            {
                [Command(Description = "Adds or updates a monitored folder.")]
                public class Add : AuthenticatedCommandBase
                {
                    [Argument(0, "folder", "The path to the folder to monitor.")]
                    [Required(AllowEmptyStrings = false)]
                    public string Folder { get; set; }

                    [Option("-m|--save-to-monitored", "Save to monitored folder.", CommandOptionType.NoValue)]
                    public bool SaveToMonitoredFolder { get; set; }

                    [Option("-d|--save-to-default", "Save to default folder.", CommandOptionType.NoValue)]
                    public bool SaveToDefault { get; set; }

                    [Option("-s|--save-to <PATH>", "Save to the specified path.", CommandOptionType.SingleValue)]
                    [MinLength(1)]
                    public string SaveTo { get; set; }

                    private bool SaveToCustom => !string.IsNullOrEmpty(SaveTo);

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var saveLocation = GetSaveLocation();
                        var prefs = await client.GetPreferencesAsync();
                        var dirs = prefs?.ScanDirectories ?? new Dictionary<string, SaveLocation>();
                        dirs[Folder] = saveLocation;
                        prefs = new Preferences { ScanDirectories = dirs };
                        await client.SetPreferencesAsync(prefs);
                        return ExitCodes.Success;

                        SaveLocation GetSaveLocation()
                        {
                            if (SaveToDefault && SaveToMonitoredFolder)
                                throw new InvalidOperationException("Only single of the options --save-to-default or --save-to-monitored can be used simultaneously.");
                            if (SaveToDefault && SaveToCustom)
                                throw new InvalidOperationException("Only single of the options --save-to-default or --save-to can be used simultaneously.");
                            if (SaveToCustom && SaveToMonitoredFolder)
                                throw new InvalidOperationException("Only single of the options --save-to or --save-to-monitored can be used simultaneously.");

                            if (SaveToCustom)
                                return new SaveLocation(SaveTo);
                            if (SaveToMonitoredFolder)
                                return new SaveLocation(StandardSaveLocation.MonitoredFolder);
                            return new SaveLocation(StandardSaveLocation.Default);
                        }
                    }
                }

                [Command(Description = "Deletes a monitored folder.")]
                public class Delete : AuthenticatedCommandBase
                {
                    [Argument(0, "folder", "The monitored folder path.")]
                    [Required(AllowEmptyStrings = false)]
                    public string Folder { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = await client.GetPreferencesAsync();
                        var dirs = prefs?.ScanDirectories ?? new Dictionary<string, SaveLocation>();
                        dirs.Remove(Folder);
                        prefs = new Preferences { ScanDirectories = dirs };
                        await client.SetPreferencesAsync(prefs);
                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes all monitored folders.")]
                public class Clear : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var prefs = new Preferences { ScanDirectories = new Dictionary<string, SaveLocation>() };
                        await client.SetPreferencesAsync(prefs);
                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Shows the monitored folder list.")]
                public class List : AuthenticatedCommandBase
                {
                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        var preferences = await client.GetPreferencesAsync();
                        var folders = preferences.ScanDirectories ?? new Dictionary<string, SaveLocation>();

                        var doc = new Document(
                            new Grid
                            {
                                Columns =
                                {
                                new Column {Width = GridLength.Star(1)},
                                new Column {Width = GridLength.Star(1)}
                                },
                                Children =
                                {
                                UIHelper.Header("Monitored Folder"),
                                UIHelper.Header("Save Location"),
                                folders.SelectMany(p => new []
                                {
                                    new Cell(p.Key),
                                    FormatSaveLocation(p.Value)
                                })
                                },
                                Stroke = LineThickness.Single
                            }
                        ).SetColors(ColorScheme.Current.Normal);

                        ConsoleRenderer.RenderDocument(doc);

                        return ExitCodes.Success;

                        Cell FormatSaveLocation(SaveLocation location)
                        {
                            switch (location.StandardFolder)
                            {
                                case StandardSaveLocation.MonitoredFolder:
                                    return new Cell("Monitored Folder");
                                case StandardSaveLocation.Default:
                                    return new Cell("Default");
                                case null:
                                    return new Cell(
                                        new Span("Custom: "),
                                        new Span(location.CustomFolder).SetColors(ColorScheme.Current.Strong));
                                default:
                                    return new Cell(location.ToString()).SetColors(ColorScheme.Current.Warning);
                            }
                        }
                    }
                }
            }
        }
    }
}
