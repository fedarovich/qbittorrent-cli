using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Commands;

namespace QBittorrent.CommandLineInterface
{
    [Command]
    [Subcommand("list", typeof(ListCommand))]
    [Subcommand("download", typeof(DownloadCommand))]
    [Subcommand("get", typeof(GetCommand))]
    [HelpOption]
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                int code = await CommandLineApplication.ExecuteAsync<Program>(args);
                Console.ReadKey();
                return code;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        async Task<int> OnExecute()
        {
            return 0;
        }
    }
}
