using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class TorrentSpecificCommandBase : AuthenticatedCommandBase
    {
        [Argument(0, "<HASH>", "Full or partial torrent hash")]
        [Required]
        [StringLength(40, MinimumLength = 1)]
        public virtual string Hash { get; set; }

        protected virtual bool AllowAll => false;

        protected bool IsAll => "ALL".Equals(Hash, StringComparison.InvariantCultureIgnoreCase);

        protected sealed override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
        {
            if (Hash.Length < 40 && !(AllowAll && IsAll))
            {
                var torrents = await client.GetTorrentListAsync();
                var matching = torrents
                    .Where(t => t.Hash.StartsWith(Hash, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();


                if (matching.Count == 0)
                {
                    console.WriteLineColored($"No torrent matching hash {Hash} is found.", ColorScheme.Current.Warning);
                    return ExitCodes.NotFound;
                }
                if (matching.Count == 1)
                {
                    Hash = matching[0].Hash;
                }
                else
                {
                    console.WriteLineColored($"The are several torrents matching partial hash {Hash}:", ColorScheme.Current.Normal);
                    var numbers = (int)Math.Log10(matching.Count) + 1;
                    var nameWidth = Console.BufferWidth - (numbers + 45);
                    for (int i = 0; i < matching.Count; i++)
                    {
                        var torrent = matching[i];
                        var name = torrent.Name.Length < nameWidth
                            ? torrent.Name
                            : torrent.Name.Substring(0, nameWidth - 3) + "...";
                        console.WriteLineColored($"[{(i + 1).ToString().PadLeft(numbers)}] {torrent.Hash} {name}", ColorScheme.Current.Normal);
                    }

                    int index = 0;
                    while (index <= 0 || index > matching.Count)
                    {
                        index = Prompt.GetInt("Please, select the required one:");
                    }
                    Hash = matching[index - 1].Hash;
                }
            }
            
            return await OnExecuteTorrentSpecificAsync(client, app, console);
        }

        protected  abstract Task<int> OnExecuteTorrentSpecificAsync(
            QBittorrentClient client,
            CommandLineApplication app, 
            IConsole console);
    }
}
