using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Searches torrents on the trackers.")]
    [Subcommand(typeof(Start))]
    public partial class SearchCommand : ClientRootCommandBase
    {
        [Command("start", "find", Description = "Starts the search.")]
        public class Start : AuthenticatedCommandBase
        {
            private const string PluginsOptionDescription =
                "The name of plugins to use for search. " +
                "You can use this option multiple times to specify several plugins. " +
                "Pass \"all\" to search using all installed plugins. " +
                "If omitted, all enabled plugins will be used.";

            [Argument(0, "PATTERN", "The search pattern.")]
            [Required]
            public string Pattern { get; set; }

            [Option("-c|--category <NAME>", "The category to search in. If omitted all categories will be searched.", CommandOptionType.SingleValue)]
            public string Category { get; set; }

            [Option("-p|--plugin <NAME>", PluginsOptionDescription, CommandOptionType.MultipleValue)]
            public IList<string> Plugins { get; set; }

            [Option("-q|--quiet", "Do not display search progress status.", CommandOptionType.NoValue)]
            public bool Quiet { get; set; }

            [Option("-o|--offset <INT>", "The offset from the beginning.", CommandOptionType.SingleValue)]
            public int Offset { get; set; }

            [Option("-l|--limit <INT>", "The number of results to show.", CommandOptionType.SingleValue)]
            public int Limit { get; set; }

            [Option("-v|--verbose", "Displays verbose information.", CommandOptionType.NoValue)]
            public bool Verbose { get; set; }

            [Option("-P|--pager", "Use pager to display the results.", CommandOptionType.NoValue)]
            public bool UsePager { get; set; }


            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var id = await client.StartSearchAsync(Pattern,
                    Plugins ?? new[] {"enabled"},
                    Category ?? "all");

                Console.CancelKeyPress += HandleCancellationRequest;

                await WaitForResultsAsync();
                var results = await client.GetSearchResultsAsync(id, Offset, Limit);
                
                string total = $"Total: {results.Total:N}";
                if (results.Status == SearchJobStatus.Running)
                {
                    total += " (search is in progress)";
                }

                var doc = Verbose ? PrintVerbose() : PrintTable();

                if (UsePager)
                {
                    using (var pager = new Pager(console))
                    {
                        using (var target = new TextRenderTarget(pager.Writer))
                        {
                            ConsoleRenderer.RenderDocument(doc, target);
                            pager.WaitForExit();
                        }
                    }
                }
                else
                {
                    ConsoleRenderer.RenderDocument(doc);
                }

                return ExitCodes.Success;

                async Task WaitForResultsAsync()
                {
                    Quiet |= console.IsOutputRedirected;
                    if (!Quiet)
                    {
                        console.WriteLine("Search is started. Press Ctrl+C to stop.");
                    }

                    string message = string.Empty;
                    int index = 0;
                    var symbols = new[] { @"[\]", "[|]", "[/]", "[-]" };

                    SearchStatus status;
                    do
                    {
                        await Task.Delay(500);
                        status = await client.GetSearchStatusAsync(id);
                        if (!Quiet)
                        {
                            console.Write(new string('\b', message.Length));
                            message = $"{symbols[index]} Found {status.Total} results";
                            index = (index + 1) % symbols.Length;
                            console.Write(message);
                        }

                    } while (status.Status != SearchJobStatus.Stopped);

                    if (!Quiet)
                    {
                        console.Write(new string('\b', message.Length));
                    }
                }

                async void HandleCancellationRequest(object sender, ConsoleCancelEventArgs e)
                {
                    Console.CancelKeyPress -= HandleCancellationRequest;
                    e.Cancel = false;
                    await client.StopSearchAsync(id);
                }

                Document PrintTable()
                {
                    return new Document(
                        new Grid
                        {
                            Columns =
                            {
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Star(1)},
                                new Column {Width = GridLength.Auto},
                            },
                            Children =
                            {
                                UIHelper.Header("File Name"),
                                UIHelper.Header("URL"),
                                UIHelper.Header("Seeds"),
                                results.Results.Select(p => new[]
                                {
                                    new Cell(p.FileName),
                                    new Cell(p.FileUrl),
                                    new Cell(p.Seeds)
                                }),
                                new Cell(total) {[Grid.ColumnSpanProperty] = 3}
                            },
                            Stroke = LineThickness.Single
                        }
                    ).SetColors(ColorScheme.Current.Normal);
                }

                Document PrintVerbose()
                {
                    return new Document(
                        new Div(total) { Margin = new Thickness(0, 0, 0, 1) },
                        new List(results.Results.Select(r =>
                            {
                                var props = UIHelper.ToDocument(r);
                                props.Margin = new Thickness(0, 0, 0, 1);
                                return props;
                            })
                        )
                    ).SetColors(ColorScheme.Current.Normal);
                }
            }
        }
    }
}
