using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Add))]
    public partial class TorrentCommand
    {
        [Command(Description = "Adds new torrents.")]
        [Subcommand(typeof(AddFile))]
        [Subcommand(typeof(AddUrl))]
        public class Add : ClientRootCommandBase
        {
            public abstract class Base : AuthenticatedCommandBase
            {
                [Option("-f|--folder <PATH>", "Download folder.", CommandOptionType.SingleValue)]
                public string Folder { get; set; }

                [Option("-C|--cookie <COOKIE>", "Cookie sent to download the .torrent file.", CommandOptionType.SingleValue)]
                public string Cookie { get; set; }

                [Option("-c|--category <CATEGORY>", "Category for the torrent.", CommandOptionType.SingleValue)]
                public string Category { get; set; }

                [Option("-p|--paused", "Add torrents in the paused state.", CommandOptionType.NoValue)]
                public bool Paused { get; set; }

                [Option("-n|--no-check", "Skip hash checking.", CommandOptionType.NoValue)]
                public bool SkipChecking { get; set; }

                [Option("-R|--create-root-folder <BOOL>", "Create root folder (true/false). Deprecated since qBittorrent 4.3.2, use --content-layout instead.", CommandOptionType.SingleValue)]
                public bool? CreateRootFolder { get; set; }

                [Option("-r|--rename <NEW_NAME>", "Rename torrent", CommandOptionType.SingleValue)]
                public string Rename { get; set; }

                [Option("-u|--upload-limit <LIMIT>", "Set torrent upload speed limit (bytes/second).", CommandOptionType.SingleValue)]
                public int? UploadLimit { get; set; }

                [Option("-d|--download-limit <LIMIT>", "Set torrent upload speed limit (bytes/second).", CommandOptionType.SingleValue)]
                public int? DownloadLimit { get; set; }

                [Option("-s|--sequential", "Enable sequential download.", CommandOptionType.NoValue)]
                public bool SequentialDownload { get; set; }

                [Option("-F|--first-last-prio", "Prioritize download of the first and the last pieces.", CommandOptionType.SingleValue)]
                public bool FirstLastPiecePrioritized { get; set; }

                [Option("-a|--automatic-torrent-management <BOOL>", "Enable/disables automatic torrent management. Requires qBittorrent 4.1.5 or later.", CommandOptionType.SingleValue)]
                public bool? AutomaticTorrentManagement { get; set; }

                [Option("-l|--ratio-limit", "The ratio limit (number|GLOBAL|NONE). Requires qBittorrent 4.3.4 or later.", CommandOptionType.SingleValue)]
                [ShareRatioLimitValidation]
                public double? RatioLimit { get; set; }

                [Option("-T|--seeding-time-limit", "Seeding time limit ([d.]HH:mm[:ss]|GLOBAL|NONE). Requires qBittorrent 4.3.4 or later.", CommandOptionType.SingleValue)]
                [ShareSeedingTimeLimitValidation]
                public TimeSpan? SeedingTimeLimit { get; set; }

                [Option("-L|--content-layout", "Content layout (Original|Subfolder|NoSubfolder). Requires qBittorrent 4.3.2 or later.", CommandOptionType.SingleValue)]
                [EnumValidation(typeof(TorrentContentLayout), AllowEmpty = true)]
                public string ContentLayout { get; set; }

                [Option("-t|--tag <TAG>", "The tag to be added to the torrent. Can be specified multiple times. Requires qBittorrent 4.3.4 or later.", CommandOptionType.MultipleValue)]
                public IList<string> Tags { get; set; }

                protected async Task WarnUnsupportedOptions(IQBittorrentClient client, IConsole console)
                {
                    var apiVersion = await client.GetApiVersionAsync();
                    if (AutomaticTorrentManagement != null && apiVersion < new ApiVersion(2, 2))
                    {
                        console.WriteLineColored(
                            "--automatic-torrent-management option is ignored by qBittorrent versions earlier than 4.1.5.",
                            ColorScheme.Current.Warning);
                    }
                    
                    if (Tags != null && apiVersion < new ApiVersion(2, 6, 2))
                    {
                        console.WriteLineColored(
                            "--tag option is ignored by qBittorrent versions earlier than 4.3.2.",
                            ColorScheme.Current.Warning);
                    }

                    if (CreateRootFolder != null && apiVersion >= new ApiVersion(2, 7))
                    {
                        console.WriteLineColored(
                            "--create-root-folder option is deprecated and has no effect on qBittorrent version 4.3.2 or later. Use --content-layout option instead.",
                            ColorScheme.Current.Warning);
                    }

                    if (ContentLayout != null && apiVersion < new ApiVersion(2, 7))
                    {
                        console.WriteLineColored(
                            "--content-layout option is not available before qBittorrent version 4.3.2. Use --create-root-folder option instead.",
                            ColorScheme.Current.Warning);
                    }

                    if (RatioLimit != null && apiVersion < new ApiVersion(2, 8, 1))
                    {
                        console.WriteLineColored(
                            "--ratio-limit option is ignored by qBittorrent versions earlier than 4.3.4.",
                            ColorScheme.Current.Warning);
                    }

                    if (SeedingTimeLimit != null && apiVersion < new ApiVersion(2, 8, 1))
                    {
                        console.WriteLineColored(
                            "--seeding-time-limit option is ignored by qBittorrent versions earlier than 4.3.4.",
                            ColorScheme.Current.Warning);
                    }
                }
            }

            [Command("file", Description = "Adds new torrents from torrent files.")]
            public class AddFile : Base
            {
                [Argument(0, "<file1 file2 ... fileN>", "The list of files.")]
                [Required]
                public List<string> Files { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var request = new AddTorrentFilesRequest(Files)
                    {
                        Category = Category,
                        Cookie = Cookie,
                        CreateRootFolder = CreateRootFolder,
                        DownloadFolder = Folder,
                        DownloadLimit = DownloadLimit,
                        FirstLastPiecePrioritized = FirstLastPiecePrioritized,
                        Paused = Paused,
                        Rename = Rename,
                        SequentialDownload = SequentialDownload,
                        SkipHashChecking = SkipChecking,
                        UploadLimit = UploadLimit,
                        AutomaticTorrentManagement = AutomaticTorrentManagement,
                        RatioLimit = RatioLimit,
                        SeedingTimeLimit = SeedingTimeLimit,
                        ContentLayout = Enum.TryParse(ContentLayout, true, out TorrentContentLayout contentLayout) ? contentLayout : null,
                        Tags = Tags
                    };
                    await client.AddTorrentsAsync(request);
                    await WarnUnsupportedOptions(client, console);
                    return ExitCodes.Success;
                }
            }

            [Command("url", Description = "Adds new torrents by URLS.")]
            public class AddUrl : Base
            {
                [Argument(0, "<URL_1 URL_2 ... URL_N>", "The list of URLS.")]
                [Required]
                public List<string> Urls { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var urls = Urls.Select(x => new Uri(x, UriKind.Absolute)).ToList();
                    var request = new AddTorrentUrlsRequest(urls)
                    {
                        Category = Category,
                        Cookie = Cookie,
                        CreateRootFolder = CreateRootFolder,
                        DownloadFolder = Folder,
                        DownloadLimit = DownloadLimit,
                        FirstLastPiecePrioritized = FirstLastPiecePrioritized,
                        Paused = Paused,
                        Rename = Rename,
                        SequentialDownload = SequentialDownload,
                        SkipHashChecking = SkipChecking,
                        UploadLimit = UploadLimit,
                        AutomaticTorrentManagement = AutomaticTorrentManagement,
                        RatioLimit = RatioLimit,
                        SeedingTimeLimit = SeedingTimeLimit,
                        ContentLayout = Enum.TryParse(ContentLayout, true, out TorrentContentLayout contentLayout) ? contentLayout : null,
                        Tags = Tags
                    };
                    await client.AddTorrentsAsync(request);
                    await WarnUnsupportedOptions(client, console);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
