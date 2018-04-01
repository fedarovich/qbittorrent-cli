using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;

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

                var colorScheme = ColorScheme.Current;
                var normal = GetColors(colorScheme, "status-normal", colorScheme.Strong);
                var info = GetColors(colorScheme, "status-info", colorScheme.Strong);
                var warn = GetColors(colorScheme, "status-warning", colorScheme.Strong);
                var critical = GetColors(colorScheme, "status-critical", colorScheme.Strong);
                var timestamp = GetColors(colorScheme, "timestamp", colorScheme.Normal);
                var message = GetColors(colorScheme, "message", colorScheme.Normal);

                var log = await client.GetLogAsync(severity, AfterId ?? -1);
                foreach (var entry in log)
                {
                    switch (entry.Severity)
                    {
                        case TorrentLogSeverity.Normal:
                            console.WriteColored("[ Normal ]", normal.fg, normal.bg);
                            break;
                        case TorrentLogSeverity.Info:
                            console.WriteColored("[  Info  ]", info.fg, info.bg);
                            break;
                        case TorrentLogSeverity.Warning:
                            console.WriteColored("[  Warn  ]", warn.fg, warn.bg);
                            break;
                        case TorrentLogSeverity.Critical:
                            console.WriteColored("[Critical]", critical.fg, critical.bg);
                            break;
                    }

                    var time = DateTimeOffset.FromUnixTimeMilliseconds(entry.Timestamp).ToString("s").Replace("T", " ");
                    console.WriteColored($" {entry.Id:D6} {time} ", timestamp.fg, timestamp.bg);
                    console.WriteLineColored(entry.Message, message.fg, message.bg);
                }

                return ExitCodes.Success;
            }

            private (ConsoleColor bg, ConsoleColor fg) GetColors(ColorScheme scheme, string name, ColorSet fallback)
            {
                if (scheme.LogColors == null || !scheme.LogColors.TryGetValue(name, out var colorSet))
                {
                    colorSet = fallback;
                }

                var bg = colorSet?.GetEffectiveBackground() ?? fallback.GetEffectiveBackground();
                var fg = colorSet?.GetEffectiveForeground() ?? fallback.GetEffectiveForeground();
                return (bg, fg);
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

                var doc = new Document
                {
                    Background = ColorScheme.Current.Normal.GetEffectiveBackground(),
                    Color = ColorScheme.Current.Normal.GetEffectiveForeground(),
                    Children =
                    {
                        new Grid
                        {
                            Columns =
                            {
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Star(1)}
                            },
                            Children =
                            {

                            }
                        }
                    }
                };

                ConsoleRenderer.RenderDocument(doc);

                return ExitCodes.Success;
            }
        }
    }
}
