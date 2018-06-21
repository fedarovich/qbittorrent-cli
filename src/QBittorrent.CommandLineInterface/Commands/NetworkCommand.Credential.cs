using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("credentials", typeof(Credentials))]
    public partial class NetworkCommand
    {
        [Command(Description = "Change network credentials.")]
        [Subcommand("add", typeof(Add))]
        [Subcommand("delete", typeof(Delete))]
        [Subcommand("clear", typeof(Clear))]
        public class Credentials
        {
            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                var networkSettings = SettingsService.Instance.GetNetwork();
                var credentials = networkSettings.Credentials;

                var doc = new Document(
                    new Grid
                    {
                        Stroke = LineThickness.Single,
                        Columns =
                        {
                            new Column {Width = GridLength.Star(1), MinWidth = 20},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                            new Column {Width = GridLength.Auto},
                        },
                        Children =
                        {
                            UIHelper.Header("URL"),
                            UIHelper.Header("Auth. Type"),
                            UIHelper.Header("Domain"),
                            UIHelper.Header("Username"),
                            credentials.Select(c => new[]
                            {
                                new Cell(c.Url.AbsoluteUri),
                                new Cell(c.AuthType),
                                new Cell(c.Domain),
                                new Cell(c.Username)
                            })
                        }
                    }
                ).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(doc);
                return ExitCodes.Success;
            }

            [Command(Description = "Adds a credential.")]
            public class Add
            {
                [Option("--url <URL>", "URL", CommandOptionType.SingleValue)]
                [Required]
                public Uri Url { get; set; }

                [Option("-a|--auth-type <AUTH_TYPE>", "Authentication type", CommandOptionType.SingleValue)]
                [Required]
                public NetworkSettings.AuthType? AuthType { get; set; }

                [Option("-u|--username <USERNAME>", "Username", CommandOptionType.SingleValue)]
                [Required]
                public string Username { get; set; }

                [Option("-p|--password <PASSWORD>", "Password", CommandOptionType.SingleValue)]
                public string Password { get; set; }

                [Option("-d|--domain <DOMAIN>", "Domain", CommandOptionType.SingleValue)]
                public string Domain { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var networkSettings = SettingsService.Instance.GetNetwork();
                    var cred = networkSettings.Credentials.FirstOrDefault(
                        c => AuthType == c.AuthType && Url == c.Url);
                    if (cred == null)
                    {
                        cred = new NetworkSettings.SiteCredentials()
                        {
                            Url = Url,
                            AuthType = AuthType.Value
                        };
                        networkSettings.Credentials.Add(cred);
                    }

                    cred.Username = Username;
                    cred.Password = Password ?? GetPassword();
                    cred.Domain = Domain;

                    SettingsService.Instance.Save(networkSettings);
                    return ExitCodes.Success;

                    string GetPassword()
                    {
                        return console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your network password: ");
                    }
                }
            }

            [Command(Description = "Deletes the credential.")]
            public class Delete
            {
                [Option("--url <URL>", "URL", CommandOptionType.SingleValue)]
                [Required]
                public Uri Url { get; set; }

                [Option("-a|--auth-type <AUTH_TYPE>", "Authentication type", CommandOptionType.SingleValue)]
                [Required]
                public NetworkSettings.AuthType? AuthType { get; set; }

                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var networkSettings = SettingsService.Instance.GetNetwork();
                    var cred = networkSettings.Credentials.FirstOrDefault(
                        c => AuthType == c.AuthType && Url == c.Url);
                    if (cred != null)
                    {
                        networkSettings.Credentials.Remove(cred);
                    }

                    SettingsService.Instance.Save(networkSettings);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Deletes all credentials.")]
            public class Clear
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var networkSettings = SettingsService.Instance.GetNetwork();
                    networkSettings.Credentials.Clear();
                    SettingsService.Instance.Save(networkSettings);
                    return ExitCodes.Success;
                }
            }
        }
    }
}
