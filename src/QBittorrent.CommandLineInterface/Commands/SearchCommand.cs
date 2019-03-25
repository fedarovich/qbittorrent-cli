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
            [Range(1, int.MaxValue)]
            public int? Limit { get; set; }

            [Option("-P|--pager", "Use pager to display the results.", CommandOptionType.NoValue)]
            public bool UsePager { get; set; }

            [Option("-V|--verbose", "Show verbose results.", CommandOptionType.NoValue)]
            public bool Verbose { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                const int resultsPerRequest = 100;
                var id = await client.StartSearchAsync(Pattern,
                    Plugins ?? new[] {"enabled"},
                    Category ?? "all");

                Console.CancelKeyPress += OnCancel;

                int offset = Offset;
                int remaining = Limit ?? int.MaxValue;
                int limit = Math.Min(remaining, resultsPerRequest);

                var pager = UsePager ? new Pager() : null;
                var target = (pager != null && pager.Enabled) ? new TextRenderTarget(pager.Writer) : null;
                
                int index = offset + 1;
                int total = 0;

                try
                {
                    SearchResults results;
                    do
                    {
                        results = await client.GetSearchResultsAsync(id, offset, limit);
                        total = results.Total;

                        foreach (var result in results.Results)
                        {
                            Print(result);
                            index += 1;
                        }

                        offset += results.Results.Count;
                        remaining -= results.Results.Count;
                        limit = Math.Min(remaining, resultsPerRequest);
                    } while (results.Status == SearchJobStatus.Running && remaining > 0);

                    (pager?.Writer ?? console.Out).WriteLine($"Total results: {total:N0}");
                }
                finally
                {
                    target?.Dispose();
                    pager?.Dispose();
                    
                    Console.CancelKeyPress -= OnCancel;
                    await client.StopSearchAsync(id);
                }

                return ExitCodes.Success;

                void Print(SearchResult result)
                {
                    var doc = new Document(
                        new Grid
                        {
                            Stroke = new LineThickness(LineWidth.None),
                            Columns = { UIHelper.FieldsColumns },
                            Children =
                            {
                                UIHelper.Row("#", index),
                                UIHelper.Row("Name", result.FileName),
                                UIHelper.Row("Size", result.FileSize != null ? $"{result.FileSize:N0} bytes" : "n/a"),
                                GetVerboseData(),
                                UIHelper.Label("URL"),
                                UIHelper.Data(string.Empty),
                                UIHelper.Data(result.FileUrl)
                                    .With(c => c[Grid.ColumnSpanProperty] = 2)
                                    .With(c => c.Padding = default)
                            },
                            Margin = new Thickness(0, 0, 0, 1)
                        }).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc, target);

                    IEnumerable<object> GetVerboseData()
                    {
                        if (!Verbose)
                            yield break;

                        yield return UIHelper.Row("Site", result.SiteUrl);
                        yield return UIHelper.Row("Description", result.DescriptionUrl);
                        yield return UIHelper.Row("Seeds", result.Seeds);
                        yield return UIHelper.Row("Leechers", result.Leechers);
                    }
                }

                async void OnCancel(object sender, ConsoleCancelEventArgs e)
                {
                    await client.StopSearchAsync(id);
                    Console.CancelKeyPress -= OnCancel;
                }
            }

            
        }
    }
}
