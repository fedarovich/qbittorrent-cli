using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Exceptions;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("move", typeof(Move))]
    public partial class SmartCommand
    {
        [Command(Description = "Moves the torrent to a configured location.")]
        public class Move : TorrentSpecificCommandBase
        {
            [Option("-r|--rule <NAME>", "The name of the rules to analyze. Use * to analyze all the rules.", CommandOptionType.SingleValue)]
            [RegularExpression(@"^([a-zA-Z0-9_]*|\*)$", 
                ErrorMessage = "The rule name can consist only of latin characters, digits and underscore characters.")]
            public string RuleName { get; set; }

            protected override async Task<int> OnExecuteTorrentSpecificAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                try
                {
                    var torrents = await client.GetTorrentListAsync();
                    var torrent = torrents.SingleOrDefault(t =>
                        string.Equals(t.Hash, Hash, StringComparison.InvariantCultureIgnoreCase));
                    if (torrent == null)
                        return ExitCodes.NotFound;

                    var rules = GetRules();
                    // TODO: Implement logic
                    
                    return ExitCodes.Success;
                }
                catch (FileNotFoundException ex)
                {
                    console.WriteLineColored(ex.Message, ConsoleColor.Red);
                    return ExitCodes.InvalidConfiguration;
                }
                catch (JsonValidationException ex)
                {
                    PrintJsonValidationError(console, ex);
                    return ExitCodes.InvalidConfiguration;
                }
            }

            private JArray GetRules()
            {
                return ((JArray)SmartService.Instance.GetConfigurationObject("$.move"));
            }
        }
    }
}
