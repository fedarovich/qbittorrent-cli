using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("settings", typeof(Settings))]
    public partial class ServerCommand
    {
        [Subcommand("downloads", typeof(Downloads))]
        [Subcommand("monitored-folder", typeof(MonitoredFolder))]
        [Subcommand("email", typeof(Email))]
        [Subcommand("connection", typeof(Connection))]
        public class Settings
        {
            [AttributeUsage(AttributeTargets.Property)]
            private class IgnoreAttribute : Attribute
            {
            }

            public abstract class SettingsCommand<T> : AuthenticatedCommandBase
            {
                protected const string ExtendedHelp =
                    "\nRun this command without options too see the current settings.";

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await Prepare(client, app, console);

                    var props =
                        (from prop in GetType().GetTypeInfo().DeclaredProperties
                         where prop.GetCustomAttribute<OptionAttribute>() != null
                         where prop.GetCustomAttribute<IgnoreAttribute>() == null
                         let value = prop.GetValue(this)
                         where value != null
                         select (prop.Name, value))
                        .ToList();

                    if (props.Any())
                    {
                        var prefs = new Preferences();
                        foreach (var prop in props)
                        {
                            typeof(Preferences).GetProperty(prop.Name).SetValue(prefs, prop.value);
                        }

                        CustomFillPrefences(prefs);
                        await client.SetPreferencesAsync(prefs);
                    }
                    else
                    {
                        var prefs = await client.GetPreferencesAsync();
                        PrintPreferences(client, prefs);
                    }

                    return ExitCodes.Success;
                }

                protected virtual Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    return Task.CompletedTask;
                }

                protected virtual void CustomFillPrefences(Preferences preferences)
                {
                }

                protected virtual void PrintPreferences(QBittorrentClient client, Preferences preferences)
                {
                    var vm = (T)Activator.CreateInstance(typeof(T), preferences);
                    UIHelper.PrintObject(vm);
                }
            }

            [Command(Description = "Manages folders and options for downloads.", ExtendedHelpText = ExtendedHelp)]
            public class Downloads : SettingsCommand<DownloadsViewModel>
            {
                [Option("-s|--save-folder <PATH>", "Default folder for downloaded files.", CommandOptionType.SingleValue)]
                public string SavePath { get; set; }

                [Option("-t|--temp-folder <PATH>", "Folder for incomplete files.", CommandOptionType.SingleValue)]
                public string TempPath { get; set; }

                [Option("-T|--temp-folder-enabled <BOOL>", "Enable/disable folder for incomplete files.", CommandOptionType.SingleValue)]
                public bool? TempPathEnabled { get; set; }

                [Option("-p|--preallocate <BOOL>", "Preallocate disk space for files.", CommandOptionType.SingleValue)]
                public bool? PreallocateAll { get; set; }

                [Option("-x|--append-extension <BOOL>", "Append .!bq extension to incomplete files.", CommandOptionType.SingleValue)]
                public bool? AppendExtensionToIncompleteFiles { get; set; }

                [Option("-e|--export-folder <PATH>", "Folder to copy .torrent files to.", CommandOptionType.SingleValue)]
                public string ExportDirectory { get; set; }

                [Option("-f|--finished-folder <PATH>", "Folder to copy finished .torrent files to.", CommandOptionType.SingleValue)]
                public string ExportDirectoryForFinished { get; set; }

                [Option("-a|--autorun-program <CMD>", "Path to the program to run for the complete torrent and its parameters.", CommandOptionType.SingleValue)]
                public string AutorunProgram { get; set; }

                [Option("-A|--autorun-enabled <BOOL>", "Enable/disables running external program for the complete torrent.", CommandOptionType.SingleValue)]
                public bool? AutorunEnabled { get; set; }
            }

            [Command(Description = "TODO:")]
            public class MonitoredFolder
            {
                // TODO: Implement.
            }

            [Command(Description = "Manages e-mail notifications.", ExtendedHelpText = ExtendedHelp)]
            public class Email : SettingsCommand<EmailViewModel>
            {
                [Option("-e|--enabled <BOOL>", "Enables/disables e-mail notifications.", CommandOptionType.SingleValue)]
                public bool? MailNotificationEnabled { get; set; }

                [Option("-a|--address <ADDRESS>", "E-mail address.", CommandOptionType.SingleValue)]
                public string MailNotificationEmailAddress { get; set; }

                [Option("-s|--smtp <SERVER>", "SMTP server URL.", CommandOptionType.SingleValue)]
                public string MailNotificationSmtpServer { get; set; }

                [Option("-S|--ssl <BOOL>", "Enables/disable SSL for sending e-mails.", CommandOptionType.SingleValue)]
                public bool? MailNotificationSslEnabled { get; set; }

                [Option("-A|--smtp-auth <BOOL>", "Enables/disables sending credentials to SMTP server.", CommandOptionType.SingleValue)]
                public bool? MailNotificationAuthenticationEnabled { get; set; }

                [Option("-u|--smtp-username", "SMTP server username.", CommandOptionType.SingleValue)]
                public string MailNotificationUsername { get; set; }

                [Option("-p|--smtp-password", "SMTP server password.", CommandOptionType.SingleValue)]
                public string MailNotificationPassword { get; set; }

                [Option("-P|--ask-smtp-password", "Ask user to enter SMTP server password.", CommandOptionType.NoValue)]
                [Ignore]
                public bool AskForSmtpPassword { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (AskForSmtpPassword)
                    {
                        MailNotificationPassword = console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your SMTP server password: ");
                    }

                    return Task.CompletedTask;
                }
            }

            public class Connection : SettingsCommand<ConnectionViewModel>
            {
                [Option("-b|--protocol <PROTOCOL>", "Bittorrent protocol: TCP, uTP, Both", CommandOptionType.SingleValue)]
                public BittorrentProtocol? BittorrentProtocol { get; set; }

                [Option("-p|--listen-port <PORT>", "Incoming connections port.", CommandOptionType.SingleValue)]
                [Range(1, 65535)]
                public int? ListenPort { get; set; }

                [Option("-r|--random-port <BOOL>", "Use different port on each startup.", CommandOptionType.SingleValue)]
                public bool? RandomPort { get; set; }

                [Option("--upnp <BOOL>", "Use UPnP / NAT-PMP port forwarding.", CommandOptionType.SingleValue)]
                public bool? UpnpEnabled { get; set; }

                [Option("-C|--max-connections <INT>", "Maximal number of connections. Use -1 to disable the limit.", CommandOptionType.SingleValue)]
                [Range(-1, int.MaxValue)]
                public int? MaxConnections { get; set; }

                [Option("-c|--max-connections-per-torrent <INT>", "Maximal number of connections per torrent. Use -1 to disable the limit.", CommandOptionType.SingleValue)]
                [Range(-1, int.MaxValue)]
                public int? MaxConnectionsPerTorrent { get; set; }

                [Option("-U|--max-uploads <INT>", "Maximal number of upload slots. Use -1 to disable the limit.", CommandOptionType.SingleValue)]
                [Range(-1, int.MaxValue)]
                public int? MaxUploads { get; set; }

                [Option("-u|--max-uploads-per-torrent <INT>", "Maximal number of upload slots per torrent. Use -1 to disable the limit.", CommandOptionType.SingleValue)]
                [Range(-1, int.MaxValue)]
                public int? MaxUploadsPerTorrent { get; set; }
            }
        }
    }
}
