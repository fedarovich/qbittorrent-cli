using Alba.CsConsoleFormat;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.ColorSchemes;
using QBittorrent.CommandLineInterface.Services;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Configure network settings, credentials, etc.")]
    [Subcommand(typeof(Settings))]
    public partial class NetworkCommand
    {
        [Command(Description = "Configure network settings.")]
        public class Settings
        {
            [Option("-d|--use-default-credentials <BOOL>",
                "Use your current domain credentials to authenticate.",
                CommandOptionType.SingleValue,
                Inherited = false)]
            public bool? UseDefaultCredentials { get; set; }

            [Option("-i|--ignore-certificate-errors <BOOL>",
                "Ignore certificate validation errors. WARNING: Settings this property to TRUE can be dangerous!",
                CommandOptionType.SingleValue,
                Inherited = false)]
            public bool? IgnoreCertificateErrors { get; set; }

            public int OnExecute(CommandLineApplication app, IConsole console)
            {
                var net = SettingsService.Instance.GetNetwork();

                bool hasChanges = false;

                if (UseDefaultCredentials.HasValue)
                {
                    net.UseDefaultCredentials = UseDefaultCredentials.Value;
                    hasChanges = true;
                }

                if (IgnoreCertificateErrors.HasValue)
                {
                    net.IgnoreCertificateErrors = IgnoreCertificateErrors.Value;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    SettingsService.Instance.Save(net);
                    return ExitCodes.Success;
                }

                var doc = new Document(
                    new Grid
                    {
                        Stroke = UIHelper.NoneStroke,
                        Columns =
                        {
                            UIHelper.FieldsColumns
                        },
                        Children =
                        {
                            UIHelper.Row("Use default credentials", net.UseDefaultCredentials),
                            UIHelper.Row("Ignore certificate errors", net.IgnoreCertificateErrors)
                        }
                    }
                ).SetColors(ColorScheme.Current.Normal);

                ConsoleRenderer.RenderDocument(doc);
                return ExitCodes.Success;
            }
        }

        public int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return ExitCodes.WrongUsage;
        }
    }
}

