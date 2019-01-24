using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("rss", Description = "Manages RSS feeds and rules.", ExtendedHelpText = ExperimentalHelpText)]
    public partial class RssCommand
    {
        private protected const string ExperimentalHelpText =
            "This command is experimental. It may have errors and its behavior may change in future.";
    }
}
