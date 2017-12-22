using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Autofac;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.CommandLineInterface.Commands;

namespace QBittorrent.CommandLineInterface
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await CommandLineApplication.ExecuteAsync<Program>(args);
        }

        private void CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<CommandBase>() && t.GetCustomAttribute<CommandAttribute>() != null)
                .AsSelf();
        }
    }
}
