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
    [Subcommand(typeof(Feed))]
    public partial class RssCommand : ClientRootCommandBase
    {
        [Command(Description = "Manages RSS feeds.", ExtendedHelpText = ExperimentalHelpText)]
        [Subcommand(typeof(List))]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(Delete))]
        public class Feed : ClientRootCommandBase
        {
            [Command(Description = "Shows the RSS feed list.", ExtendedHelpText = ExperimentalHelpText)]
            public class List : AuthenticatedCommandBase
            {
                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var root = await client.GetRssItemsAsync(false);

                    var doc = new Document(RenderItem(root, true, true)).SetColors(ColorScheme.Current.Normal);
                    ConsoleRenderer.RenderDocument(doc);
                    return ExitCodes.Success;

                    Stack RenderFolderContent(RssFolder folder)
                    {
                        return new Stack(folder.Items.Select(
                            (item, index) => RenderItem(item, index == folder.Items.Count - 1)));
                    }

                    Grid RenderItem(RssItem item, bool last, bool isRoot = false)
                    {
                        return new Grid
                        {
                            Columns =
                            {
                                new Column { Width = GridLength.Char(2) },
                                new Column { Width = GridLength.Star(1) }
                            },
                            Children =
                            {
                                new object[]
                                {
                                    new Cell(isRoot
                                        ? "<R"
                                        : last ? "\u2514\u2500" : "\u251c\u2500") { Stroke = LineThickness.None },
                                    new Cell(isRoot ? "oot>" : item.Name) { Stroke = LineThickness.None }
                                },
                                new object[] 
                                {
                                    new Cell(last ? null : new Separator() { Orientation = Orientation.Vertical }) { Stroke = LineThickness.None },
                                    new Cell(item is RssFolder folder
                                        ? (Element)RenderFolderContent(folder)
                                        : new Cell($"URL: {((RssFeed)item).Url}")) { Stroke = LineThickness.None }
                                }
                            },
                            Stroke = LineThickness.None
                        };

                    }
                }
            }

            [Command(Description = "Adds the RSS feed or folder.", ExtendedHelpText = ExperimentalHelpText)]
            [Subcommand(typeof(AddFeed))]
            [Subcommand(typeof(AddForder))]
            public class Add : ClientRootCommandBase
            {
                [Command("feed", "url", Description = "Adds the RSS feed.", ExtendedHelpText = ExperimentalHelpText)]
                public class AddFeed : AuthenticatedCommandBase
                {
                    [Argument(0, "<FEED_URL>", "RSS feed URL.")]
                    [Required]
                    public Uri FeedUrl { get; set; }

                    [Option("-p|--path <PATH>", "Virtual path for the feed. Use backslash \\ as a separator.", CommandOptionType.SingleValue)]
                    public string Path { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        await client.AddRssFeedAsync(FeedUrl, Path ?? string.Empty);
                        return ExitCodes.Success;
                    }
                }

                [Command("folder", Description = "Adds the RSS folder.", ExtendedHelpText = ExperimentalHelpText)]
                public class AddForder : AuthenticatedCommandBase
                {
                    [Argument(0, "<PATH>", "Virtual path for the folder. Use backslash \\ as a separator.")]
                    [Required]
                    public string Path { get; set; }

                    protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                    {
                        await client.AddRssFolderAsync(Path);
                        return ExitCodes.Success;
                    }
                }
            }

            [Command(Description = "Deletes the RSS feed or folder.", ExtendedHelpText = ExperimentalHelpText)]
            public class Delete : AuthenticatedCommandBase
            {
                [Argument(0, "<PATH>", "Virtual path for the folder. Use backslash \\ as a separator.")]
                [Required]
                public string Path { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.DeleteRssItemAsync(Path);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
