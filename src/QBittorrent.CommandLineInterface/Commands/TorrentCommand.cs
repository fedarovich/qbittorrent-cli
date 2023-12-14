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

        [Command(Description = "Pauses the specified torrents or all torrents.")]
        public class Pause : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to pause all torrents.")]
            public override IList<string> Hashes { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.PauseAsync() : client.PauseAsync(Hashes));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Resumes the specified torrents or all torrents.")]
        public class Resume : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to resume all torrents.")]
            public override IList<string> Hashes { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.ResumeAsync() : client.ResumeAsync(Hashes));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Force Resumes the specified torrents or all torrents.")]
        public class ForceResume : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to force resume all torrents.")]
            public override IList<string> Hashes { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.SetForceStartAsync(true) : client.SetForceStartAsync(Hashes, true));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Deletes the specified torrents or all torrents.")]
        public class Delete : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to delete all torrents.")]
            public override IList<string> Hashes { get; set; }

            [Option("-f|--delete-files|--with-files", "Delete downloaded files", CommandOptionType.NoValue)]
            public bool DeleteFiles { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.DeleteAsync(DeleteFiles) : client.DeleteAsync(Hashes, DeleteFiles));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Moves the downloaded files to the other folder.")]
        public class Move : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Option("-f|--folder <FOLDER>", CommandOptionType.SingleValue)]
            public string Folder { get; set; }

            protected override Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                if (Folder == null)
                {
                    if (Hashes.Count != 2)
                        throw new InvalidOperationException("The --folder field is required.");

                    // Compatibility mode
#warning Remove this code for qbt 2.0
                    Folder = Hashes[1];
                    Hashes.RemoveAt(1);
                    console.WriteLineColored(
                        "Passing folder as unnamed argument is deprecated and will be removed in the next version. Use --folder option instead.",
                        ColorScheme.Current.Warning);
                }

                return base.OnExecuteAuthenticatedAsync(client, app, console);
            }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.SetLocationAsync(Folder) : client.SetLocationAsync(Hashes, Folder));
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
        public class Category : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to set category for all torrents.")]
            public override IList<string> Hashes { get; set; }

            [Option("--set <CATEGORY>", "The category name to set.", CommandOptionType.SingleValue)]
            [Required(AllowEmptyStrings = true)]
            public string CategoryName { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.SetTorrentCategoryAsync(CategoryName) : client.SetTorrentCategoryAsync(Hashes, CategoryName));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Rechecks the specified torrents or all torrents.")]
        public class Check : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to recheck all torrents.")]
            public override IList<string> Hashes { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.RecheckAsync() : client.RecheckAsync(Hashes));
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Reannounces the specified torrents or all torrents.", ExtendedHelpText = "Requires qBittorrent v4.1.2 or later.")]
        public class Reannounce : MultiTorrentCommandBase
        {
            protected override bool AllowAll => true;

            [Argument(0, "<HASH_1 HASH_2 ... HASH_N|ALL>", "Full or partial torrent hashes, or keyword ALL to reannounce all torrents.")]
            public override IList<string> Hashes { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await (IsAll ? client.ReannounceAsync() : client.ReannounceAsync(Hashes));
                return ExitCodes.Success;
            }
        }
    }
}
