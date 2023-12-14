using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class MultiTorrentCommandBase : AuthenticatedCommandBase
    {
        [Argument(0, "<HASH_1 HASH_2 ... HASH_N>", "Full or partial torrent hashes.")]
        [Required]
        [StringLength(40, MinimumLength = 1)]
        public virtual IList<string> Hashes { get; set; }

        protected virtual bool AllowAll => false;

        protected bool IsAll { get; private set; } 

        protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
        {
            IsAll = AllowAll && Hashes.Any(h => "ALL".Equals(h, StringComparison.OrdinalIgnoreCase));

            if (!IsAll)
            {
                IReadOnlyList<TorrentInfo> torrents = null;
                for (int hashIndex = Hashes.Count - 1; hashIndex >= 0; hashIndex--)
                {
                    var hash = Hashes[hashIndex];
                    if (hash.Length == 40)
                        continue;

                    torrents ??= await client.GetTorrentListAsync();
                    var matching = torrents
                        .Where(t => t.Hash.StartsWith(hash, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    switch (matching.Count)
                    {
                        case 0:
                            console.WriteLineColored($"No torrent matching partial hash {hash} is found.", ColorScheme.Current.Warning);
                            Hashes.RemoveAt(hashIndex);
                            break;
                        case 1:
                            Hashes[hashIndex] = matching[0].Hash;
                            break;
                        case > 1 when Hashes.Count == 1:
                            console.WriteLineColored($"The are several torrents matching partial hash {hash}:", ColorScheme.Current.Normal);
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
                            Hashes[hashIndex] = matching[index - 1].Hash;
                            break;
                        default:
                            throw new InvalidOperationException(
                                $"There are several torrents matching partial hash {hash}. Please, use the full hash or unambiguous partial hash.");
                    }
                }
            }

            return await OnExecuteTorrentSpecificAsync(client, app, console);
        }

        protected abstract Task<int> OnExecuteTorrentSpecificAsync(
            QBittorrentClient client,
            CommandLineApplication app,
            IConsole console);
    }
}
