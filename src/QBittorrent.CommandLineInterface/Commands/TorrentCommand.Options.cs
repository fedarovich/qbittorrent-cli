using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("options", typeof(Options))]
    public partial class TorrentCommand
    {
        [Command(Description = "Sets torrent options.")]
        public class Options : TorrentSpecificCommandBase
        {
            [Option("-a|--atm|--automatic-torrent-management <BOOL>", "Enable/disables automatic torrent management.", CommandOptionType.SingleValue)]
            public bool? AutomaticTorrentManagement { get; set; }

            [Option("-p|--first-last-prio <BOOL>", "Enables/disables prioritized download of the first and last pieces.", CommandOptionType.SingleValue)]
            public bool? FirstLastPriority { get; set; }

            [Option("-f|--force-start <BOOL>", "Enables/disables force start.", CommandOptionType.SingleValue)]
            public bool? ForceStart { get; set; }

            [Option("-s|--sequential <BOOL>", "Enables/disables sequential download.", CommandOptionType.SingleValue)]
            public bool? Sequential { get; set; }

            [Option("-z|--super-seeding <BOOL>", "Enables/disables super seeding.", CommandOptionType.SingleValue)]
            public bool? SuperSeeding { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                if (AutomaticTorrentManagement == null
                    && FirstLastPriority == null
                    && ForceStart == null
                    && Sequential == null
                    && SuperSeeding == null)
                {
                    var torrent = await GetTorrent();

                    var doc = new Document(
                        new Grid
                        {
                            Stroke = UIHelper.NoneStroke,
                            Columns = { UIHelper.FieldsColumns },
                            Children =
                            {
                                //UIHelper.Row("Automatic Torrent Management", ???),
                                UIHelper.Row("First/last piece prioritized", torrent.FirstLastPiecePrioritized),
                                UIHelper.Row("Force start", torrent.ForceStart),
                                UIHelper.Row("Sequential download", torrent.SequentialDownload),
                                UIHelper.Row("Super seeding", torrent.SuperSeeding),
                            }
                        }
                    ).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc);
                }
                else
                {
                    var torrentTask = new Lazy<Task<TorrentInfo>>(GetTorrent, LazyThreadSafetyMode.ExecutionAndPublication);
                    await Task.WhenAll(
                        SetAutomaticTorrentManagement(),
                        SetForceStart(),
                        SetSuperSeeding(),
                        SetFirstLastPriority(torrentTask),
                        SetSequential(torrentTask));
                }
                return ExitCodes.Success;

                async Task<TorrentInfo> GetTorrent()
                {
                    var torrents = await client.GetTorrentListAsync();
                    return torrents.Single(t => string.Equals(t.Hash, Hash, StringComparison.InvariantCultureIgnoreCase));
                }

                void Print(string header, bool value)
                {
                    console.WriteColored(header, ConsoleColor.Yellow).WriteLine(value.ToString());
                }

                async Task SetFirstLastPriority(Lazy<Task<TorrentInfo>> torrentTask)
                {
                    if (FirstLastPriority != null)
                    {
                        var torrent = await torrentTask.Value;
                        if (torrent.FirstLastPiecePrioritized != FirstLastPriority)
                        {
                            await client.ToggleFirstLastPiecePrioritizedAsync(Hash);
                        }
                    }
                }

                async Task SetSequential(Lazy<Task<TorrentInfo>> torrentTask)
                {
                    if (Sequential != null)
                    {
                        var torrent = await torrentTask.Value;
                        if (torrent.SequentialDownload != Sequential)
                        {
                            await client.ToggleSequentialDownloadAsync(Hash);
                        }
                    }
                }

                async Task SetAutomaticTorrentManagement()
                {
                    if (AutomaticTorrentManagement != null)
                    {
                        await client.SetAutomaticTorrentManagementAsync(Hash, AutomaticTorrentManagement.Value);
                    }
                }

                async Task SetForceStart()
                {
                    if (ForceStart != null)
                    {
                        await client.SetForceStartAsync(Hash, ForceStart.Value);
                    }
                }

                async Task SetSuperSeeding()
                {
                    if (SuperSeeding != null)
                    {
                        await client.SetSuperSeedingAsync(Hash, SuperSeeding.Value);
                    }
                }
            }


        }
    }
}
