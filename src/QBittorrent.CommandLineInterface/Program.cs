using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Attributes;
using QBittorrent.CommandLineInterface.Commands;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand("list", typeof(ListCommand))]
    [Subcommand("download", typeof(DownloadCommand))]
    [Subcommand("get", typeof(GetCommand))]
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
                Console.WriteLine(e);
                throw;
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

            return 0;
        }
    }
}
