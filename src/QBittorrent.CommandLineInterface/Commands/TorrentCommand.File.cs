using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.ViewModels;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(File))]
    public partial class TorrentCommand
    {
        [Command(Description = "Gets and manipulates torrent contents.")]
        [Subcommand(typeof(List))]
        [Subcommand(typeof(Priority))]
        [Subcommand(typeof(Rename))]
        public class File : ClientRootCommandBase
        {
            [Command(Description = "Gets the torrent contents.", ExtendedHelpText = FormatHelpText)]
            public class List : TorrentSpecificListCommandBase<TorrentContentViewModel>
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var contents = await client.GetTorrentContentsAsync(Hash);
                    var viewModels = contents.Select((c, i) => new TorrentContentViewModel(c, i));
                    Print(viewModels, true);
                    return ExitCodes.Success;
                }

                protected override void PrintTable(IEnumerable<TorrentContentViewModel> list)
                {
                    var doc = new Document(
                            new Grid
                            {
                                Columns =
                                {
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Star(1)},
                                    new Column {Width = GridLength.Auto},
                                    new Column {Width = GridLength.Auto},
                                },
                                Children =
                                {
                                    UIHelper.Header("Id"),
                                    UIHelper.Header("Name"),
                                    UIHelper.Header("Size"),
                                    UIHelper.Header("Progress"),
                                    list.Select(c => new[]
                                    {
                                        new Cell(c.Id), 
                                        new Cell(c.Name),
                                        new Cell(c.Size.ToString("N0")),
                                        new Cell(c.Progress.ToString("P0")), 
                                    })
                                },
                                Stroke = LineThickness.Single
                            })
                        .SetColors(ColorScheme.Current.Normal);
                    ConsoleRenderer.RenderDocument(doc);
                }
            }

            [Command(Description = "Gets or sets file priority.")]
            public class Priority : TorrentSpecificCommandBase
            {
                [Option("-f|--file <FILE_ID>", "File Id. Use \"torrent file list <HASH>\" command to get the possible values.", CommandOptionType.SingleValue)]
                [Required]
                public int File { get; set; }

                [Option("-s|--set <PRIORITY>", "Sets the file priority (SKIP|NORMAL|HIGH|MAXIMAL).", CommandOptionType.SingleValue)]
                [EnumValidation(typeof(TorrentContentPriority), AllowEmpty = true)]
                public string Value { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Enum.TryParse(Value, true, out TorrentContentPriority priority))
                    {
                        await client.SetFilePriorityAsync(Hash, File, priority);
                    }
                    else
                    {
                        var contents = (await client.GetTorrentContentsAsync(Hash))?.ToList();
                        console.WriteLineColored(contents?[File]?.Priority.ToString(), ColorScheme.Current.Normal);
                    }

                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Renames the file.")]
            public class Rename : TorrentSpecificCommandBase
            {
                [Option("-f|--file <FILE_ID>", "File Id. Use \"torrent file list <HASH>\" command to get the possible values.", CommandOptionType.SingleValue)]
                [Required]
                public int File { get; set; }

                [Option("-n|--name <NEW_NAME>", "New file name.", CommandOptionType.SingleValue)]
                [Required]
                public string NewName { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.RenameFileAsync(Hash, File, NewName);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
