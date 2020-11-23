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
    [Subcommand(typeof(Tags))]
    public partial class TorrentCommand
    {
        [Command("tags", "tag", Description = "Manages the torrent tags.")]
        [Subcommand(typeof(List))]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(Delete))]
        [Subcommand(typeof(Clear))]
        public class Tags : ClientRootCommandBase
        {
            [Command(Description = "Shows the list of torrent tags.")]
            public class List : TorrentSpecificListCommandBase<string>
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var partialData = await client.GetPartialDataAsync();
                    if (!partialData.TorrentsChanged.TryGetValue(Hash, out var info))
                    {
                        console.WriteLineColored($"No torrent matching hash {Hash} is found.", ColorScheme.Current.Warning);
                        return ExitCodes.NotFound;
                    }

                    Print(info.Tags, true);
                    return ExitCodes.Success;
                }

                protected override void PrintTable(IEnumerable<string> tags)
                {
                    if (tags?.Any() == true)
                    {
                        var doc = new Document(
                                new Grid
                                {
                                    Columns =
                                    {
                                        new Column {Width = GridLength.Auto},
                                    },
                                    Children =
                                    {
                                        UIHelper.Header("Tags"),
                                        tags.Select(t => new[] { new Cell(t)})
                                    },
                                    Stroke = LineThickness.Single
                                })
                            .SetColors(ColorScheme.Current.Normal);
                        ConsoleRenderer.RenderDocument(doc);
                    }
                }

                protected override void PrintList(IEnumerable<string> tags)
                {
                    foreach (var tag in tags)
                    {
                        Console.WriteLine(tag);
                    }
                }
            }

            [Command(Description = "Adds the tags to the torrent.")]
            public class Add : TorrentSpecificCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to add tags to all torrents.")]
                public override string Hash { get; set; }

                [Argument(1, "<TAG_1 TAG_2 ... TAG_N>", "The tags to add.")]
                [Required]
                public List<string> Tags { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll
                        ? client.AddTorrentTagsAsync(Tags)
                        : client.AddTorrentTagsAsync(Hash, Tags));
                    return ExitCodes.Success;
                }
            }

            [Command("delete", "remove", Description = "Removes the tags from the torrent.")]
            public class Delete : TorrentSpecificCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to remove tags from all torrents.")]
                public override string Hash { get; set; }

                [Argument(1, "<TAG_1 TAG_2 ... TAG_N>", "The tags to remove.")]
                [Required]
                public List<string> Tags { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll
                        ? client.DeleteTorrentTagsAsync(Tags)
                        : client.DeleteTorrentTagsAsync(Hash, Tags));
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Clears the tags from the torrent.")]
            public class Clear : TorrentSpecificCommandBase
            {
                protected override bool AllowAll => true;

                [Argument(0, "<HASH|ALL>", "Full or partial torrent hash, or keyword ALL to clear tags from all torrents.")]
                public override string Hash { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await (IsAll
                        ? client.ClearTorrentTagsAsync()
                        : client.ClearTorrentTagsAsync(Hash));
                    return ExitCodes.Success;
                }
            }
        }
    }
}
