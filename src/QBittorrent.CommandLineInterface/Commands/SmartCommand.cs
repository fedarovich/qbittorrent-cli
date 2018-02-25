using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Manage torrents based on scripted rules.")]
    public partial class SmartCommand : ClientRootCommandBase
    {
    }
}
