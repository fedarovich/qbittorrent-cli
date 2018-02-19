using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
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
                    var settings = SettingsService.Instance.Get();
                    settings.Url = Value;
                    SettingsService.Instance.Save(settings);
                    return 0;
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
                    return 0;
                }
            }

            [Command(Description = "Sets the default password.")]
            public class Password
            {
                [Argument(0, "password", "The default password.")]
                public string Value { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    if (Value == null)
                    {
                        Value = Prompt.GetPassword("Please, enter the password: ");
                    }

                    var settings = SettingsService.Instance.Get();
                    settings.Password = Value;
                    SettingsService.Instance.Save(settings);
                    return 0;
                }
            }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                app.ShowHelp();
                return 0;
            }
        }

        [Command(Description = "Resets the specified settings to their default values.")]
        [Subcommand("url", typeof(Url))]
        [Subcommand("username", typeof(Username))]
        [Subcommand("password", typeof(Password))]
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
                    return 0;
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
                    return 0;
                }
            }

            [Command(Description = "Resets the password")]
            public class Password
            {
                [Argument(0, "password", "The password.")]
                public string Value { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var settings = SettingsService.Instance.Get();
                    settings.Password = null;
                    SettingsService.Instance.Save(settings);
                    return 0;
                }
            }

            [Command(Description = "Resets all settings to their defaults.")]
            public class All
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    SettingsService.Instance.Save(new Settings());
                    return 0;
                }
            }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                app.ShowHelp();
                return 0;
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

            return 0;
        }
    }
}
