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
    [Command("torrent", Description = "Manage torrents.")]
    [Subcommand(typeof(Properties))]
    [Subcommand(typeof(Content))]
    [Subcommand(typeof(WebSeeds))]
    [Subcommand(typeof(Pause))]
    [Subcommand(typeof(Resume))]
    [Subcommand(typeof(ForceResume))]
    [Subcommand(typeof(Delete))]
    [Subcommand(typeof(Move))]
    [Subcommand(typeof(Rename))]
    [Subcommand(typeof(Category))]
    [Subcommand(typeof(Check))]
    [Subcommand(typeof(Reannounce))]
    public partial class TorrentCommand : ClientRootCommandBase
    {
        [Command(Description = "Shows the torrent properties.", ExtendedHelpText = FormatHelpText)]
        public class Properties : TorrentSpecificFormattableCommandBase<TorrentPropertiesViewModel>
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var props = await client.GetTorrentPropertiesAsync(Hash);
                Print(new TorrentPropertiesViewModel(props));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Shows the torrent content. Alias for \"torrent file list\"", ExtendedHelpText = FormatHelpText)]
        public class Content : TorrentSpecificListCommandBase<TorrentContentViewModel>
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var contents = await client.GetTorrentContentsAsync(Hash);
                var viewModels = contents.Select((c, i) => new TorrentContentViewModel(c, i));
                Print(viewModels, true);
                return ExitCodes.Success;
            }

            protected override void PrintTable(IEnumerable<TorrentContentViewModel> list)
            {
                var doc = new Document(
                        new Grid
                        {
                            Columns =
                            {
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Star(1)},
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Auto},
                            },
                            Children =
                            {
                                    UIHelper.Header("Id"),
                                    UIHelper.Header("Name"),
                                    UIHelper.Header("Size"),
                                    UIHelper.Header("Progress"),
                                    list.Select(c => new[]
                                    {
                                        new Cell(c.Id),
                                        new Cell(c.Name),
                                        new Cell(c.Size.ToString("N0")),
                                        new Cell(c.Progress.ToString("P0")),
                                    })
                            },
                            Stroke = LineThickness.Single
                        })
                    .SetColors(ColorScheme.Current.Normal);
                ConsoleRenderer.RenderDocument(doc);
            }
        }

        [Command("web-seeds", "webseeds", "ws", Description = "Shows the torrent web seeds.")]
        public class WebSeeds : TorrentSpecificCommandBase
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var urls = await client.GetTorrentWebSeedsAsync(Hash);
                foreach (var url in urls)
                {
                    console.WriteLineColored(url.AbsoluteUri, ColorScheme.Current.Normal);
                }
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Pauses the specified torrent or all torrents.")]
        public class Pause : TorrentSpecificCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to pause all torrents.")]
            public override string Hash { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.PauseAsync() : client.PauseAsync(Hash));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Resumes the specified torrent or all torrents.")]
        public class Resume : TorrentSpecificCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to resume all torrents.")]
            public override string Hash { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.ResumeAsync() : client.ResumeAsync(Hash));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Force Resumes the specified torrent or all torrents.")]
        public class ForceResume : TorrentSpecificCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to force resume all torrents.")]
            public override string Hash { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.SetForceStartAsync(true) : client.SetForceStartAsync(new[] { Hash }, true));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Deletes the torrent.")]
        public class Delete : TorrentSpecificCommandBase
        {
            [Option("-f|--delete-files|--with-files", "Delete downloaded files", CommandOptionType.NoValue)]
            public bool DeleteFiles { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.DeleteAsync(Hash, DeleteFiles);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Moves the downloaded files to the other folder.")]
        public class Move : TorrentSpecificCommandBase
        {
            [Argument(1, "<FOLDER>", "The folder to move the torrent to.")]
            [Required]
            public string Folder { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.SetLocationAsync(Hash, Folder);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Renames the torrent.")]
        public class Rename : TorrentSpecificCommandBase
        {
            [Argument(1, "<NAME>", "The new torrent name.")]
            [Required]
            public string Name { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.RenameAsync(Hash, Name);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Sets the torrent category.")]
        public class Category : TorrentSpecificCommandBase
        {
            [Option("--set <CATEGORY>", "The category name to set.", CommandOptionType.SingleValue)]
            [Required(AllowEmptyStrings = true)]
            public string CategoryName { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.SetTorrentCategoryAsync(Hash, CategoryName);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Rechecks the torrent.")]
        public class Check : TorrentSpecificCommandBase
        {
            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.RecheckAsync(Hash);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Reannounces the torrent.", ExtendedHelpText = "Requires qBittorrent v4.1.2 or later.")]
        public class Reannounce : TorrentSpecificCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to reannounce all torrents.")]
            public override string Hash { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.ReannounceAsync() : client.ReannounceAsync(Hash));
                return ExitCodes.Success;
            }
        }
    }
}
