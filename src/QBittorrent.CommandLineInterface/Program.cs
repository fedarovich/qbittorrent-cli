using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Commands;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand("category", typeof(CategoryCommand))]
    [Subcommand("global", typeof(GlobalCommand))]
    [Subcommand("server", typeof(ServerCommand))]
    [Subcommand("settings", typeof(SettingsCommand))]
    [Subcommand("torrent", typeof(TorrentCommand))]
    [HelpOption]
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                int code = await CommandLineApplication.ExecuteAsync<Program>(args);
                return code;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.Message);
                Console.ResetColor();
                return ExitCodes.Failure;
            }
            finally
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
#endif
            }
        }

        [Option("--version", "Displays the program version.", CommandOptionType.NoValue)]
        public bool ShowVersion { get; set; }

        int OnExecute(CommandLineApplication app, IConsole console)
        {
            if (ShowVersion)
            {
                var attr = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                console.WriteLine(attr.InformationalVersion);
            }
            else
            {
                app.ShowHelp();
            }

            return ExitCodes.Success;
        }
    }
}
