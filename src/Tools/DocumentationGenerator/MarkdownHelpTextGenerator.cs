using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;

namespace DocumentationGenerator
{
    public class MarkdownHelpTextGenerator : IHelpTextGenerator
    {
        /// <summary>
        /// Determines if commands are ordered by name in generated help text
        /// </summary>
        public bool SortCommandsByName { get; set; } = true;

        /// <inheritdoc />
        public virtual void Generate(CommandLineApplication application, TextWriter output)
        {
            GenerateHeader(application, output);
            GenerateBody(application, output);
            GenerateFooter(application, output);
        }

        public void GenerateCommandIndex(CommandLineApplication application, TextWriter output)
        {
            output.WriteLine("# Command Reference");

            RenderCommand(application);

            void RenderCommand(CommandLineApplication cmd, string prefix = "")
            {
                var link = string.Join('-', EnumerateCommandParts(cmd));
                output.WriteLine($"{prefix}* [{cmd.Name}]({link})");

                prefix += "  ";
                foreach (var subCommand in cmd.Commands.OrderBy(c => c.Name))
                {
                    RenderCommand(subCommand, prefix);
                }
            }
        }

        /// <summary>
        /// Generate the first few lines of help output text
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        protected virtual void GenerateHeader(
            CommandLineApplication application,
            TextWriter output)
        {
            output.WriteLine("# Command Reference");

            output.Write("## ");
            var parts = EnumerateCommandParts(application).ToList();
            for (int i = 0; i < parts.Count - 1; i++)
            {
                var part = parts[i];
                var link = string.Join('-', parts.Take(i + 1));
                output.Write($"[{part}]({link}) ");
            }

            output.WriteLine(application.Name);
            output.WriteLine();

            if (!string.IsNullOrEmpty(application.Description))
            {
                output.WriteLine(application.Description);
                output.WriteLine();
            }
        }

        /// <summary>
        /// Generate detailed help information
        /// </summary>
        /// <param name="application">The application</param>
        /// <param name="output">Help text output</param>
        protected virtual void GenerateBody(
            CommandLineApplication application,
            TextWriter output)
        {
            var arguments = application.Arguments.Where(a => a.ShowInHelpText).ToList();
            var options = application.GetOptions().Where(o => o.ShowInHelpText).ToList();
            var commands = application.Commands.Where(c => c.ShowInHelpText).ToList();

            var firstColumnWidth = 2 + Math.Max(
                arguments.Count > 0 ? arguments.Max(a => a.Name.Length) : 0,
                Math.Max(
                    options.Count > 0 ? options.Max(o => Format(o).Length) : 0,
                    commands.Count > 0 ? commands.Max(c => c.Name?.Length ?? 0) : 0));

            GenerateUsage(application, output, arguments, options, commands);
            GenerateArguments(application, output, arguments, firstColumnWidth);
            GenerateOptions(application, output, options, firstColumnWidth);
            GenerateCommands(application, output, commands, firstColumnWidth);
        }

        /// <summary>
        /// Generate the line that shows usage
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        /// <param name="visibleArguments">Arguments not hidden from help text</param>
        /// <param name="visibleOptions">Options not hidden from help text</param>
        /// <param name="visibleCommands">Commands not hidden from help text</param>
        protected virtual void GenerateUsage(
            CommandLineApplication application,
            TextWriter output,
            IReadOnlyList<CommandArgument> visibleArguments,
            IReadOnlyList<CommandOption> visibleOptions,
            IReadOnlyList<CommandLineApplication> visibleCommands)
        {
            output.WriteLine("### Usage:");
            output.WriteLine();
            foreach (var alias in application.Names)
            {
                output.WriteLine("```");
                output.Write(string.Join(' ', EnumerateCommandParts(application, alias)));

                if (visibleArguments.Any())
                {
                    output.Write(" [arguments]");
                }

                if (visibleOptions.Any())
                {
                    output.Write(" [options]");
                }

                if (visibleCommands.Any())
                {
                    output.Write(" [command]");
                }

                if (application.AllowArgumentSeparator)
                {
                    output.Write(" [[--] <arg>...]");
                }

                output.WriteLine();
                output.WriteLine("```");
                output.WriteLine();
            }
        }

        /// <summary>
        /// Generate the lines that show information about arguments
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        /// <param name="visibleArguments">Arguments not hidden from help text</param>
        /// <param name="firstColumnWidth">The width of the first column of commands, arguments, and options</param>
        protected virtual void GenerateArguments(
            CommandLineApplication application,
            TextWriter output,
            IReadOnlyList<CommandArgument> visibleArguments,
            int firstColumnWidth)
        {
            if (visibleArguments.Any())
            {
                output.WriteLine();
                output.WriteLine("### Arguments");
                output.WriteLine();

                output.WriteLine("| Argument | Description |");
                output.WriteLine("| -------- | ----------- |");

                foreach (var arg in visibleArguments)
                {
                    output.WriteLine($"| `{arg.Name}` | {arg.Description} |");
                }
                output.WriteLine();
            }
        }

