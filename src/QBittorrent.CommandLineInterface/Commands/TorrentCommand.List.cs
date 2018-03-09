using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("list", typeof(List))]
    public partial class TorrentCommand
    {
        [Command(Description = "Shows the torrent list.", ExtendedHelpText = ExtendedHelpText)]
        public class List : AuthenticatedCommandBase
        {
            private const string ExtendedHelpText =
                "\n" +
                "Torrent Statuses in ST column:\n" +
                "   D - Downloading\n" +
                "  CD - Checking Download\n" +
                "  FD - Forced Download\n" +
                "  MD - Downloading Metadata\n" +
                "  PD - Paused Download\n" +
                "  QD - Queued Download\n" +
                "  SD - Stalled Download\n" +
                "   E - Error\n" +
                "   U - Uploading\n" +
                "  CU - Checking Upload\n" +
                "  FU - Forced Upload\n" +
                "  PU - Paused Upload\n" +
                "  QU - Queued Upload\n" +
                "  SU - Stalled Upload\n"
                ;


            private static readonly Dictionary<string, string> SortColumns;

            static List()
            {
                var fields =
                    from property in typeof(TorrentInfo).GetProperties()
                    let json = property.GetCustomAttribute<JsonPropertyAttribute>().PropertyName
                    select (property.Name, json);

                SortColumns = fields.ToDictionary(x => x.Name, x => x.json, StringComparer.InvariantCultureIgnoreCase);
            }

            [Option("--verbose", "Displays verbose information.", CommandOptionType.NoValue)]
            public bool Verbose { get; set; }

            [Option("-f|--filter <STATUS>", "Filter by status: all|downloading|completed|paused|active|inactive", CommandOptionType.SingleValue)]
            [EnumValidation(typeof(TorrentListFilter), AllowEmpty = true)]
            public string Filter { get; set; }

            [Option("-c|--category <CATEGORY>", "Filter by category.", CommandOptionType.SingleValue)]
            public string Category { get; set; }

            [Option("-s|--sort <PROPERTY>", "Sort by property.", CommandOptionType.SingleValue)]
            [SortValidation]
            public string Sort { get; set; }

            [Option("-r|--reverse", "Reverse the sort order.", CommandOptionType.NoValue)]
            public bool Reverse { get; set; }

            [Option("-l|--limit <LIMIT>", "Number of torrents to display.", CommandOptionType.SingleValue)]
            public int? Limit { get; set; }

            [Option("-o|--offset <OFFSET>", "Offset from the beginning of the list.", CommandOptionType.SingleValue)]
            public int? Offset { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var query = new TorrentListQuery
                {
                    Category = Category,
                    Filter = Enum.TryParse(Filter, out TorrentListFilter filter) ? filter : TorrentListFilter.All,
                    SortBy = Sort != null
                        ? (SortColumns.TryGetValue(Sort, out var sort) ? sort : null)
                        : null,
                    ReverseSort = Reverse,
                    Limit = Limit,
                    Offset = Offset
                };
                var torrents = await client.GetTorrentListAsync(query);
                PrintTorrents(console, torrents);
                return ExitCodes.Success;
            }

            private void PrintTorrents(IConsole console, IEnumerable<TorrentInfo> torrents)
            {
                if (Verbose)
                {
                    PrintTorrentsVerbose(torrents);
                }
                else
                {
                    PrintTorrentsTable(torrents);
                }
            }

            private void PrintTorrentsVerbose(IEnumerable<TorrentInfo> torrents)
            {
                var cellStroke = new LineThickness(LineWidth.None, LineWidth.None);
                var doc = new Document
                {
                    Children =
                     {
                         torrents.Select(torrent =>
                            new Grid
                            {
                                 Stroke = new LineThickness(LineWidth.None),
                                 Columns =
                                 {
                                     new Column() { Width = GridLength.Auto },
                                     new Column() { Width = GridLength.Star(1) }
                                 },
                                 Children =
                                 {
                                     Row("Name", torrent.Name),
                                     Row("State", torrent.State),
                                     Row("Hash", torrent.Hash),
                                     Row("Size", $"{torrent.Size:N0} bytes"),
                                     Row("Progress", $"{torrent.Progress:P0}"),
                                     Row("DL Speed", $"{FormatSpeed(torrent.DownloadSpeed)}"),
                                     Row("UP Speed", $"{FormatSpeed(torrent.UploadSpeed)}"),
                                     Row("Priority", torrent.Priority),
                                     Row("Seeds", $"{torrent.ConnectedSeeds} of {torrent.TotalSeeds}"),
                                     Row("Leechers", $"{torrent.ConnectedLeechers} of {torrent.TotalLeechers}"),
                                     Row("Ratio", $"{torrent.Ratio:F2}"),
                                     Row("ETA", FormatEta(torrent.EstimatedTime)),
                                     Row("Category", torrent.Category),
                                     Row("Options", GetOptions(torrent)),
                                 },
                                 Margin = new Thickness(0, 0, 0, 2)
                            }
                         )
                     }
                };

                ConsoleRenderer.RenderDocument(doc);

                Cell Label(string text) => new Cell(text + ":") { Color = ConsoleColor.Yellow, Stroke = cellStroke };

                Cell Data<T>(T data) => new Cell(data.ToString()) { Stroke = cellStroke, Padding = new Thickness(3, 0, 0, 0) };

                object[] Row<T>(string label, T data) => new object[] { Label(label), Data(data) };

                string GetOptions(TorrentInfo torrent)
                {
                    var options = new List<string>(4);
                    if (torrent.SequentialDownload)
                        options.Add("Sequential");
                    if (torrent.SuperSeeding)
                        options.Add("Super seeding");
                    if (torrent.ForceStart)
                        options.Add("Force start");
                    if (torrent.FirstLastPiecePrioritized)
                        options.Add("First & last pieces are prioritized");
                    return string.Join(", ", options);
                }
            }

            private void PrintTorrentsTable(IEnumerable<TorrentInfo> torrents)
            {
                var headerStroke = new LineThickness(LineWidth.Single, LineWidth.Double);

                var doc = new Document
                {
                    Children =
                    {
                        new Grid
                        {
                            Columns =
                            {
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Star(1)},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                            },
                            Children =
                            {
                                new Cell("ST") { Stroke = headerStroke },
                                new Cell("Name") { Stroke = headerStroke },
                                new Cell("Hash") { Stroke = headerStroke },
                                new Cell("DL Speed") { Stroke = headerStroke, TextAlign = TextAlign.Center },
                                new Cell("UL Speed") { Stroke = headerStroke, TextAlign = TextAlign.Center },
                                new Cell("EAT") { Stroke = headerStroke, TextAlign = TextAlign.Center, MinWidth = 9 },
                                torrents.Select(t => new[]
                                {
                                    FormatState(t.State),
                                    new Cell(t.Name),
                                    new Cell(t.Hash.Substring(0, 6)),
                                    new Cell(FormatSpeed(t.DownloadSpeed).PadLeft(10)),
                                    new Cell(FormatSpeed(t.UploadSpeed).PadLeft(10)),
                                    new Cell(FormatEta(t.EstimatedTime))
                                })
                            },
                            Stroke = LineThickness.Single
                        }
                    },
                };

                ConsoleRenderer.RenderDocument(doc);

                Cell FormatState(TorrentState state)
                {
                    switch (state)
                    {
                        case TorrentState.Error:
                            return new Cell(" E") { Color = ConsoleColor.Red };
                        case TorrentState.PausedUpload:
                            return new Cell("PU") { Color = ConsoleColor.DarkGray };
                        case TorrentState.PausedDownload:
                            return new Cell("PD") { Color = ConsoleColor.DarkGray };
                        case TorrentState.QueuedUpload:
                            return new Cell("QU") { Color = ConsoleColor.DarkBlue };
                        case TorrentState.QueuedDownload:
                            return new Cell("QD") { Color = ConsoleColor.DarkBlue };
                        case TorrentState.Uploading:
                            return new Cell(" U") { Color = ConsoleColor.Green };
                        case TorrentState.StalledUpload:
                            return new Cell("SU") { Color = ConsoleColor.DarkYellow };
                        case TorrentState.CheckingUpload:
                            return new Cell("CU") { Color = ConsoleColor.Yellow };
                        case TorrentState.CheckingDownload:
                            return new Cell("CD") { Color = ConsoleColor.Yellow };
                        case TorrentState.Downloading:
                            return new Cell(" D") { Color = ConsoleColor.Green };
                        case TorrentState.StalledDownload:
                            return new Cell("SD") { Color = ConsoleColor.DarkYellow };
                        case TorrentState.FetchingMetadata:
                            return new Cell("MD") { Color = ConsoleColor.Blue };
                        case TorrentState.ForcedUpload:
                            return new Cell("FU") { Color = ConsoleColor.Cyan };
                        case TorrentState.ForcedDownload:
                            return new Cell("FD") { Color = ConsoleColor.Cyan };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                }
            }

            private static string FormatSpeed(long speed)
            {
                if (speed < 1024)
                {
                    return $"{speed}  B/s";
                }
                if (speed < 1024 * 1024)
                {
                    return $"{speed / 1024} kB/s";
                }
                if (speed < 1024 * 1024 * 1024)
                {
                    return $"{speed / (1024 * 1024)} MB/s";
                }
                return $"{speed / (1024 * 1024 * 1024)} GB/s";
            }

            private static string FormatEta(int eta)
            {
                var ts = TimeSpan.FromSeconds(eta);
                if (ts < TimeSpan.FromHours(100))
                {
                    return $" {ts.Hours}.{ts.Minutes}.{ts.Seconds}";
                }
                return string.Empty;
            }

            private void PrintState(IConsole console, TorrentState state)
            {
                switch (state)
                {
                    case TorrentState.Error:
                        console.WriteColored(" E", ConsoleColor.Red);
                        break;
                    case TorrentState.PausedUpload:
                        console.WriteColored("PU", ConsoleColor.DarkGray);
                        break;
                    case TorrentState.PausedDownload:
                        console.WriteColored("PD", ConsoleColor.DarkGray);
                        break;
                    case TorrentState.QueuedUpload:
                        console.WriteColored("QU", ConsoleColor.DarkBlue);
                        break;
                    case TorrentState.QueuedDownload:
                        console.WriteColored("QD", ConsoleColor.DarkBlue);
                        break;
                    case TorrentState.Uploading:
                        console.WriteColored(" U", ConsoleColor.Green);
                        break;
                    case TorrentState.StalledUpload:
                        console.WriteColored("SU", ConsoleColor.DarkYellow);
                        break;
                    case TorrentState.CheckingUpload:
                        console.WriteColored("CU", ConsoleColor.Yellow);
                        break;
                    case TorrentState.CheckingDownload:
                        console.WriteColored("CD", ConsoleColor.Yellow);
                        break;
                    case TorrentState.Downloading:
                        console.WriteColored(" D", ConsoleColor.Green);
                        break;
                    case TorrentState.StalledDownload:
                        console.WriteColored("SD", ConsoleColor.DarkYellow);
                        break;
                    case TorrentState.FetchingMetadata:
                        console.WriteColored("MD", ConsoleColor.Blue);
                        break;
                    case TorrentState.ForcedUpload:
                        console.WriteColored("FU", ConsoleColor.Cyan);
                        break;
                    case TorrentState.ForcedDownload:
                        console.WriteColored("FD", ConsoleColor.Cyan);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                Console.ResetColor();
            }

            private class SortValidationAttribute : ValidationAttribute
            {
                protected override ValidationResult IsValid(object value, ValidationContext validationContext)
                {
                    var str = value as string;
                    if (str == null || SortColumns.ContainsKey(str))
                        return ValidationResult.Success;

                    return new ValidationResult(
                        $"The value of {validationContext.DisplayName} must be one of the following: {string.Join(", ", SortColumns.Keys)}");
                }
            }
        }
    }
}
