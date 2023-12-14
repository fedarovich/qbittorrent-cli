using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Folder))]
    public partial class TorrentCommand
    {
        [Command(Description = "Manipulates torrent folders.")]
        [Subcommand(typeof(Rename))]
        public class Folder : ClientRootCommandBase
        {
            [Command(Description = "Renames the folder of the torrent.")]
            public class Rename : TorrentSpecificCommandBase
            {
                [Option("-o|--old-path <OLD_PATH>", "Old folder path.", CommandOptionType.SingleValue)]
                [Required]
                public string OldPath { get; set; }

                [Option("-n|--new-path <NEW_PATH>", "New folder path.", CommandOptionType.SingleValue)]
                [Required]
                public string NewPath { get; set; }

                protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.RenameFolderAsync(Hash, OldPath, NewPath);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
