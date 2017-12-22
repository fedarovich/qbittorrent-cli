using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command("apiVersion")]
    public class GetApiVersionCommand : CommandBase
    {
        public async Task<int> OnExecute()
        {
            throw new NotImplementedException();

        }
    }
}