        /// <summary>
        /// Generate the lines that show information about options
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        /// <param name="visibleOptions">Options not hidden from help text</param>
        /// <param name="firstColumnWidth">The width of the first column of commands, arguments, and options</param>
        protected virtual void GenerateOptions(
            CommandLineApplication application,
            TextWriter output,
            IReadOnlyList<CommandOption> visibleOptions,
            int firstColumnWidth)
        {
            if (visibleOptions.Any())
            {
                output.WriteLine();
                output.WriteLine("### Options");
                output.WriteLine();
                output.WriteLine("| Option | Description |");
                output.WriteLine("| ------ | ----------- |");

                foreach (var opt in visibleOptions)
                {
                    var message = $"| {Format(opt)} | {opt.Description.Replace("|", " \\| ").Replace("\n", "")} |";
                    output.WriteLine(message);
                }
                output.WriteLine();
            }
        }

        /// <summary>
        /// Generate the lines that show information about subcommands
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        /// <param name="visibleCommands">Commands not hidden from help text</param>
        /// <param name="firstColumnWidth">The width of the first column of commands, arguments, and options</param>
        protected virtual void GenerateCommands(
            CommandLineApplication application,
            TextWriter output,
            IReadOnlyList<CommandLineApplication> visibleCommands,
            int firstColumnWidth)
        {
            if (visibleCommands.Any())
            {
                output.WriteLine();
                output.WriteLine("### Commands");
                output.WriteLine();
                output.WriteLine("| Command | Aliases | Description |");
                output.WriteLine("| ------- | ------  | ----------- |");

                var orderedCommands = SortCommandsByName
                    ? visibleCommands.OrderBy(c => c.Name).ToList()
                    : visibleCommands;
                foreach (var cmd in orderedCommands)
                {
                    var link = string.Join('-', EnumerateCommandParts(cmd));
                    var aliases = string.Join("<br/>", cmd.Names.Skip(1));
                    var message = $"| [{cmd.Name}]({link}) | {aliases} | {cmd.Description} |";
                    output.Write(message);
                    output.WriteLine();
                }
                output.WriteLine();

                if (application.OptionHelp != null)
                {
                    var fullName = string.Join(' ', EnumerateCommandParts(application));
                    output.WriteLine();
                    output.WriteLine($"Run `{fullName} [command] --{application.OptionHelp.LongName}` for more information about a command.");
                }
            }

        }

        /// <summary>
        /// Generate the last lines of help text output
        /// </summary>
        /// <param name="application">The app</param>
        /// <param name="output">Help text output</param>
        protected virtual void GenerateFooter(
            CommandLineApplication application,
            TextWriter output)
        {
            if (string.IsNullOrWhiteSpace(application.ExtendedHelpText))
                return;

            output.WriteLine("### Details");
            var paragraphs = application.ExtendedHelpText.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.None);
            foreach (var paragraph in paragraphs)
            {
                output.Write(paragraph);
                output.WriteLine();
            }
        }

        /// <summary>
        /// Generates the template string in the format "-{Symbol}|-{Short}|--{Long} &lt;{Value}&gt;" for display in help text.
        /// </summary>
        /// <returns>The template string</returns>
        protected virtual string Format(CommandOption option)
        {
            var value = GetValueName();
            var parts = new []
            {
                GetOptionName("--", option.LongName),
                GetOptionName("-", option.ShortName),
                GetOptionName("-", option.SymbolName)
            };

            return string.Join("<br/>", parts.Where(p => !string.IsNullOrEmpty(p)));

            string GetOptionName(string prefix, string name)
            {
                return string.IsNullOrEmpty(name) ? string.Empty : $"`{prefix}{name}{value}`";
            }

            string GetValueName()
            {
                if (string.IsNullOrEmpty(option.ValueName) || option.OptionType == CommandOptionType.NoValue)
                    return string.Empty;

                if (option.OptionType == CommandOptionType.SingleOrNoValue)
                {
                    return $"[`*`:<{option.ValueName}>`*`]";
                }
                else
                {
                    return $" `*`<{option.ValueName}>`*` ";
                }
            }
        }

        private IEnumerable<string> EnumerateCommandParts(CommandLineApplication command, string alias = null)
        {
            if (command.Parent != null)
            {
                foreach (var parent in EnumerateCommandParts(command.Parent))
                {
                    yield return parent;
                }
            }

            yield return alias ?? command.Name;
        }
    }
}
