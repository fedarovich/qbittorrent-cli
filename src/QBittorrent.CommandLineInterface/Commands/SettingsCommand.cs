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
    public class SettingsCommand
    {
        [Command(Description = "Sets the new value for the specified setting.")]
        [Subcommand("url", typeof(Url))]
        [Subcommand("username", typeof(Username))]
        [Subcommand("password", typeof(Password))]
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
                    var settings = SettingsService.Instance.GetGeneral();
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
                    var settings = SettingsService.Instance.GetGeneral();
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

                    var settings = SettingsService.Instance.GetGeneral();
                    settings.Password = value;
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
        [Subcommand("all", typeof(All))]
        public class Reset
        {
            [Command(Description = "Resets the server URL to " + GeneralSettings.DefaultUrl)]
            public class Url
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.GetGeneral();
                    settings.Url = GeneralSettings.DefaultUrl;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets the user name.")]
            public class Username
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.GetGeneral();
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
                    var settings = SettingsService.Instance.GetGeneral();
                    settings.Password = null;
                    SettingsService.Instance.Save(settings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Resets all settings to their defaults.")]
            public class All
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    SettingsService.Instance.Save(new GeneralSettings());
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
            var settings = SettingsService.Instance.GetGeneral();

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
                    }
                }
            ).SetColors(ColorScheme.Current.Normal);

            ConsoleRenderer.RenderDocument(doc);
            return ExitCodes.Success;
        }
    }
}
