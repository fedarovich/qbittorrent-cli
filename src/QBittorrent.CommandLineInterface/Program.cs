using System;
using System.Diagnostics;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.Commands;
using QBittorrent.CommandLineInterface.Conventions;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand(typeof(CategoryCommand))]
    [Subcommand(typeof(GlobalCommand))]
    [Subcommand(typeof(ServerCommand))]
    [Subcommand(typeof(SettingsCommand))]
    [Subcommand(typeof(NetworkCommand))]
    [Subcommand(typeof(TorrentCommand))]
    [Subcommand(typeof(InspectCommand))]
    [Subcommand(typeof(RssCommand))]
    [Subcommand(typeof(SearchCommand))]
    [HelpOption(Inherited = true)]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    public class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            try
            {
                app.Conventions
                    .UseDefaultConventions()
                    .UsePagerForHelpText(false)
                    .MakeSuggestionsInErrorMessage();
                int code = app.Execute(args);
                return code;
            }
            catch (ApiNotSupportedException e)
            {
                PrintApiNotSupported(e);
                return ExitCodes.Failure;
            }
            catch (TargetInvocationException e) when (e.InnerException is ApiNotSupportedException ex)
            {
                PrintApiNotSupported(ex);
                return ExitCodes.Failure;
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

            void PrintApiNotSupported(ApiNotSupportedException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                var apiVersion = ApiVersion.Parse(ex.RequiredApiVersion.ToString(3));
                var qBittorrentVersion = GetQBittorrentVersion();
                Console.WriteLine(qBittorrentVersion != null
                    ? $"qBittorrent v{qBittorrentVersion} or later is required for this command."
                    : $"A newer version of qBittorrent is required for this command.{Environment.NewLine}API {apiVersion} must be supported.");
                Console.ResetColor();

                string GetQBittorrentVersion()
                {
                    if (apiVersion == new ApiVersion(2, 0 , 0))
                        return "4.1";
                    if (apiVersion == new ApiVersion(2, 0, 1))
                        return "4.1.1";
                    if (apiVersion == new ApiVersion(2, 0, 2))
                        return "4.1.2";
                    if (apiVersion == new ApiVersion(2, 1))
                        return "4.1.3";
                    if (apiVersion == new ApiVersion(2, 1, 1))
                        return "4.1.4";
                    if (apiVersion == new ApiVersion(2, 2))
                        return "4.1.5";
                    if (apiVersion == new ApiVersion(2, 3))
                        return "4.2";
                    return null;
                }
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
