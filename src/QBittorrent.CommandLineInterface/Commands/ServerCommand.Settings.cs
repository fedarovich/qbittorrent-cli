using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.Formats;
using QBittorrent.CommandLineInterface.ViewModels.ServerPreferences;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Settings))]
    public partial class ServerCommand
    {
        [Subcommand(typeof(Downloads))]
        [Subcommand(typeof(Email))]
        [Subcommand(typeof(Connection))]
        [Subcommand(typeof(Proxy))]
        [Subcommand(typeof(Speed))]
        [Subcommand(typeof(Privacy))]
        [Subcommand(typeof(Queue))]
        [Subcommand(typeof(Seeding))]
        [Subcommand(typeof(Dns))]
        [Subcommand(typeof(AutoTorrentManagement))]
        [Command(Description = "Manage qBittorrent server settings.")]
        public partial class Settings : ClientRootCommandBase
        {
            [AttributeUsage(AttributeTargets.Property)]
            private class NoAutoSetAttribute : Attribute
            {
            }

            [AttributeUsage(AttributeTargets.Property)]
            private class IgnoreAttribute : Attribute
            {
            }

            [AttributeUsage(AttributeTargets.Property)]
            private class MinApiVersionAttribute : Attribute
            {
                public MinApiVersionAttribute(string minVersion, string message)
                {
                    MinVersion = ApiVersion.Parse(minVersion);
                    Message = message ?? throw new ArgumentNullException(nameof(message));
                }

                public ApiVersion MinVersion { get; }

                public string Message { get; }
            }

            public abstract class SettingsCommand<T> : AuthenticatedFormattableCommandBase<T>
            {
                protected const string ExtendedHelp =
                    "\nRun this command without options too see the current settings.";

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await Prepare(client, app, console);

                    var props =
                        (from prop in GetType().GetTypeInfo().DeclaredProperties
                         let option = prop.GetCustomAttribute<OptionAttribute>()
                         where option != null
                         where prop.GetCustomAttribute<IgnoreAttribute>() == null
                         let value = prop.GetValue(this)
                         where value != null && (option.OptionType != CommandOptionType.NoValue || !false.Equals(value))
                         let autoSet = prop.GetCustomAttribute<NoAutoSetAttribute>() == null
                         let minApiVersion = prop.GetCustomAttribute<MinApiVersionAttribute>()
                         select (prop.Name, value, autoSet, minApiVersion))
                        .ToList();

                    var versionSpecificProps = props
                        .Where(p => p.minApiVersion != null)
                        .ToLookup(
                            p => (p.minApiVersion.MinVersion, p.minApiVersion.Message),
                            p => p.value);
                    foreach (var pair in versionSpecificProps)
                    {
                        var (minVersion, message) = pair.Key;
                        var values = pair.ToArray();
                        await WarnIfNotSupported(client, console, minVersion, message, values);
                    }

                    if (props.Any())
                    {
                        var prefs = new Preferences();
                        foreach (var prop in props.Where(p => p.autoSet))
                        {
                            typeof(Preferences).GetProperty(prop.Name).SetValue(prefs, prop.value);
                        }

                        CustomFillPreferences(prefs);
                        await client.SetPreferencesAsync(prefs);
                    }
                    else
                    {
                        var prefs = await client.GetPreferencesAsync();
                        PrintPreferences(client, prefs);
                    }

                    return ExitCodes.Success;
                }

                protected async Task WarnIfNotSupported(QBittorrentClient client, IConsole console,
                    ApiVersion minVersion,
                    string message,
                    params object[] properties)
                {
                    if (properties == null || properties.All(p => p == null))
                        return;

                    if (await client.GetApiVersionAsync() < minVersion)
                    {
                        console.WriteLineColored(message, ColorScheme.Current.Warning);
                    }
                }

                protected virtual Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    return Task.CompletedTask;
                }

                protected virtual void CustomFillPreferences(Preferences preferences)
                {
                }

                protected virtual void PrintPreferences(QBittorrentClient client, Preferences preferences)
                {
                    var vm = (T)Activator.CreateInstance(typeof(T), preferences);
                    Print(vm);
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

                [Option("-S|--create-subfolder <BOOL>", "Create subfolder for multi-file torrents", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"create-subfolder\" option requires qBittorrent 4.1.5 or later.")]
                public bool? CreateTorrentSubfolder { get; set; }

                [Option("-P|--add-paused <BOOL>", "Add new torrents in paused state", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"add-paused\" option requires qBittorrent 4.1.5 or later.")]
                public bool? AddTorrentPaused { get; set; }

                [Option("-d|--delete-torrent-file <VALUE>", "Delete .torrent files after added (Never|IfAdded|Always)", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"delete-torrent-file\" option requires qBittorrent 4.1.5 or later.")]
                public TorrentFileAutoDeleteMode? TorrentFileAutoDeleteMode { get; set; }
            }

            [Command("email", "e-mail", "mail", Description = "Manages e-mail notifications.", ExtendedHelpText = ExtendedHelp)]
            public class Email : SettingsCommand<EmailViewModel>
            {
                [Option("-e|--enabled <BOOL>", "Enables/disables e-mail notifications.", CommandOptionType.SingleValue)]
                public bool? MailNotificationEnabled { get; set; }

                [Option("-a|--address <ADDRESS>", "E-mail address.", CommandOptionType.SingleValue)]
                public string MailNotificationEmailAddress { get; set; }

                [Option("-f|--from <ADDRESS>", "From e-mail address. Requires qBittorrent 4.1.5 or later.", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"from\" option requires qBittorrent 4.1.5 or later.")]
                public string MailNotificationSender { get; set; }
                
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
                [NoAutoSet]
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
                [NoAutoSet]
                public bool AskForProxyPassword { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (AskForProxyPassword)
                    {
                        ProxyPassword = console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your proxy server password: ");
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
                [NoAutoSet]
                public string ScheduleFrom { get; set; }

                [Option("-t|--to <TIME>", "Apply alternative limits to time. Use either your local format or HH:mm", CommandOptionType.SingleValue)]
                [NoAutoSet]
                public string ScheduleTo { get; set; }

                [Option("-S|--day <DAY>", "Apply alternative limits on day (Every|Weekday|Weekend|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)", CommandOptionType.SingleValue)]
                public SchedulerDay? SchedulerDays { get; set; }

                [Option("-m|--limit-utp <BOOL>", "Apply rate limit to uTP protocol", CommandOptionType.SingleValue)]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? LimitUTPRate { get; set; }

                [Option("-o|--limit-tcp <BOOL>", "Apply rate limit to TCP overhead", CommandOptionType.SingleValue)]
                public bool? LimitTcpOverhead { get; set; }

                [Option("-l|--limit-lan <BOOL>", "Apply rate limit to peers on LAN", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"Apply rate limit to peers on LAN\" option requires qBittorrent 4.1.5 or later.")]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? LimitLAN { get; set; }

                protected override void CustomFillPreferences(Preferences preferences)
                {
                    base.CustomFillPreferences(preferences);
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

                [Option("-D|--slow-download-rate <VALUE>", "Slow download rate threshold (KiB/S)", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"slow-download-rate\" option requires qBittorrent 4.1.5 or later.")]
                public int? SlowTorrentDownloadRateThreshold { get; set; }

                [Option("-U|--slow-upload-rate <VALUE>", "Slow upload rate threshold (KiB/S)", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"slow-upload-rate\" option requires qBittorrent 4.1.5 or later.")]
                public int? SlowTorrentUploadRateThreshold { get; set; }

                [Option("-I|--slow-inactivity-time <VALUE>", "Torrent inactivity timeout (seconds)", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"slow-inactivity-time\" option requires qBittorrent 4.1.5 or later.")]
                public int? SlowTorrentInactiveTime { get; set; }
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

            [Command(Description = "Manages dynamic DNS settings.", ExtendedHelpText = ExtendedHelp)]
            public class Dns : SettingsCommand<DnsViewModel>
            {
                [Option("-e|--enabled <BOOL>", "Enable/disable dynamic DNS", CommandOptionType.SingleValue)]
                public bool? DynamicDnsEnabled { get; set; }

                [Option("-s|--service <SERVICE>", "Dynamic DNS service (DynDNS|NoIP)", CommandOptionType.SingleValue)]
                public DynamicDnsService? DynamicDnsService { get; set; }

                [Option("-d|--domain <NAME>", "Domain name", CommandOptionType.SingleValue)]
                [MinLength(1)]
                public string DynamicDnsDomain { get; set; }

                [Option("-u|--dns-username <USERNAME>", "Dynamic DNS username", CommandOptionType.SingleValue)]
                [MinLength(1)]
                public string DynamicDnsUsername { get; set; }

                [Option("-p|--dns-password <PASSWORD>", "Dynamic DNS password", CommandOptionType.SingleValue)]
                public string DynamicDnsPassword { get; set; }

                [Option("-P|--ask-dns-password", "Ask user to enter dynamic DNS password", CommandOptionType.NoValue)]
                [NoAutoSet]
                public bool AskForDynamicDnsPassword { get; set; }

                protected override Task Prepare(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    if (AskForDynamicDnsPassword)
                    {
                        DynamicDnsPassword = console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your dynamic DNS password: ");
                    }

                    return Task.CompletedTask;
                }
            }

            [Command("auto-tmm", "autotmm", Description = "Manages automatic torrent management mode (Auto TMM).")]
            public class AutoTorrentManagement : SettingsCommand<AutoTorrentManagementViewModel>
            {
                [Option("-e|--enabled <BOOL>", "Enable/disable automatic torrent management by default", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"enabled\" option requires qBittorrent 4.1.5 or later.")]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? AutoTMMEnabledByDefault { get; set; }

                [Option("-c|--retain-on-category-change <BOOL>", "Retain auto TMM when category changes", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"retain-on-category-change\" option requires qBittorrent 4.1.5 or later.")]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? AutoTMMRetainedWhenCategoryChanges { get; set; }

                [Option("-d|--retain-on-default-save-path-change <BOOL>", "Retain auto TMM when default save path changes", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"retain-on-default-save-path-change\" option requires qBittorrent 4.1.5 or later.")]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? AutoTMMRetainedWhenDefaultSavePathChanges { get; set; }

                [Option("-s|--retain-on-category-save-path-change <BOOL>", "Retain auto TMM when category save path changes", CommandOptionType.SingleValue)]
                [MinApiVersion("2.2.0", "\"retain-on-category-save-path-change\" option requires qBittorrent 4.1.5 or later.")]
                [SuppressMessage("ReSharper", "InconsistentNaming")]
                public bool? AutoTMMRetainedWhenCategorySavePathChanges { get; set; }
            }
        }
    }
}
