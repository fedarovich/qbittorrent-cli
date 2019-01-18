using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
        public class File : ClientRootCommandBase
        {
            [Command(Description = "Gets the torrent contents.")]
            public class List : TorrentSpecificCommandBase
            {
                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    var contents = await client.GetTorrentContentsAsync(Hash);
                    foreach (var (content, id) in contents.Select((x, i) => (x, i)))
                    {
                        UIHelper.PrintObject(new TorrentContentViewModel(content, id));
                        console.WriteLine();
                    }
                    return ExitCodes.Success;
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
        }
    }
}
