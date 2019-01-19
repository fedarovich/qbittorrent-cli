using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;

namespace DocumentationGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            try
            {
                app.Conventions.UseDefaultConventions();
                int code = app.Execute(args);
                return code;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            finally
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
#endif
            }
        }

        [Option("-i|--input <ASSEMBLY>", "Input assembly path", CommandOptionType.SingleValue)]
        [Required]
        [FileExists]
        public string InputAssembly { get; set; }

        [Option("-o|--out-dir <PATH>", "Output directory", CommandOptionType.SingleValue)]
        [Required]
        [DirectoryExists]
        public string OutputDir { get; set; }

        [Option("-r|--root-command <CLASS>", "Root command class", CommandOptionType.SingleValue)]
        [DefaultValue("Program")]
        public string RootCommand { get; set; }

        [Option("-n|--name <ROOT_COMMAND_NAME>", "", CommandOptionType.SingleValue)]
        public string RootCommandName { get; set; }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            var assembly = Assembly.LoadFrom(InputAssembly);
            var rootCommandTypeName = string.IsNullOrEmpty(RootCommand) ? "Program" : RootCommand;
            var rootCommandType = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == rootCommandTypeName)
                                  ?? assembly.GetExportedTypes().Single(t => t.Name == rootCommandTypeName);
            var appType = typeof(CommandLineApplication<>).MakeGenericType(rootCommandType);
            var root = (CommandLineApplication) Activator.CreateInstance(appType, new object[] {true});
            root.Conventions.UseDefaultConventions();
            root.Name = RootCommandName ?? assembly.GetName().Name;

            var outDir = new DirectoryInfo(OutputDir);
            foreach (var item in outDir.GetFileSystemInfos())
            {
                item.Delete();
            }

            var helpTextGenerator = new MarkdownHelpTextGenerator();
            Generate(root, root.Name);

            using (var writer = new StreamWriter(Path.Combine(OutputDir, "command-reference.md"), false))
            {
                helpTextGenerator.GenerateCommandIndex(root, writer);
            }

            return 0;

            void Generate(CommandLineApplication command, string name)
            {
                using (var writer = new StreamWriter(Path.Combine(OutputDir, name + ".md"), false))
                {
                    helpTextGenerator.Generate(command, writer);
                }

                foreach (var subCommand in command.Commands)
                {
                    Generate(subCommand, $"{name}-{subCommand.Name}");
                }
            }
        }
    }
}
