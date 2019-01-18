using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Limit))]
    public partial class GlobalCommand
    {
        [Command(Description = "Gets or sets global download and upload speed limits.")]
        [Subcommand(typeof(Download))]
        [Subcommand(typeof(Upload))]
        [Subcommand(typeof(Alternative))]
        public class Limit : ClientRootCommandBase
        {
            protected static void PrintLimit(IConsole console, long? limit)
            {
                if (limit == null || limit < 0)
                {
                    console.WriteLineColored("n/a", ColorScheme.Current.Normal);
                }
                else if (limit == 0)
                {
                    console.WriteLineColored("unlimited", ColorScheme.Current.Normal);
                }
                else
                {
                    console.WriteLineColored($"{limit:N0} bytes/s", ColorScheme.Current.Normal);
                }
            }

            [Command(Description = "Gets or sets global download speed limit.")]
            public class Download : AuthenticatedCommandBase
            {
                [Range(0, int.MaxValue)]
                [Option("-s|--set <VALUE>", "The download speed limit in bytes/s to set. Pass 0 to remove the limit.", CommandOptionType.SingleValue)]
                public int? Set { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Set != null)
                    {
                        await client.SetGlobalDownloadLimitAsync(Set.Value);
                    }
                    else
                    {
                        var limit = await client.GetGlobalDownloadLimitAsync();
                        PrintLimit(console, limit);
                    }

                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Gets or sets global upload speed limit.")]
            public class Upload : AuthenticatedCommandBase
            {
                [Range(0, int.MaxValue)]
                [Option("-s|--set <VALUE>", "The upload speed limit in bytes/s to set. Pass 0 to remove the limit.", CommandOptionType.SingleValue)]
                public int? Set { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Set != null)
                    {
                        await client.SetGlobalUploadLimitAsync(Set.Value);
                    }
                    else
                    {
                        var limit = await client.GetGlobalUploadLimitAsync();
                        PrintLimit(console, limit);
                    }

                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Enables/disables alternative speed mode.")]
            public class Alternative : AuthenticatedCommandBase
            {
                [Option("-s|--set <BOOL>", "TRUE to enable alternative speed mode; FALSE to disable", CommandOptionType.SingleValue)]
                public bool? Set { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var isAlternative = await client.GetAlternativeSpeedLimitsEnabledAsync();
                    if (Set == null)
                    {
                        console.WriteLineColored($"Alternative speed mode enabled: {isAlternative}", ColorScheme.Current.Normal);
                    }
                    else if (isAlternative != Set)
                    {
                        await client.ToggleAlternativeSpeedLimitsAsync();
                    }

                    return ExitCodes.Success;
                }
            }
        }
    }
}
