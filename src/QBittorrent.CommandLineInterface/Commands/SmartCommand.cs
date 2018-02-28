using System;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Manage torrents based on scripted rules.")]
    [Subcommand("init", typeof(Init))]
    public partial class SmartCommand : ClientRootCommandBase
    {
        [Command(Description = "Initializes the smart command settings for the current user.")]
        public class Init
        {
            public int OnExecute(IConsole console)
            {
                bool initialized = SmartService.Instance.Initialize();
                if (initialized)
                {
                    console.WriteLine("The smart command settings has been initialized.");
                }
                else
                {
                    console.WriteLineColored("The smart command settings are already initialized.", ConsoleColor.Yellow);
                }

                console.WriteLine("Run \"qbt smart configure\" to edit the script.");

                return ExitCodes.Success;
            }
        }
    }
}
