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
    [Subcommand(typeof(Proxy))]
    public partial class NetworkCommand
    {
        [Command(Description = "Manages proxy connection.")]
        [Subcommand(typeof(Set))]
        [Subcommand(typeof(Reset))]
        public class Proxy
        {
            [Command(Description = "Configures proxy to use.")]
            public class Set
            {
                [Option("-a|--address <URL>", "Proxy server address (URL)", CommandOptionType.SingleValue)]
                [Required]
                public Uri Address { get; set; }

                [Option("-u|--username <USERNAME>",
                    "Proxy server user name. Pass empty string to use default credentials.",
                    CommandOptionType.SingleValue)]
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
                        Password = Password ?? GetPassword();
                    }

                    var networkSettings = SettingsService.Instance.GetNetwork();
                    networkSettings.Proxy = new ProxySettings
                    {
                        Address = Address,
                        Username = Username,
                        Password = Password,
                        BypassLocal = BypassLocal,
                        Bypass = Bypass
                    };
                    SettingsService.Instance.Save(networkSettings);
                    return ExitCodes.Success;

                    string GetPassword()
                    {
                        return console.IsInputRedirected
                            ? console.In.ReadLine()
                            : Prompt.GetPassword("Please, enter your proxy password: ");
                    }
                }
            }

            [Command(Description = "Resets proxy settings.")]
            public class Reset
            {
                public int OnExecute(CommandLineApplication app, IConsole console)
                {
                    var networkSettings = SettingsService.Instance.GetNetwork();
                    networkSettings.Proxy = null;
                    SettingsService.Instance.Save(networkSettings);
                    return ExitCodes.Success;
                }
            }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                var networkSettings = SettingsService.Instance.GetNetwork();
                var proxy = networkSettings.Proxy;
                if (proxy == null)
                {
                    console.WriteLineColored("<not set>", ColorScheme.Current.Inactive);
                }
                else
                {
                    var doc = new Document(
                        new Grid
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
                                        ? (Element) new List(proxy.Bypass.Select(x => new Span(x)))
                                        : new Span("<not set>").SetColors(ColorScheme.Current.Inactive))
                            }
                        }).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc);
                }

                return ExitCodes.Success;
            }
        }
    }
}
