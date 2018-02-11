using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public class List : ClientCommandBase
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

            public async Task<int> OnExecute(IConsole console)
            {
                var client = CreateClient();
                try
                {
                    await AuthenticateAsync(client);
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
                    return 0;
                }
                finally
                {
                    client.Dispose();
                }
            }

            private void PrintTorrents(IConsole console, IEnumerable<TorrentInfo> torrents)
            {
                if (Verbose)
                {
                    PrintTorrentsVerbose(console, torrents);
                }
                else
                {
                    PrintTorrentsTable(console, torrents);
                }
            }

            private void PrintTorrentsVerbose(IConsole console, IEnumerable<TorrentInfo> torrents)
            {
                foreach (var torrent in torrents)
                {
                    console.WriteLine($"Name:       {torrent.Name}");
                    console.WriteLine($"State:      {torrent.State}");
                    console.WriteLine($"Hash:       {torrent.Hash}");
                    console.WriteLine($"Size:       {torrent.Size:N0} bytes");
                    console.WriteLine($"Progress:   {torrent.Progress:P0}");
                    console.WriteLine($"DL Speed:   {FormatSpeed(torrent.DownloadSpeed)}");
                    console.WriteLine($"UP Speed:   {FormatSpeed(torrent.UploadSpeed)}");
                    console.WriteLine($"Priority:   {torrent.Priority}");
                    console.WriteLine($"Seeds:      {torrent.ConnectedSeeds} of {torrent.TotalSeeds}");
                    console.WriteLine($"Leechers:   {torrent.ConnectedLeechers} of {torrent.TotalLeechers}");
                    console.WriteLine($"Ratio:      {torrent.Ratio:F2}");
                    console.WriteLine($"ETA:        {FormatEta(torrent.EstimatedTime)}");
                    console.WriteLine($"Category:   {torrent.Category}");
                    console.WriteLine($"Options:    {GetOptions()}");

                    console.WriteLine(string.Empty);

                    string GetOptions()
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


            }

            private void PrintTorrentsTable(IConsole console, IEnumerable<TorrentInfo> torrents)
            {
                const int stateWidth = 2;
                const int hashWidth = 6;
                const int upSpeedWidth = 10;
                const int downSpeedWidth = 10;
                const int eatWidth = 9;
                const int spaceWidth = 5;
                int nameWidth = Console.WindowWidth -
                                (stateWidth + hashWidth + upSpeedWidth + downSpeedWidth + eatWidth + spaceWidth) - 1;

                console.WriteLine($"ST\u2502{"Name".PadRight(nameWidth)}\u2502 Hash \u2502 DL Speed \u2502 UL Speed \u2502   EAT   ");
                console.WriteLine($"  \u253c{"    ".PadRight(nameWidth)}\u253c      \u253c          \u253c          \u253c         ".Replace(' ', '\u2500'));
                foreach (var torrent in torrents)
                {
                    PrintState(console, torrent.State);
                    console.Write("\u2502");
                    console.Write(TrimPadName(torrent.Name));
                    console.Write("\u2502");
                    console.Write(torrent.Hash.Substring(0, 6));
                    console.Write("\u2502");
                    console.Write(FormatSpeed(torrent.DownloadSpeed).PadLeft(10));
                    console.Write("\u2502");
                    console.Write(FormatSpeed(torrent.UploadSpeed).PadLeft(10));
                    console.Write("\u2502");
                    console.Write(FormatEta(torrent.EstimatedTime));
                    console.WriteLine(string.Empty);
                }

                string TrimPadName(string name)
                {
                    return name.Length > nameWidth
                        ? name.Substring(0, nameWidth - 3) + "..."
                        : name.PadRight(nameWidth);
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
