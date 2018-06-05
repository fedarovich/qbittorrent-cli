using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand("network", typeof(Network))]
    public partial class SettingsCommand
    {
        [Command(Description = "Change network settings.")]
        [Subcommand("credential", typeof(Credential))]
        public class Network
        {
            [Command(Description = "Change network credentials.")]
            [Subcommand("add", typeof(Add))]
            [Subcommand("delete", typeof(Delete))]
            [Subcommand("clear", typeof(Clear))]
            public class Credential
            {
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
                    [Required(AllowEmptyStrings = true)]
                    public string Password { get; set; }

                    [Option("-d|--domain <DOMAIN>", "Domain", CommandOptionType.SingleValue)]
                    public string Domain { get; set; }

                    public int OnExecute(CommandLineApplication app, IConsole console)
                    {
                        var settings = SettingsService.Instance.Get();
                        var cred = settings.NetworkSettings.Credentials.FirstOrDefault(
                            c => AuthType == c.AuthType && Url == c.Url);
                        if (cred == null)
                        {
                            cred = new NetworkSettings.SiteCredentials()
                            {
                                Url = Url,
                                AuthType = AuthType.Value
                            };
                            settings.NetworkSettings.Credentials.Add(cred);
                        }

                        cred.Username = Username;
                        cred.Password = Password;
                        cred.Domain = Domain;

                        SettingsService.Instance.Save(settings);
                        return ExitCodes.Success;
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
                        var settings = SettingsService.Instance.Get();
                        var cred = settings.NetworkSettings.Credentials.FirstOrDefault(
                            c => AuthType == c.AuthType && Url == c.Url);
                        if (cred != null)
                        {
                            settings.NetworkSettings.Credentials.Remove(cred);
                        }

                        SettingsService.Instance.Save(settings);
                        return ExitCodes.Success;
                    }
                }

                [Command(Description = "Deletes all credentials.")]
                public class Clear
                {
                    public int OnExecute(CommandLineApplication app, IConsole console)
                    {
                        var settings = SettingsService.Instance.Get();
                        settings.NetworkSettings.Credentials.Clear();
                        SettingsService.Instance.Save(settings);
                        return ExitCodes.Success;
                    }
                }
            }
        }
    }
}

