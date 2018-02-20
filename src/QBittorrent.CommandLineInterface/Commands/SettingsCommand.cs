using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Gets or sets this application's settings.")]
    [Subcommand("set", typeof(Set))]
    [Subcommand("reset", typeof(Reset))]
    [HelpOption]
    public class SettingsCommand
    {
        [Command(Description = "Sets the new value for the specified setting.")]
        [Subcommand("url", typeof(Url))]
        [Subcommand("username", typeof(Username))]
        [Subcommand("password", typeof(Password))]
        [HelpOption]
        public class Set
        {
            [Command(Description = "Sets the default server URL.")]
            [HelpOption]
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
            [HelpOption]
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
            [HelpOption]
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
        [HelpOption]
        public class Reset
        {
            [Command(Description = "Resets the server URL to " + Settings.DefaultUrl)]
            [HelpOption]
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
            [HelpOption]
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
            [HelpOption]
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

            [Command(Description = "Resets all settings to their defaults.")]
            [HelpOption]
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

            console.WriteColored("URL:       ", ConsoleColor.Yellow).WriteLineColored(settings.Url, ConsoleColor.White);

            console.WriteColored("User name: ", ConsoleColor.Yellow);
            if (settings.Username == null)
            {
                console.WriteLineColored("<not set>", ConsoleColor.DarkGray);
            }
            else
            {
                console.WriteLineColored(settings.Username, ConsoleColor.White);
            }

            console.WriteColored("Password:  ", ConsoleColor.Yellow);
            if (settings.Password != null)
            {
                console.WriteLineColored("<encrypted>", ConsoleColor.Green);
            }
            else
            {
                console.WriteLineColored("<not set>", ConsoleColor.DarkGray);
            }

            return ExitCodes.Success;
        }
    }
}
