using System;
using System.ComponentModel;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Manage qBittorrent server.")]
    [Subcommand("log", typeof(Log))]
    [Subcommand("info", typeof(Info))]
    public class ServerCommand : ClientRootCommandBase
    {
        [Command(Description = "Gets the qBittorrent log.")]
        public class Log : AuthenticatedCommandBase
        {
            [Option("--after-id <ID>", "Display only log entries with ID more than the specified one.", CommandOptionType.SingleValue)]
            public int? AfterId { get; set; }

            [Option("-s|--severity <S1,S2,...,SN>",
                "Comma separated list of severities to display (NORMAL,INFO,WARNING,CRITICAL) or ALL",
                CommandOptionType.SingleValue)]
            [EnumValidation(typeof(TorrentLogSeverity), AllowEmpty = true)]
            [DefaultValue("ALL")]
            public string Severity { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                if (!Enum.TryParse(Severity, true, out TorrentLogSeverity severity))
                {
                    severity = TorrentLogSeverity.All;
                }

                var log = await client.GetLogAsync(severity, AfterId ?? -1);
                foreach (var entry in log)
                {
                    switch (entry.Severity)
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

                    var time = DateTimeOffset.FromUnixTimeMilliseconds(entry.Timestamp).ToString("s").Replace("T", " ");
                    console.WriteColored($" {entry.Id:D6} {time} ", ConsoleColor.White);
                    console.WriteLine(entry.Message);
                }

                return ExitCodes.Success;
            }
        }

        [Command(Description = "Gets the qBittorrent server info.")]
        public class Info : AuthenticatedCommandBase
        {
            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                var (apiVersion, apiMinVersion, qVersion) = await TaskHelper.WhenAll(
                    client.GetApiVersionAsync(),
                    client.GetMinApiVersionAsync(),
                    client.GetQBittorrentVersionAsync());

                console
                    .WriteLine($"QBittorrent version: {qVersion}")
                    .WriteLine($"API version: {apiVersion}")
                    .WriteLine($"API min version: {apiMinVersion}");

                return ExitCodes.Success;
            }
        }
    }
}
