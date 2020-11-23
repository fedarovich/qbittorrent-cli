using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Peer))]
    public partial class TorrentCommand
    {
        [Command(Description = "Manages torrent peers.")]
        [Subcommand(typeof(Add))]
        [Subcommand(typeof(List))]
        public class Peer : ClientRootCommandBase
        {
            [Command(Description = "Adds peers to the torrent.")]
            public class Add : TorrentSpecificCommandBase
            {
                [Argument(1, "PEER_1 PEER_2 ... PEER_N", Description = "The peers to add. Each peer must be in format IP:PORT.")]
                [Required]
                [IpEndpointValidation]
                public IList<string> Peers { get; set; }

                [Option("-V|--verbose", "Show verbose results.", CommandOptionType.NoValue)]
                public bool Verbose { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var result = await client.AddTorrentPeersAsync(Hash, Peers);
                    if (Verbose)
                    {
                        console.WriteLineColored($"Successfully added {result.Added} peer(s).", ColorScheme.Current.Normal);
                    }

                    if (result.Failed > 0)
                    {
                        console.WriteLineColored($"Failed to add {result.Failed} peer(s).", ColorScheme.Current.Warning);
                    }

                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Shows the list of torrent peers.")]
            public class List : TorrentSpecificListCommandBase<PeerPartialInfoViewModel>
            {
                private static readonly Dictionary<string, Func<object, object>> CustomFormatters;

                static List()
                {
                    CustomFormatters = new Dictionary<string, Func<object, object>>
                    {
                        [nameof(PeerPartialInfoViewModel.Files)] = FormatFiles,
                    };
                }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var response = await client.GetPeerPartialDataAsync(Hash);
                    if (response == null)
                        return ExitCodes.Failure;

                    var peers = response.PeersChanged?.Values ?? Enumerable.Empty<PeerPartialInfo>();

                    Print(peers.Select(p => new PeerPartialInfoViewModel(p)));

                    return ExitCodes.Success;
                }


                protected override void PrintTable(IEnumerable<PeerPartialInfoViewModel> peers)
                {
                    var doc = new Document(
                        new Grid
                        {
                            Columns =
                            {
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Star(1)},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Auto},
                            },
                            Children =
                            {
                                UIHelper.Header("CC"),
                                UIHelper.Header("Endpoint"),
                                UIHelper.Header("Client"),
                                UIHelper.Header("Progress"),
                                UIHelper.Header("DL Speed", TextAlign.Center),
                                UIHelper.Header("UL Speed", TextAlign.Center),
                                UIHelper.Header("DL", TextAlign.Center, 8),
                                UIHelper.Header("UL", TextAlign.Center, 8),
                                peers.Select(p => new[]
                                {
                                    new Cell(p.CountryCode),
                                    new Cell(p.Endpoint),
                                    new Cell(p.Client),
                                    new Cell($"{p.Progress:P0}"),
                                    new Cell(FormatSpeed(p.DownloadSpeed).PadLeft(10)),
                                    new Cell(FormatSpeed(p.UploadSpeed).PadLeft(10)),
                                    new Cell(FormatData(p.Downloaded).PadLeft(8)),
                                    new Cell(FormatData(p.Uploaded).PadLeft(8)),
                                })
                            },
                            Stroke = LineThickness.Single
                        }
                    ).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc);
                }

                protected override IReadOnlyDictionary<string, Func<object, object>> ListCustomFormatters => CustomFormatters;

                private string FormatData(long? amount)
                {
                    if (amount == null)
                    {
                        return string.Empty;
                    }

                    if (amount < 1024)
                    {
                        return $"{amount}  B";
                    }

                    if (amount < 1024 * 1024)
                    {
                        return $"{amount / 1024} kB";
                    }

                    if (amount < 1024 * 1024 * 1024)
                    {
                        return $"{amount / (1024 * 1024)} MB";
                    }

                    return $"{amount / (1024 * 1024 * 1024)} GB";
                }

                private string FormatSpeed(int? speed)
                {
                    if (speed == null)
                    {
                        return string.Empty;
                    }

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

                private static object FormatFiles(object list)
                {
                    if (list is not IReadOnlyList<string> files)
                        return string.Empty;

                    return string.Join(Environment.NewLine, files);
                }
            }
        }
    }
}
