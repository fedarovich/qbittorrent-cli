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

            [Command(Description = "Gets or sets torrent's files priority.")]
            public class Priority : TorrentSpecificListCommandBase<TorrentFilePriorityViewModel>
            {
                [Option("-f|--file <FILE_ID>", "File Id. Use \"torrent file list <HASH>\" command to get the possible values.", CommandOptionType.MultipleValue)]
                [Required]
                public IList<int> Files { get; set; }

                [Option("-s|--set <PRIORITY>", "Sets the file priority (SKIP|NORMAL|HIGH|MAXIMAL).", CommandOptionType.SingleValue)]
                [EnumValidation(typeof(TorrentContentPriority), AllowEmpty = true)]
                public string Value { get; set; }

                [Option("-F|--format <LIST_FORMAT>", "Output format: plain|table|list|csv|json", CommandOptionType.SingleValue)]
                public override string Format { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (Enum.TryParse(Value, true, out TorrentContentPriority priority))
                    {
                        await client.SetFilePriorityAsync(Hash, Files, priority);
                    }
                    else
                    {
                        var contents = await client.GetTorrentContentsAsync(Hash);
                        if (string.IsNullOrEmpty(Format) || Format.Equals("plain", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var file in Files)
                            {
                                console.WriteLineColored(contents?[file]?.Priority.ToString(), ColorScheme.Current.Normal);
                            }
                        }
                        else
                        {
                            var idSet = Files.ToHashSet();
                            var viewModels = contents
                                .Select((c, i) => new TorrentFilePriorityViewModel(c, i))
                                .Where(vm => idSet.Contains(vm.Id));
                            Print(viewModels);
                        }
                    }

                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Renames the file.")]
            public class Rename : TorrentSpecificCommandBase
            {
                [Option("-f|--file <FILE_ID>", "File Id. Use \"torrent file list <HASH>\" command to get the possible values.", CommandOptionType.SingleValue)]
                public int? File { get; set; }

                [Option("-o|--old-name <OLD_NAME>", "Old file path/name.", CommandOptionType.SingleValue)]
                public string OldName { get; set; }

                [Option("-n|--name <NEW_NAME>", "New file path/name.", CommandOptionType.SingleValue)]
                [Required]
                public string NewName { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if ((File == null) == (OldName == null))
                    {
                        throw new InvalidOperationException("Either --file or --old-name option must be specified.");
                    }

                    var version = await client.GetApiVersionAsync();
                    if (version < new ApiVersion(2, 8, 0))
                    {
                        await RenameFileLegacy(client);
                    }
                    else
                    {
                        await RenameFile(client);
                    }
                    return ExitCodes.Success;
                }

                private async Task RenameFileLegacy(QBittorrentClient client)
                {
                    if (File != null)
                    {
                        await client.RenameFileAsync(Hash, File.Value, NewName);
                    }
                    else
                    {
                        var contents = await client.GetTorrentContentsAsync(Hash);
                        var file = contents.Select((content, index) => (content, index)).Single(t => t.content.Name == OldName).index;
                        await client.RenameFileAsync(Hash, file, NewName);
                    }
                }

                private async Task RenameFile(QBittorrentClient client)
                {
                    if (File != null)
                    {
                        var content = await client.GetTorrentContentsAsync(Hash);
                        var oldName = content[File.Value].Name;
                        await client.RenameFileAsync(Hash, oldName, NewName);
                    }
                    else
                    {
                        await client.RenameFileAsync(Hash, OldName, NewName);
                    }
                }
            }
        }
    }
}
