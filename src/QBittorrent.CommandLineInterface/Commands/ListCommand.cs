using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("list")]
    [Subcommand("torrents", typeof(Torrents))]
    public class ListCommand : QBittorrentRootCommandBase
    {
        [Command("torrents")]
        public class Torrents : QBittorrentCommandBase
        {
            private static readonly Dictionary<string, string> SortColumns;

            static Torrents()
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
                    var torrents = await client.GetTorrenListAsync(query);
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
                const int stateWidth = 1;
                const int hashWidth = 6;
                const int upSpeedWidth = 10;
                const int downSpeedWidth = 10;
                const int eatWidth = 9;
                const int spaceWidth = 5;
                int nameWidth = Console.WindowWidth -
                                (stateWidth + hashWidth + upSpeedWidth + downSpeedWidth + eatWidth + spaceWidth) - 1;

                console.WriteLine($"S|{"Name".PadRight(nameWidth)}| Hash | DL Speed | UL Speed |   EAT");
                console.WriteLine(new string('-', Console.BufferWidth - 1));
                foreach (var torrent in torrents)
                {
                    PrintState(console, torrent.State);
                    console.Write(" ");
                    console.Write(TrimPadName(torrent.Name));
                    console.Write(" ");
                    console.Write(torrent.Hash.Substring(0, 6));
                    console.Write(" ");
                    console.Write(FormatSpeed(torrent.DownloadSpeed).PadLeft(10));
                    console.Write(" ");
                    console.Write(FormatSpeed(torrent.UploadSpeed).PadLeft(10));
                    console.Write(" ");
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
                        console.WriteColored("E", ConsoleColor.Red);
                        break;
                    case TorrentState.PausedUpload:
                        console.WriteColored("U", ConsoleColor.DarkGray);
                        break;
                    case TorrentState.PausedDownload:
                        console.WriteColored("D", ConsoleColor.DarkGray);
                        break;
                    case TorrentState.QueuedUpload:
                        console.WriteColored("U", ConsoleColor.DarkBlue);
                        break;
                    case TorrentState.QueuedDownload:
                        console.WriteColored("D", ConsoleColor.DarkBlue);
                        break;
                    case TorrentState.Uploading:
                        console.WriteColored("U", ConsoleColor.Green);
                        break;
                    case TorrentState.StalledUpload:
                        console.WriteColored("U", ConsoleColor.DarkYellow);
                        break;
                    case TorrentState.CheckingUpload:
                        console.WriteColored("U", ConsoleColor.Yellow);
                        break;
                    case TorrentState.CheckingDownload:
                        console.WriteColored("D", ConsoleColor.Yellow);
                        break;
                    case TorrentState.Downloading:
                        console.WriteColored("D", ConsoleColor.Green);
                        break;
                    case TorrentState.StalledDownload:
                        console.WriteColored("D", ConsoleColor.DarkYellow);
                        break;
                    case TorrentState.FetchingMetadata:
                        console.WriteColored("M", ConsoleColor.Blue);
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
