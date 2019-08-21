using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Tracker))]
    public partial class TorrentCommand
    {
        [Command(Description = "Gets or adds torrent trackers.")]
        [Subcommand(typeof(List))]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(Edit))]
        [Subcommand(typeof(Delete))]
        public class Tracker : ClientRootCommandBase
        {
            [Command(Description = "Shows the torrent trackers.", ExtendedHelpText = FormatHelpText)]
            public class List : TorrentSpecificListCommandBase<TorrentTrackerViewModel>
            {
                [Option("-s|--sticky", "Include \"sticky tracker\": DHT, PeX, LSD.", CommandOptionType.NoValue)]
                public bool Sticky { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var trackers = await client.GetTorrentTrackersAsync(Hash);
                    var viewModels = trackers.Where(t => Sticky || t.Url.IsAbsoluteUri).Select(t => new TorrentTrackerViewModel(t));
                    Print(viewModels, true);
                    return ExitCodes.Success;
                }

                protected override void PrintTable(IEnumerable<TorrentTrackerViewModel> list)
                {
                    var doc = new Document(
                            new Grid
                            {
                                Columns =
                                {
                                    new Column {Width = GridLength.Star(1)},
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Auto},
                                },
                                Children =
                                {
                                    UIHelper.Header("URL"),
                                    UIHelper.Header("Status"),
                                    UIHelper.Header("Seeds"),
                                    UIHelper.Header("Leechers"),
                                    list.Select(c => new[]
                                    {
                                        new Cell(c.Url),
                                        new Cell(c.Status),
                                        new Cell(c.Seeds),
                                        new Cell(c.Leeches),
                                    })
                                },
                                Stroke = LineThickness.Single
                            })
                        .SetColors(ColorScheme.Current.Normal);
                    ConsoleRenderer.RenderDocument(doc);
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

            [Command("edit", "change", Description = "Changes URL of the torrent tracker.",
                ExtendedHelpText = "qBittorrent 4.1.5 or later is required for this command.")]
            public class Edit : TorrentSpecificCommandBase
            {
                [Argument(1, "<CURRENT_URL>", Description = "Current URL of the tracker.")]
                [Required]
                public Uri From { get; set; }

                [Argument(2, "<NEW_URL>", Description = "New URL of the tracker.")]
                [Required]
                public Uri To { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.EditTrackerAsync(Hash, From, To);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Deletes tracker from the torrent.",
                ExtendedHelpText = "qBittorrent 4.1.5 or later is required for this command.")]
            public class Delete : TorrentSpecificCommandBase
            {
                [Argument(1, "<URL>", Description = "URL of the tracker.")]
                [Required]
                public Uri TrackerUrl { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.DeleteTrackerAsync(Hash, TrackerUrl);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
