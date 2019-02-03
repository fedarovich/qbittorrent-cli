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
    [Subcommand(typeof(Feed))]
    public partial class RssCommand : ClientRootCommandBase
    {
        [Command(Description = "Manages RSS feeds.", ExtendedHelpText = ExperimentalHelpText)]
        [Subcommand(typeof(List))]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(Delete))]
        [Subcommand(typeof(Move))]
        [Subcommand(typeof(Info))]
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

                    object RenderFeed(RssFeed feed)
                    {
                        return new Div(
                            feed.Name,
                            " ",
                            new Span($"[{feed.Url}]").SetColors(ColorScheme.Current.Inactive));
                    }

                    Grid RenderItem(RssItem item, bool last, bool isRoot = false)
                    {
                        var folder = item as RssFolder;
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
                                    new Cell(isRoot 
                                        ? "oot>" 
                                        : item is RssFeed feed ? RenderFeed(feed) : item.Name) { Stroke = LineThickness.None }
                                },
                                new object[] 
                                {
                                    new Cell(folder == null || last ? null : new Separator { Orientation = Orientation.Vertical }) { Stroke = LineThickness.None },
                                    new Cell(folder == null ? null : RenderFolderContent(folder)) { Stroke = LineThickness.None }
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
                [Argument(0, "<PATH>", "Virtual path of the feed or folder. Use backslash \\ as a separator.")]
                [Required]
                public string Path { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.DeleteRssItemAsync(Path);
                    return ExitCodes.Success;
                }
            }

            [Command("move", "mv", "rename", Description = "Moves or renames the RSS feed of folder.", ExtendedHelpText = ExperimentalHelpText)]
            public class Move : AuthenticatedCommandBase
            {
                [Argument(0, "<SOURCE>", "Current virtual path of the feed or folder. Use backslash \\ as a separator.")]
                public string Source { get; set; }

                [Argument(1, "<DESTINATION>", "New path of the feed or folder. Use backslash \\ as a separator.")]
                public string Destination { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.MoveRssItemAsync(Source, Destination);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Shows RSS feed information.", ExtendedHelpText = ExperimentalHelpText)]
            public class Info : AuthenticatedCommandBase
            {
                [Argument(0, "<PATH>", "Virtual path of the feed. Use backslash \\ as a separator.")]
                [Required]
                public string Path { get; set; }

                [Option("-a|--with-articles", "Include article data to the output.", CommandOptionType.NoValue)]
                public bool IncludeArticles { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var root = await client.GetRssItemsAsync(IncludeArticles);
                    var feed = GetFeedByPath(root, Path);
                    if (feed == null)
                        throw new Exception($"Cannot find feed with the path \"{Path}\"");

                    var vm = new RssFeedViewModel(Path, feed);
                    UIHelper.PrintObject(vm,
                        new Dictionary<string, Func<object, object>>
                        {
                            [nameof(vm.Articles)] = FormatArticles
                        });
                    return ExitCodes.Success;
                }

                private RssFeed GetFeedByPath(RssFolder folder, string path)
                {
                    var segments = new Queue<string>(Path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

                    while (segments.Count > 1)
                    {
                        var name = segments.Dequeue();
                        folder = folder.Folders.SingleOrDefault(f => f.Name == name);
                        if (folder == null)
                            return null;
                    }

                    if (folder != null && segments.Count == 1)
                    {
                        var name = segments.Dequeue();
                        return folder.Feeds.SingleOrDefault(f => f.Name == name);
                    }

                    return null;
                }

                private object FormatArticles(object obj)
                {
                    if (!(obj is IEnumerable<RssArticleViewModel> articles))
                        return null;

                    return new Alba.CsConsoleFormat.List(articles.Select(ToDocument));

                    Document ToDocument(RssArticleViewModel article)
                    {
                        var document = UIHelper.ToDocument(article);
                        document.Margin = new Thickness(0, 0, 0, 1);
                        return document;
                    }
                }
            }
        }
    }
}
