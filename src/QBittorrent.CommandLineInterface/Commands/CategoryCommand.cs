using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.Commands
{
    [Command(Description = "Manage categories.")]
    [Subcommand("add", typeof(Add))]
    [Subcommand("delete", typeof(Delete))]
    public class CategoryCommand : ClientRootCommandBase
    {
        [Command(Description = "Adds a new category.")]
        public class Add : AuthenticatedCommandBase
        {
            [Argument(0, "<NAME>", "Category name.")]
            [Required]
            [StringLength(255, MinimumLength = 1)]
            public string Name { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.AddCategoryAsync(Name);
                return ExitCodes.Success;
            }
        }

        [Command(Description = "Deletes one or several categories.")]
        public class Delete : AuthenticatedCommandBase
        {
            [Argument(0, "<NAME_1 NAME_2 ... NAME_N>", "The names of categories to delete.")]
            [Required]
            public List<string> Names { get; set; }

            protected override async Task<int> OnExecuteAuthenticatedAsync(QBittorrentClient client, CommandLineApplication app, IConsole console)
            {
                await client.DeleteCategoriesAsync(Names);
                return ExitCodes.Success;
            }
        }
    }
}
