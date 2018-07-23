using System;
using System.Diagnostics;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Commands;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand("category", typeof(CategoryCommand))]
    [Subcommand("global", typeof(GlobalCommand))]
    [Subcommand("server", typeof(ServerCommand))]
    [Subcommand("settings", typeof(SettingsCommand))]
    [Subcommand("network", typeof(NetworkCommand))]
    [Subcommand("torrent", typeof(TorrentCommand))]
    [Subcommand("inspect", typeof(InspectCommand))]
    [HelpOption(Inherited = true)]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            try
            {
                app.Conventions.UseDefaultConventions();
                int code = app.Execute(args);
                return code;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                PrintError(e.InnerException);
                return ExitCodes.Failure;
            }
            catch (Exception e)
            {
                PrintError(e);
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

            void PrintError(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                if (app.Model.PrintStackTrace)
                {
                    Console.WriteLine(ex);
                }
                else
                {
                    var exception = ex;
                    string prevMessage = null;
                    do
                    {
                        if (exception.Message != prevMessage)
                        {
                            Console.Error.WriteLine(exception.Message);
                            prevMessage = exception.Message;
                        }
                    } while ((exception = exception.InnerException) != null);
                }
                
                Console.ResetColor();
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

        [Option("--print-stacktrace", "Prints exception stacktrace", CommandOptionType.NoValue, ShowInHelpText = false, Inherited = true)]
        public bool PrintStackTrace { get; set; }
    }
}
