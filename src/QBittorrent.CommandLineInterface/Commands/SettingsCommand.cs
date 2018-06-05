using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Gets or sets this application's settings.")]
    [Subcommand("set", typeof(Set))]
    [Subcommand("reset", typeof(Reset))]
    public partial class SettingsCommand
    {
        [Command(Description = "Sets the new value for the specified setting.")]
        [Subcommand("url", typeof(Url))]
        [Subcommand("username", typeof(Username))]
        [Subcommand("password", typeof(Password))]
        [Subcommand("proxy", typeof(Proxy))]
        public class Set
        {
            [Command(Description = "Sets the default server URL.")]
            public class Url
            {
                [Argument(0, "URL", "The default server URL.")]
                [Required(AllowEmptyStrings = false)]
                [Url]
                public string Value { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Url = Value;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Sets the default user name.")]
            public class Username
            {
                [Argument(0, "username", "The default user name.")]
                [Required(AllowEmptyStrings = false)]
                public string Value { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Username = Value;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Sets the default password.", ExtendedHelpText = ExtendedHelp)]
            public class Password
            {
                private const string ExtendedHelp = 
                    "\n" +
                    "It may be not safe to store the password this way.\n" +
                    "This command does not accept the password to set as a parameter." +
                    "It will ask you input your password instead, the input being masked with an asterisk.\n" +
                    "If you want to pass the password to this command in an automation script, use pipe:\n" +
                    "  echo <YOUR_PASSWORD> | qbt settings set password\n";

                [Option("-y|--no-warn", "Do not warn about storing password may be unsecure.", CommandOptionType.NoValue)]
                public bool NoWarn { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    string value;
                    if (console.IsInputRedirected)
                    {
                        value = console.In.ReadLine();
                    }
                    else
                    {
                        if (NoWarn
                            || Prompt.GetYesNo("Storing password is not guaranteed to be secure. Do you want to continue?", false, ConsoleColor.Yellow))
                        {
                            value = Prompt.GetPassword("Please, enter the password: ");
                        }
                        else
                        {
                            return ExitCodes.Cancelled;
                        }
                    }

                    var settings = SettingsService.Instance.Get();
                    settings.Password = value;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Sets the proxy settings.")]
            public class Proxy
            {
                [Option("-a|--address <URL>", "Proxy server address (URL)", CommandOptionType.SingleValue)]
                [Required(AllowEmptyStrings = false)]
                public string Address { get; set; }

                [Option("-u|--username <USERNAME>", "Proxy server user name. Pass empty string to use default credentials.", CommandOptionType.SingleValue)]
                [Required(AllowEmptyStrings = true)]
                public string Username { get; set; }

                [Option("-p|--password <PASSWORD>", "Proxy server password.", CommandOptionType.SingleValue)]
                public string Password { get; set; }

                [Option("-l|--bypass-local", "Bypass proxy for local addresses.", CommandOptionType.NoValue)]
                public bool BypassLocal { get; set; }

                [Option("-b|--bypass <expressions>", 
                    "List of regular expressions that describe URIs that do not use the proxy server when accessed", 
                    CommandOptionType.MultipleValue)]
                public List<string> Bypass { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    if (!string.IsNullOrEmpty(Username))
                    {
                        Password = Password ?? string.Empty;
                    }

                    var settings = SettingsService.Instance.Get();
                    settings.Proxy = new ProxySettings
                    {
                        Address = new Uri(Address),
                        Username = Username,
                        Password = Password,
                        BypassLocal = BypassLocal,
                        Bypass = Bypass
                    };
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                app.ShowHelp();
                return ExitCodes.WrongUsage;
            }
        }

        [Command(Description = "Resets the specified settings to their default values.")]
        [Subcommand("url", typeof(Url))]
        [Subcommand("username", typeof(Username))]
        [Subcommand("password", typeof(Password))]
        [Subcommand("proxy", typeof(Proxy))]
        [Subcommand("all", typeof(All))]
        public class Reset
        {
            [Command(Description = "Resets the server URL to " + Settings.DefaultUrl)]
            public class Url
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Url = Settings.DefaultUrl;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets the user name.")]
            public class Username
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Username = null;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets the password")]
            public class Password
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Password = null;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets the proxy settings")]
            public class Proxy
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Proxy = null;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets all settings to their defaults.")]
            public class All
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    SettingsService.Instance.Save(new Settings());
                    return ExitCodes.Success;
                }
            }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                app.ShowHelp();
                return ExitCodes.WrongUsage;
            }
        }

        public int OnExecute(CommandLineApplication app, IConsole console)
        {
            var settings = SettingsService.Instance.Get();

            var doc = new Document(
                new Grid
                {
                    Stroke = UIHelper.NoneStroke,
                    Columns = { UIHelper.FieldsColumns },
                    Children =
                    {
                        UIHelper.Row("URL", settings.Url),
                        UIHelper.Row("User name",
                            settings.Username != null
                                ? new Span(settings.Username).SetColors(ColorScheme.Current.Normal)
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive)),
                        UIHelper.Row("Password",
                            settings.Password != null
                                ? new Span("<encrypted>").SetColors(ColorScheme.Current.Active)
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive)),
                        UIHelper.Row("Proxy",
                            settings.Proxy != null
                                ? FormatProxySettings(settings.Proxy)
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive))
                    }
                }
            ).SetColors(ColorScheme.Current.Normal);

            ConsoleRenderer.RenderDocument(doc);
            return ExitCodes.Success;

            Element FormatProxySettings(ProxySettings proxy)
            {
                return new Grid
                {
                    Stroke = UIHelper.NoneStroke,
                    Columns = {UIHelper.FieldsColumns},
                    Children =
                    {
                        UIHelper.Row("Address", proxy.Address),
                        UIHelper.Row("Username",
                            !string.IsNullOrEmpty(proxy.Username)
                                ? new Span(proxy.Username).SetColors(ColorScheme.Current.Normal)
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive)),
                        UIHelper.Row("Password",
                            proxy.Password != null
                                ? new Span("<encrypted>").SetColors(ColorScheme.Current.Active)
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive)),
                        UIHelper.Row("Bypass Local", proxy.BypassLocal),
                        UIHelper.Row("Bypass", 
                            proxy.Bypass?.Any() == true
                                ? (Element)new List(proxy.Bypass.Select(x => new Span(x)))
                                : new Span("<not set>").SetColors(ColorScheme.Current.Inactive))
                    }
                };
            }
        }
    }
}
