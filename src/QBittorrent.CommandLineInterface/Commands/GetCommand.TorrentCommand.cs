using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("torrent", typeof(TorrentCommand))]
    public partial class GetCommand
    {
        [Subcommand("properties", typeof(Properties))]
        public class TorrentCommand : QBittorrentRootCommandBase
        {
            public class Properties : QBittorrentCommandBase
            {
                [Argument(0, "<HASH>", "Full or partial torrent hash")]
                [Required]
                [StringLength(40, MinimumLength = 1)]
                public string Hash { get; set; }

                public async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
                {
                    var client = CreateClient();
                    try
                    {
                        await AuthenticateAsync(client);
                        string fullHash = "";
                        if (Hash.Length < 40)
                        {
                            var torrents = await client.GetTorrenListAsync();
                            var matching = torrents
                                .Where(t => t.Hash.StartsWith(Hash, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();

                            
                            if (matching.Count == 0)
                            {
                                console.WriteLine($"No torrent matching hash {Hash} is found.");
                                return 1;
                            }
                            if (matching.Count == 1)
                            {
                                fullHash = matching[0].Hash;
                            }
                            else
                            {
                                console.WriteLine($"The are several torrents matching partial hash {Hash}:");
                                var numbers = (int) Math.Log10(matching.Count) + 1;
                                var nameWidth = Console.BufferWidth - (numbers + 45);
                                for (int i = 0; i < matching.Count; i++)
                                {
                                    var torrent = matching[i];
                                    var name = torrent.Name.Length < nameWidth
                                        ? torrent.Name
                                        : torrent.Name.Substring(0, nameWidth - 3) + "...";
                                    console.WriteLine($"[{(i + 1).ToString().PadLeft(numbers)}] {torrent.Hash} {name}");
                                }

                                int index = -1;
                                while (index < 0 || index >= matching.Count)
                                {
                                    index = Prompt.GetInt("Please, select the required one:");
                                }
                                fullHash = matching[index].Hash;
                            }
                        }
                        else
                        {
                            fullHash = Hash;
                        }

                        var props = await client.GetTorrentPropertiesAsync(fullHash);
                        var properties =
                            (from prop in typeof(TorrentProperties).GetRuntimeProperties()
                            let attr = prop.GetCustomAttribute<DisplayAttribute>()
                            let name = attr?.Name ?? prop.Name
                            orderby attr?.GetOrder() ?? 0
                            select (name, value: prop.GetValue(props))).ToList();

                        var columnWidth = properties.Max(p => p.name.Length) + 1;
                        foreach (var property in properties)
                        {
                            console.WriteLine($"{(property.name + ":").PadRight(columnWidth)} {property.value}");
                        }

                        return 0;
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            }
        }
    }
}
