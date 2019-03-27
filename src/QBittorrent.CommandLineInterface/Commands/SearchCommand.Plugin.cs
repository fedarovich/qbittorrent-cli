using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;
using QBittorrent.CommandLineInterface.ColorSchemes;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Subcommand(typeof(Plugin))]
    public partial class SearchCommand
    {
        [Command("plugin", "plug-in", Description = "Manages search plugins.")]
        [Subcommand(typeof(Install))]
        [Subcommand(typeof(Uninstall))]
        [Subcommand(typeof(Enable))]
        [Subcommand(typeof(Disable))]
        [Subcommand(typeof(Update))]
        [Subcommand(typeof(List))]
        public class Plugin : ClientRootCommandBase
        {
            [Command("install", "add", Description = "Installs the search plugins.")]
            public class Install : AuthenticatedCommandBase
            {
                [Argument(0, "URL_1 URL_2 ... URL_N", "The URLs of the plugins.")]
                [Required]
                public IList<Uri> Urls { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.InstallSearchPluginsAsync(Urls);
                    return ExitCodes.Success;
                }
            }

            [Command("uninstall", "delete", Description = "Uninstalls the search plugins.")]
            public class Uninstall : AuthenticatedCommandBase
            {
                [Argument(0, "Name1 Name2 ... NameN", "The short names of the plugins.")]
                [Required]
                public IList<string> Names { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.UninstallSearchPluginsAsync(Names);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Enables the search plugins.")]
            public class Enable : AuthenticatedCommandBase
            {
                [Argument(0, "Name1 Name2 ... NameN", "The short names of the plugins.")]
                [Required]
                public IList<string> Names { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.EnableSearchPluginsAsync(Names);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Disables the search plugins.")]
            public class Disable : AuthenticatedCommandBase
            {
                [Argument(0, "Name1 Name2 ... NameN", "The short names of the plugins.")]
                [Required]
                public IList<string> Names { get; set; }

                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.DisableSearchPluginsAsync(Names);
                    return ExitCodes.Success;
                }
            }

            [Command(Description = "Updates the search plugins.", 
                ExtendedHelpText = "This command will also install default search plugins if they are missing.")]
            public class Update : AuthenticatedCommandBase
            {
                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
                {
                    await client.UpdateSearchPluginsAsync();
                    return ExitCodes.Success;
                }
            }

            [Command("list", "show", Description = "Shows the installed search plugins.")]
            public class List : AuthenticatedCommandBase
            {
                [Option("-v|--verbose", "Displays verbose information.", CommandOptionType.NoValue)]
                public bool Verbose { get; set; }


                protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client,
                    CommandLineApplication app, IConsole console)
                {
                    var plugins = await client.GetSearchPluginsAsync();

                    if (Verbose)
                    {
                        PrintPluginsVerbose(plugins);
                    }
                    else
                    {
                        PrintPluginsTable(plugins);
                    }

                    return ExitCodes.Success;
                }

                private void PrintPluginsTable(IEnumerable<SearchPlugin> plugins)
                {
                    var doc = new Document(
                        new Grid
                        {
                            Columns =
                            {
                                new Column {Width = GridLength.Auto},
                                new Column {Width = GridLength.Star(1)},
                                new Column {Width = GridLength.Auto},
                            },
                            Children =
                            {
                                UIHelper.Header("Name"),
                                UIHelper.Header("Full Name"),
                                UIHelper.Header("Enabled"),
                                plugins.Select(p => new[]
                                {
                                    new Cell(p.Name),
                                    new Cell(p.FullName),
                                    new Cell(p.IsEnabled)
                                })
                            },
                            Stroke = LineThickness.Single
                        }
                    ).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc);
                }

                private void PrintPluginsVerbose(IEnumerable<SearchPlugin> plugins)
                {
                    var doc = new Document(
                        plugins.Select(plugin =>
                            new Grid
                            {
                                Stroke = new LineThickness(LineWidth.None),
                                Columns = {UIHelper.FieldsColumns},
                                Children =
                                {
                                    UIHelper.Row("Name", plugin.Name),
                                    UIHelper.Row("Full name", plugin.FullName),
                                    UIHelper.Row("Version", plugin.Version),
                                    UIHelper.Row("Is enabled", plugin.IsEnabled),
                                    UIHelper.Row("URL", plugin.Url),
                                    UIHelper.Row("Categories",
                                        new Alba.CsConsoleFormat.List(plugin.SupportedCategories.ToArray<object>()))
                                },
                                Margin = new Thickness(0, 0, 0, 2)
                            }
                        )
                    ).SetColors(ColorScheme.Current.Normal);

                    ConsoleRenderer.RenderDocument(doc);
                }
            }
        }
    }
}
