using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(List))]
    public partial class TorrentCommand
    {
        [Command(Description = "Shows the torrent list.", ExtendedHelpText = FormatHelpText + ExtendedHelpText)]
        public class List : ListCommandBase<TorrentInfo>
        {
            private const string ExtendedHelpText =
                "\n" +
                "Torrent Statuses in ST column:\n" +
                "   A - Allocating space on disk\n" +
                "   D - Downloading\n" +
                "  CD - Checking Download\n" +
                "  FD - Forced Download\n" +
                "  MD - Downloading Metadata\n" +
                "  PD - Paused Download\n" +
                "  QD - Queued Download\n" +
                "  SD - Stalled Download\n" +
                "   E - Error\n" +
                "  MF - Missing File\n" +
                "   U - Uploading\n" +
                "  CU - Checking Upload\n" +
                "  FU - Forced Upload\n" +
                "  PU - Paused Upload\n" +
                "  QU - Queued Upload\n" +
                "  SU - Stalled Upload\n" +
                "  QC - Queued for Checking\n" +
                "  CR - Checking Resume Data\n" +
                "  MV - Moving\n" +
                "   ? - Unknown";

            private static readonly IReadOnlyDictionary<TorrentState, string> TorrentStateColorKeys;

            private static readonly Dictionary<string, string> SortColumns;

            static List()
            {
                var fields =
                    from property in typeof(TorrentInfo).GetProperties()
                    let json = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName
                    where json != null
                    select (property.Name, json);

                SortColumns = fields.ToDictionary(x => x.Name, x => x.json, StringComparer.InvariantCultureIgnoreCase);

                var regex = new Regex("[A-Z]{1}[a-z]*");
                var states = 
                    from state in Enum.GetValues(typeof(TorrentState)).Cast<TorrentState>()
                    let name = string.Join("-", regex.Matches(state.ToString()).Cast<Match>().Select(m => m.Value.ToLowerInvariant()))
                    select (state, name);

                TorrentStateColorKeys = states.ToDictionary(x => x.state, x => x.name);
            }

            [Option("--verbose", "Displays verbose information.", CommandOptionType.NoValue)]
            public bool Verbose { get; set; }

            [Option("-f|--filter <STATUS>", 
                "Filter by status: \nall|downloading|seeding|completed|paused|resumed|\nactive|inactive|errored|stalled|stalledDownloading|stalledUploading", 
                CommandOptionType.SingleValue)]
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
                    Filter = Enum.TryParse(Filter, true, out TorrentListFilter filter) ? filter : TorrentListFilter.All,
                    SortBy = Sort != null
                        ? (SortColumns.TryGetValue(Sort, out var sort) ? sort : null)
                        : null,
                    ReverseSort = Reverse,
                    Limit = Limit,
                    Offset = Offset
                };
                var torrents = await client.GetTorrentListAsync(query);
                Print(torrents, Verbose);
                return ExitCodes.Success;
            }

            protected override void PrintList(IEnumerable<TorrentInfo> torrents)
            {
                var doc = new Document(
                    torrents.Select(torrent =>
                        new Grid
                        {
                            Stroke = new LineThickness(LineWidth.None),
                            Columns = { UIHelper.FieldsColumns },
                            Children =
                                {
                                    UIHelper.Row("Name", torrent.Name),
                                    UIHelper.Row("State", torrent.State),
                                    UIHelper.Row("Hash", torrent.Hash),
                                    UIHelper.Row("Size", $"{torrent.Size:N0} bytes"),
                                    UIHelper.Row("Progress", $"{torrent.Progress:P0}"),
                                    UIHelper.Row("DL Speed", $"{FormatSpeed(torrent.DownloadSpeed)}"),
                                    UIHelper.Row("UP Speed", $"{FormatSpeed(torrent.UploadSpeed)}"),
                                    UIHelper.Row("Priority", torrent.Priority),
                                    UIHelper.Row("Seeds", $"{torrent.ConnectedSeeds} of {torrent.TotalSeeds}"),
                                    UIHelper.Row("Leechers", $"{torrent.ConnectedLeechers} of {torrent.TotalLeechers}"),
                                    UIHelper.Row("Ratio", $"{torrent.Ratio:F2}"),
                                    UIHelper.Row("ETA", FormatEta(torrent.EstimatedTime)),
                                    UIHelper.Row("Category", torrent.Category),
                                    UIHelper.Row("Tags", $"{string.Join(", ", torrent.Tags ?? Enumerable.Empty<string>())}"),
                                    UIHelper.Row("Save path", torrent.SavePath),
                                    UIHelper.Row("Added", $"{torrent.AddedOn?.ToLocalTime():G}"),
                                    UIHelper.Row("Completion", $"{torrent.CompletionOn?.ToLocalTime():G}"),
                                    UIHelper.Row("Options", GetOptions(torrent))
                                },
                            Margin = new Thickness(0, 0, 0, 2)
                        }
                    )
                ).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(doc);

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
                    if (torrent.AutomaticTorrentManagement)
                        options.Add("Automatic torrent management");
                    return string.Join(", ", options);
                }
            }

            protected override void PrintTable(IEnumerable<TorrentInfo> torrents)
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
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                        },
                        Children =
                        {
                            UIHelper.Header("ST"),
                            UIHelper.Header("Name"),
                            UIHelper.Header("Hash"),
                            UIHelper.Header("DL Speed", TextAlign.Center),
                            UIHelper.Header("UL Speed", TextAlign.Center),
                            UIHelper.Header("ETA", TextAlign.Center, 9),
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
                ).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(doc);
                
                Cell FormatState(TorrentState state)
                {
                    var colorSet = GetStateColors(state);
                    switch (state)
                    {
                        case TorrentState.Error:
                            return new Cell(" E").SetColors(colorSet);
                        case TorrentState.PausedUpload:
                            return new Cell("PU").SetColors(colorSet);
                        case TorrentState.PausedDownload:
                            return new Cell("PD").SetColors(colorSet);
                        case TorrentState.QueuedUpload:
                            return new Cell("QU").SetColors(colorSet);
                        case TorrentState.QueuedDownload:
                            return new Cell("QD").SetColors(colorSet);
                        case TorrentState.Uploading:
                            return new Cell(" U").SetColors(colorSet);
                        case TorrentState.StalledUpload:
                            return new Cell("SU").SetColors(colorSet);
                        case TorrentState.CheckingUpload:
                            return new Cell("CU").SetColors(colorSet);
                        case TorrentState.CheckingDownload:
                            return new Cell("CD").SetColors(colorSet);
                        case TorrentState.Downloading:
                            return new Cell(" D").SetColors(colorSet);
                        case TorrentState.StalledDownload:
                            return new Cell("SD").SetColors(colorSet);
                        case TorrentState.FetchingMetadata:
                            return new Cell("MD").SetColors(colorSet);
                        case TorrentState.ForcedUpload:
                            return new Cell("FU").SetColors(colorSet);
                        case TorrentState.ForcedDownload:
                            return new Cell("FD").SetColors(colorSet);
                        case TorrentState.MissingFiles:
                            return new Cell("MF").SetColors(colorSet);
                        case TorrentState.Allocating:
                            return new Cell(" A").SetColors(colorSet);
                        case TorrentState.QueuedForChecking:
                            return new Cell("QC").SetColors(colorSet);
                        case TorrentState.CheckingResumeData:
                            return new Cell("CR").SetColors(colorSet);
                        case TorrentState.Moving:
                            return new Cell("MV").SetColors(colorSet);
                        case TorrentState.Unknown:
                        default:
                            return new Cell(" ?").SetColors(colorSet);
                    }
                }

                ColorSet GetStateColors(TorrentState state)
                {
                    var colorScheme = ColorScheme.Current;

                    if (!TorrentStateColorKeys.TryGetValue(state, out var name))
                        return colorScheme.Normal;

                    if (colorScheme.TorrentStateColors == null || !colorScheme.TorrentStateColors.TryGetValue(name, out var colorSet))
                        return colorScheme.Normal;

                    return colorSet;
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

            private static string FormatEta(TimeSpan? eta)
            {
                if (eta < TimeSpan.FromHours(100))
                {
                    var ts = eta.Value;
                    return $" {ts.Hours:00}.{ts.Minutes:00}.{ts.Seconds:00}";
                }
                return string.Empty;
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
