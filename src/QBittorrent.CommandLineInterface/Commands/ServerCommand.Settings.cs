using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("settings", typeof(Settings))]
    public partial class ServerCommand
    {
        [Subcommand("downloads", typeof(Downloads))]
        [Subcommand("email", typeof(Email))]
        [Subcommand("connection", typeof(Connection))]
        [Subcommand("proxy", typeof(Proxy))]
        [Subcommand("speed", typeof(Speed))]
        [Subcommand("privacy", typeof(Privacy))]
        [Subcommand("queue", typeof(Queue))]
        [Subcommand("seeding", typeof(Seeding))]
        [Subcommand("web", typeof(WebInterface))]
        public partial class Settings
        {
            [AttributeUsage(AttributeTargets.Property)]
            private class IgnoreAttribute : Attribute
            {
            }

            public abstract class SettingsCommand<T> : AuthenticatedCommandBase
            {
                protected const string ExtendedHelp =
                    "\nRun this command without options too see the current settings.";

                protected Dictionary<string, Func<object, string>> CustomFormatters { get; } = new Dictionary<string, Func<object, string>>();

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
                    UIHelper.PrintObject(vm, CustomFormatters);
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

            [Command(Description = "Manages connection settings.", ExtendedHelpText = ExtendedHelp)]
            public class Connection : SettingsCommand<ConnectionViewModel>
            {
                public Connection()
                {
                    CustomFormatters[nameof(ConnectionViewModel.BittorrentProtocol)] =
                        value => Client.BittorrentProtocol.Both.Equals(value) ? "TCP and uTP" : null;
                }

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

            [Command(Description = "Manages proxy settings.", ExtendedHelpText = ExtendedHelp)]
            public class Proxy : SettingsCommand<ProxyViewModel>
            {
                public Proxy()
                {
                    CustomFormatters[nameof(ProxyViewModel.ProxyType)] =
                        value => value != null ? (Enum.IsDefined(typeof(ProxyType), value) ? value.ToString() : "None") : null;
                }

                [Option("-t|--type <TYPE>", "Proxy type (None|Http|HttpAuth|Socks4|Socks5|Socks5Auth)", CommandOptionType.SingleValue)]
                public ProxyType? ProxyType { get; set; }

                [Option("-a|--address <ADDRESS>", "Address", CommandOptionType.SingleValue)]
                public string ProxyAddress { get; set; }

                [Option("--port <PORT>", "Port", CommandOptionType.SingleValue)]
                [Range(1, 65535)]
                public int? ProxyPort { get; set; }

                [Option("-c|--proxy-peer-connections <BOOL>", "Use proxy for peer connections", CommandOptionType.SingleValue)]
                public bool? ProxyPeerConnections { get; set; }

                [Option("-f|--force-proxy <BOOL>", "Disable connections not supported by proxies", CommandOptionType.SingleValue)]
                public bool? ForceProxy { get; set; }

                [Option("-u|--proxy-username <USERNAME>", "Proxy username", CommandOptionType.SingleValue)]
                public string ProxyUsername { get; set; }

                [Option("-p|--proxy-password <PASSWORD>", "Proxy password", CommandOptionType.SingleValue)]
                public string ProxyPassword { get; set; }

                [Option("-P|--ask-proxy-password", "Ask user to enter SMTP server password.", CommandOptionType.NoValue)]
                [Ignore]
                public bool AskForProxyPassword { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (AskForProxyPassword)
                    {
                        ProxyPassword = console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your SMTP server password: ");
                    }

                    return Task.CompletedTask;
                }
            }

            [Command(Description = "Manages speed limits.", ExtendedHelpText = ExtendedHelp)]
            public class Speed : SettingsCommand<SpeedViewModel>
            {
                private (int? hour, int? minute) _from;
                private (int? hour, int? minute) _to;

                [Option("-d|--download <SPEED>", "Download speed limit (bytes/s). Pass 0 to disable the limit.", CommandOptionType.SingleValue)]
                public int? DownloadLimit { get; set; }

                [Option("-u|--upload <SPEED>", "Upload speed limit (bytes/s). Pass 0 to disable the limit.", CommandOptionType.SingleValue)]
                public int? UploadLimit { get; set; }

                [Option("-D|--alt-download <SPEED>", "Alternative download speed limit (bytes/s). Pass 0 to disable the limit.", CommandOptionType.SingleValue)]
                public int? AlternativeDownloadLimit { get; set; }

                [Option("-U|--alt-upload <SPEED>", "Alternative upload speed limit (bytes/s). Pass 0 to disable the limit.", CommandOptionType.SingleValue)]
                public int? AlternativeUploadLimit { get; set; }

                [Option("-s|--enable-scheduler <BOOL>", "Apply alternative limits with scheduler", CommandOptionType.SingleValue)]
                public bool? SchedulerEnabled { get; set; }

                [Option("-f|--from <TIME>", "Apply alternative limits from time. Use either your local format or HH:mm", CommandOptionType.SingleValue)]
                [Ignore]
                public string ScheduleFrom { get; set; }

                [Option("-t|--to <TIME>", "Apply alternative limits to time. Use either your local format or HH:mm", CommandOptionType.SingleValue)]
                [Ignore]
                public string ScheduleTo { get; set; }

                [Option("-S|--day <DAY>", "Apply alternative limits on day (Every|Weekday|Weekend|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)", CommandOptionType.SingleValue)]
                public SchedulerDay? SchedulerDays { get; set; }

                [Option("-m|--limit-utp <BOOL>", "Apply rate limit to uTP protocol", CommandOptionType.SingleValue)]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? LimitUTPRate { get; set; }

                [Option("-o|--limit-tcp <BOOL>", "Apply rate limit to TCP overhead", CommandOptionType.SingleValue)]
                public bool? LimitTcpOverhead { get; set; }

                protected override void CustomFillPrefences(Preferences preferences)
                {
                    base.CustomFillPrefences(preferences);
                    (preferences.ScheduleFromHour, preferences.ScheduleFromMinute) = _from;
                    (preferences.ScheduleToHour, preferences.ScheduleToMinute) = _to;
                }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    _from = TryParseTime(ScheduleFrom);
                    _to = TryParseTime(ScheduleTo);

                    return base.Prepare(client, app, console);

                    (int? hour, int? minute) TryParseTime(string input)
                    {
                        const DateTimeStyles styles = 
                            DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.NoCurrentDateDefault;

                        if (input == null)
                            return (null, null);

                        DateTime dt;
                        if (DateTime.TryParseExact(input, "t", CultureInfo.CurrentCulture, styles, out dt))
                            return (dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes);
                        if (DateTime.TryParseExact(input, "t", CultureInfo.InvariantCulture, styles, out dt))
                            return (dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes);
                        throw new InvalidOperationException($"Invalid time format: \"{input}\". Please, specify time either in your local format or as HH:mm.");
                    }
                }
            }

            [Command(Description = "Manages BitTorrent privacy settings.", ExtendedHelpText = ExtendedHelp)]
            public class Privacy : SettingsCommand<PrivacyViewModel>
            {
                [Option("-d|--dht <BOOL>", "Enable/disable DHT", CommandOptionType.SingleValue)]
                public bool? DHT { get; set; }

                //public bool? DHTSameAsBT { get; set; }

                //public int? DHTPort { get; set; }

                [Option("-p|--pex <BOOL>", "Enable/disable Peer Exchange", CommandOptionType.SingleValue)]
                public bool? PeerExchange { get; set; }

                [Option("-l|--lpd <BOOL>", "Enable/disable Local Peer Discovery", CommandOptionType.SingleValue)]
                public bool? LocalPeerDiscovery { get; set; }

                [Option("-e|--encryption <MODE>", "Encryption mode (ForceOff|Prefer|ForceOn)", CommandOptionType.SingleValue)]
                public Encryption? Encryption { get; set; }

                [Option("-a|--anonymous <BOOL>", "Enable/disable anonymous mode", CommandOptionType.SingleValue)]
                public bool? AnonymousMode { get; set; }
            }

            [Command(Description = "Manages BitTorrent queueing settings.", ExtendedHelpText = ExtendedHelp)]
            public class Queue : SettingsCommand<QueueViewModel>
            {
                [Option("-q|--queueing <BOOL>", "Enable/disable torrent queueing", CommandOptionType.SingleValue)]
                public bool? QueueingEnabled { get; set; }

                [Option("-d|--max-downloads <COUNT>", "Maximum active downloads", CommandOptionType.SingleValue)]
                public int? MaxActiveDownloads { get; set; }

                [Option("-t|--max-torrents <COUNT>", "Maximum active torrents", CommandOptionType.SingleValue)]
                public int? MaxActiveTorrents { get; set; }

                [Option("-u|--max-uploads <COUNT>", "Maximum active uploads", CommandOptionType.SingleValue)]
                public int? MaxActiveUploads { get; set; }

                [Option("-n|--no-slow <BOOL>", "Do not count slow torrents in these limits", CommandOptionType.SingleValue)]
                public bool? DoNotCountSlowTorrents { get; set; }
            }

            [Command(Description = "Manages BitTorrent seeding settings.", ExtendedHelpText = ExtendedHelp)]
            public class Seeding : SettingsCommand<SeedingViewModel>
            {
                [Option("-R|--max-ratio-enabled <BOOL>", "Enable/disable maximal ratio limit", CommandOptionType.SingleValue)]
                public bool? MaxRatioEnabled { get; set; }

                [Option("-r|--max-ratio <VALUE>", "Maximal ratio", CommandOptionType.SingleValue)]
                [Range(-1d, Double.MaxValue)]
                public double? MaxRatio { get; set; }

                [Option("-S|--max-seeding-time-enabled <BOOL>", "Enable/disable maximal seeding time limit", CommandOptionType.SingleValue)]
                public bool? MaxSeedingTimeEnabled { get; set; }

                [Option("-s|--max-seeding-time <MINUTES>", "Maximal seeding time in minutes", CommandOptionType.SingleValue)]
                [Range(-1, int.MaxValue)]
                public int? MaxSeedingTime { get; set; }

                [Option("-a|--action <ACTION>", "Action to perform when maximal ratio or seeding time limit is reached (Pause|Remove)", 
                    CommandOptionType.SingleValue)]
                public MaxRatioAction? MaxRatioAction { get; set; }
            }

            [Command(Description = "Manages web UI and API settings.")]
            public class WebInterface : SettingsCommand<WebInterfaceViewModel>
            {
                public WebInterface()
                {
                    CustomFormatters[nameof(WebInterfaceViewModel.Locale)] = FormatLanguage;
                    //CustomFormatters[nameof(WebInterfaceViewModel.WebUISslCertificate)] = FormatCertificate;

                    string FormatLanguage(object arg)
                    {
                        if (!(arg is string name))
                            return null;

                        try
                        {
                            var englishName = CultureInfo.GetCultureInfo(name.Replace('_', '-')).EnglishName;
                            return $"{name} ({englishName})";
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }

                [Option("-l|--lang <LANGUAGE>", "Web UI language", CommandOptionType.SingleValue)]
                [MinLength(2)]
                public string Locale { get; set; }

                [Option("-a|--address <ADDRESS>", "Web interface address. Pass empty string to listen on any address.", CommandOptionType.SingleValue)]
                public string WebUIAddress { get; set; }

                [Option("-p|--port <PORT>", "Web interface port", CommandOptionType.SingleValue)]
                [Range(1, 65535)]
                public int? WebUIPort { get; set; }

                [Option("-d|--domain <DOMAIN>", "Web interface domain. Pass empty string to listen on any domain.", CommandOptionType.SingleValue)]
                public string WebUIDomain { get; set; }

                [Option("-u|--upnp <BOOL>", "Use UPnP/NAT-PMP", CommandOptionType.SingleValue)]
                public bool? WebUIUpnp { get; set; }

                [Option("-s|--https <BOOL>", "Use HTTPS", CommandOptionType.SingleValue)]
                public bool? WebUIHttps { get; set; }

                //[Display(Name = "SSL Certificate")]
                //public string WebUISslCertificat { get; set; }

                //[Display(Name = "SSL Key")]
                //[DisplayFormat(DataFormatString = "**********")]
                //public string WebUISslKey { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (WebUIAddress?.Trim() == string.Empty)
                    {
                        WebUIAddress = "*";
                    }

                    if (WebUIDomain?.Trim() == string.Empty)
                    {
                        WebUIDomain = "*";
                    }

                    return Task.CompletedTask;
                }
            }
        }
    }
}
