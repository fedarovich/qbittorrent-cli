using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("download")]
    [Subcommand("torrents", typeof(Torrents))]
    [Subcommand("urls", typeof(DownloadWithUrls))]
    public class DownloadCommand : QBittorrentRootCommandBase
    {
        public abstract class Base : QBittorrentCommandBase
        {
            [Option("-f|--folder <PATH>", "Download folder.", CommandOptionType.SingleValue)]
            public string Folder { get; set; }

            [Option("--cookie <COOKIE>", "Cookie sent to download the .torrent file.", CommandOptionType.SingleValue)]
            public string Cookie { get; set; }

            [Option("-c|--category <CATEGORY>", "Category for the torrent.", CommandOptionType.SingleValue)]
            public string Category { get; set; }

            [Option("-p|--paused", "Add torrents in the paused state.", CommandOptionType.NoValue)]
            public bool Paused { get; set; }

            [Option("--no-check", "Skip hash checking.", CommandOptionType.NoValue)]
            public bool SkipChecking { get; set; }

            [Option("--create-root-folder <BOOL>", "Create root folder (true/false).", CommandOptionType.SingleValue)]
            public bool? CreateRootFolder { get; set; }

            [Option("-r|--rename <NEW_NAME>", "Rename torrent", CommandOptionType.SingleValue)]
            public string Rename { get; set; }

            [Option("--ul|--upload-limit <LIMIT>", "Set torrent upload speed limit (bytes/second).", CommandOptionType.SingleValue)]
            public int? UploadLimit { get; set; }

            [Option("--dl|--download-limit <LIMIT>", "Set torrent upload speed limit (bytes/second).", CommandOptionType.SingleValue)]
            public int? DownloadLimit { get; set; }

            [Option("-s|--sequential", "Enable sequential download.", CommandOptionType.NoValue)]
            public bool SequentialDownload { get; set; }

            [Option("--first-last-prio", "Prioritize download of the first and the last pieces.", CommandOptionType.SingleValue)]
            public bool FirstLastPiecePrioritized { get; set; }
        }

        [Command("torrents")]
        public class Torrents : Base
        {
            [Argument(0, "<file1 file2 ... fileN>", "The list of files.")]
            [Required]
            public List<string> Files { get; set; }

            public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
            {
                var client = CreateClient();
                try
                {
                    await AuthenticateAsync(client);
                    var request = new DownloadWithTorrentFilesRequest(Files)
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
                        UploadLimit = UploadLimit
                    };
                    await client.DownloadAsync(request);
                    return 0;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        [Command("urls")]
        public class DownloadWithUrls : Base
        {
            [Argument(0, "<URL_1 URL_2 ... URL_N>", "The list of URLS.")]
            [Required]
            public List<string> Urls { get; set; }

            public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
            {
                var client = CreateClient();
                var urls = Urls.Select(x => new Uri(x, UriKind.Absolute)).ToList();
                try
                {
                    await AuthenticateAsync(client);
                    var request = new DownloadWithTorrentUrlsRequest(urls)
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
                        UploadLimit = UploadLimit
                    };
                    await client.DownloadAsync(request);
                    return 0;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }
    }
}
