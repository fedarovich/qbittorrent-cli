using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("tag", "tags", Description = "Manages the tags.")]
    [Subcommand(typeof(Add))]
    [Subcommand(typeof(Delete))]
    [Subcommand(typeof(List))]
    public class TagCommand
    {
        [Command("add", "create", Description = "Adds new tags.")]
        public class Add : AuthenticatedCommandBase
        {
            [Argument(0, "<TAG_1 TAG_2 ... TAG_N>", "The tags to add.")]
            [Required]
            public IList<string> Tags { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.CreateTagsAsync(Tags);
                return ExitCodes.Success;
            }
        }

        [Command("delete", "remove", Description = "Deletes tags.")]
        public class Delete : AuthenticatedCommandBase
        {
            [Argument(0, "<TAG_1 TAG_2 ... TAG_N>", "The tags to delete.")]
            [Required]
            public IList<string> Tags { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.DeleteTagsAsync(Tags);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Shows the list of the tags.")]
        public class List : ListCommandBase<string>
        {
            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var tags = await client.GetTagsAsync();
                Print(tags, true);
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
    }
}
