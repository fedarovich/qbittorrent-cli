using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("log", typeof(Log))]
    public class ServerCommand : ClientRootCommandBase
    {

        public class Log : AuthenticatedCommandBase
        {
            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var log = await client.GetLogAsync();
                foreach (var item in log)
                {
                    switch (item.Severity)
                    {
                        case TorrentLogSeverity.Normal:
                            console.WriteColored("[ Normal ]", ConsoleColor.Green);
                            break;
                        case TorrentLogSeverity.Info:
                            console.WriteColored("[  Info  ]", ConsoleColor.Blue);
                            break;
                        case TorrentLogSeverity.Warning:
                            console.WriteColored("[  Warn  ]", ConsoleColor.Yellow);
                            break;
                        case TorrentLogSeverity.Critical:
                            console.WriteColored("[Critical]", ConsoleColor.Red);
                            break;
                    }

                    var time = DateTimeOffset.FromUnixTimeMilliseconds(item.Timestamp).ToString("s").Replace("T", " ");
                    console.WriteLine($" {item.Id:D6} {time} {item.Message}");
                }

                return ExitCodes.Success;
            }
        }        
    }
}
