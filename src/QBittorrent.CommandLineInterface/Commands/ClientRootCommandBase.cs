using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface.Commands
{
    public abstract class ClientRootCommandBase : ClientCommandBase
    {
        public virtual int OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
            return 1;
        }
    }
}
