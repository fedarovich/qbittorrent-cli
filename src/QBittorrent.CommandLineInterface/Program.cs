using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Commands;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand("category", typeof(CategoryCommand))]
    [Subcommand("global", typeof(GlobalCommand))]
    [Subcommand("server", typeof(ServerCommand))]
    [Subcommand("settings", typeof(SettingsCommand))]
    [Subcommand("torrent", typeof(TorrentCommand))]
    [Subcommand("inspect", typeof(InspectCommand))]
    [HelpOption(Inherited = true)]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var app = new CommandLineApplication<Program>();
                app.Conventions.UseDefaultConventions();
                int code = app.Execute(args);
                return code;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(e.InnerException.Message);
                Console.ResetColor();
                return ExitCodes.Failure;
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

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return ExitCodes.Success;
        }

        private string GetVersion()
        {
            var attr = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attr.InformationalVersion;
        }
    }
}
